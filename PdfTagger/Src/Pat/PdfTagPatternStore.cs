using PdfTagger.Dat;
using PdfTagger.Dat.Txt;
using PdfTagger.Pdf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace PdfTagger.Pat
{

    /// <summary>
    /// Almacén de patrones
    /// </summary>
    [Serializable]
    [XmlRoot("PdfPatternStore")]
    public class PdfTagPatternStore
    {

        #region Private Member Variables

        Dictionary<Type, dynamic> _Converters;

        #endregion

        #region Constructors

        /// <summary>
        /// Construye un nuevo almacén de patrones.
        /// </summary>
        public PdfTagPatternStore()
        {
            PdfPatterns = new List<PdfTagPattern>();
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
        /// Nombre del catálogo de jerarquías
        /// utilizado.
        /// </summary>
        public string HierarchySetName { get; set; }

        /// <summary>
        /// Número total de comparaciones realizadas
        /// con este store.
        /// </summary>
        public int CompareCount { get; set; }

        /// <summary>
        /// Clase que implementa la interfaz IMetadata
        /// asociada al resultado de identificación de
        /// patrones.
        /// utilizado.
        /// </summary>
        public string MetadataName { get; set; }

        /// <summary>
        /// Colección de patrones.
        /// </summary>
        public List<PdfTagPattern> PdfPatterns { get; set; }

        /// <summary>
        /// Diccionario de colecciones de patrones por página.
        /// </summary>
        [XmlIgnore]
        public Dictionary<int, List<PdfTagPattern>> PdfPatternsPage { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ejecuta los patrones de extracción de textos 
        /// almacenados.
        /// </summary>
        /// <param name="pdf">Archivo PDF sobre el que extraer.</param>
        /// <returns></returns>
        public PdfTagExtractionResult Extract(PdfUnstructuredDoc pdf)
        {

            PdfTagExtractionResult result = new PdfTagExtractionResult()
            {
                Pdf = pdf,
                MetadataType = Type.GetType(MetadataName)
            };

            _Converters = new Dictionary<Type, object>();

            IHierarchySet hierarchySet = GetHierarchySet();

            PdfPatternsPage = new Dictionary<int, List<PdfTagPattern>>();
            foreach (PdfTagPattern pattern in PdfPatterns) // Evitar que los bucles de extracción recorran siempre todos los patrones idependientemente del número de página.
            {

                if (PdfPatternsPage.ContainsKey(pattern.PdfPageN))
                    PdfPatternsPage[pattern.PdfPageN].Add(pattern);
                else
                    PdfPatternsPage[pattern.PdfPageN] = new List<PdfTagPattern>() {pattern};
                
            }

            foreach (var page in pdf.PdfUnstructuredPages)
            {
                ExtractFromRectangles(page.WordGroups,
                    result.MetadataType, hierarchySet, result, page.PdfPageN);

                ExtractFromRectangles(page.Lines,
                    result.MetadataType, hierarchySet, result, page.PdfPageN, "LinesInfos");

                ExtractFromText(result.MetadataType, result, page, hierarchySet);

                ExtractFromColorFontText(page.ColorFontWordGroups,
                    result.MetadataType, hierarchySet, result, page.PdfPageN);
            }

            result.Converters = _Converters;

            result.GetMetadata();

            return result;
        }

        /// <summary>
        /// Extrae el texto con los patrones aprendidos con tal de comparar si son falsos positivos.
        /// </summary>
        /// <param name="checkResult">PdfCheckResult para comparar los falsos positivos.</param>
        /// <returns></returns>
        public List<PdfTagPattern> ExtractToCheck(PdfCheckResult checkResult)
        {

            PdfTagExtractionResult result = new PdfTagExtractionResult()
            {
                Pdf = checkResult.Pdf,
                MetadataType = Type.GetType(MetadataName)
            };

            _Converters = new Dictionary<Type, object>();

            IHierarchySet hierarchySet = GetHierarchySet();

            foreach (var page in checkResult.Pdf.PdfUnstructuredPages)
            {
                ExtractFromRectangles(page.WordGroups,
                    result.MetadataType, hierarchySet, result, page.PdfPageN);

                ExtractFromRectangles(page.Lines,
                    result.MetadataType, hierarchySet, result, page.PdfPageN, "LinesInfos");

                ExtractFromText(result.MetadataType, result, page, hierarchySet);

                ExtractFromColorFontText(page.ColorFontWordGroups,
                    result.MetadataType, hierarchySet, result, page.PdfPageN);
            }

            result.Converters = _Converters;
            
            return result.CheckWithRightMetadata(checkResult.InvoiceMetadata); // Comprobamos que los patrones que pasemos como resultado hayan extraído el texto correctamente.
        }

        /// <summary>
        /// Devuelve los patrones aprendidos para el sourceTypeName
        /// y el metadataItemName proporcionados como parámetros.
        /// </summary>
        /// <param name="sourceTypeName">WordGroupsInfos / LinesInfos ...</param>
        /// <param name="metadataItemName">Nombre de la propiedad de metadatos.</param>
        /// <returns></returns>
        public List<PdfTagPattern> GetPdfPatterns(string sourceTypeName, 
            string metadataItemName)
        {

            List<PdfTagPattern> pdfPatterns = new List<PdfTagPattern>();

            foreach (var patt in PdfPatterns)
                if (patt.SourceTypeName == sourceTypeName && 
                    patt.MetadataItemName == metadataItemName)
                    pdfPatterns.Add(patt);

            return pdfPatterns;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ejecuata la extracción basada en limites
        /// textuales.
        /// </summary>
        /// <param name="metadataType">Tipo de la clase que implementa IMetadata.</param>
        /// <param name="result">Resultado de extracción.</param>
        /// <param name="page">PdfUnstructuredPage del doc. pdf.</param>
        /// <param name="hierarchySet">Catálogo de jerarquías.</param>
        private void ExtractFromText(Type metadataType, 
            PdfTagExtractionResult result, PdfUnstructuredPage page, 
            IHierarchySet hierarchySet)
        {
            foreach (var pattern in PdfPatterns)
            {
                if (pattern.PdfPageN == page.PdfPageN || pattern.IsLastPage) // Comprobamos que los patrones realicen la extracción sobre la página que les corresponde. 
                                                                             // Se comprueba la última página porque en algunos documentos viene primero los albaranes y al final la factura.
                {
                    if (pattern.SourceTypeName == "PdfTextInfos")
                    {
                        foreach (Match match in Regex.Matches(page.PdfText, pattern.RegexPattern))
                        {
                            PropertyInfo pInf = metadataType.GetProperty(pattern.MetadataItemName);

                            if (pInf.PropertyType == typeof(string))
                            {
                                result.AddResult(pattern, match.Value);
                            }
                            else
                            {
                                dynamic converter = null;

                                if (_Converters.ContainsKey(pInf.PropertyType))
                                {
                                    converter = _Converters[pInf.PropertyType];
                                }
                                else
                                {
              
                                    ITextParserHierarchy parserHierarchy = hierarchySet.GetParserHierarchy(pInf);
                                    converter = parserHierarchy.GetConverter(pInf.PropertyType);

                                    if (converter == null)
                                    {
                                        Type converterGenType = typeof(Converter<>).MakeGenericType(pInf.PropertyType);
                                        converter = Activator.CreateInstance(converterGenType);
                                    } 
                                                              
                                }
                           
                                object pValue = converter.Convert(match.Value);
                                result.AddResult(pattern, pValue);

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta el proceso de extracción de metadatos
        /// en base a los patrones almacenados.
        /// </summary>
        /// <param name="pdfDocRectangles">rectángulos del pdf doc.</param>
        /// <param name="metadataType">Implementa IMetadata.</param>
        /// <param name="hierarchySet">Catálogo de jerarquías.</param>
        /// <param name="result">Resultados.</param>
        /// <param name="pageNumber">Número de la página sobre la que se realiza la extracción.</param>
        /// <param name="sourceTypeName">Nombre de la fuente.</param>
        private void ExtractFromRectangles(List<PdfTextRectangle> pdfDocRectangles, 
            Type metadataType, IHierarchySet hierarchySet, PdfTagExtractionResult result,
            int pageNumber,
            string sourceTypeName= "WordGroupsInfos")
        {
            foreach (var pdfDocRectangle in pdfDocRectangles)
            {
                foreach (var pattern in PdfPatterns)
                {
                    if (pattern.PdfPageN == pageNumber || pattern.IsLastPage) // Comprobamos que los patrones realicen la extracción sobre la página que les corresponde. 
                                                                                 // Se comprueba la última página porque en algunos documentos viene primero los albaranes y al final la factura.
                    {
                        if (pattern.SourceTypeName == sourceTypeName)
                        {
                            if (IsAlmostSameArea(pdfDocRectangle, pattern.PdfRectangle))
                            {
                                string textInput = pdfDocRectangle.Text;
                                PropertyInfo pInf = metadataType.GetProperty(pattern.MetadataItemName);
                                ITextParserHierarchy parserHierarchy = hierarchySet.GetParserHierarchy(pInf);

                                if (pInf.PropertyType == typeof(string))
                                    parserHierarchy.SetParserRegexPattern(0, pattern.RegexPattern);

                                dynamic converter = parserHierarchy.GetConverter(pattern.RegexPattern);

                                MatchCollection matches = Regex.Matches(pdfDocRectangle.Text, pattern.RegexPattern);

                                int p = pattern.Position;
                                int m = matches.Count;

                                string val = (pattern.Position < matches.Count) ?
                                    matches[pattern.Position].Value : null;

                                object pValue = null;

                                if (val != null && converter != null)
                                    pValue = converter.Convert(val);

                                if (pValue != null && !PdfCompare.IsZeroNumeric(pValue))
                                {
                                    result.AddResult(pattern, pValue);
                                    if (!_Converters.ContainsKey(pInf.PropertyType))
                                        _Converters.Add(pInf.PropertyType, converter);
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta el proceso de extracción de metadatos
        /// en base a los patrones almacenados.
        /// </summary>
        /// <param name="pdfDocColorFontText">Rectángulos del pdf doc con color, tamaño y nombre de fuente.</param>
        /// <param name="metadataType">Implementa IMetadata.</param>
        /// <param name="hierarchySet">Catálogo de jerarquías.</param>
        /// <param name="result">Resultados.</param>
        /// <param name="pageNumber">Número de la página sobre la que se realiza la extracción.</param>
        private void ExtractFromColorFontText(List<PdfColorFontTextRectangle> pdfDocColorFontText,
            Type metadataType, IHierarchySet hierarchySet, PdfTagExtractionResult result,
            int pageNumber)
        {
            string sourceTypeName = "ColorFontWordGroupsInfos";

            foreach (var pdfDocColorFontWord in pdfDocColorFontText)
            {
                foreach (var pattern in PdfPatterns)
                {
                    if (pattern.PdfPageN == pageNumber || pattern.IsLastPage) // Comprobamos que los patrones realicen la extracción sobre la página que les corresponde. 
                                                                              // Se comprueba la última página porque en algunos documentos viene primero los albaranes y al final la factura.
                    {
                        if (pattern.SourceTypeName == sourceTypeName)
                        {
                            if (pdfDocColorFontWord.FillColor == pattern.FillColor &&
                                pdfDocColorFontWord.StrokeColor == pattern.StrokeColor &&
                                pdfDocColorFontWord.FontName == pattern.FontName &&
                                pdfDocColorFontWord.FontSize.ToString() == pattern.FontSize
                                ) // Comprobamos que tienen el mismo color, tamaño y nombre de fuente. 
                                  // No comprobamos el CFType porque cuando llega aquí, pdfDocColorFontWord no tiene un CFType asignado aún.
                            {
                                if (pattern.CFType.Equals("NA") ||
                                    (pattern.CFType.Equals("X") && (pattern.PdfRectangle.Llx.Equals(pdfDocColorFontWord.Llx) || pattern.PdfRectangle.Urx.Equals(pdfDocColorFontWord.Urx))) ||
                                    (pattern.CFType.Equals("Y") && (pattern.PdfRectangle.Lly.Equals(pdfDocColorFontWord.Lly) || pattern.PdfRectangle.Ury.Equals(pdfDocColorFontWord.Ury))))
                                {
                                    string textInput = pdfDocColorFontWord.Text;
                                    PropertyInfo pInf = metadataType.GetProperty(pattern.MetadataItemName);
                                    ITextParserHierarchy parserHierarchy = hierarchySet.GetParserHierarchy(pInf);

                                    if (pInf.PropertyType == typeof(string))
                                        parserHierarchy.SetParserRegexPattern(0, pattern.RegexPattern);

                                    dynamic converter = parserHierarchy.GetConverter(pattern.RegexPattern);

                                    MatchCollection matches = Regex.Matches(pdfDocColorFontWord.Text, pattern.RegexPattern);

                                    string val = (pattern.Position < matches.Count) ?
                                        matches[pattern.Position].Value : null;

                                    object pValue = null;

                                    if (val != null && converter != null)
                                        pValue = converter.Convert(val);

                                    if (pValue != null && !PdfCompare.IsZeroNumeric(pValue))
                                    {
                                        result.AddResult(pattern, pValue);
                                        if (!_Converters.ContainsKey(pInf.PropertyType))
                                            _Converters.Add(pInf.PropertyType, converter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve el coeficiente calculado como la división del área
        /// del rectángulo intersección con el área del rectángulo facilitado
        /// como primero parámetro (first) que se debe corresponder con
        /// un rectángulo procedente de las colecciones WordGroups o Lines
        /// de una página que se están comparando con el rectángulo primero (first)
        /// el cual proviene de un patrón almacenado.
        /// </summary>
        /// <returns>Coeficiente de área del rectángulo first contenido
        /// en el rectangulo second. Un valor
        /// de 1 significa el rectángulo first está totalmente contenido
        /// en second.</returns>
        private float GetCommonAreaCoef(PdfTextBaseRectangle first, 
            PdfTextBaseRectangle second)
        {
            iTextSharp.text.Rectangle firstRect = new iTextSharp.text.Rectangle(first.Llx,
                             first.Lly, first.Urx, first.Ury);

            iTextSharp.text.Rectangle secondRect = new iTextSharp.text.Rectangle(second.Llx,
                          second.Lly, second.Urx, second.Ury);

            iTextSharp.text.Rectangle intersectRect = PdfTextBaseRectangle.Intersect(firstRect, secondRect);

            if (intersectRect == null)
                return 0;
          
            float firstRectArea = PdfTextBaseRectangle.GetArea(firstRect);
            float intersectRectArea = PdfTextBaseRectangle.GetArea(intersectRect);

            return intersectRectArea / firstRectArea;           

        }

        /// <summary>
        /// Devuelve true si el rectángulo first está contenido en
        /// el rectángulo second en una proporción superior al límite 
        /// establecido en las opciones de configuración en el valor
        /// de MinRectangleCommon. Es decir, cuando determinada 
        /// proporción del área del rectángulo first esté contenida
        /// en el rectángulo second.
        /// </summary>
        /// <param name="first">Rectángulo 1 (de WordGroup o Lines).</param>
        /// <param name="second">Rectángulo 2 (de Pattern).</param>
        /// <returns>True si el rectángulo de entrada está suficientemente
        /// contenido en el rectángulo second.</returns>
        private bool IsAlmostSameArea(PdfTextBaseRectangle first,
            PdfTextBaseRectangle second)
        {
            return (GetCommonAreaCoef(first, second) > 
                Settings.Current.MinRectangleCommon);

        }

        /// <summary>
        /// Devuelve una nueva instancia del catálogo
        /// de jerarquías asociado con el presenta almacén
        /// de patrones.
        /// </summary>
        /// <returns>Catálogo de jerarquías.</returns>
        private IHierarchySet GetHierarchySet()
        {
            Type hierarchySetType = Type.GetType(HierarchySetName);
            return (IHierarchySet)Activator.CreateInstance(hierarchySetType);
        }

        #endregion

    }
}
