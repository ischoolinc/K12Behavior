using System.Collections.Generic;
using K12.Data;

namespace K12.缺曠獎懲週報表
{
    internal class SeatNoComparer : IComparer<string>
    {
        private Dictionary<string, StudentRecord> _mapping;

        public SeatNoComparer(Dictionary<string, StudentRecord> mapping)
        {
            _mapping = mapping;
        }

        #region IComparer<string> 成員

        public int Compare(string x, string y)
        {
            StudentRecord X = _mapping[x];
            StudentRecord Y = _mapping[y];

            int intX = X.SeatNo.HasValue ? X.SeatNo.Value : 0;
            int intY = Y.SeatNo.HasValue ? Y.SeatNo.Value : 0;
            return intX.CompareTo(intY);
        }

        #endregion
    }
}
