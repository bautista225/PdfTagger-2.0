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
using System.Text.RegularExpressions;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Representa un resultado del conjunto de resultados 
    /// obtenidos de la ejecución de un TextParser. 
    /// </summary>
    public class TextParserMatch<T> : ITextMatch
    {

        #region Private Member Variables

        Match _TextMatch;

        #endregion

        #region Public Properties

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public string TextValue { get; private set; }

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public string TextContext { get; private set; }

        /// <summary>
        /// Indice resultado.
        /// </summary>
        public int MatchIndex
        {
            get
            {
                if (_TextMatch == null)
                    return -1;

                return _TextMatch.Index;
            }
        }

        /// <summary>
        /// En caso de varias coincidencias en un mismo
        /// contexto con el patrón, devuelve la posición
        /// de la que coincide.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Patrón regex utilizado.
        /// </summary>
        public string Pattern { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de la clase TextParserMatch.
        /// </summary>
        /// <param name="value">Valor convertido.</param>
        /// <param name="text">Valor sin conversión.</param>
        /// <param name="context">Texto total orígen
        /// de la comparación. Tras ejecutar la comparación
        /// sobre resulta text almacenado como TextValue en
        /// la instancia de TextParserMacth.</param>
        /// <param name="position">Posición de la coincidencia
        /// en caso de varias.</param>
        /// <param name="pattern">Patrón reges aplicado.</param>
        /// <param name="match">Match del regex del que procede con
        /// respecto al texto original.</param>
        internal TextParserMatch(T value, string text, 
            string context, int position,
            string pattern = null, Match match = null)
        {
            Value = value;
            TextValue = text;
            TextContext = context;
            Position = position;
            Pattern = pattern;
            _TextMatch = match;
        }

        #endregion

        #region Public Methods    

        /// <summary>
        /// Devuelve la coincidencia obtenida
        /// mediante Regex sobre la que se ha
        /// creado la presente instancia.
        /// </summary>
        /// <returns></returns>
        public Match GetRegexMatch()
        {
            return _TextMatch;
        }

        /// <summary>
        /// Sirve como la función hash predeterminada.
        /// </summary>
        /// <returns>Código hash para el objeto actual.</returns>
        public int GetMatchHashCode()
        {
            int hash = 17;  // Un número primo
            int prime = 31; // Otro número primo.

            hash = hash * prime + MatchIndex.GetHashCode();
            hash = hash * prime + TextValue.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Devuelve una representación textual
        /// de la intancia actual de TextParserMatch.
        /// </summary>
        /// <returns>Representación textual 
        /// de la intancia actual</returns>
        public override string ToString()
        {
            
            return $"{((_TextMatch == null) ? "" : $"{_TextMatch.Index}")}{TextValue}: {{{Value}}}";
        }

        #endregion

    }
}
