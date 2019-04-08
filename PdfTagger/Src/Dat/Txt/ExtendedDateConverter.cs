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
using System.Text.RegularExpressions;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Conversor por defecto de fecha.
    /// </summary>
    public class ExtendedDateConverter : IConverter<DateTime?>
    {

        #region Public Properties

        /// <summary>
        /// Distintas nomenclaturas para nombrar los meses
        /// del año.
        /// </summary>
        public static string[] MonthNomenclatures = { "ene|feb|mar|abr|may|jun|jul|ago|sep|oct|nov|dic",
                                                "Ene|Feb|Mar|Abr|May|Jun|Jul|Ago|Sep|Oct|Nov|Dic",
                                                "Gen|Feb|Mar|Abr|Mai|Jun|Jul|Ago|Sep|Oct|Nov|Dec",
                                                "ENE|FEB|MAR|ABR|MAY|JUN|JUL|AGO|SEP|OCT|NOV|DIC",
                                                " Enero| Febrero| Marzo| Abril| Mayo| Junio| Julio| Agosto| Septiembre| Octubre| Noviembre| Diciembre",
                                                "Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre",
                                                "enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre",
                                                "ENERO|FEBRERO|MARZO|ABRIL|MAYO|JUNIO|JULIO|AGOSTO|SEPTIEMBRE|OCTUBRE|NOVIEMBRE|DICIEMBRE",
                                                "Gener|Febrer|Març|Abril|Maig|Juny|Juliol|Agost|Septembre|Octubre|Novembre|Desembre",
                                                "Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec",
                                                "January|February|March|April|May|June|July|August|September|October|November|December",
                                                "janvier|février|mars|avril|mai|juin|juillet|août|septembre|octobre|novembre|décembre",
                                                "Janvier|Février|Mars|Avril|Mai|Juin|Juillet|Août|Septembre|Octobre|Novembre|Décembre",
        };

        /// <summary>
        /// Separador ddMMyyyy.
        /// </summary>
        public static string Separator = @"(\sde\s|\sDE\s|\s|-|\/|\.)";

        #endregion

        #region Public Methods

        /// <summary>
        /// Valor del tipo DateTime? obtenido mediante
        /// el conversor.
        /// </summary>
        /// <param name="text">Texto a convertir.</param>
        /// <returns>Valor DateTime? representado por el texto.</returns>
        public DateTime? Convert(string text)
        { 

            foreach (var nomenclature in MonthNomenclatures)
            {
                int m = 0;
                foreach (var month in nomenclature.Split('|'))
                {
                    m++;

                    string pattern = Separator + month + Separator;
                    Match match = Regex.Match(text, pattern);

                    if (match.Success)
                    {
                        string date = Regex.Replace(text, pattern, $"/{m}/".PadLeft(2, '0'));
                        date = Regex.Replace(date, @"[^\d^\/]", "");

                        if (DateTime.TryParse(date, out DateTime result))
                            return result;
                        else
                            return null;
                    }                    

                }
            }

            return null;
        }

        #endregion

    }
}
