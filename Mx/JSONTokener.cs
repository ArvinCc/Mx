using System;
using System.IO;
using System.Text;

namespace Mx
{
    namespace Json
    {
        public class JSONTokener
        {
            private int a;

            private StringReader b;

            private char c;

            private bool d;

            public JSONTokener(StringReader reader)
            {
                this.b = reader;
                this.d = false;
                this.a = 0;
            }

            public JSONTokener(string s)
                : this(new StringReader(s))
            {
            }


            public void back()
            {
                if (this.d || this.a <= 0)
                {
                    throw new JSONException("Stepping back two steps is not supported");
                }
                this.a--;
                this.d = true;
            }


            public static int dehexchar(char c)
            {
                if (c >= '0' && c <= '9')
                {
                    return (int)(c - '0');
                }
                if (c >= 'A' && c <= 'F')
                {
                    return (int)(c - '7');
                }
                if (c >= 'a' && c <= 'f')
                {
                    return (int)(c - 'W');
                }
                return -1;
            }

            public bool more()
            {
                if (this.next() == '\0')
                {
                    return false;
                }
                this.back();
                return true;
            }

            public char next()
            {
                if (this.d)
                {
                    this.d = false;
                    if (this.c != '\0')
                    {
                        this.a++;
                    }
                    return this.c;
                }
                int num;
                try
                {
                    num = this.b.Read();
                }
                catch (IOException ex)
                {
                    throw new JSONException("IOException;" + ex.Message);
                }
                if (num <= 0)
                {
                    this.c = '\0';
                    return '\0';
                }
                this.a++;
                this.c = (char)num;
                return this.c;
            }

            public char next(char c)
            {
                char c2 = this.next();
                if (c2 != c)
                {
                    throw this.syntaxError(string.Concat(new object[]
				{
					"Expected '",
					c,
					"' and instead saw '",
					c2,
					"'"
				}));
                }
                return c2;
            }

            public string next(int n)
            {
                if (n == 0)
                {
                    return "";
                }
                char[] array = new char[n];
                int num = 0;
                if (this.d)
                {
                    this.d = false;
                    array[0] = this.c;
                    num = 1;
                }
                try
                {
                    int num2;
                    while (num < n && (num2 = this.b.Read(array, num, n - num)) != -1)
                    {
                        num += num2;
                    }
                }
                catch (IOException ex)
                {
                    throw new JSONException("IOException:" + ex.Message);
                }
                this.a += num;
                if (num < n)
                {
                    throw this.syntaxError("Substring bounds error");
                }
                this.c = array[n - 1];
                return new string(array);
            }

            public char nextClean()
            {
                char c = this.next();
                if (c == '\0' || c > ' ')
                {
                    return c;
                }
                return c;
            }

            public string nextString(char quote)
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (true)
                {
                    char c = this.next();
                    char c2 = c;
                    if (c2 <= '\n')
                    {
                        if (c2 == '\0' || c2 == '\n')
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (c2 == '\r')
                        {
                            break;
                        }
                        if (c2 == '\\')
                        {
                            c = this.next();
                            char c3 = c;
                            if (c3 <= '\\')
                            {
                                if (c3 <= '\'')
                                {
                                    if (c3 != '"' && c3 != '\'')
                                    {
                                        goto Block_9;
                                    }
                                }
                                else if (c3 != '/' && c3 != '\\')
                                {
                                    goto Block_11;
                                }
                                stringBuilder.Append(c);
                                continue;
                            }
                            if (c3 <= 'f')
                            {
                                if (c3 == 'b')
                                {
                                    stringBuilder.Append('\b');
                                    continue;
                                }
                                if (c3 != 'f')
                                {
                                    goto Block_14;
                                }
                                stringBuilder.Append('\f');
                                continue;
                            }
                            else
                            {
                                if (c3 != 'n')
                                {
                                    switch (c3)
                                    {
                                        case 'r':
                                            stringBuilder.Append('\r');
                                            continue;
                                        case 't':
                                            stringBuilder.Append('\t');
                                            continue;
                                        case 'u':
                                            stringBuilder.Append((char)Convert.ToInt32(this.next(4), 16));
                                            continue;
                                    }
                                    goto Block_16;
                                }
                                stringBuilder.Append('\n');
                                continue;
                            }
                        }
                    }
                    if (c == quote)
                    {
                        goto Block_17;
                    }
                    stringBuilder.Append(c);
                }
                throw this.syntaxError("Unterminated string");
            Block_9:
            Block_11:
            Block_14:
            Block_16:
                throw this.syntaxError("Illegal escape.");
            Block_17:
                return stringBuilder.ToString();
            }

            public string nextTo(char d)
            {
                StringBuilder stringBuilder = new StringBuilder();
                char c;
                while (true)
                {
                    c = this.next();
                    if (c == d || c == '\0' || c == '\n' || c == '\r')
                    {
                        break;
                    }
                    stringBuilder.Append(c);
                }
                if (c != '\0')
                {
                    this.back();
                }
                return stringBuilder.ToString().Trim();
            }

            public string nextTo(string delimiters)
            {
                StringBuilder stringBuilder = new StringBuilder();
                char c;
                while (true)
                {
                    c = this.next();
                    if (delimiters.IndexOf(c) >= 0 || c == '\0' || c == '\n' || c == '\r')
                    {
                        break;
                    }
                    stringBuilder.Append(c);
                }
                if (c != '\0')
                {
                    this.back();
                }
                return stringBuilder.ToString().Trim();
            }

            public object nextValue()
            {
                char c = this.nextClean();
                char c2 = c;
                if (c2 <= '(')
                {
                    if (c2 != '"')
                    {
                        switch (c2)
                        {
                            case '\'':
                                break;
                            case '(':
                                goto IL_47;
                            default:
                                goto IL_54;
                        }
                    }
                    return this.nextString(c);
                }
                if (c2 != '[')
                {
                    if (c2 != '{')
                    {
                        goto IL_54;
                    }
                    this.back();
                    return new JSONObject(this);
                }
            IL_47:
                this.back();
                return new JSONArray(this);
            IL_54:
                StringBuilder stringBuilder = new StringBuilder();
                while (c >= ' ' && ",:]}/\\\"[{;=#".IndexOf(c) < 0)
                {
                    stringBuilder.Append(c);
                    c = this.next();
                }
                this.back();
                string text = stringBuilder.ToString().Trim();
                if (text.Equals(""))
                {
                    throw this.syntaxError("Missing value");
                }
                return JSONObject.stringToValue(text);
            }

            public char skipTo(char to)
            {
                char c;
                try
                {
                    int num = this.a;
                    while (true)
                    {
                        c = this.next();
                        if (c == '\0')
                        {
                            break;
                        }
                        if (c == to)
                        {
                            goto Block_3;
                        }
                    }
                    this.a = num;
                    return c;
                Block_3: ;
                }
                catch (IOException ex)
                {
                    throw new JSONException("IOException:" + ex.Message);
                }
                this.back();
                return c;
            }

            public JSONException syntaxError(string message)
            {
                return new JSONException(message + this.toString());
            }

            public string toString()
            {
                return " at character " + this.a;
            }
        }
    }
}
