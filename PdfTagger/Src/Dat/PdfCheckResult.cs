using PdfTagger.Pat;
using PdfTagger.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTagger.Dat
{
    /// <summary>
    /// Clase mediante la cual obtenemos los patrones que extraen falsos positivos
    /// y actualizamos su ErrorsCount.
    /// </summary>
    public class PdfCheckResult
    {
        #region Public Properties

        /// <summary>
        /// Datos estructurados sobre los que comparar.
        /// </summary>
        public Dictionary<string, string> InvoiceMetadata { get; set; }

        /// <summary>
        /// Información desestructurada de un archivo PDF.
        /// </summary>
        public PdfUnstructuredDoc Pdf { get; set; }
        
        /// <summary>
        /// ID del documento pdf.
        /// </summary>
        public string DocID
        {
            get
            {
                return Pdf.DocID;
            }
        }

        /// <summary>
        /// Patrones sobre los que se ha encontrado que el valor extraído no era el correcto 
        /// (falsos positivos).
        /// </summary>
        public List<PdfTagPattern> ErrorPatterns { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor de la clase PdfCheckResult.
        /// </summary>
        /// <param name="pdf">Información dessestructurada de un PDF.</param>
        /// <param name="invoiceMetadata">Metadatos correctos procedentes de una B.DD.</param>
        public PdfCheckResult(PdfUnstructuredDoc pdf, Dictionary<string, string> invoiceMetadata)
        {
            Pdf = pdf;
            InvoiceMetadata = invoiceMetadata;
        }

        #endregion
    }
}
