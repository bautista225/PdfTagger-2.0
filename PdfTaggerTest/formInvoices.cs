using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfTagger.Dat.Met.Bus;
using PdfTagger.Pat;
using PdfTagger.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfTaggerTest
{
    public partial class formInvoices : Form
    {

        formInvoiceModel _Model;

        public formInvoices()
        {
            InitializeComponent();
            _Model = new formInvoiceModel();            
        }

        private void mnMainOpen_Click(object sender, EventArgs e)
        {
            Open();
            FillDataWordGroups();
            FillDataLines();
        }

        /// <summary>
        /// Abre el archivo pdf.
        /// </summary>
        private void Open()
        {
            dlgOpen.ShowDialog();
            if (File.Exists(dlgOpen.FileName))
            {
                ClearView();
                _Model.LoadPdfInvoiceDoc(dlgOpen.FileName);
                ShowInBrowser(_Model.PdfPath);
            }
        }

        private void FillDataWordGroups()
        {

            grdDataWords.Rows.Clear();

            foreach (var page in _Model.Pdf.PdfUnstructuredPages)
                foreach (var word in page.WordGroups)
                    grdDataWords.Rows.Add(_Model.Pdf.PdfUnstructuredPages.IndexOf(page)+1, word.Text);
        }

        private void FillDataLines()
        {

            grdDataLines.Rows.Clear();

            foreach (var page in _Model.Pdf.PdfUnstructuredPages)
                foreach (var line in page.Lines)
                    grdDataLines.Rows.Add(_Model.Pdf.PdfUnstructuredPages.IndexOf(page)+1, line.Text);
        }

        /// <summary>
        /// Carga 
        /// </summary>
        /// <param name="pdfPath">Ruta pdf.</param>
        private void ShowInBrowser(string pdfPath)
        {
            if (string.IsNullOrEmpty(pdfPath))
            {
                wBr.Visible = false;         
            }
            else
            {
                wBr.Visible = true;
                wBr.Navigate(pdfPath);
            }
        }

        private InvoiceMetadata GetMetadataFromView()
        {
            //ExtraerDatosCSV();

            InvoiceMetadata invoice = new InvoiceMetadata();

            NumberFormatInfo numFormatInfo = new NumberFormatInfo();
            numFormatInfo.NumberDecimalSeparator = ",";
            NumberFormatInfo numFormatInfo2 = new NumberFormatInfo();
            numFormatInfo2.NumberDecimalSeparator = ".";


            foreach (PropertyInfo pInf in invoice.GetType().GetProperties())
            {

                Control[] ctlrs = Controls.Find($"tx{pInf.Name}", true);

                if (ctlrs.Length != 0)
                {
                    TextBox txb = (TextBox)ctlrs[0];

                    if (!string.IsNullOrEmpty(txb.Text))
                    {

                        object pValue = null;

                        if (txb.TextAlign == HorizontalAlignment.Right)
                            try
                            {
                                pValue = System.Convert.ToDecimal(txb.Text, numFormatInfo);
                            }
                            catch
                            {
                                pValue = System.Convert.ToDecimal(txb.Text, numFormatInfo2);
                            }
                        else
                            pValue = Convert(pInf.PropertyType, txb.Text);

                        pInf.SetValue(invoice, pValue);
                    }
                }
                
            }

            return invoice;
        }

        private void GetViewFromMetadata()
        {

            if (_Model.Invoice == null)
                return;

            InvoiceMetadata invoice = _Model.Invoice;

            NumberFormatInfo numFormatInfo = new NumberFormatInfo();
            numFormatInfo.NumberDecimalSeparator = ".";


            foreach (PropertyInfo pInf in invoice.GetType().GetProperties())
            {

                Control[] ctlrs = Controls.Find($"tx{pInf.Name}", true);

                if (ctlrs.Length != 0 && pInf.Name != "SellerPartyID")
                {
                    TextBox txb = (TextBox)ctlrs[0];

                    object pValue = pInf.GetValue(invoice);

                    if (pValue != null)
                    {
                        if (pInf.PropertyType == typeof(decimal) &&
                            System.Convert.ToDecimal(pValue) != 0)
                            txb.Text = ((decimal)pValue).ToString(numFormatInfo);
                        else if (pInf.PropertyType == typeof(DateTime?) && pValue != null)
                            txb.Text = ((DateTime)pValue).ToString("dd/MM/yyyy");
                        else
                            txb.Text = $"{pValue}";
                    }
                      
                    
                }

            }

        }

        private void ClearView()
        {

            foreach (PropertyInfo pInf in typeof(InvoiceMetadata).GetProperties())
            {
                Control[] ctlrs = Controls.Find($"tx{pInf.Name}", true);

                if (ctlrs.Length != 0)
                    ((TextBox)ctlrs[0]).Text="";
            }
           
        }

        private object Convert(Type type, string text)
        {
            try
            {

                var converter = TypeDescriptor.GetConverter(type);

                if (converter != null)
                    return converter.ConvertFromString(text);

                return Activator.CreateInstance(type); 
            }
            catch (NotSupportedException)
            {
                return Activator.CreateInstance(type);
            }
        }

        private void LoadPreView(bool lines = false)
        {

            if(string.IsNullOrEmpty(_Model.PdfPath))
            {
                MessageBox.Show("Debe abrir un pdf.");
                return;
            }

            if (_Model.Pdf==null)
            {
                MessageBox.Show("Debe abrir un pdf.");
                return;
            }

            string fileSufix = (lines) ? "_ln" : "_wg";

            string fileName = $"{Path.GetDirectoryName(_Model.PdfPath)}{Path.DirectorySeparatorChar}";
            fileName += $"{Path.GetFileNameWithoutExtension(_Model.PdfPath)}{fileSufix}";
            fileName += $"{Path.GetExtension(_Model.PdfPath)}";

            BaseColor color = (lines) ? BaseColor.BLUE : BaseColor.RED; 

            Util.PrintRectangles(_Model.PdfPath, fileName, _Model.Pdf, color, lines);

            wBr.Navigate(fileName);
        }

        private void formInvoices_Shown(object sender, EventArgs e)
        {
            sPnH.SplitterDistance = Height - mnMain.Height - 
                tbMain.Height - stMain.Height - 90;
        }

        private void txSellerPartyID_Validated(object sender, EventArgs e)
        {
            if (_Model.Pdf != null && !String.IsNullOrEmpty(txSellerPartyID.Text))
                new Thread(Render).Start();
        }

        private void Render()
        {
            if (_Model.Pdf != null && !String.IsNullOrEmpty(txSellerPartyID.Text))
            {
                _Model.Pdf.DocID = txSellerPartyID.Text;
                _Model.Extract();

                Invoke(new Action(()=> {
                    GetViewFromMetadata();
                    LoadPatternStore();
                }));                
            }
        }

        private void LoadPatternStore()
        {

            grdPatternStore.Rows.Clear();

            if (_Model.Store == null)
                return;

            tbPatternStore.Text = $"PatternStore({ _Model.Store.PdfPatterns.Count})";

            _Model.Store.PdfPatterns.Sort();

            foreach (var patt in _Model.Store.PdfPatterns)
            {
                int index = grdPatternStore.Rows.Add(patt.MetadataItemName,
                  patt.PdfPageN, patt.PdfRectangle, patt.MatchesCount,
                  patt.RegexPattern, patt, "", patt.IsLastPage, 
                  patt.SourceTypeName, patt.Position, patt.FillColor??" ", patt.StrokeColor??" ", patt.FontName, patt.FontSize);
            }
        }

        private void ExecuteLearning()
        {            
            _Model.Invoice = GetMetadataFromView();

            /*if (string.IsNullOrEmpty(_Model.Invoice.InvoiceNumber))
            {
                MessageBox.Show("InvoiceNumber no válido.");
                return;
            }

            if (string.IsNullOrEmpty(_Model.Invoice.BuyerPartyID))
            {
                MessageBox.Show("BuyerPartyID no válido.");
                return;
            }

            if (_Model.Invoice.IssueDate == null)
            {
                MessageBox.Show("IssueDate no válido.");
                return;
            }

            if(_Model.Invoice.GrossAmount == 0)
            {
                MessageBox.Show("GrossAmount no válido.");
                return;
            }

            decimal cuadre = _Model.Invoice.GrossAmount - _Model.Invoice.TaxesOutputsBase01 -
                _Model.Invoice.TaxesOutputsBase02 - _Model.Invoice.TaxesOutputsBase03 -
                _Model.Invoice.TaxesOutputsBase04 - _Model.Invoice.TaxesOutputsAmount01 -
                _Model.Invoice.TaxesOutputsAmount02 - _Model.Invoice.TaxesOutputsAmount03 -
                _Model.Invoice.TaxesOutputsAmount04 + _Model.Invoice.TaxesWithholdingAmount01;

            if (cuadre != 0)
            {
                MessageBox.Show("Cuadre importes no válido.");
                return;
            }*/

            _Model.ExecutePatternsLearning();
        }

        private void mnMainAnalyze_Click(object sender, EventArgs e)
        {
            ExecuteLearning();
        }

        private void mnMainViewWordGroups_Click(object sender, EventArgs e)
        {
            LoadPreView();
        }

        private void mnMainViewLines_Click(object sender, EventArgs e)
        {
            LoadPreView(true);
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox next = GetNextText(sender as TextBox);
                if (next != null)
                {
                    next.Focus();
                    next.SelectAll();
                }
            }

            if (e.KeyCode == Keys.Escape)
                (sender as TextBox).Text = "";

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                e.Handled = e.SuppressKeyPress = true;

        }

        private TextBox GetNextText(TextBox txb)
        {
            foreach (var ctrl in tbInvoice.Controls)
            {
                TextBox nxt = ctrl as TextBox;

                if (nxt != null && nxt.TabIndex == txb.TabIndex + 1)
                    return nxt;
            }

            return null;

        }

        private void ctxMnTxViewPatterns_Click(object sender, EventArgs e)
        {

            if(_Model.Store==null)
            {
                MessageBox.Show("Almacén de patrones no seleccionado.");
                return;
            }

            ToolStripMenuItem mnItem = (sender as ToolStripMenuItem);
            ContextMenuStrip ctx = (ContextMenuStrip)mnItem.GetCurrentParent();
            TextBox txb = (TextBox)ctx.SourceControl;

            string itemName = txb.Name;

            itemName = itemName.Substring(2, itemName.Length - 2);

            var patterns = _Model.Store.GetPdfPatterns("WordGroupsInfos", itemName);

            string fileSufix = $"_{itemName}_wg";

            string fileName = $"{Path.GetDirectoryName(_Model.PdfPath)}{Path.DirectorySeparatorChar}";
            fileName += $"{Path.GetFileNameWithoutExtension(_Model.PdfPath)}{fileSufix}";
            fileName += $"{Path.GetExtension(_Model.PdfPath)}";
            

            Util.PrintRectangles(_Model.PdfPath, fileName, patterns);

            wBr.Navigate(fileName);

        }

        private void LoadResultPatterns(object sender, EventArgs e)
        {

            TextBox txb = (sender as TextBox);

            string metaDataItemName = txb.Name.Substring(2, txb.Name.Length-2);

            _Model.LoadWordGroupFromStore(metaDataItemName, "WordGroupsInfos");
            _Model.LoadWordGroupFromStore(metaDataItemName, "PdfTextInfos");
            _Model.LoadWordGroupFromStore(metaDataItemName, "ColorFontWordGroupsInfos");

            FillGrid(grdWordGroups, _Model.WordGroupsFiltered, "WordGroupsInfos");
            FillGrid(grdPdfText, _Model.PdfTextInfosFiltered, "PdfTextInfos");
            FillGrid(grdColorFontWordGroups, _Model.ColorFontWordGroupsFiltered, "ColorFontWordGroupsInfos");

            if(_Model.WordGroupsFiltered!=null)
                tbWordGroups.Text = $"WordGroups {metaDataItemName}({_Model.WordGroupsFiltered.Count})";

            if (_Model.PdfTextInfosFiltered != null)
                tbPdfText.Text = $"PdfText {metaDataItemName}({_Model.PdfTextInfosFiltered.Count})";

            if (_Model.ColorFontWordGroupsFiltered != null)
                tbColorFontWordGroups.Text = $"CFWG {metaDataItemName}({_Model.ColorFontWordGroupsFiltered.Count}";

            Refresh();

        }

        private void FillGrid(DataGridView grd, List<PdfTagPattern> patterns, string sourceTypeName)
        {
            grd.Rows.Clear();

            if (patterns == null)
                return;
            if (sourceTypeName.Equals("ColorFontWordGroupsInfos"))
                foreach (var patt in patterns)
                {
                    int index = grd.Rows.Add(patt.MetadataItemName,
                        patt.PdfPageN, patt.PdfRectangle, patt.MatchesCount,
                        patt.RegexPattern, patt, patt.FillColor ?? " ",
                        patt.StrokeColor ?? " ", patt.FontName, patt.FontSize, patt.CFType);


                    if (IsInResults(patt, out object value))
                    {
                        grd.Rows[index].DefaultCellStyle.BackColor = Color.Green;
                        grd.Rows[index].Cells[11].Value = value;
                    }
                }
            else
                foreach (var patt in patterns)
                {
                    int index = grd.Rows.Add(patt.MetadataItemName,
                        patt.PdfPageN, patt.PdfRectangle, patt.MatchesCount,
                        patt.RegexPattern, patt);


                    if (IsInResults(patt, out object value))
                    {
                        grd.Rows[index].DefaultCellStyle.BackColor = Color.Green;
                        grd.Rows[index].Cells[6].Value = value;
                    }
                }
        }

        private bool IsInResults(PdfTagPattern patt, out object value)
        {

            value = null;

            if (_Model.ExtractionResult == null)
                return false;

            if (_Model.ExtractionResult.Results.ContainsKey(patt.MetadataItemName))
            {
                foreach (var pattResult in _Model.ExtractionResult.Results[patt.MetadataItemName])
                {
                    if (pattResult.Pattern.Equals(patt))
                    {
                        value = pattResult.Value;
                        return true;
                    }
                }
            }

            return false;

        }

        private bool ExistsInPdf(object sender, EventArgs e)
        {

            TextBox txb = (TextBox)sender;

            foreach (var page in _Model.Pdf.PdfUnstructuredPages)
                foreach (var line in page.Lines)
                    if (line.Text.IndexOf(txb.Text) != -1)
                        return true;
            return false;

        }

        private void text_Validated(object sender, EventArgs e)
        {

            TextBox txb = (TextBox)sender;

            if (string.IsNullOrEmpty(txb.Text))
                return;

            if (!ExistsInPdf(sender, e))
                txb.BackColor = Color.Red;
            else
                txb.BackColor = Color.White;

        }

        private void analizarFicherosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS PRUEBA ERRORCOUNTS\");//Assuming Test is your Folder
            FileInfo[] Files = directory.GetFiles("*.pdf"); //Getting Text files

            int contador = 0;
            foreach (FileInfo file in Files)
            {
                var isInTxt = false;
                var txt = File.ReadAllText(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\datosFacturas.csv");

                var lines = txt.Split('\n');

                foreach (var line in lines)
                {
                    try
                    {
                        var textGroup = line.Split(';');
                        //var filePath = _Model.PdfPath.Split('\\');
                        string[] fileAux = file.Name.Split('.');
                        int filename = int.Parse(fileAux[0]);



                        if (textGroup[0].Equals(filename.ToString()))
                        {
                            isInTxt = true;
                            contador++;
                            Console.WriteLine("Mirando el fichero número: " + contador + " de " + Files.Length + " con nombre : " + file.Name + " a la hora: " + DateTime.Now.ToString("h:mm:ss"));

                            // Compruebo si el fichero pdf tiene muchas páginas.
                            try
                            {
                                PdfReader reader = new PdfReader(file.FullName);
                                if (reader.NumberOfPages > 10)
                                {
                                    reader.Close();
                                    Console.WriteLine("Es un fichero con muchas páginas");
                                    Console.WriteLine("Fichero ya visitado a la hora: " + DateTime.Now.ToString("h:mm:ss") + "\n\n\n");
                                    File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EJEMPLO DE MUCHAS PÁGINAS\" + file.Name);

                                }
                                else
                                {
                                    reader.Close();
                                    try
                                    {
                                        RecorrerRuta(file.FullName);
                                        Console.WriteLine("Fichero ya visitado a la hora: " + DateTime.Now.ToString("h:mm:ss") + "\n\n\n");
                                        File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS VISITADAS ERRORCOUNTS\" + file.Name);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                                        File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);
                                    }

                                }
                            }
                            catch
                            {
                                Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                                File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);

                            }
                            break;
                        }
                    }
                    catch
                    {

                    }
                }
                if (!isInTxt)
                    File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);

                Console.WriteLine("Empieza Garbage Collector\n");
                System.GC.Collect();

                System.GC.WaitForPendingFinalizers();
                Console.WriteLine("Finaliza Garbage Collector\n\n\n");
            }
        }

        private void analizarListaFicherosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\Users\Juan Bautista\Documents\FACTURAS EJEMPLO\FACTURAS EJEMPLO VISITADAS\");//Assuming Test is your Folder
            FileInfo[] Files = directory.GetFiles("*.pdf"); //Getting Text files

            int contador = 0;
            int contadorFiles = 0;
            foreach (FileInfo file in Files)
            {
                var txt = File.ReadAllText(@"C:\Users\Juan Bautista\Downloads\ficherosConcretos.txt");

                var lines = txt.Split('\n');
                contadorFiles++;
                Console.WriteLine("Fichero núm: " + contadorFiles + " de " + Files.Length);
                foreach (var line in lines)
                {
                    string[] fileAux = file.Name.Split('.');
                    int filename = int.Parse(fileAux[0]);
                    string[] lineAux = line.Split('\r');

                    //Console.WriteLine(line);
                    if (lineAux[0].Equals(filename.ToString()))
                    {
                        contador++;
                        Console.WriteLine("Mirando el fichero número: " + contador + " de " + Files.Length + " con nombre : " + file.Name + " a la hora: " + DateTime.Now.ToString("h:mm:ss"));
                        RecorrerRuta(file.FullName);
                        Console.WriteLine("Fichero ya visitado a la hora: " + DateTime.Now.ToString("h:mm:ss") + "\n\n\n");
                    }
                }
                Console.WriteLine("Empieza GC");
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Console.WriteLine("Finaliza GC\n\n\n");
            }
            Console.WriteLine("He termiando");
        }

        /// <summary>
        /// Recorre y ejecuta, para cada pdf, el aprendizaje.
        /// </summary>
        /// <param name="path"></param>
        public void RecorrerRuta(string path)
        {
            ClearView();
            _Model.LoadPdfInvoiceDoc(path);
            ExecuteLearning();
        }

        /// <summary>
        /// Extraer los datos del CSV en vez de la vista.
        /// </summary>
        /// <param name="path"></param>
        public Dictionary<string,string> ExtraerDatosCSV(bool allInfo = true)
        {
            var txt = File.ReadAllText(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\jFras.csv");

            var lines = txt.Split('\n');

            string[] data;
            Dictionary<string, string> invoiceMetadata = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                var textGroup = line.Split(';');
                var filePath = _Model.PdfPath.Split('\\');
                string[] file = filePath[filePath.Length - 1].Split('.');
                int filename = int.Parse(file[0]);

                if (textGroup[0].Equals(filename.ToString()))
                {
                    data = textGroup;
                    txSellerPartyID.Text = data[7];
                    _Model.Pdf.DocID = txSellerPartyID.Text;
                    if (allInfo)
                    {
                        txBuyerPartyID.Text = data[5];
                        txInvoiceNumber.Text = data[10];
                        txIssueDate.Text = data[9];
                        txGrossAmount.Text = data[12];
                        txCurrencyCode.Text = data[13];
                    }
                    else
                    {
                        invoiceMetadata.Add("BuyerPartyID", data[5]);
                        invoiceMetadata.Add("InvoiceNumber", data[10]);
                        invoiceMetadata.Add("IssueDate", data[9]);
                        invoiceMetadata.Add("GrossAmount", data[12]);
                        invoiceMetadata.Add("CurrencyCode", data[13]);
                    }
                }
            }
            return invoiceMetadata;
        }

        /// <summary>
        /// Extrae la información del PDF y la almacena en un CSV.
        /// </summary>
        private void GetCsvFromMetadata()
        {

            if (_Model.Invoice == null)
                return;

            InvoiceMetadata invoice = _Model.Invoice;
            Dictionary<string, string> data = new Dictionary<string, string>();
            NumberFormatInfo numFormatInfo = new NumberFormatInfo();
            numFormatInfo.NumberDecimalSeparator = ",";
            string issueDate = null;
            string grossAmount = null;

            foreach (PropertyInfo pInf in invoice.GetType().GetProperties())
            {
                if (pInf.Name == "InvoiceNumber" ||
                    pInf.Name == "SellerPartyID" ||
                    pInf.Name == "BuyerPartyID" ||
                    pInf.Name == "CurrencyCode")
                {
                    object pValue = pInf.GetValue(invoice);
                    var text = $"{pValue}";
                    data.Add(pInf.Name, text);
                }
                else if (pInf.Name == "IssueDate")
                {
                    if (pInf.GetValue(invoice) != null)
                        issueDate = ((DateTime)pInf.GetValue(invoice)).ToString("dd/MM/yyyy");
                }
                else if (pInf.Name == "GrossAmount")
                {
                    if (System.Convert.ToDecimal(pInf.GetValue(invoice)) != 0)
                        grossAmount = ((decimal)pInf.GetValue(invoice)).ToString(numFormatInfo);
                }
            }

            string[] file = _Model.PdfPath.Split('\\');
            string nameFile = file[file.Length - 1];

            string line = string.Format("{0};{1};{2};{3};{4};{5};{6}\n",
            data["SellerPartyID"], data["BuyerPartyID"], data["InvoiceNumber"],
            issueDate, grossAmount, data["CurrencyCode"], nameFile);

            string path = @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\2.0-datosPdfTagger2Modif.csv";

            File.AppendAllText(path, line);
        }

        private void extraerDatosFicherosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS PRUEBA ERRORCOUNTS\");//Assuming Test is your Folder
            FileInfo[] Files = directory.GetFiles("*.pdf"); //Getting Text files

            int contador = 0;
            foreach (FileInfo file in Files)
            {
                var txt = File.ReadAllText(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\datosFacturas.csv");

                var lines = txt.Split('\n');

                foreach (var line in lines)
                {
                    try
                    {
                        var textGroup = line.Split(';');
                        string[] fileAux = file.Name.Split('.');
                        int filename = int.Parse(fileAux[0]);

                        if (textGroup[0].Equals(filename.ToString()))
                        {
                            contador++;
                            Console.WriteLine("Mirando el fichero número: " + contador + " de " + Files.Length + " con nombre : " + file.Name + " a la hora: " + DateTime.Now.ToString("h:mm:ss"));

                            try
                            {
                            ExtraerPdfACsv(file.FullName);

                            Console.WriteLine("Fichero ya visitado a la hora: " + DateTime.Now.ToString("h:mm:ss") + "\n\n\n");
                            File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS VISITADAS ERRORCOUNTS\" + file.Name);
                            }
                            catch
                            {
                                Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                                File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);
                            }

                            break;
                        }
                    }
                    catch {
                        Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                        File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);
                    }
                }

                Console.WriteLine("Empieza GC");
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Console.WriteLine("Finaliza GC\n\n\n");
            }
        }

        /// <summary>
        /// Llamadas a métodos para obtener el CSV
        /// </summary>
        /// <param name="path">Ruta del PDF donde se extrae la información.</param>
        private void ExtraerPdfACsv(string path)
        {
            ClearView();
            _Model.LoadPdfInvoiceDoc(path);

            ExtraerDatosCSV(false);
            _Model.Extract();

            GetCsvFromMetadata();
        }


        private void ExtraerPdfACsvConCheck(string path)
        {
            Dictionary<string, string> invoiceMetadata = new Dictionary<string, string>();

            ClearView();
            _Model.LoadPdfInvoiceDoc(path);

            invoiceMetadata = ExtraerDatosCSV(false);

            _Model.ExtractWithCheck(invoiceMetadata);
            
        }

        /// <summary>
        /// Extraemos los datos de los PDFs y comprobamos la exatitud del texto introducido en los patrones.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extraerYComprobarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS PRUEBA ERRORCOUNTS\");
            FileInfo[] Files = directory.GetFiles("*.pdf"); //Getting PDF files

            int contador = 0;
            foreach (FileInfo file in Files)
            {
                var txt = File.ReadAllText(@"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\datosFacturas.csv");

                var lines = txt.Split('\n');

                foreach (var line in lines)
                {
                    try
                    {
                        var textGroup = line.Split(';');
                        string[] fileAux = file.Name.Split('.');
                        int filename = int.Parse(fileAux[0]);

                        if (textGroup[0].Equals(filename.ToString()))
                        {
                            contador++;
                            Console.WriteLine("Mirando el fichero número: " + contador + " de " + Files.Length + " con nombre : " + file.Name + " a la hora: " + DateTime.Now.ToString("h:mm:ss"));

                            try
                            {
                                ExtraerPdfACsvConCheck(file.FullName);

                                Console.WriteLine("Fichero ya visitado a la hora: " + DateTime.Now.ToString("h:mm:ss") + "\n\n\n");
                                File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS VISITADAS ERRORCOUNTS\" + file.Name);
                            }
                            catch
                            {
                                Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                                File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);
                            }

                            break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("El fichero ha dado una excepción\n\n\n");
                        File.Move(file.FullName, @"C:\Users\Juan Bautista\Documents\Util\FicherosPdfTagger\FACTURAS EXCEPCIÓN\" + file.Name);
                    }
                }

                Console.WriteLine("Empieza GC");
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Console.WriteLine("Finaliza GC\n\n\n");
            }
        }
    }
}
