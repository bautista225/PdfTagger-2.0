using iTextSharp.text.pdf.parser;
using PdfTagger.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTagger.Pdf
{
    /// <summary>
    /// Aplicación de la clase PdfTextChunk
    /// para almacenar el color, nombre y tamaño de la fuente.
    /// </summary>
    public class PdfColorFontTextChunk : PdfTextChunk
    {
        #region Public Properties

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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor del PdfColorFontTextChunk.
        /// </summary>
        /// <param name="str">Texto contenido.</param>
        /// <param name="location">Extractor.</param>
        /// <param name="ll">Coordenada izquierda abajo.</param>
        /// <param name="ur">Coordenada derecha arriba.</param>
        /// <param name="fillColor">Color del texto.</param>
        /// <param name="strokeColor">Color del texto.</param>
        /// <param name="fontName">Nombre de la fuente.</param>
        /// <param name="fontSize">Tamaño de la fuente.</param>
        public PdfColorFontTextChunk(
            string str, 
            LocationTextExtractionStrategy.ITextChunkLocation location, 
            Vector ll, Vector ur,
            string fillColor, string strokeColor,
            string fontName, double? fontSize
            ) : base(str, location, ll, ur)
        {
            FillColor = fillColor;
            StrokeColor = strokeColor;
            FontName = fontName;
            FontSize = fontSize;
        }

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
                $"Coordenadas: {Ll[Vector.I1]}, {Ll[Vector.I2]}, " +
                $"{Ur[Vector.I1]}, {Ur[Vector.I2]}, Texto: {Text}";
        }

        #endregion
    }
}
