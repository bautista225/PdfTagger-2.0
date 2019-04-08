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
using iTextSharp.text.pdf.parser;
using System;

namespace PdfTagger.Pdf
{

    /// <summary>
    /// Ampliación de la clase TextChunck de iText para almacenar
    /// los puntos lower left y upper right.
    /// </summary>
    public class PdfTextChunk : LocationTextExtractionStrategy.TextChunk
    {

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de la clase PdfTextChunk.
        /// </summary>
        /// <param name="str">Texto contenido.</param>
        /// <param name="location">Localización</param>
        /// <param name="ll"></param>
        /// <param name="ur"></param>
        public PdfTextChunk(String str, LocationTextExtractionStrategy.ITextChunkLocation location,
            Vector ll, Vector ur) : base(str, location)
        {
            Ll = ll;
            Ur = ur;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Lower left
        /// </summary>
        public Vector Ll { get; private set; }

        /// <summary>
        /// Upper Right
        /// </summary>
        public Vector Ur { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return $"{Ll[Vector.I1]}, {Ll[Vector.I2]}, " +
                $"{Ur[Vector.I1]}, {Ur[Vector.I2]}: {Text}";
        }

        #endregion

    }
}
