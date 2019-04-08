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

namespace PdfTagger.Pdf
{

    /// <summary>
    /// Rectangulo en una pagina de documento
    /// pdf con información de posición.
    /// </summary>
    public class PdfTextBaseRectangle
    {

        #region Private Properties

        /// <summary>
        /// Rectangulo en una pagina de documento
        /// pdf.
        /// </summary>
        protected iTextSharp.text.Rectangle iTextRectangle;

        #endregion

        #region Private Methods

        /// <summary>
        /// Carga las coordenadas del rectangulo de itext
        /// en las coordenadas de la intancia actual de PdfTextRectangle.
        /// </summary>
        private void SetPointsFromRectangle()
        {
            Llx = iTextRectangle.Left;
            Lly = iTextRectangle.Bottom;
            Urx = iTextRectangle.Right;
            Ury = iTextRectangle.Top;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una instancia de la clase PdfTextRectangle.
        /// </summary>
        public PdfTextBaseRectangle()
        {
        }

        /// <summary>
        /// Construye una nueva instancia de la clase iTextSharp.text.Rectangle
        /// a partir de un rectangulo de itext.
        /// </summary>
        /// <param name="itextRectangle"></param>
        public PdfTextBaseRectangle(iTextSharp.text.Rectangle itextRectangle)
        {
            iTextRectangle = itextRectangle;
            SetPointsFromRectangle();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Lower left point x.
        /// </summary>
        public float Llx { get; set; }

        /// <summary>
        /// Lower left point y.
        /// </summary>
        public float Lly { get; set; }

        /// <summary>
        /// Upper right point x.
        /// </summary>
        public float Urx { get; set; }

        /// <summary>
        /// Upper right point y.
        /// </summary>
        public float Ury { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determina si el objeto especificado es igual al objeto actual.
        /// </summary>
        /// <param name="obj">Objeto que se va a comparar con el objeto actual.</param>
        /// <returns>Es true si el objeto especificado es igual al objeto actual; 
        /// en caso contrario, es false.</returns>
        public override bool Equals(object obj)
        {
            PdfTextBaseRectangle input = (obj as PdfTextBaseRectangle);

            if (input == null)
                return false;

            return (Llx == input.Llx &&
                    Lly == input.Lly &&
                    Urx == input.Urx &&
                    Ury == input.Ury);
        }

        /// <summary>
        /// Sirve como la función hash predeterminada.
        /// </summary>
        /// <returns>Código hash para el objeto actual.</returns>
        public override int GetHashCode()
        {
            int hash = 17;  // Un número primo
            int prime = 31; // Otro número primo.

            hash = hash * prime + Llx.GetHashCode();
            hash = hash * prime + Lly.GetHashCode();
            hash = hash * prime + Urx.GetHashCode();
            hash = hash * prime + Ury.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return $"({Llx}, {Lly}, {Urx}, {Ury},)";
        }

        /// <summary>
        /// Devuelve el rectangulo instersección de dos rectangulos
        /// facilitados como parámetros.
        /// </summary>
        /// <param name="first">Rectangulo 1.</param>
        /// <param name="second">Rectangulo 2.</param>
        /// <returns>Rectangulo interseccion.</returns>
        public static iTextSharp.text.Rectangle Intersect(iTextSharp.text.Rectangle first, 
            iTextSharp.text.Rectangle second)
        {
            // lx = max : Left x es el valor máximo de los dos rectangulos.

            float lx = (first.Left > second.Left) ? first.Left : second.Left;

            // rx = min : Right x es el valor mínimo de los dos rectángulos.

            float rx = (first.Right < second.Right) ? first.Right : second.Right;

            if (lx > rx)
                return null;

            // uy = min : Upper y es el valor mínimo de los dos rectangulos.

            float uy = (first.Top < second.Top) ? first.Top : second.Top;

            // ly = max : Lower y es el valor máximo de los dos rectángulos.

            float ly = (first.Bottom > second.Bottom) ? first.Bottom : second.Bottom;

            if (uy < ly)
                return null;

            // Con esto datos compongo el rectangulo intersección.

            return new iTextSharp.text.Rectangle(lx, ly, rx, uy);
        }

        /// <summary>
        /// Devuelve el área de un rectángulo.
        /// </summary>
        /// <param name="rectangle">Área de un rectángulo.</param>
        /// <returns>Área de un rectángulo.</returns>
        public static float GetArea(iTextSharp.text.Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }

        #endregion

    }
}
