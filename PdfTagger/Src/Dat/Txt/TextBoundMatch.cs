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
    /// Coincidencia encontrada de TextBound.
    /// </summary>
    public class TextBoundMatch<T> : ITextMatch, ITextBoundMatch
    {

        #region Private Members Variables

        /// <summary>
        /// Segunda palabra por abajo.
        /// </summary>
        Match _MatchLowerSecond;

        /// <summary>
        /// Primera palabra por abajo.
        /// </summary>
        Match _MatchLowerFirst;

        /// <summary>
        /// Coincidencia central.
        /// </summary>
        Match _MatchToken;

        /// <summary>
        /// Límite superior.
        /// </summary>
        Match _MatchUpper;

        /// <summary>
        /// TextMach orígen.
        /// </summary>
        ITextMatch _ParserMatch;

        #endregion

        #region Private Methods

        /// <summary>
        /// Obtiene un texto colindante por la izquierda de un
        /// texto objetivo.
        /// </summary>
        /// <param name="match">Match con el texto objetivo utilizada
        /// para obtener el texto objetivo y la posición inicial
        /// del mismo (pardescartar coincidencia múltiples).</param>
        /// <param name="container">texto contenedor del texto objetivo.</param>
        /// <returns>Match con el texto colindante encontrado.</returns>
        private Match GetLower(Match match, string container)
        {
            string token = match.Value;

            string patternLower = string.Format(@"[^\s\n]+[\s\n]+(?={0})",
                TxtRegex.EscapeReserved(token));

            MatchCollection lowerMatches = Regex.Matches(_ParserMatch.TextContext, patternLower);

            foreach (Match matchLower in lowerMatches)
            {
                string lower = matchLower.Value;
                int tokenStart = matchLower.Index + lower.Length;
                if (match.Index == tokenStart)
                    return matchLower;
            }

            return null;

        }

        /// <summary>
        /// Encuentra el primer texto colindante más próximo
        /// por la izquierda.
        /// </summary>
        /// <returns>Texto colindante más próximo por la izquierda.</returns>
        private Match GetLowerFirst()
        {
            return GetLower(_ParserMatch.GetRegexMatch(), _ParserMatch.TextContext);
        }

        /// <summary>
        /// Obtiene el segundo texto colindante, a la izquierda del
        /// primer texto colindante.
        /// </summary>
        /// <returns>Segundo texto colindante por la izquierda.</returns>
        private Match GetLowerSecond()
        {

            if (_MatchLowerFirst == null)
                return null;

            Match match = Regex.Match(_ParserMatch.TextContext,
                TxtRegex.EscapeReserved(_MatchLowerFirst.Value) +
                TxtRegex.EscapeReserved(_MatchToken.Value));

            if (!match.Success)
                return null;

            return GetLower(match, _ParserMatch.TextContext);

        }

        /// <summary>
        /// Obtiene el texto colindante más próximo por la derecha.
        /// </summary>
        /// <returns>Texto colindante más próximo por la derecha.</returns>
        private Match GetUpper()
        {

            string token = _ParserMatch.TextValue;

            string patternUpper = string.Format(@"(?<={0})[\s\n]+[^\s\n]+",
                TxtRegex.EscapeReserved(token));

            MatchCollection upperMatches = Regex.Matches(_ParserMatch.TextContext, patternUpper);

            foreach (Match matchUpper in upperMatches)
            {
                int tokenEnd = _ParserMatch.MatchIndex + _ParserMatch.TextValue.Length;
                if (matchUpper.Index == tokenEnd)
                    return matchUpper;
            }

            return null;

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una instancia de la clase TextBoundMatch
        /// a partir de una coincidencia anterior.
        /// </summary>
        /// <param name="parserMatch">Coincidencia anterior.</param>
        public TextBoundMatch(ITextMatch parserMatch)
        {
            _ParserMatch = parserMatch;
            _MatchToken = _ParserMatch.GetRegexMatch();
            _MatchLowerFirst = GetLowerFirst();
            _MatchLowerSecond = GetLowerSecond();
            _MatchUpper = GetUpper();
            UseLengthOnPatternDigitReplacement = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Texto del que se ha obtenido el valor.
        /// </summary>
        public string TextValue
        {
            get
            {
                return _MatchToken.Value;
            }
        }

        /// <summary>
        /// Contexto del que se ha obtenido el texto.
        /// </summary>
        public string TextContext { get; private set; }

        /// <summary>
        /// Indice resultado.
        /// </summary>
        public int MatchIndex
        {
            get
            {
                if (_MatchToken == null)
                    return -1;

                return _MatchToken.Index;
            }
        }

        /// <summary>
        /// Patrón regex utilizado.
        /// </summary>
        public string Pattern
        {
            get
            {

                if (_MatchLowerFirst == null)
                    return null;

                string patternLower = (_MatchLowerSecond == null) ? "" :
                    TxtRegex.Escape(_MatchLowerSecond.Value) +
                    TxtRegex.Escape(_MatchLowerFirst.Value);

                patternLower = TxtRegex.ReplaceDigits(patternLower, 
                    UseLengthOnPatternDigitReplacement);

                patternLower = $"(?<={patternLower})";

                string patternUpper = (_MatchUpper == null) ? "" :  
                    TxtRegex.Escape(_MatchUpper.Value);

                if (!string.IsNullOrEmpty(patternUpper))
                {
                    patternUpper = TxtRegex.ReplaceDigits(patternUpper,
                        UseLengthOnPatternDigitReplacement);
                    patternUpper = $"(?={patternUpper})";
                }

                return $"{patternLower}{_ParserMatch.Pattern}{patternUpper}";
              

            }
        }

        /// <summary>
        /// En caso de varias coincidencias en un mismo
        /// contexto con el patrón, devuelve la posición
        /// de la que coincide.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Indica si de debe utilizar la longitud o no
        /// en la rutina de reemplazo de dígitos para confección
        /// del patrón regex.
        /// </summary>
        public bool UseLengthOnPatternDigitReplacement { get; set; }

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
            return _MatchToken;
        }

        /// <summary>
        /// Obtiene una representación textual de
        /// la instancia actual.
        /// </summary>
        /// <returns>Texto que representa a la instancia actual.</returns>
        public override string ToString()
        {
            return $"({((_MatchLowerSecond==null) ? "" :_MatchLowerSecond.Value)})"+
                $"({((_MatchLowerFirst==null) ? "" : _MatchLowerFirst.Value)})"+
                $"{((_MatchToken==null) ? "" : _MatchToken.Value)}"+
                $"({((_MatchUpper==null) ? "" :_MatchUpper.Value)})";
        }

        #endregion

    }
}
