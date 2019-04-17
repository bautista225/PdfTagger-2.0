using PdfTagger.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTagger.Pdf
{
    /// <summary>
    /// Rectangulo en una pagina de documento
    /// pdf con información en forma de texto;
    /// con su color, tamaño y nombre de fuente.
    /// </summary>
    public class PdfColorFontTextRectangle : PdfTextRectangle
    {
        #region Constructors

        /// <summary>
        /// Construye una instancia de la clase PdfColorFontTextRectangle.
        /// </summary>
        public PdfColorFontTextRectangle() : base()
        {
        }

        /// <summary>
        /// Construye una nueva instancia de la clase iTextSharp.text.Rectangle
        /// a partir de un rectangulo de itext.
        /// </summary>
        /// <param name="itextRectangle">Rectangulo que contiene texto</param>
        public PdfColorFontTextRectangle(iTextSharp.text.Rectangle itextRectangle) : base(itextRectangle)
        {
        }

        /// <summary>
        /// Construye una nueva instancia de la clase iTextSharp.text.Rectangle
        /// a partir de un rectangulo de itext.
        /// </summary>
        /// <param name="fillColor">Color del texto.</param>
        /// <param name="strokeColor">Color del texto.</param>
        /// <param name="fontName">Nombre de la fuente.</param>
        /// <param name="fontSize">Tamaño de la fuente.</param>
        /// <param name="itextRectangle">Rectangulo que contiene texto</param>
        public PdfColorFontTextRectangle(
            string fillColor, string strokeColor,
            string fontName, double? fontSize,
            iTextSharp.text.Rectangle itextRectangle) : base(itextRectangle)
        {
            FillColor = fillColor;
            StrokeColor = strokeColor;
            FontName = fontName;
            FontSize = fontSize;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Color del texto.
        /// </summary>
        public string FillColor { get; private set; }

        /// <summary>
        /// Color del texto.
        /// </summary>
        public string StrokeColor { get; private set; }

        /// <summary>
        /// Nombre de la fuente.
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Tamaño de la fuente.
        /// </summary>
        public double? FontSize { get; private set; }

        /// <summary>
        /// Tipo de patron: genérico, con X, con Y.
        /// </summary>
        public string Type { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Representación textual de la instancia.
        /// </summary>
        /// <returns>Representación textual de la instancia.</returns>
        public override string ToString()
        {
            return $"ColorFill: {FillColor}, ColorStroke: {StrokeColor}, " +
                $"FontName: {FontName}, FontSize: {FontSize}, " +
                base.ToString();
        }

        #endregion
    }
}
