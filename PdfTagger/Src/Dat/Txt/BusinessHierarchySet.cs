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
using System.Reflection;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Conjunto de jerarquías de analizadores de texto a aplicar según
    /// el tipo de datos de que se trate.
    /// </summary>
    public class BusinessHierarchySet : HierarchySet
    {

        #region Constructors

        /// <summary>
        /// Construye un nuevo catálogo de jerarúías
        /// de analizadores de textos.
        /// </summary>
        public BusinessHierarchySet() : base()
        {
            HierarchyByType.Add(typeof(DateTime?), GetDateParser());
            HierarchyByType.Add(typeof(decimal), GetAmountParser());
            HierarchyByType.Add(typeof(string), GetTextParser());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Devuelve la jerarquía estandard para fechas.
        /// </summary>
        /// <returns></returns>
        private static TextParserHierarchy<DateTime?> GetDateParser()
        {
            TextParserHierarchy<DateTime?> hierarchyDate = new TextParserHierarchy<DateTime?>()
            {
                Parsers = new List<TextParser<DateTime?>>() {
                    new TextParser<DateTime?>(@"\d{2}\/\d{2}\/\d{4}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{4}\/\d{2}\/\d{2}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\.\d{2}\.\d{4}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}-\d{2}-\d{4}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\/\d{2}\/\d{2}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\.\d{2}\.\d{2}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}-\d{2}-\d{2}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\s{1}-\s{1}\d{2}\s{1}-\s{1}\d{4}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\s{1}-\s{1}\d{2}\s{1}-\s{1}\d{2}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\s{1}\/\s{1}\d{2}\s{1}\/\s{1}\d{4}", new DefaultDateConverter()),
                    new TextParser<DateTime?>(@"\d{2}\s{1}\/\s{1}\d{2}\s{1}\/\s{1}\d{2}", new DefaultDateConverter()),

                }
            };

            foreach (var months in ExtendedDateConverter.MonthNomenclatures)
            {
                string separator = ExtendedDateConverter.Separator;
                string pattern = $"\\d{{1,2}}{separator}({months}){separator}\\d{{4}}";
                hierarchyDate.Parsers.Add(new TextParser<DateTime?>(pattern, new ExtendedDateConverter()));

                pattern = $"\\d{{1,2}}{separator}({months}){separator}\\d{{1}}\\.\\d{{3}}";
                hierarchyDate.Parsers.Add(new TextParser<DateTime?>(pattern, new ExtendedDateConverter()));

                pattern = $"\\d{{1,2}}{separator}({months}){separator}\\d{{2}}";
                hierarchyDate.Parsers.Add(new TextParser<DateTime?>(pattern, new ExtendedDateConverter()));

            }

            return hierarchyDate;


        }

        /// <summary>
        /// Devuelve la jerarquía estandar para importes.
        /// </summary>
        /// <returns>Jerarquía estandar para importes.</returns>
        private static TextParserHierarchy<decimal> GetAmountParser()
        {
            TextParserHierarchy<decimal> hierarchyAmount = new TextParserHierarchy<decimal>()
            {
                Parsers = new List<TextParser<decimal>>() {
                    new TextParser<decimal>( @"[\d.-]+,\d{2}", new DefaultAmountConverter()),           // MonedaComun
                    new TextParser<decimal>( @"[\d,-]+\.\d{2}", new DefaultAmountConverter()),          // MonedaAnglo
                    new TextParser<decimal>(@"[\d\s-]+,\d{2}", new DefaultAmountConverter()),           // MonedaMillSpaceComun
                    new TextParser<decimal>(@"[\d\s-]+\.\d{2}", new DefaultAmountConverter()),          // MonedaMillSpaceAnglo
                    new TextParser<decimal>(@"[\d.-]+,\d{1,2}", new DefaultAmountConverter()),          // MonedaComunVarDec
                    new TextParser<decimal>(@"[\d,-]+\.\d{1,2}", new DefaultAmountConverter()),         // MonedaAngloVarDec
                    new TextParser<decimal>( @"[\d.]+,\d{1,2}-{0,1}", new DefaultAmountConverter()),    // MonedaComunVarDecNegFinal
                    new TextParser<decimal>( @"[\d,]+\.\d{1,2}-{0,1}", new DefaultAmountConverter()),   // MonedaAngloVarDecNegFinal
                    new TextParser<decimal>( @"\d{2}(?=\s*%)", new DefaultAmountConverter()),           // ImpuestoDosDec
                    new TextParser<decimal>( @"\d{1}(?=\s*%)", new DefaultAmountConverter()),           // ImpuestoUnDec
                    new TextParser<decimal>( @"\d+", new DefaultAmountConverter()),                     // MonedaSinDecSinMill
                }
            };

            return hierarchyAmount;


        }

        /// <summary>
        /// Devuelve la jerarquía estandard para textos.
        /// </summary>
        /// <returns>Jerarquía estandard para textos.</returns>
        protected static TextParserHierarchy<string> GetTextParser()
        {
            TextParserHierarchy<string> hierarchyAmount = new TextParserHierarchy<string>()
            {
                Parsers = new List<TextParser<string>>()
                {
                    new TextParser<string>(null, new DefaultTextConverter())
                }
            };

            return hierarchyAmount;

        }

        #endregion

    }
}
