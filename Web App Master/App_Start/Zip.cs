using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace Compression
{
    public class Zip
    {
        public static bool FromFile(string dest, List<string> files, bool mapserver=false)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var name = file;
                            if (mapserver)
                            {                                
                                name = HttpContext.Current.Server.MapPath(file);
                            }
                            archive.CreateEntryFromFile(file, Path.GetFileName(name));
                        }
                     
                    }
                    if (mapserver)
                    {
                        dest = HttpContext.Current.Server.MapPath(dest);
                    }
                    using (var fileStream = new FileStream(dest, FileMode.Create))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}