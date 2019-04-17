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
using System;
using System.Collections.Generic;

namespace PdfTagger.Pat
{

    /// <summary>
    /// Almacena el resultado encontrado con
    /// la ejecución de un patrón.
    /// </summary>
    public class PdfTagExtractionItemResult : IComparable
    {
        /// <summary>
        /// Diccionario sobre el que se representa el orden para clasificar las comparaciones por tipo de Info.
        /// </summary>
        static Dictionary<string, int> SourceTypeNameOrder = new Dictionary<string, int>() {
            {"ColorFontWordGroupsInfos", 1},
            {"PdfTextInfos", 2},
            {"LinesInfos", 3},
            {"WordGroupsInfos", 4},
        };

        /// <summary>
        /// Numéro de aciertos asociados al patrón.
        /// </summary>
        public int MatchesCount
        {
            get
            {
                return Pattern.MatchesCount;
            }
        }

        /// <summary>
        /// Número de falsos positivos asociados al patrón.
        /// </summary>
        public int ErrorsCount
        {
            get
            {
                return Pattern.ErrorsCount;
            }
        }

        /// <summary>
        /// Valor encontrado.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Patrón que ha encontrado el valor.
        /// </summary>
        public PdfTagPattern Pattern { get; set; }

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

            PdfTagExtractionItemResult input = (obj as PdfTagExtractionItemResult);

            if (input == null)
                throw new ArgumentException("Parámetro de tipo incorrecto.");
            

            if ((MatchesCount - ErrorsCount) > (input.MatchesCount - input.ErrorsCount))
            {
                return -1;
            }

            else if ((MatchesCount - ErrorsCount) == (input.MatchesCount - input.ErrorsCount))
            {
                if (MatchesCount > input.MatchesCount)
                    return -1;

                else if (MatchesCount == input.MatchesCount)
                {
                    // Ordenamos los patrones según su tipo con tal de evitar falsos positivos
                    if (SourceTypeNameOrder[Pattern.SourceTypeName] > SourceTypeNameOrder[input.Pattern.SourceTypeName])
                        return -1;
                    else if (SourceTypeNameOrder[Pattern.SourceTypeName] < SourceTypeNameOrder[input.Pattern.SourceTypeName])
                        return 1;
                    else
                        return 0;
                }
                else
                    return 1;
            }   
            else
                return 1;
        }

        /*public int CompareTo(object obj)
        {
            if (this == obj)
                return 0;

            PdfTagExtractionItemResult input = (obj as PdfTagExtractionItemResult);

            if (input == null)
                throw new ArgumentException("Parámetro de tipo incorrecto.");

            if (MatchesCount > input.MatchesCount)
                return -1;
            else if (MatchesCount < input.MatchesCount)
                return 1;
            else
            {
                // Ordenamos los patrones según su tipo con tal de evitar falsos positivos
                if (SourceTypeNameOrder[Pattern.SourceTypeName] > SourceTypeNameOrder[input.Pattern.SourceTypeName])
                    return -1;
                else if (SourceTypeNameOrder[Pattern.SourceTypeName] < SourceTypeNameOrder[input.Pattern.SourceTypeName])
                    return 1;
                else
                    return 0;
            }
        }*/

        /// <summary>
        /// Sirve como la función hash predeterminada.
        /// </summary>
        /// <returns>Código hash para el objeto actual.</returns>
        public override int GetHashCode()
        {
            int hash = 17;  // Un número primo
            int prime = 31; // Otro número primo.

            hash = hash * prime + MatchesCount.GetHashCode();
            hash = hash * prime + ((Value==null) ? 0 : Value.GetHashCode());
            hash = hash * prime + Pattern.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Determina si el objeto especificado es igual al objeto actual.
        /// </summary>
        /// <param name="obj">Objeto que se va a comparar con el objeto actual.</param>
        /// <returns>Es true si el objeto especificado es igual al objeto actual; 
        /// en caso contrario, es false.</returns>
        public override bool Equals(object obj)
        {
            PdfTagExtractionItemResult input = (obj as PdfTagExtractionItemResult);

            if (input == null)
                throw new ArgumentException("Parámetro de tipo incorrecto.");

            bool equalsValue = false;

            if (Value == null)
            {
                if (input.Value == null)
                    equalsValue = true;
            }
            else
            {
                equalsValue = Value.Equals(input.Value);
            }

            bool equalsPattern = false;

            if (Pattern == null)
            {
                if (input.Pattern == null)
                    equalsPattern = true;
            }
            else
            {
                equalsPattern = Pattern.Equals(input.Pattern);
            }

            return (MatchesCount == input.MatchesCount &&
                   equalsValue && equalsPattern);
        }

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>UNa representación texttual de la instancia.</returns>
        public override string ToString()
        {
            return $"{Pattern.MetadataItemName}, {MatchesCount}, {ErrorsCount}";
        }

    }
}
