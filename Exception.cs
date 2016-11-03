using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fabio.SharpTools
{
    [Serializable]
    public class Exception : System.Exception
    {
        public Exception() : base() { }
        public Exception(string message) : base(message) { }
    }
}
