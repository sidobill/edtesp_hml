using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Reporting.WebForms;

namespace EDTESP.Infrastructure.CC.ReportViewer
{
    public class PrintReport
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        public void Export(LocalReport report)
        {
            var deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>21.1cm</PageWidth>
                <PageHeight>29.8cm</PageHeight>
                <MarginTop>0.5cm</MarginTop>
                <MarginLeft>0.5cm</MarginLeft>
                <MarginRight>0.5cm</MarginRight>
                <MarginBottom>0.5cm</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        public byte[] Export2Pdf(LocalReport rpt)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = rpt.Render(
                "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            return bytes;
        }
        
        public void Export2Pdf(LocalReport rpt, string filename)
        {
            var bytes = Export2Pdf(rpt);

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        public void Print(string printerName, LocalReport rpt)
        {
            Export(rpt);

            if (m_streams == null || m_streams.Count == 0)
                throw new NullReferenceException();

            var printSet = new PrinterSettings()
            {
                PrinterName = printerName
            };
            var printDoc = new PrintDocument();

            printDoc.PrinterSettings = printSet;

            var paperSizes = printDoc.PrinterSettings.PaperSizes.Cast<PaperSize>();

            var sizeA4 = paperSizes.First(size => size.Kind == PaperKind.A4); // setting paper size to A4 size

            printDoc.DefaultPageSettings.PaperSize = sizeA4;
            printSet.DefaultPageSettings.PaperSize = sizeA4;
            //printSet.DefaultPageSettings.Landscape = true;


            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new InvalidPrinterException(printDoc.PrinterSettings);
            }


            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            m_currentPageIndex = 0;
            printDoc.Print();

        }

        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
    }
}
