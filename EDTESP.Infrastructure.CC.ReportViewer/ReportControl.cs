using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reporting.WebForms;

namespace EDTESP.Infrastructure.CC.ReportViewer
{
    public class ReportControl
    {
        private static Microsoft.Reporting.WebForms.ReportViewer DefineReport(string rdlcpath, string rptname, Dictionary<string, IEnumerable> ds = null,
            Dictionary<string, string> prms = null)
        {
            var rpt = new Microsoft.Reporting.WebForms.ReportViewer
            {
                SizeToReportContent = true,
                ProcessingMode = ProcessingMode.Local,
                ZoomMode = ZoomMode.FullPage,
                ShowExportControls = false,
                LocalReport =
                {
                    ReportPath = rdlcpath,
                    EnableExternalImages = true,
                }
            };

            if (ds != null && ds.ToList().Any())
                foreach (var d in ds)
                    rpt.LocalReport.DataSources.Add(new ReportDataSource(d.Key, d.Value));

            if (prms != null && prms.Any())
            {
                var pms = prms.Select(x => new ReportParameter(x.Key, x.Value)).ToList();
                rpt.LocalReport.SetParameters(pms);
            }

            rpt.LocalReport.DisplayName = rptname;

            return rpt;
        }

        public static byte[] GeneratePdf(string rdlcpath, string rptname, Dictionary<string, IEnumerable> ds = null,
            Dictionary<string, string> prms = null)
        {
            var rpt = DefineReport(rdlcpath, rptname, ds, prms);
            return new PrintReport().Export2Pdf(rpt.LocalReport);
        }

        public static void GeneratePdf(string filename, string rdlcpath, string rptname, Dictionary<string, IEnumerable> ds = null,
            Dictionary<string, string> prms = null)
        {
            var rpt = DefineReport(rdlcpath, rptname, ds, prms);
            new PrintReport().Export2Pdf(rpt.LocalReport,filename);
        }
    }
}