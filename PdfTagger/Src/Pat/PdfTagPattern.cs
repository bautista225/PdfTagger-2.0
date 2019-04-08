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
using PdfTagger.Pdf;
using System;

namespace PdfTagger.Pat
{

    /// <summary>
    /// Patrón
    /// </summary>
    public class PdfTagPattern : IComparable
    {

        #region Constructors

        /// <summary>
        /// Construye una clase de PdfTagPattern.
        /// </summary>
        public PdfTagPattern()
        {
            MatchesCount = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Nombre del item de metadatos del cual
        /// se ha utilizado el valor para buscar la
        /// coincidendia encontrada en el pdf que da
        /// orígen a esta instancia de info.
        /// </summary>
        public string MetadataItemName { get; set; }

        /// <summary>
        /// Número de página del pdf de la que
        /// se ha obtenido la coincidencia orígen del
        /// info.
        /// </summary>
        public int PdfPageN { get; set; }

        /// <summary>
        /// Indica si es la última página.
        /// </summary>
        public bool IsLastPage { get; set; }

        /// <summary>
        /// Coordenadas de posición.
        /// </summary>
        public PdfTextBaseRectangle PdfRectangle { get; set; }

        /// <summary>
        /// Expresión regex del valor a buscar.
        /// </summary>
        public string RegexPattern { get; set; }     

        /// <summary>
        /// <summary>
        /// Posición de la coincidendia en caso de
        /// varias.
        /// </summary>
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Tipo orígen del patrón: grupo de palabras,
        /// lineas o texto delimitado.
        /// </summary>
        public string SourceTypeName { get; set; }

        /// <summary>
        /// Número de aciertos acumulados del patrón.
        /// </summary>
        public int MatchesCount { get; set; }

        /// <summary>
        /// Color del texto.
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// Color del texto.
        /// </summary>
        public string StrokeColor { get; set; }

        /// <summary>
        /// Nombre de la fuente.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Tamaño de la fuente.
        /// </summary>
        public string FontSize { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///  Compara la instancia actual con otro objeto del mismo tipo y devuelve un entero
        ///  que indica si la posición de la instancia actual es anterior, posterior o igual
        ///  que la del otro objeto en el criterio de ordenación.
        /// </summary>
        /// <param name="obj"> Objeto que se va a comparar con esta instancia.</param>
        /// <returns>
        ///  Un valor que indica el orden relativo de los objetos que se están comparando.El
        ///  valor devuelto tiene los siguientes significados:Valor Significado Menor que
        ///  cero Esta instancia es anterior a obj en el criterio de ordenación. Zero Esta
        ///  instancia se produce en la misma posición del criterio de ordenación que obj.
        ///  Mayor que cero Esta instancia sigue a obj en el criterio de ordenación.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (this == obj)
                return 0;

            PdfTagPattern input = (obj as PdfTagPattern);

            if (input == null)
                throw new ArgumentException("Parámetro de tipo incorrecto.");

            if (MatchesCount > input.MatchesCount)
                return -1;
            else
                return 1;

        }

        /// <summary>
        /// Determina si el objeto especificado es igual al objeto actual.
        /// </summary>
        /// <param name="obj">Objeto que se va a comparar con el objeto actual.</param>
        /// <returns>Es true si el objeto especificado es igual al objeto actual; 
        /// en caso contrario, es false.</returns>
        public override bool Equals(object obj)
        {
            PdfTagPattern input = (obj as PdfTagPattern);

            if (input == null)
                throw new ArgumentException("Parámetro de tipo incorrecto.");

            bool equalsRectangle = false;

            if (SourceTypeName.Equals("ColorFontWordGroupsInfos"))
            {
                equalsRectangle = true;
            }
            else if (PdfRectangle == null)
            {
                if (input.PdfRectangle == null)
                    equalsRectangle = true;
            }
            else
            {
                equalsRectangle = PdfRectangle.Equals(input.PdfRectangle);
            }

            if (SourceTypeName.Equals("ColorFontWordGroupsInfos")) // Comprobamos que las propiedades del ColorFontWordGroups sean las mismas.
                return (MetadataItemName == input.MetadataItemName &&
                    PdfPageN == input.PdfPageN &&
                    IsLastPage == input.IsLastPage &&
                    equalsRectangle &&
                    RegexPattern == input.RegexPattern &&
                    Position == input.Position &&
                    SourceTypeName == input.SourceTypeName &&
                    FillColor == input.FillColor &&
                    StrokeColor == input.StrokeColor &&
                    FontName == input.FontName &&
                    FontSize == input.FontSize);
            else
                return (MetadataItemName == input.MetadataItemName &&
                    PdfPageN == input.PdfPageN &&
                    IsLastPage == input.IsLastPage &&
                    equalsRectangle &&
                    RegexPattern == input.RegexPattern &&
                    Position == input.Position &&
                    SourceTypeName == input.SourceTypeName);
        }

        /// <summary>
        /// Sirve como la función hash predeterminada.
        /// </summary>
        /// <returns>Código hash para el objeto actual.</returns>
        public override int GetHashCode()
        {
            int hash = 17;  // Un número primo
            int prime = 31; // Otro número primo.

            hash = hash * prime + MetadataItemName.GetHashCode();
            hash = hash * prime + PdfPageN.GetHashCode();
            hash = hash * prime + IsLastPage.GetHashCode();
            hash = hash * prime + ((PdfRectangle==null) ? 0 : PdfRectangle.GetHashCode());
            hash = hash * prime + (RegexPattern??"").GetHashCode();
            hash = hash * prime + Position.GetHashCode();
            hash = hash * prime + (SourceTypeName??"").GetHashCode();
            hash = hash * prime + ((FillColor == null) ? 0 : FillColor.GetHashCode());
            hash = hash * prime + ((StrokeColor == null) ? 0 : StrokeColor.GetHashCode());
            hash = hash * prime + ((FontName == null) ? 0 : FontName.GetHashCode());
            hash = hash * prime + ((FontSize == null) ? 0 : FontSize.GetHashCode());


            return hash;
        }

        /// <summary>
        /// Devuelve una cadena que representa el objeto actual.
        /// </summary>
        /// <returns>Devuelve una cadena que representa el objeto actual.</returns>
        public override string ToString()
        {
            return $"({MatchesCount}) {MetadataItemName}";
        }

        #endregion


    }
}
