using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Excel
{
    public class ExcelService
    {
        public MemoryStream GerarExcel(DataSet dataSet)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Style.Font.FontSize = 14;

                wb.Worksheets.Add(dataSet);
                

                var stream = new MemoryStream();

                wb.SaveAs(stream);

                return stream;
            }
        }
    }
}
