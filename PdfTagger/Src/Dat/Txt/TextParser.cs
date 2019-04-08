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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Analizador/Conversor asociado a un item 
    /// de metadatos.
    /// </summary>
    public class TextParser<T>
    {

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de la clase
        /// ItemParser.
        /// </summary>
        /// <param name="pattern">Patron regex a aplicar.</param>
        /// <param name="converter">Conversor asociado.</param>
        public TextParser(string pattern, IConverter<T> converter)
        {
            Pattern = pattern;
            Converter = converter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Patrón de expresión regular asociado.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Conversor asociado.
        /// </summary>
        public IConverter<T> Converter { get; private set; }

        #endregion       

        #region Public Methods

        /// <summary>
        /// Devuelve el conjunto de resultados encontrados
        /// en el texto facilitado.
        /// </summary>
        /// <param name="text">Texto en el que buscar</param>
        /// <returns>Resultado obtenidos.</returns>
        public List<TextParserMatch<T>> GetMatches(string text)
        {
            List<TextParserMatch<T>> matches = new List<TextParserMatch<T>>();

            int position = 0;

            if (Pattern == null)
                matches.Add(new TextParserMatch<T>(Converter.Convert(text), text, text, 0));
            else
                foreach (Match match in Regex.Matches(text, Pattern))
                    matches.Add(new TextParserMatch<T>(Converter.Convert(match.Value), 
                        match.Value, text, position++, Pattern, match));

            return matches;

        }

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return $"{Pattern}, {Converter}";
        }

        #endregion

    }
}
