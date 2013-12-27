using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.獎懲統計表
{
    public static class Utility
    {
        public static string ToStr(this int Value)
        {
            return Value > 0 ? ""+Value : string.Empty;
        }
    }
}