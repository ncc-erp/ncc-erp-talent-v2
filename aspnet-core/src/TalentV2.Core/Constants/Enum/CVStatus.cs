﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Constants.Enum
{
    public enum CVStatus : byte
    {
        New = 0,

        Contacting = 1,

        Passed = 2,

        Failed = 10,

        Draft = 20
    }
}