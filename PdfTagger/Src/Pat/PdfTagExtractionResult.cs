/*
    This file is part of the PdfTagger (R) project.
    Copyright (c) 2017-2018 Irene Solutions SL
    Authors: Irene Solutions SL.

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    IRENE SOLUTIONS SL. IRENE SOLUTIONS SL DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
    OF THIRD PARTY RIGHTS
    
    This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
        http://pdftagger.com/terms-of-use.pdf
    
    The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
    
    You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the PdfTagger software without
    disclosing the source code of your own applications.
    These activities include: offering paid services to customers as an ASP,
    serving extract PDFs data on the fly in a web application, shipping PdfTagger
    with a closed source product.
    
    For more information, please contact Irene Solutions SL. at this
    address: info@irenesolutions.com
 */
using PdfTagger.Dat;
using PdfTagger.Pdf;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PdfTagger.Pat
{

    /// <summary>
    /// Resultados de una extracción de datos utilizando
    /// un PdfTagPatternStore.
    /// </summary>
    public class PdfTagExtractionResult
    {

        /// <summary>
        /// Resultados obtenidos.
        /// </summary>
        public Dictionary<string, List<PdfTagExtractionItemResult>> Results { get; set; }

        /// <summary>
        /// Datos no estructurados de entrada para el 
        /// resultado.
        /// </summary>
        public PdfUnstructuredDoc Pdf { get; set; }

        /// <summary>
        /// Converters utilizados.
        /// </summary>
        public Dictionary<Type, dynamic> Converters;

        /// <summary>
        /// Tipo de datos de la clase de referencia que 
        /// implementa IMetadata.
        /// </summary>
        public Type MetadataType { get; set; }

        /// <summary>
        /// Propuesta de metadatos.
        /// </summary>
        public IMetadata Metadata { get; private set; }

        /// <summary>
        /// Construye una nueva instancia de 
        /// la clase PdfTagExtractionResult.
        /// </summary>
        public PdfTagExtractionResult()
        {
            Results = new Dictionary<string, List<PdfTagExtractionItemResult>>();
        }

        ///// <summary>
        ///// Añade un nuevo resultado.
        ///// </summary>
        ///// <param name="name">Nombre del item.</param>
        ///// <param name="matchesCount">Número de matches del patrón.</param> 
        ///// <param name="value">Valor de resultado encontrado.</param>
        //public void AddResult(string name, int matchesCount, object value)
        //{
        //    if (!Results.ContainsKey(name))
        //        Results.Add(name, new List<PdfTagExtractionItemResult>());

        //    PdfTagExtractionItemResult itemResult = new PdfTagExtractionItemResult()
        //    {
        //        MatchesCount = matchesCount,
        //        Value = value
        //    };

        //    if (Results[name].IndexOf(itemResult) == -1)
        //        Results[name].Add(itemResult);

        //}

        /// <summary>
        /// Añade un nuevo resultado.
        /// </summary>
        /// <param name="pattern">PdfTagPattern que obtiene el resultado.</param>
        /// <param name="value">Valor de resultado encontrado.</param>
        public void AddResult(PdfTagPattern pattern, object value)
        {

            if (!Results.ContainsKey(pattern.MetadataItemName))
                Results.Add(pattern.MetadataItemName, new List<PdfTagExtractionItemResult>());

            PdfTagExtractionItemResult itemResult = new PdfTagExtractionItemResult()
            {
                Pattern = pattern,
                Value = value
            };

            if (Results[pattern.MetadataItemName].IndexOf(itemResult) == -1)
                Results[pattern.MetadataItemName].Add(itemResult);

        }

        /// <summary>
        /// Comprobamos que los datos insertados sean los correctos mediante datos de entrada revisados, actualizando posteriormente el registro de errores de los patrones.
        /// </summary>
        public List<PdfTagPattern> CheckWithRightMetadata(IMetadata metadata)
        {
            foreach (var resulList in Results)
                resulList.Value.Sort();

            List<PdfTagPattern> patterns = new List<PdfTagPattern>();

            foreach (PropertyInfo pInf in MetadataType.GetProperties())
            {
                var property = pInf.GetValue(metadata);

                if (Results.ContainsKey(pInf.Name))
                    if (Results[pInf.Name].Count > 0)
                    {
                        for (int i = 0; i < Results[pInf.Name].Count; i++)
                        {
                            if (!Results[pInf.Name][i].Value.Equals(property))
                                patterns.Add(Results[pInf.Name][i].Pattern);
                        }
                    }
            }

            return patterns;
        }

        /// <summary>
        /// Devuelve un objeto de metadatos propuesto
        /// con los valores de la extracción.
        /// </summary>
        /// <returns>Propuesta de metadatos.</returns>
        public IMetadata GetMetadata()
        {

            Metadata = (IMetadata)Activator.CreateInstance(MetadataType);

            foreach (var resulList in Results)
                resulList.Value.Sort();

            Dictionary<string, bool> metadatas = new Dictionary<string, bool>();

            foreach (PropertyInfo pInf in MetadataType.GetProperties())
            {
                metadatas.Add(pInf.Name, false);

                if (Results.ContainsKey(pInf.Name))
                    if (Results[pInf.Name].Count > 0)
                    {
                        for (int i = 0; i < Results[pInf.Name].Count; i++)
                        {
                            pInf.SetValue(Metadata, Results[pInf.Name][i].Value);
                            metadatas[pInf.Name] = true;
                            break;

                            //if (Results[pInf.Name][i].Pattern.SourceTypeName != "ColorFontWordGroupsInfos")
                            //{
                            //    pInf.SetValue(Metadata, Results[pInf.Name][i].Value);
                            //    metadatas[pInf.Name] = true;
                            //    break;
                            //}
                        }
                    }
            }

            //// Rellenamos los pInf vacíos con los posibles valores de ColorFontWordGroupsInfos.
            //foreach (PropertyInfo pInf in MetadataType.GetProperties())
            //    if (Results.ContainsKey(pInf.Name))
            //        if (!metadatas[pInf.Name]) // Devuelve false si no se ha rellenado en el bucle anterior.
            //        {
            //            if (Results[pInf.Name].Count > 0)
            //            {
            //                for (int i = 0; i < Results[pInf.Name].Count; i++)
            //                {
            //                    if (Results[pInf.Name][i].Pattern.SourceTypeName == "ColorFontWordGroupsInfos")
            //                    {
            //                        pInf.SetValue(Metadata, Results[pInf.Name][i].Value);
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            return Metadata;

        }
    }
}
