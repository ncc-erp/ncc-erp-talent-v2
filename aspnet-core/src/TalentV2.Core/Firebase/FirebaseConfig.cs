using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Firebase
{
    public class FirebaseConfig
    {

        public int IntervalMilisecond { get; set; }
        public bool RunFirebaseBackgroundService { get; set; }
        public string SecretKey { get; set; }
        public string Url { get; set; }
    }
}
