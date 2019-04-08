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
namespace PdfTagger.Pdf
{

    /// <summary>
    /// Rectangulo en una pagina de documento
    /// pdf con información en forma de texto.
    /// </summary>
    public class PdfTextRectangle : PdfTextBaseRectangle
    {

        #region Private Properties

        ///// <summary>
        ///// Rectangulo en una pagina de documento
        ///// pdf.
        ///// </summary>
        //private iTextSharp.text.Rectangle iTextRectangle;

        #endregion

        #region Private Methods

        ///// <summary>
        ///// Carga las coordenadas del rectangulo de itext
        ///// en las coordenadas de la intancia actual de PdfTextRectangle.
        ///// </summary>
        //private void SetPointsFromRectangle()
        //{
        //    Llx = iTextRectangle.Left;
        //    Lly = iTextRectangle.Bottom;
        //    Urx = iTextRectangle.Right;
        //    Ury = iTextRectangle.Top;
        //}

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una instancia de la clase PdfTextRectangle.
        /// </summary>
        public PdfTextRectangle() :base()
        {
        }

        /// <summary>
        /// Construye una nueva instancia de la clase iTextSharp.text.Rectangle
        /// a partir de un rectangulo de itext.
        /// </summary>
        /// <param name="itextRectangle"></param>
        public PdfTextRectangle(iTextSharp.text.Rectangle itextRectangle) : base(itextRectangle)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Texto dentro de el rectángulo.
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return base.ToString() + $"= '{Text}'";
        }

        #endregion

    }
}
