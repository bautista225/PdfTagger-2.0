using iTextSharp.text;
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
    /// Extrategia de extracción de texto utilizada en el 
    /// método estático GetTextFromPage de la clase
    /// PdfTextExtractor. En esta clase que hereda de la
    /// clase LocationTextExtractionStrategy, sobreescribimos
    /// el método RenderText, para guardar la información 
    /// espacial de la situación del texto en los rectángulos
    /// de la colección PdfRectangles, así como color, nombre y tamaño de la fuente.
    /// </summary>
    public class PdfColorFontRectangleTextExtractorStrategy : PdfTextRectangleTextExtractionStrategy
    {
        #region Private Properties

        /// <summary>
        /// Almacena todos los ColorFontTextChunks obtenidos con 
        /// el método RenderText.
        /// </summary>       
        private List<PdfColorFontTextChunk> _PdfColorFontTextChunks = new List<PdfColorFontTextChunk>();

        #endregion

        #region Constructors

        /// <summary>
        /// Crea un nuevo text extraction renderer.
        /// </summary>
        public PdfColorFontRectangleTextExtractorStrategy() : base()
        {
        }

        /// <summary>
        /// Crea un nuevo text extraction renderer, con una estrategia 
        /// personalizada de creación de nuevos objetos TextChunkLocation 
        /// basados en el input de los TextRenderInfo.
        /// </summary>
        /// <param name="strat">Estrategia personalizada de
        /// creación de TextChunkLocation.</param>
        public PdfColorFontRectangleTextExtractorStrategy(ITextChunkLocationStrategy strat) : base(strat)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Indica si el PdfTextChunck pasado como argumento
        /// es el último de la matriz _PdfTextChunks.
        /// </summary>
        /// <param name="chunk">True si es el último de la colección.</param>
        /// <returns>True si es el último de la colección, false si no.</returns>
        protected bool IsLastChunck(PdfColorFontTextChunk chunk)
        {
            return _PdfColorFontTextChunks.IndexOf(chunk) == (_PdfColorFontTextChunks.Count - 1);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtiene el texto contenido en un pdf en función del parámetro facilitado.
        /// </summary>
        /// <param name="renderInfo">Información para la obtención del texto.</param>
        public override void RenderText(TextRenderInfo renderInfo)
        {

            base.RenderText(renderInfo);

            LineSegment segment = renderInfo.GetBaseline();
            if (renderInfo.GetRise() != 0)
            {
                // remove the rise from the baseline - we do this because the text from a
                // super /subscript render operations should probably be considered as part
                // of the baseline of the text the super/sub is relative to 
                Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                segment = segment.TransformBy(riseOffsetTransform);
            }

            var ll = renderInfo.GetDescentLine().GetStartPoint(); // lower left
            var ur = renderInfo.GetAscentLine().GetEndPoint(); // upper right

            string text = renderInfo.GetText(); //mirando

            string fillColor = renderInfo.GetFillColor()?.ToString(); // Color del texto.
            string strokeColor = renderInfo.GetStrokeColor()?.ToString(); // Color del texto.

            Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();
            Vector topRight = renderInfo.GetAscentLine().GetEndPoint();
            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(curBaseline[Vector.I1], curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
            double fontSize = Math.Round(rect.Height); // Tamaño de la fuente a partir del rectángulo extraído.

            string fontName = renderInfo.GetFont()?.PostscriptFontName; //Nombre de la fuente.

            //base._PdfTextChunks.Add(new PdfTextChunk(renderInfo.GetText(), base.tclStrat.CreateLocation(renderInfo, segment), ll, ur));
            _PdfColorFontTextChunks.Add(new PdfColorFontTextChunk(renderInfo.GetText(), base.tclStrat.CreateLocation(renderInfo, segment), ll, ur, fillColor, strokeColor, fontName, fontSize));

        }

        /// <summary>
        /// Implementa la extracción de grupos de palabras
        /// como texto contenido en rectangulos;
        /// con el color, tamaño y nombre de la fuente.
        /// </summary>
        /// <returns>Matriz con los rectángulos obtenidos.</returns>
        public List<PdfColorFontTextRectangle> GetColorFontWordGroups()
        {
            List<PdfColorFontTextRectangle> CFWordGroups = new List<PdfColorFontTextRectangle>();

            _PdfColorFontTextChunks.Sort();

            StringBuilder sb = new StringBuilder();
            Rectangle rec = null;
            PdfColorFontTextChunk lastChunk = null;

            foreach (PdfColorFontTextChunk chunk in _PdfColorFontTextChunks)
            {
                if (lastChunk == null)
                {
                    sb.Append(chunk.Text);
                    rec = new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                        chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]);
                }
                else
                {

                    bool isLastChunk = IsLastChunck(chunk);

                    if ((IsChunkAtWordBoundary(chunk, lastChunk)) ||
                        !chunk.SameLine(lastChunk) ||
                        isLastChunk)
                    {

                        if (isLastChunk) // Guardo la última palabra
                        {
                            if (!IsChunkAtWordBoundary(chunk, lastChunk)) // Si son la misma palabra, lo junto.
                            {
                                rec = Merge(rec, new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                                                                chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]));
                                sb.Append(chunk.Text);

                                CFWordGroups.Add(new PdfColorFontTextRectangle(chunk.FillColor, chunk.StrokeColor, chunk.FontName, chunk.FontSize, rec)
                                {
                                    Text = sb.ToString().Trim()
                                });
                            }
                            else
                            {   // En caso contrario, lo guardo separado.
                                CFWordGroups.Add(new PdfColorFontTextRectangle(lastChunk.FillColor, lastChunk.StrokeColor, lastChunk.FontName, lastChunk.FontSize, rec) // Añadimos el valor del último chunk visto.
                                {
                                    Text = sb.ToString().Trim()
                                });

                                // reset sb + rec para almacenar el chunk que vemos en siguientes iteraciones como WordGroup
                                rec = new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                                    chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]);
                                sb = new StringBuilder();

                                sb.Append(chunk.Text);

                                CFWordGroups.Add(new PdfColorFontTextRectangle(chunk.FillColor, chunk.StrokeColor, chunk.FontName, chunk.FontSize, rec)
                                {
                                    Text = sb.ToString().Trim()
                                });
                            }
                        }
                        else
                        {
                            CFWordGroups.Add(new PdfColorFontTextRectangle(lastChunk.FillColor, lastChunk.StrokeColor, lastChunk.FontName, lastChunk.FontSize, rec) // Añadimos el valor del último chunk visto.
                            {
                                Text = sb.ToString().Trim()
                            });

                            // reset sb + rec para almacenar el chunk que vemos en siguientes iteraciones como WordGroup
                            rec = new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                                chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]);
                            sb = new StringBuilder();
                        }
                    }
                    else
                    {
                        rec = Merge(rec, new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                            chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]));
                    }

                    if (IsChunkAtWordBoundary(chunk, lastChunk) &&
                        !StartsWithSpace(chunk.Text) &&
                        !EndsWithSpace(lastChunk.Text))
                        sb.Append(' ');

                    sb.Append(chunk.Text);

                }

                lastChunk = chunk;
            }

            return CFWordGroups;
        }

        #endregion
    }
}
