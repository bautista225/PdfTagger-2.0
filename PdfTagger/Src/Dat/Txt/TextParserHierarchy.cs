
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
using System.Linq;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Jerarquía de anlizadores aplicables a un texto
    /// de item determinado, ordenada de mayor a menor
    /// probabilidad de ocurrencia del patrón regex.
    /// </summary>
    public class TextParserHierarchy<T> : ITextParserHierarchy
    {

        #region Private Methods

        /// <summary>
        /// Resultados unívocos obtenidos mediante 
        /// la aplicación ordenada de parsers.
        /// </summary>
        /// <param name="text">texto a analizar.</param>
        /// <returns></returns>
        private List<TextParserMatch<T>> GetMatches(string text)
        {

            Dictionary<int, TextParserMatch<T>> matches = new Dictionary<int, TextParserMatch<T>>();

            foreach (var parser in Parsers)
                foreach (var match in parser.GetMatches(text))
                    if (!matches.ContainsKey(match.GetMatchHashCode()))
                        matches.Add(match.GetMatchHashCode(), match);

            return matches.Values.ToList<TextParserMatch<T>>();

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Jerarquía de TextParsers a aplicar.
        /// </summary>
        public List<TextParser<T>> Parsers { get; set; }

        /// <summary>
        /// Numero de de parsers asociados
        /// a la jerarquía.
        /// </summary>
        public int ParserCount
        {
            get
            {
                return Parsers.Count;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Establece el regex pattern para el parser
        /// seleccionado.
        /// </summary>
        /// <param name="parserIndex">Indice de pattern a actualizar.</param>
        /// <param name="pattern">Patrón regex.</param>
        public void SetParserRegexPattern(int parserIndex, string pattern)
        {
            Parsers[parserIndex].Pattern = pattern;
        }

        /// <summary>
        /// Devuelve el regex pattern para el parser
        /// seleccionado.
        /// </summary>
        /// <param name="parserIndex">Indice de pattern a actualizar.</param>
        /// <returns>Devuelve el regex pattern para el parser.</returns>
        public string GetParserRegexPattern(int parserIndex)
        {
            return Parsers[parserIndex].Pattern;
        }

        /// <summary>
        /// Resultados unívocos obtenidos mediante 
        /// la aplicación ordenada de parsers.
        /// </summary>
        /// <param name="input">Input utilizado para comparar.</param>
        /// <param name="text">Texto a analizar en la comparación.</param>
        /// <returns></returns>
        public List<ITextMatch> GetMatches(object input, string text)
        {
            List<ITextMatch> matches = new List<ITextMatch>();

            foreach (var match in GetMatches(text))
                if(match.Value.Equals(input))
                    matches.Add(match);

            return matches;
        }

        /// <summary>
        /// Devuelve el converter asociado al patrón regex
        /// facilitado.
        /// </summary>
        /// <param name="pattern">Patrón regex.</param>
        /// <returns>Primer converter asociado
        /// al patrón de la jerarquía.</returns>
        public object GetConverter(string pattern)
        {
            foreach (var parser in Parsers)
                if (parser.Pattern == pattern)
                    return parser.Converter;
            return null;
        }

        /// <summary>
        /// Devuelve el primer converter asociado al tipo
        /// facilitado.
        /// </summary>
        /// <param name="type">Tipo.</param>
        /// <returns>Primer converter asociado
        /// al tipo facilitado.</returns>
        public object GetConverter(Type type)
        {
            foreach (var parser in Parsers)
            {
                var parserType = parser.GetType().GetGenericArguments()?[0];
                
                if (parserType == type)
                    return parser.Converter;
            }
            return null;
        }

        #endregion

    }
}
