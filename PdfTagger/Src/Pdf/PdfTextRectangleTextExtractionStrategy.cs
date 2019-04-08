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
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.util;

namespace PdfTagger.Pdf
{

    /// <summary>
    /// Extrategia de extracción de texto utilizada en el 
    /// método estático GetTextFromPage de la clase
    /// PdfTextExtractor. En esta clase que hereda de la
    /// clase LocationTextExtractionStrategy, sobreescribimos
    /// el método RenderText, para guardar la información 
    /// espacial de la situación del texto en los rectángulos
    /// de la colección PdfRectangles.
    /// </summary>
    public class PdfTextRectangleTextExtractionStrategy : LocationTextExtractionStrategy
    {

        #region Protected Properties

        /// <summary>
        /// Almaceno la implementación de extrategia de extracción
        /// de texto asociada.
        /// </summary>
        protected readonly ITextChunkLocationStrategy tclStrat;

        /// <summary>
        /// Almacena todos los TextChunks obtenidos con 
        /// el método RenderText.
        /// </summary>       
        protected List<PdfTextChunk> _PdfTextChunks = new List<PdfTextChunk>();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Devuelve la unión de dos rectángulos.
        /// </summary>
        /// <param name="rec1">Rectangulo 1.</param>
        /// <param name="rec2">Rectangulo 2.</param>
        /// <returns></returns>
        protected Rectangle Merge(Rectangle rec1, Rectangle rec2)
        {
            float llx = (rec1.Left < rec2.Left) ? rec1.Left : rec2.Left;
            float lly = (rec1.Bottom < rec2.Bottom) ? rec1.Bottom : rec2.Bottom;
            float urx = (rec1.Right > rec2.Right) ? rec1.Right : rec2.Right;
            float ury = (rec1.Top > rec2.Top) ? rec1.Top : rec2.Top;

            return new Rectangle(llx, lly, urx, ury);
        }

        /// <summary>
        /// Indica si el PdfTextChunck pasado como argumento
        /// es el último de la matriz _PdfTextChunks.
        /// </summary>
        /// <param name="chunk">True si es el último de la colección.</param>
        /// <returns>True si es el último de la colección, false si no.</returns>
        protected bool IsLastChunck(PdfTextChunk chunk)
        {
            return _PdfTextChunks.IndexOf(chunk) == (_PdfTextChunks.Count - 1);
        }

        /// <summary>
        /// Devuelve true si la cadena empieza con espacio.
        /// </summary>
        /// <param name="str">Cadena a analizar.</param>
        /// <returns>True si empieza por espacio false si no.</returns>
        protected bool StartsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }

        /// <summary>
        /// Devuelve true si la cadena termina con espacio.
        /// </summary>
        /// <param name="str">Cadena a analizar.</param>
        /// <returns>True si termina por espacio false si no.</returns>
        protected bool EndsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[str.Length - 1] == ' ';
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Crea un nuevo text extraction renderer.
        /// </summary>
        public PdfTextRectangleTextExtractionStrategy() : this(new TextChunkLocationStrategyDefaultImp()) {
        }

        /// <summary>
        /// Crea un nuevo text extraction renderer, con una estrategia 
        /// personalizada de creación de nuevos objetos TextChunkLocation 
        /// basados en el input de los TextRenderInfo.
        /// </summary>
        /// <param name="strat">Estrategia personalizada de
        /// creación de TextChunkLocation.</param>
        public PdfTextRectangleTextExtractionStrategy(ITextChunkLocationStrategy strat)
        {
            tclStrat = strat;
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

            _PdfTextChunks.Add(new PdfTextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment), ll, ur));

        }

        /// <summary>
        /// Implementa la extracción de grupos de palabras
        /// como texto contenido en rectangulos.
        /// </summary>
        /// <param name="fullLine">Indica si se quiere obtener el contenido
        /// por líneas completas.</param>
        /// <returns>Matriz con los rectángulos obtenidos.</returns>
        public List<PdfTextRectangle> GetWordGroups(bool fullLine = false)
        { 

            List<PdfTextRectangle> rectangles = new List<PdfTextRectangle>();

            _PdfTextChunks.Sort();

            StringBuilder sb = new StringBuilder();
            Rectangle rec = null;
            PdfTextChunk lastChunk = null;

            foreach (PdfTextChunk chunk in _PdfTextChunks)
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

                    if ((IsChunkAtWordBoundary(chunk, lastChunk) && !fullLine) || 
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

                                rectangles.Add(new PdfTextRectangle(rec)
                                {
                                    Text = sb.ToString().Trim()
                                });
                            }
                            else
                            {   // En caso contrario, lo guardo separado.
                                rectangles.Add(new PdfTextRectangle(rec)
                                {
                                    Text = sb.ToString().Trim()
                                });

                                // reset sb + rec
                                rec = new Rectangle(chunk.Ll[Vector.I1], chunk.Ll[Vector.I2],
                                    chunk.Ur[Vector.I1], chunk.Ur[Vector.I2]);
                                sb = new StringBuilder();

                                sb.Append(chunk.Text);

                                rectangles.Add(new PdfTextRectangle(rec)
                                {
                                    Text = sb.ToString().Trim()
                                });
                            }
                        }
                        else
                        {
                            rectangles.Add(new PdfTextRectangle(rec)
                            {
                                Text = sb.ToString().Trim()
                            });

                            // reset sb + rec
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

            return rectangles;
        }

        #endregion

    }
}
