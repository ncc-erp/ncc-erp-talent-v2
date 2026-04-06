using System;
using System.IO;
using System.Net;

namespace TalentV2.Utils
{
    public static class FileNameHelper
    {
        public static string Sanitize(string fileName)
        {
            var safeFileName = WebUtility.UrlDecode(Path.GetFileName(fileName ?? string.Empty));
            safeFileName = safeFileName.Replace(" ", "_");
            safeFileName = string.Join("_", safeFileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

            return string.IsNullOrWhiteSpace(safeFileName) ? "file" : safeFileName;
        }
    }
}
