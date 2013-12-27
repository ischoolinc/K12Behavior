using System.Collections.Generic;
using K12.Data;

namespace K12.ClassMeritDemerit.Detail
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

            int intX = X.SeatNo.HasValue ? X.SeatNo.Value : 99999;
            int intY = Y.SeatNo.HasValue ? Y.SeatNo.Value : 99999;

            return intX.CompareTo(intY);
        }

        #endregion
    }
}
