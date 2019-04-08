using PdfTagger.Dat;
using PdfTagger.Pdf;
using PdfTagger.Xml;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfTagger.Pat
{

    /// <summary>
    /// Operaciones con patrones.
    /// </summary>
    public class PdfTagPatternFactory
    {

        #region Private Methods

        /// <summary>
        /// Devuelve el directorio de almacenamiento de patrones.
        /// </summary>
        /// <param name="docCategory">Categoría de documento.</param>
        /// <returns>Ruta del directorio de almacenamiento de patrones.</returns>
        private static string GetDirectory(string docCategory)
        {
            string dir = $"{Settings.Current.PatternsPath}{docCategory}" +
                $"{Path.DirectorySeparatorChar}";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        /// <summary>
        /// Devuelve un nombre de archivo para almacenar
        /// los patrones de un documento determinado.
        /// </summary>
        /// <param name="docID">Id. del documento.</param>
        /// <returns>Nombre del archivo de almacenamiento
        /// de patrones.</returns>
        private static string GetFileName(string docID)
        {
            return $"{docID}.xml";
        }

        /// <summary>
        /// Devuelve el nombre de archivo incluyendo la ruta
        /// donde se almacenan los patrones para un documento
        /// determinado.
        /// </summary>
        /// <param name="compareResult">Resuiltado de comparación.</param>
        /// <returns>Ruta completa al archivo.</returns>
        public static string GetPath(PdfCompareResult compareResult)
        {
           /*return $"{GetDirectory(compareResult.DocCategory)}"+
                $"{GetFileName(compareResult.DocID)}";*/

            return $"C:\\ProgramData\\PdfTagger\\Patterns\\InvoicePdfTaggerOriginalModif\\" +
                $"{GetFileName(compareResult.DocID)}";
        }

        /// <summary>
        /// Crea un nuevo almacén de patrones.
        /// </summary>
        /// <param name="compareResult">Resultado de una comparación.</param>
        /// <param name="path">Ruta del store.</param>
        private static void Create(PdfCompareResult compareResult, string path)
        {

            PdfTagPatternStore store = GetStore(compareResult);            
            XmlParser.SaveAsXml(store, path);

        }

        /// <summary>
        /// Obtiene un almacén de patrones a partir de unos
        /// resultados de comparación.
        /// </summary>
        /// <param name="compareResult">Resultados de comparación.</param>
        /// <returns>Almacén de patrones.</returns>
        private static PdfTagPatternStore GetStore(PdfCompareResult compareResult)
        {
            PdfTagPatternStore store = new PdfTagPatternStore
            {
                DocCategory = compareResult.DocCategory,
                DocID = compareResult.DocID,
                HierarchySetName = compareResult.HierarchySetName,
                MetadataName = compareResult.MetadataName,
                CompareCount = 1
            };

            foreach (var info in compareResult.WordGroupsInfos)
            {
                PdfTagPattern pattern = info.GetPdfTagPattern();
                pattern.SourceTypeName = "WordGroupsInfos";

                if (store.PdfPatterns.IndexOf(pattern) ==-1)
                    store.PdfPatterns.Add(pattern);
            }

            foreach (var info in compareResult.LinesInfos)
            {
                PdfTagPattern pattern = info.GetPdfTagPattern();
                pattern.SourceTypeName = "LinesInfos";

                if (store.PdfPatterns.IndexOf(pattern) == -1)
                    store.PdfPatterns.Add(pattern);
            }

            foreach (var info in compareResult.PdfTextInfos)
            {
                PdfTagPattern pattern = info.GetPdfTagPattern();
                pattern.SourceTypeName = "PdfTextInfos";

                if (store.PdfPatterns.IndexOf(pattern) == -1)
                    store.PdfPatterns.Add(pattern);
            }

            List<PdfTagPattern> usedPatterns = new List<PdfTagPattern>();

            foreach (var info in compareResult.ColorFontWordGroupsInfos)
            {
                PdfTagPattern pattern = info.GetPdfTagPattern();
                pattern.SourceTypeName = "ColorFontWordGroupsInfos";

                if (store.PdfPatterns.IndexOf(pattern) == -1)
                    store.PdfPatterns.Add(pattern);
                else
                {   // Intentamos quitar los falsos positivos.
                    if (!usedPatterns.Contains(pattern))
                        usedPatterns.Add(pattern);
                }
            }

            // Rutina de eliminación de patrones que tengan más de una aparición. (Quita los falsos positivos).
            foreach (PdfTagPattern usedPattern in usedPatterns)
            {
                store.PdfPatterns.Remove(usedPattern);
            }

            return store;

        }

        /// <summary>
        /// Recupera un archivo de almacén de patrones.
        /// </summary>
        /// <param name="path">Ruta al archivo.</param>
        /// <returns>Almacén de patrones.</returns>
        private static PdfTagPatternStore GetStore(string path)
        {
            return XmlParser.FromXml<PdfTagPatternStore>(path);
        }
  
        /// <summary>
        /// Actualiza un almacén de patrones.
        /// </summary>
        /// <param name="compareResult">Resultado de una comparación.</param>
        /// <param name="path">Ruta del store.</param>
        private static void Update(PdfCompareResult compareResult, string path)
        {

            // Si el archivo de patrones existe, lo actualizo con los nuevos
            // resultados (inserto nuevos, elimina sobrantes poco efectivos, y
            // actualizo matchCount.


            PdfTagPatternStore originalStore = GetStore(path);
            PdfTagPatternStore currentStore = GetStore(compareResult);

            List<PdfTagPattern> newPdfPatterns = new List<PdfTagPattern>();

            foreach (var tagPattern in currentStore.PdfPatterns)
            {
                int indexOriginal = originalStore.PdfPatterns.IndexOf(tagPattern);

                if (indexOriginal == -1)
                    newPdfPatterns.Add(tagPattern); 
                else
                    originalStore.PdfPatterns[indexOriginal].MatchesCount++;
            }

            int originalCount = originalStore.PdfPatterns.Count;
            int available = Settings.Current.MaxPatternCount -
                originalCount - newPdfPatterns.Count;

            if (available < 0)
                for (int i = originalCount - 1; 
                    (i >= (originalCount + available)) && i >= 0; i--)
                    originalStore.PdfPatterns.RemoveAt(i);

            originalStore.PdfPatterns.AddRange(newPdfPatterns);
            originalStore.CompareCount++;

            originalStore.PdfPatterns.Sort();
            XmlParser.SaveAsXml(originalStore, path);

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Guarda patrones obtenidos de una oparación de comparación
        /// determinada.
        /// </summary>
        /// <param name="compareResult">Resultado de una comparación.</param>
        public static void Save(PdfCompareResult compareResult)
        {

            // Si el archivo de patrones no existe creo un nuevo

            string file = GetPath(compareResult);

            if (!File.Exists(file))
                Create(compareResult, file);
            else
                Update(compareResult, file);

        } 

        /// <summary>
        /// Recupera un archivo de almacén de patrones.
        /// </summary>
        /// <param name="pdf">Documento.</param>
        /// <returns>Almacén de patrones.</returns>
        public static PdfTagPatternStore GetStore(PdfUnstructuredDoc pdf)
        {

            string path = $"C:\\ProgramData\\PdfTagger\\Patterns\\InvoicePdfTaggerOriginalModif\\" +
                $"{GetFileName(pdf.DocID)}";

            //string path = $"{GetDirectory(pdf.DocCategory)}" +
            //$"{GetFileName(pdf.DocID)}";

            return XmlParser.FromXml<PdfTagPatternStore>(path);

        }
        #endregion



    }
}
