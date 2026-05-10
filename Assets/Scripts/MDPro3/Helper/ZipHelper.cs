using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using MDPro3.UI;

namespace MDPro3
{
    public class ZipHelper
    {
        public static List<ZipFile> zips = new();

        public static void Initialize()
        {
            Dispose();

            if (Config.GetBool("Expansions", true))
            {
                if (!Directory.Exists("Expansions"))
                    Directory.CreateDirectory("Expansions");
                foreach (var zip in Directory.GetFiles("Expansions", "*.ypk"))
                    zips.Add(new ZipFile(zip));
                foreach (var zip in Directory.GetFiles("Expansions", "*.zip"))
                    zips.Add(new ZipFile(zip));
            }

            zips.Add(new ZipFile(Program.PATH_DATA + Program.SCRIPT_ZIP));//Make "Data/script.zip" the last one to read.


        }

        public static void Dispose()
        {
            foreach (var zip in zips)
                zip.Dispose();
            zips.Clear();
        }

        public static List<string> GetAllCdbTempPath()
        {
            var returnValue = new List<string>();
            foreach (var zip in zips)
            {
                if (zip.Name.ToLower().EndsWith(Program.SCRIPT_ZIP))
                    continue;
                foreach (var file in zip.EntryFileNames)
                {
                    if (file.ToLower().EndsWith(".cdb"))
                    {
                        var e = zip[file];
                        if (!Directory.Exists(Program.PATH_TEMP_FOLDER))
                            Directory.CreateDirectory(Program.PATH_TEMP_FOLDER);
                        var tempFile = Path.Combine(Path.GetFullPath(Program.PATH_TEMP_FOLDER), file);
                        e.Extract(Path.GetFullPath(Program.PATH_TEMP_FOLDER), ExtractExistingFileAction.OverwriteSilently);
                        returnValue.Add(tempFile);
                    }
                }
            }
            return returnValue;
        }

        public static List<string> GetAllTextByFileName(string fileName, bool isScript)
        {
            var returnValue = new List<string>();
            foreach (var zip in zips)
            {
                var isScriptZip = zip.Name.ToLower().EndsWith(Program.SCRIPT_ZIP);
                if (isScript != isScriptZip)
                    continue;
                if (zip.ContainsEntry(fileName))
                {
                    var e = zip[fileName];
                    using var sr = new StreamReader(e.OpenReader());
                    returnValue.Add(sr.ReadToEnd());
                }
            }
            return returnValue;
        }

        public static List<string> GetAllTextByExtension(string extension, bool isScript)
        {
            var result = new List<string>();
            var ext = extension.Trim().TrimStart('.').ToLowerInvariant();

            foreach (var zip in zips)
            {
                var isScriptZip = zip.Name.ToLower().EndsWith(Program.SCRIPT_ZIP);
                if (isScript != isScriptZip)
                    continue;
                foreach (var entry in zip.Entries)
                {
                    if (entry.IsDirectory) continue;
                    var entryExt = Path.GetExtension(entry.FileName).TrimStart('.').ToLowerInvariant();
                    if (entryExt != ext) continue;
                    using var sr = new StreamReader(entry.OpenReader());
                    result.Add(sr.ReadToEnd());
                }
            }
            return result;
        }
    }
}
