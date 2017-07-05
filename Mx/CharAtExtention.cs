using System;

namespace Mx
{
    namespace Json
    {
        public static class CharAtExtention
        {
            public static string CharAt(this string s, int index)
            {
                if (index >= s.Length || index < 0)
                {
                    return "";
                }
                return s.Substring(index, 1);
            }
        }
    }
}
