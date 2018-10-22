using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdateLib.Utils
{
    public class RequestResult<T>
    {
        public bool Success => Exception == null;
        public Exception Exception { get; set; }
        public T Result { get; set; }

        public RequestResult()
        {
        }

        public RequestResult(T result)
        {
            Result = result;
        }

        public RequestResult(Exception exception)
        {
            Exception = exception;
        }
    }
}
