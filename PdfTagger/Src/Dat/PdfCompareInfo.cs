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

using PdfTagger.Dat.Txt;
using PdfTagger.Pat;
using PdfTagger.Pdf;
using System.Reflection;

namespace PdfTagger.Dat
{

    /// <summary>
    /// Resultado positivo de una búsqueda en un pdf.
    /// </summary>
    public class PdfCompareInfo
    {

        #region Private Members Variables

        PdfUnstructuredDoc _Pdf;
        PdfUnstructuredPage _PdfPage;
        PdfTextRectangle _PdfTextRectangle;
        PdfColorFontTextRectangle _PdfColorFontTextRectangle;
        ITextMatch _TextMatch;
        PropertyInfo _PropertyInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de la clase PdfCompareInfo.
        /// </summary>
        /// <param name="pdf">Instacia de la clase PdfUnstructuredDoc
        /// que se utilizó para la comparación a la que pertenence
        /// el info.</param>
        /// <param name="pdfPage">Instancia de la clase PdfUnstructuredPage
        /// de la colección PdfUnstructuredPages del pdf, sobre
        /// la que se obtuvo el resultado contenido en el
        /// info a crear.</param>
        /// <param name="pdfTextRectangle">PdfTextRectangle sobre el que
        /// se ha obetnido el resultado contenido en el info.</param>
        /// <param name="textParserMatch">ITextParserMatch orígen del info.</param>
        /// <param name="propertyInfo">PropetyInfo de la propiedad
        /// de los metadatos de la cual se a comparado el valor y se
        /// ha obtenido la coincidencia que ha generado el info.</param>
        public PdfCompareInfo (PdfUnstructuredDoc pdf,
            PdfUnstructuredPage pdfPage,            
            PdfTextRectangle pdfTextRectangle,
            ITextMatch textParserMatch,
            PropertyInfo propertyInfo)
        {
            _Pdf = pdf;
            _PdfPage = pdfPage;            
            _PdfTextRectangle = pdfTextRectangle;
            _TextMatch = textParserMatch;
            _PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Construye una nueva instancia de la clase PdfCompareInfo.
        /// <param name="pdf">Instacia de la clase PdfUnstructuredDoc
        /// que se utilizó para la comparación a la que pertenence
        /// el info.</param>
        /// <param name="pdfPage">Instancia de la clase PdfUnstructuredPage
        /// de la colección PdfUnstructuredPages del pdf, sobre
        /// la que se obtuvo el resultado contenido en el
        /// info a crear.</param>
        /// <param name="pdfColorFontTextRectangle">PdfColorFontTextRectangle sobre el que
        /// se ha obetnido el resultado contenido en el info.</param>
        /// <param name="textParserMatch">ITextParserMatch orígen del info.</param>
        /// <param name="propertyInfo">PropetyInfo de la propiedad
        /// de los metadatos de la cual se a comparado el valor y se
        /// ha obtenido la coincidencia que ha generado el info.</param>
        /// </summary>
        public PdfCompareInfo (PdfUnstructuredDoc pdf,
            PdfUnstructuredPage pdfPage,
            PdfColorFontTextRectangle pdfColorFontTextRectangle,
            ITextMatch textParserMatch,
            PropertyInfo propertyInfo)
        {
            _Pdf = pdf;
            _PdfPage = pdfPage;
            _PdfColorFontTextRectangle = pdfColorFontTextRectangle;
            _TextMatch = textParserMatch;
            _PropertyInfo = propertyInfo;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Número de página del pdf de la que
        /// se ha obtenido la coincidencia orígen del
        /// info.
        /// </summary>
        public int PdfPageN
        {
            get
            {
                return _Pdf.PdfUnstructuredPages.IndexOf(_PdfPage) + 1;
            }

        }

        /// <summary>
        /// Nombre del item de metadatos del cual
        /// se ha utilizado el valor para buscar la
        /// coincidendia encontrada en el pdf que da
        /// orígen a esta instancia de info.
        /// </summary>
        public string MetadataItemName
        {
            get
            {
                return _PropertyInfo.Name;
            }
        }

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public string TextValue
        {
            get
            {
                return _TextMatch.TextValue;
            }
        }

        /// <summary>
        /// Valor obtenido.
        /// </summary>
        public string TextContext
        {
            get
            {
                return _TextMatch.TextContext;
            }
        }

        /// <summary>
        /// Indice resultado.
        /// </summary>
        public int MatchIndex
        {
            get
            {
                return _TextMatch.MatchIndex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Devuelve un patrón de búsqueda a partir de la 
        /// instancia de coicidencia.
        /// </summary>
        /// <returns>Patrón de búsqueda.</returns>
        public PdfTagPattern GetPdfTagPattern()
        {

            PdfTextBaseRectangle rectangle = null;

            if (_PdfTextRectangle != null)
                rectangle = new PdfTextBaseRectangle()
                {
                    Llx = _PdfTextRectangle.Llx,
                    Lly = _PdfTextRectangle.Lly,
                    Urx = _PdfTextRectangle.Urx,
                    Ury = _PdfTextRectangle.Ury
                };
            else if (_PdfColorFontTextRectangle != null)
            {
                rectangle = new PdfTextBaseRectangle()
                {
                    Llx = _PdfColorFontTextRectangle.Llx,
                    Lly = _PdfColorFontTextRectangle.Lly,
                    Urx = _PdfColorFontTextRectangle.Urx,
                    Ury = _PdfColorFontTextRectangle.Ury
                };
            }
                

            string regexPattern = _TextMatch.Pattern ??
                    TxtRegex.Replace(_TextMatch.TextValue);


            if(_PdfColorFontTextRectangle != null)
                return new PdfTagPattern()
                {
                    RegexPattern = regexPattern,
                    Position = _TextMatch.Position,
                    IsLastPage = (PdfPageN == _Pdf.PdfUnstructuredPages.Count),
                    PdfPageN = PdfPageN,
                    PdfRectangle = rectangle,
                    MetadataItemName = MetadataItemName,
                    FillColor = _PdfColorFontTextRectangle.FillColor,
                    StrokeColor = _PdfColorFontTextRectangle.StrokeColor,
                    FontName = _PdfColorFontTextRectangle.FontName,
                    FontSize = _PdfColorFontTextRectangle.FontSize.ToString()
                };
            else
                return new PdfTagPattern()
                {
                    RegexPattern = regexPattern,
                    Position = _TextMatch.Position,
                    IsLastPage = (PdfPageN == _Pdf.PdfUnstructuredPages.Count),
                    PdfPageN = PdfPageN,
                    PdfRectangle = rectangle,
                    MetadataItemName = MetadataItemName,
                };
        }

        /// <summary>
        /// Devuelve una representación textual
        /// de la intancia actual de PdfCompareInfo.
        /// </summary>
        /// <returns>Representación textual 
        /// de la intancia actual</returns>
        public override string ToString()
        {
            return $"({PdfPageN}-{_TextMatch.MatchIndex})" + 
                $" {((_PropertyInfo==null) ? "" :_PropertyInfo.Name)}: [{_TextMatch}]";
        }

        #endregion

    }
}
