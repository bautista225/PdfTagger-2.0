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

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Conversor por defecto de importes.
    /// </summary>
    public class DefaultAmountConverter : Converter<decimal>
    {

        #region Private Methods

        /// <summary>
        /// Convierte un texto en valor decimal.
        /// </summary>
        /// <param name="text">Texto a convertir.</param>
        /// <returns>Valor decimal del texto convertido.</returns>
        private static decimal ToDecimal(string text)
        {
            try
            {
                decimal decResult = 0;

                if (text.EndsWith("-"))
                    text = "-" + text.Substring(0, text.Length - 1);

                char[] possibleValues = new char[] { '.', ',', ' ', (char)160 };
                char thousandsSeparator = '\0';
                char decimalSeparator = '\0';

                for (int c = text.Length - 1; c > -1; c--)
                {
                    if (Array.IndexOf(possibleValues, text[c]) > -1)
                    {
                        decimalSeparator = text[c];
                        break;
                    }
                }

                for (int c = 0; c < text.Length; c++)
                {
                    if ((Array.IndexOf(possibleValues, text[c]) > -1) && text[c] != decimalSeparator && decimalSeparator != '\0')
                    {
                        thousandsSeparator = text[c];
                        break;
                    }
                }

                string strNumber = text.Trim();

                if (decimalSeparator != '\0')
                {
                    if (thousandsSeparator != '\0')
                        strNumber = strNumber.Replace(Char.ToString(thousandsSeparator), "");

                    string[] numberValues = strNumber.Split(decimalSeparator);

                    if (!double.TryParse(numberValues[0], out double entero))
                        return 0;

                    if (!double.TryParse(numberValues[1], out double decimales))
                        return 0;

                    double divisor = Math.Pow(10, numberValues[1].Length);

                    if (entero != 0)
                        decResult = System.Convert.ToDecimal(entero + ((entero < 0) ? -1 : 1) * decimales / divisor);
                    else
                        decResult = System.Convert.ToDecimal((text.StartsWith("-") ? -1 : 1) * decimales / divisor);
                }
                else
                {
                    decResult = System.Convert.ToDecimal(strNumber);
                }

                return decResult;

            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Valor del tipo T obtenido mediante
        /// el conversor.
        /// </summary>
        /// <param name="text">Texto a convertir.</param>
        /// <returns>Valor T representado por el texto.</returns>
        public override decimal Convert(string text)
        {
            return ToDecimal(text);
        }

        #endregion

    }
}
