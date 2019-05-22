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

namespace PdfTagger.Pdf
{

    /// <summary>
    /// Representa una página de documento pdf
    /// con la información no estructurada obtenida
    /// mediante itextsharp.
    /// </summary>
    public class PdfUnstructuredPage
    {

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de PdfUnstructuredPage.
        /// </summary>
        public PdfUnstructuredPage()
        {
        }

        /// <summary>
        /// Construye una nueva instancia de PdfUnstructuredPage.
        /// </summary>
        /// <param name="wordGroups">Conjunto de rectangulos
        /// con información no estructurada en su interior.</param>
        /// <param name="lines">Colección de líneas.</param>
        /// <param name="pdfText">Texto total de la página.</param>
        /// <param name="colorFontWordGroups">Conjunto de rectángulos con información no estructurada,
        /// además de su color, tamaño y tipo de fuente.</param>
        public PdfUnstructuredPage(List<PdfTextRectangle> wordGroups, 
            List<PdfTextRectangle> lines, string pdfText, List<PdfColorFontTextRectangle> colorFontWordGroups)
        {
            WordGroups = wordGroups;
            Lines = lines;
            PdfText = pdfText;
            ColorFontWordGroups = colorFontWordGroups;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Rectangulos con información  de grupos relacionados
        /// de palabras obtenidos de la página; además de su color, tamaño y nombre de la fuente.
        /// </summary>
        public List<PdfColorFontTextRectangle> ColorFontWordGroups { get; set; }

        /// <summary>
        /// Rectangulos con información  de grupos relacionados
        /// de palabras obtenidos de la página.
        /// </summary>
        public List<PdfTextRectangle> WordGroups { get; set; }

        /// <summary>
        /// Rectangulos con información  del texto por
        /// líneas obtenidos de la página.
        /// </summary>
        public List<PdfTextRectangle> Lines { get; set; }

        /// <summary>
        /// Información no estructurada total de la 
        /// página de pdf en forma de texto.
        /// </summary>
        public string PdfText { get; set; }

        /// <summary>
        /// Altura de la página.
        /// </summary>
        public float PageHeight { get; set; }

        /// <summary>
        /// Anchura de la página.
        /// </summary>
        public float PageWidth { get; set; }

        /// <summary>
        /// Número de página en el PDF.
        /// </summary>
        public int PdfPageN { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Representación textual de esta
        /// instancia de PdfUnstructuredPage.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return $"{PdfText.Substring(0, 70)}...";
        }

        #endregion

    }
}
