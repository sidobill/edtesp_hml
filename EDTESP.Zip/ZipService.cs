using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Zip
{
    public class ZipService
    {
        public MemoryStream AddFilesToZip(List<string> listPathFiles)
        {
            var zipFile = new ZipFile();

            foreach (var item in listPathFiles)
            {
                if (File.Exists(item))
                    zipFile.AddFile(item, "Boletos");
            }

            if (zipFile.Count > 0)
            {
                var memoryStream = new MemoryStream();

                zipFile.Save(memoryStream);

                zipFile.Dispose();

                return memoryStream;
            }

            return null;
        }
    }
}
