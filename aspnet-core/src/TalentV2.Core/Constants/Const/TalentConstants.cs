using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Constants.Const
{
    public static class TalentConstants
    {
        public const float LAUNCH_ALLOWANCE = 800000;
        public const string PROJECT_NAME = "talent";
        public const long MEGA_BYTE = 1024 * 1024;
        public static string UploadFileProvider { get; set; }
        public static string[] AllowImageFileTypes { get; set; }
        public static readonly string AmazoneS3 = "AWS";
        public static readonly string InternalUploadFile = "InternalUploadFile";
        public static string BaseBEAddress { get; set; }
        public static string BaseFEAddress { get; set; }
        public static string PublicClientRootAddress { get; set; }
        public static long MaxSizeFile { get; set; }
        public static string LMSClientRootAddress { get; set; }
        public static int MAX_SCORE = 5;
    }
}
