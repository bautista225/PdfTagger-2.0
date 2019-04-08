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
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;

namespace PdfTagger.Pdf
{

    /// <summary>
    /// Representa la información no estructurada
    /// contenida en un documento pdf.
    /// </summary>
    public class PdfUnstructuredDoc
    {

        #region Private Methods

        /// <summary>
        /// Obtiene la información no estructurada.
        /// </summary>
        /// <param name="pdfReader">PdfReader del que obtener la información.</param>
        private void GetPdfData(PdfReader pdfReader)
        {
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {

                PdfColorFontRectangleTextExtractorStrategy colorFontRectangleStrategy =
                    new PdfColorFontRectangleTextExtractorStrategy();

                string pdfText = PdfTextExtractor.GetTextFromPage(pdfReader, page,
                  colorFontRectangleStrategy);

                var rectSize = pdfReader.GetPageSize(page);

                PdfUnstructuredPages.Add(new PdfUnstructuredPage(colorFontRectangleStrategy.GetWordGroups(),
                    colorFontRectangleStrategy.GetWordGroups(true), pdfText, colorFontRectangleStrategy.GetColorFontWordGroups())
                {
                    PageHeight = rectSize.Height,
                    PageWidth = rectSize.Width
                });

            }
            pdfReader.Close();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de PdfUnstructuredDoc
        /// sin datos.
        /// <code lang="C#">
        /// // Creamos un PdfUnstructuredDoc para extracción de datos
        /// PdfUnstructuredDoc pdf = new PdfUnstructuredDoc();
        /// </code> 
        /// <code lang="VB">
        /// ' Creamos un PdfUnstructuredDoc para extracción de datos
        /// Dim pdf As PdfUnstructuredDoc = New PdfUnstructuredDoc()
        /// </code> 
        /// </summary>
        public PdfUnstructuredDoc()
        {
        }

        /// <summary>
        /// Construye una nueva instancia de PdfUnstructuredDoc
        /// a partir del pdf cuya ruta se facilita como parámetro.
        /// <code lang="C#">
        /// // Creamos un PdfUnstructuredDoc para extracción de datos
        /// PdfUnstructuredDoc pdf = new PdfUnstructuredDoc(pdfPath);
        /// 
        /// // Almacenamos los datos extraidos en un archivo xml
        /// XmlParser.SaveAsXml(pdf, xmlPath);
        /// </code> 
        /// <code lang="VB">
        /// ' Creamos un PdfUnstructuredDoc para extracción de datos
        /// Dim pdf As PdfUnstructuredDoc = New PdfUnstructuredDoc("C:\test.pdf")
        /// 
        /// ' Almacenamos los datos extraidos en un archivo xml
        /// XmlParser.SaveAsXml(pdf, "C:\test.xml")
        /// </code> 
        /// </summary>
        public PdfUnstructuredDoc(string pdfPath)
        {

            PdfUnstructuredPages = new List<PdfUnstructuredPage>();

            using (PdfReader pdfReader = new PdfReader(pdfPath))
                GetPdfData(pdfReader);

        }

        /// <summary>
        /// Construye una nueva instancia de PdfUnstructuredDoc
        /// a partir de los bytes del pdf facilitados como parámetro.
        /// <code lang="C#">
        /// // Creamos un PdfUnstructuredDoc para extracción de datos
        /// PdfUnstructuredDoc pdf = new PdfUnstructuredDoc(pdfFile);
        /// 
        /// // Almacenamos los datos extraidos en un archivo xml
        /// XmlParser.SaveAsXml(pdf, xmlPath);
        /// </code> 
        /// <code lang="VB">
        /// ' Creamos un PdfUnstructuredDoc para extracción de datos
        /// Dim pdf As PdfUnstructuredDoc = New PdfUnstructuredDoc(pdfFile)
        /// 
        /// ' Almacenamos los datos extraidos en un archivo xml
        /// XmlParser.SaveAsXml(pdf, "C:\test.xml")
        /// </code> 
        /// </summary>
        public PdfUnstructuredDoc(byte[] pdfFile)
        {

            PdfUnstructuredPages = new List<PdfUnstructuredPage>();

            using (PdfReader pdfReader = new PdfReader(pdfFile))
                GetPdfData(pdfReader);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Categoría de documento a la que pertenece el pdf.
        /// </summary>
        public string DocCategory { get; set; }

        /// <summary>
        /// ID del documento pdf.
        /// </summary>
        public string DocID { get; set; }

        /// <summary>
        /// Páginas del documento pdf con su información
        /// no estructurada.
        /// </summary>
        public List<PdfUnstructuredPage> PdfUnstructuredPages { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Representación textual de una instancia.
        /// </summary>
        /// <returns>Representacion textual del 
        /// PdfUnstructuredDoc actual.</returns>
        public override string ToString()
        {
            return $"{DocCategory}:{DocID}";
        }

        #endregion

    }
}
