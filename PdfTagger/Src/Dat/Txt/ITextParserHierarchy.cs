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

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Jerarquía de detección y conversión de valores.
    /// </summary>
    public interface ITextParserHierarchy
    {

        #region Public Properties

        /// <summary>
        /// Numero de parsers asociados
        /// a la jerarquía.
        /// </summary>
        int ParserCount { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resultados unívocos obtenidos mediante 
        /// la aplicación ordenada de parsers.
        /// </summary>
        /// <param name="input">Valor a comparar con el 
        /// valor obtenido del análisis del texto.</param>
        /// <param name="text">texto a analizar.</param>
        /// <returns>Conjunto de resultados que satisfacen
        /// la igualdad.</returns>
        List<ITextMatch> GetMatches(object input, string text);

        /// <summary>
        /// Establece el regex pattern para el parser
        /// seleccionado.
        /// </summary>
        /// <param name="parserIndex">Indice de pattern a actualizar.</param>
        /// <param name="pattern">Patrón regex.</param>
        void SetParserRegexPattern(int parserIndex, string pattern);

        /// <summary>
        /// Devuelve el regex pattern para el parser
        /// seleccionado.
        /// </summary>
        /// <param name="parserIndex">Indice de pattern a actualizar.</param>
        /// <returns>Devuelve el regex pattern para el parser.</returns>
        string GetParserRegexPattern(int parserIndex);

        /// <summary>
        /// Devuelve el converter asociado al patrón regex
        /// facilitado.
        /// </summary>
        /// <param name="pattern">Patrón regex.</param>
        /// <returns>Primer converter asociado
        /// al patrón de la jerarquía.</returns>
        object GetConverter(string pattern);

        /// <summary>
        /// Devuelve el converter asociado al patrón regex
        /// facilitado.
        /// </summary>
        /// <param name="type">Tipo.</param>
        /// <returns>Primer converter asociado
        /// al tipo.</returns>
        object GetConverter(Type type);

        #endregion  

    }
}
