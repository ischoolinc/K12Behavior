using System;
using System.Collections.Generic;
using System.Text;

namespace K12.Behavior
{
    public class QueryRequestAttribute : FISCA.Authentication.AutoRetryOnWebExceptionAttribute
    {
        private int _ReTryTimes = 3;
        public int ReTryTimes
        {
            get { return _ReTryTimes; }
        }
        public QueryRequestAttribute() { }
        public QueryRequestAttribute(int retryTimes) 
        {
            _ReTryTimes = retryTimes;
        }
    }
}
