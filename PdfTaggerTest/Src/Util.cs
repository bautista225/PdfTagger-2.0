using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfTagger.Pat;
using PdfTagger.Pdf;
using System.Collections.Generic;
using System.IO;

namespace PdfTaggerTest
{
    public class Util
    {

        /// <summary>
        /// Dibuja los rectángulos configurados en un pdf.
        /// </summary>
        /// <param name="pathTarget">Pdf destino.</param>
        /// <param name="baseColor">Color de los rectangulos.</param>
        public static void PrintRectangles(string pathSource, string pathTarget, 
            PdfUnstructuredDoc pdf, BaseColor baseColor, bool lines = false)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pathSource);

                PdfStamper pdfStamper = new PdfStamper(pdfReader,
                      new FileStream(pathTarget, FileMode.OpenOrCreate));


                int p = 0;

                foreach (var page in pdf.PdfUnstructuredPages)
                {
                    p++;
                    PdfContentByte cb = pdfStamper.GetOverContent(p);

                    List<PdfTextRectangle> rectangles = (lines) ? page.Lines : page.WordGroups;

                    foreach (var reg in rectangles)
                    {
                        cb.SetColorStroke(baseColor);

                        iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(
                            reg.Llx, reg.Lly, reg.Urx, reg.Ury);

                        cb.Rectangle(rect.Left, rect.Bottom,
                            rect.Width, rect.Height);
                        cb.Stroke();
                    }

                }
                pdfStamper.Close();

            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (DocumentException ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Dibuja los rectángulos configurados en un pdf invertidos x=y,
        /// y=x, x=width-x ... Rutina utilizada en casos extraños como el de
        /// Moinsa.
        /// </summary>
        /// <param name="pathTarget">Pdf destino.</param>
        /// <param name="baseColor">Color de los rectangulos.</param>
        public static void PrintInvertRectangles(string pathSource, string pathTarget,
            PdfUnstructuredDoc pdf, BaseColor baseColor, bool lines = false)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pathSource);

                PdfStamper pdfStamper = new PdfStamper(pdfReader,
                      new FileStream(pathTarget, FileMode.OpenOrCreate));


                int p = 0;

                foreach (var page in pdf.PdfUnstructuredPages)
                {
                    p++;
                    PdfContentByte cb = pdfStamper.GetOverContent(p);

                    List<PdfTextRectangle> rectangles = (lines) ? page.Lines : page.WordGroups;

                    foreach (var reg in rectangles)
                    {
                        cb.SetColorStroke(baseColor);

                        Rectangle pageSize = pdfReader.GetPageSize(p);

                        iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(
                            (pageSize.Height - reg.Lly), reg.Llx, (pageSize.Height - reg.Ury),  reg.Urx);

                        cb.Rectangle(rect.Left, rect.Bottom,
                            rect.Width, rect.Height);
                        cb.Stroke();
                    }

                }
                pdfStamper.Close();

            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (DocumentException ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Dibuja los rectángulos configurados en un pdf.
        /// </summary>
        /// <param name="pathTarget">Pdf destino.</param>
        /// <param name="baseColor">Color de los rectangulos.</param>
        public static void PrintRectangles(string pathSource, string pathTarget,
            List<PdfTagPattern> patterns)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pathSource);

                PdfStamper pdfStamper = new PdfStamper(pdfReader,
                      new FileStream(pathTarget, FileMode.OpenOrCreate));

                int maxPage = 1;

                foreach (var patt in patterns)
                    maxPage = (maxPage < patt.PdfPageN) ? patt.PdfPageN : maxPage;

                if (maxPage > pdfReader.NumberOfPages)
                    maxPage = pdfReader.NumberOfPages;


                for (int p = 1; p <= maxPage ; p++)
                {

                    PdfContentByte cb = pdfStamper.GetOverContent(p);
                    int count = 0;

                    foreach (var patt in patterns)
                    {

                        if (patt.PdfPageN == p)
                        {

                            BaseColor color = BaseColor.GREEN;

                            if(count==1)
                                color = BaseColor.ORANGE;

                            if (count == 2)
                                color = BaseColor.RED;

                            cb.SetColorStroke(color);

                            var reg = patt.PdfRectangle;

                            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(
                                reg.Llx, reg.Lly, reg.Urx, reg.Ury);

                            cb.Rectangle(rect.Left, rect.Bottom,
                                rect.Width, rect.Height);
                            cb.Stroke();

                            if (count == 2)
                                break;

                            count++;
                        }
                    }

                }
                pdfStamper.Close();

            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (DocumentException ex)
            {
                throw ex;
            }

        }

    }
}
