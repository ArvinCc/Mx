using System;

namespace Mx
{
    namespace Json
    {
        public class JSONException : ApplicationException
        {
            public JSONException()
            {
            }

            public JSONException(string message)
            {
                Console.WriteLine("JSONException=" + message);
            }

            public JSONException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
