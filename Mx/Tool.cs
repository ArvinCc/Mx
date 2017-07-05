using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mx
{
    namespace Utils
    {
        class Tool
        {
            public static string StringHandler(string msg)
            {
                Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
                var ss = reg.Replace(msg, delegate(Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
                return ss;
            }

            /// <summary>
            /// 生成一个指定的随机数,混合字母和数字
            /// </summary>
            /// <param name="length"></param>
            /// <returns></returns>
            public static string GetRandomId(int length)
            {
                string val = "";

                System.Random random = new System.Random();

                for (int i = 0; i < length; i++)
                {
                    string charOrNum = random.Next(2) % 2 == 0 ? "char" : "num";

                    if (charOrNum.Equals("char"))
                    {
                        int choice = random.Next(2) % 2 == 0 ? 65 : 97;

                        val += (char)(choice + random.Next(26));

                    }
                    else if (charOrNum.Equals("num"))
                    {
                        val += Convert.ToString(random.Next(10));
                    }
                }
                val = val.ToLowerInvariant();

                return val;
            }

            /// <summary>
            /// 生成一个指定的随机数列表,混合字母和数字
            /// </summary>
            /// <param name="length"></param>
            /// <returns></returns>
            public static List<string> GetRandomId(int length, int num)
            {
                List<string> id = new List<string>();

                for (int j = 0; j < num; j++)
                {
                    string val = "";

                    System.Random random = new System.Random();

                    for (int i = 0; i < length; i++)
                    {
                        string charOrNum = random.Next(2) % 2 == 0 ? "char" : "num";

                        if (charOrNum.Equals("char"))
                        {
                            int choice = random.Next(2) % 2 == 0 ? 65 : 97;

                            val += (char)(choice + random.Next(26));

                        }
                        else if (charOrNum.Equals("num"))
                        {
                            val += Convert.ToString(random.Next(10));
                        }
                    }
                    val = val.ToLowerInvariant();

                    if (id.Contains(val))
                    {
                        continue;
                    }
                    else
                    {
                        id.Add(val);
                    }
                }
                return id;
            }
        }
    }
}
