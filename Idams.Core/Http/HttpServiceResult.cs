using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Http
{
    public class HttpServiceResult<T>
    {
        public bool IsSuccessful { get; }
        public bool IsFailure => !IsSuccessful;
        public string ErrorDescription { get; }
        public T Value { get; }
        public string ErrorCode { get; }
        public int HttpStatusCode { get; }

        internal HttpServiceResult(T value, bool isSuccessful, string errorDescription, string errorCode, int httpStatusCode)
        {
            Value = value;
            IsSuccessful = isSuccessful;
            ErrorDescription = errorDescription;
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
        }

        public static HttpServiceResult<T> Ok(T value, int httpStatusCode)
        {
            return new HttpServiceResult<T>(value, true, null, null, httpStatusCode);
        }

        public static HttpServiceResult<T> Fail(string errorDescription, string errorCode, int httpStatusCode)
        {
            return new HttpServiceResult<T>(default(T), false, errorDescription, errorCode, httpStatusCode);
        }
    }
}
