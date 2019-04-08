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
    /// Soporte para regex.
    /// </summary>
    public class TxtRegex
    {

        #region Public Methods

        /// <summary>
        /// Devuelva una cadena sustituyendo los carácteres reservados 
        /// de la cadena facilitada como parámetro por
        /// la secuencia de escape correspondiente.
        /// </summary>
        /// <param name="text">Texto a 'escapar'.</param>
        /// <returns></returns>
        public static string EscapeReserved(string text)
        {

            string reserved = @"+*?^$\.[]{}()|/";

            char dummy = (char)126;
            char escape = '\\';

            string result = Regex.Replace(text, $"\\{escape}", $"{dummy}");

            foreach (var c in reserved)
                if(c != escape)
                    result = Regex.Replace(result, $"\\{c}", $"\\{c}");

            return Regex.Replace(result, $"{dummy}", $"\\{escape}"); 

        }

        /// <summary>
        /// Devuelve una cadena en la que se sustituyen los grupos
        /// de espacios por su equivalente para regex pattern \s+.
        /// </summary>
        /// <param name="text">Texto de entrada en el que
        /// 'escapar' los espacios.</param>
        /// <returns>Cadena con los grupo de espacios
        /// sustituidos por \s+.</returns>
        public static string EscapeSpaces(string text)
        {
            return Regex.Replace(text, @"\s+", @"\s+");
        }

        /// <summary>
        /// Devuelve una cadena en la que se sustituyen los grupos
        /// de saltos de línea por su equivalente para regex pattern \n+.
        /// </summary>
        /// <param name="text">Texto de entrada en el que
        /// 'escapar' los saltos de linea.</param>
        /// <returns>Cadena con los grupo de saltos de líne
        /// sustituidos por \n+.</returns>
        public static string EscapeNewLines(string text)
        {
            return Regex.Replace(text, @"\n+", @"\n+");
        }

        /// <summary>
        /// Escapa los caráteres especiales, espacios y
        /// saltos de línea del texto de entrada.
        /// </summary>
        /// <param name="text">Texto a 'escapar'.</param>
        /// <returns>Cadena escapada preparada para pattern regex.</returns>
        public static string Escape(string text)
        {
            string ret = EscapeReserved(text);
            ret = EscapeNewLines(ret);
            return EscapeSpaces(ret);
        }

        /// <summary>
        /// Devuelve una cadena basada en el texto de entrada, dónde
        /// se sustituyen los grupos de dígitos por \d{length}.
        /// </summary>
        /// <param name="text">Texto en el que sustituir los dígitos.</param>
        /// <param name="useLength">Indica si se debe tener
        /// en cuenta la longitud de los número o no.</param>
        /// <returns>Cadena con dígitos sustituidos por \d{length}.</returns>
        public static string ReplaceDigits(string text, bool useLength = true)
        {

            string result = text;

            List<Match> matches = new List<Match>();

            foreach (Match match in Regex.Matches(result, @"\d+"))
                matches.Add(match);

            for (int m = matches.Count - 1; m > -1; m--)
            {
                Match numero = matches[m];

                string patt = "";

                if (!useLength)
                    patt = @"\d+";
                else
                    patt = @"\d{" + numero.Length + @"}";

                string firstToken = result.Substring(0, numero.Index);
                string lastToken = result.Substring(numero.Index + numero.Length, result.Length - (numero.Index + numero.Length));
                result = $"{firstToken}{patt}{lastToken}";
            }


            return result;

        }

        /// <summary>
        /// Devuelve una cadena basada en el texto de entrada, dónde
        /// se sustituyen los grupos de caracteres A-Z o a-z por 
        /// [A-Z]{length} o [a-z]{length}.
        /// </summary>
        /// <param name="text">Texto en el que sustituir los grupos de letras.</param>
        /// <returns>Cadena con dígitos sustituidos por [A-Z]{length} o [a-z]{length}.</returns>
        public static string ReplaceLetters(string text)
        {

            string result = text;

            foreach (Match letras in Regex.Matches(result, @"[A-Z]+"))
                result = result.Replace(letras.Value, @"[A-Z]{" + letras.Length + @"}");

            foreach (Match letras in Regex.Matches(result, @"[a-z]+"))
                result = result.Replace(letras.Value, @"[a-z]{" + letras.Length + @"}");

            return result;

        }

        /// <summary>
        /// Devuelve una cadena basada en el texto de entrada, dónde
        /// se sustituyen los grupos de dígitos por \d{length} y 
        /// los grupos de caracteres A-Z o a-z por 
        /// [A-Z]{length} o [a-z]{length}.
        /// </summary>
        /// <param name="text">Texto en el que sustituir los dígitos.</param>
        /// <returns>Cadena con dígitos sustituidos por \d{length}.</returns>
        public static string Replace(string text)
        {

            string result = ReplaceDigits(text);
            string dummy = ((char)126).ToString();
            result = result.Replace(@"\d", dummy);

            result = ReplaceLetters(result);

            result = result.Replace(dummy, @"\d");         

            return result;

        }

        #endregion

    }
}
