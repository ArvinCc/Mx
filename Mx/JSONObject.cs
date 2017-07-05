using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mx
{

    namespace Json
    {
        public class JSONObject
        {
            public class Null
            {
                protected object clone()
                {
                    return this;
                }

                public bool equals(object obj)
                {
                    return obj == null || obj == this;
                }

                public string toString()
                {
                    return "null";
                }
            }

            public static JSONObject.Null NULL = new JSONObject.Null();

            [CompilerGenerated]
            private Map<string, object> a;

            private Map<string, object> map
            {
                get;
                set;
            }

            public JSONObject()
            {
                this.map = new Map<string, object>();
            }

            public JSONObject(Map<string, object> map)
            {
                this.map = ((map == null) ? new Map<string, object>() : map);
            }

            public JSONObject(string s)
                : this(new JSONTokener(s.Replace(" ", "")))
            {
            }

            public JSONObject(JSONTokener x)
                : this()
            {
                if (x.nextClean() != '{')
                {
                    throw x.syntaxError("A JSONObject text must begin with '{'");
                }
                char c3;
                while (true)
                {
                    char c = x.nextClean();
                    char c2 = c;
                    if (c2 == '\0')
                    {
                        break;
                    }
                    if (c2 == '}')
                    {
                        return;
                    }
                    x.back();
                    string key = x.nextValue().ToString();
                    c = x.nextClean();
                    if (c != ':')
                    {
                        goto Block_4;
                    }
                    this.putOnce(key, x.nextValue());
                    c3 = x.nextClean();
                    if (c3 != ',' && c3 != ';')
                    {
                        goto Block_6;
                    }
                    if (x.nextClean() == '}')
                    {
                        return;
                    }
                    x.back();
                }
                throw x.syntaxError("A JSONObject text must end with '}'");
            Block_4:
                throw x.syntaxError("Expected a ':' after a key");
            Block_6:
                if (c3 != '}')
                {
                    throw x.syntaxError("Expected a ',' or '}'");
                }
                return;
            }

            public static string quote(string str)
            {
                if (str == null || str.Length == 0)
                {
                    return "\"\"";
                }
                char c = '\0';
                int length = str.Length;
                StringBuilder stringBuilder = new StringBuilder(length + 4);
                char[] array = str.ToCharArray();
                stringBuilder.Append('"');
                int i = 0;
                while (i < length)
                {
                    char c2 = c;
                    c = array[i];
                    char c3 = c;
                    if (c3 <= '"')
                    {
                        switch (c3)
                        {
                            case '\b':
                                stringBuilder.Append("\\b");
                                break;
                            case '\t':
                                stringBuilder.Append("\\t");
                                break;
                            case '\n':
                                stringBuilder.Append("\\n");
                                break;
                            case '\v':
                                goto IL_F9;
                            case '\f':
                                stringBuilder.Append("\\f");
                                break;
                            case '\r':
                                stringBuilder.Append("\\r");
                                break;
                            default:
                                if (c3 != '"')
                                {
                                    goto IL_F9;
                                }
                                goto IL_84;
                        }
                    }
                    else if (c3 != '/')
                    {
                        if (c3 == '\\')
                        {
                            goto IL_84;
                        }
                        goto IL_F9;
                    }
                    else
                    {
                        if (c2 == '<')
                        {
                            stringBuilder.Append('\\');
                        }
                        stringBuilder.Append(c);
                    }
                IL_11A:
                    i++;
                    continue;
                IL_84:
                    stringBuilder.Append('\\');
                    stringBuilder.Append(c);
                    goto IL_11A;
                IL_F9:
                    if (c >= ' ' && (c < '?' || c >= '?') && (c < '?' || c >= '?'))
                    {
                        stringBuilder.Append(c);
                        goto IL_11A;
                    }
                    goto IL_11A;
                }
                stringBuilder.Append('"');
                return stringBuilder.ToString();
            }

            public static StringWriter quote(string str, StringWriter w)
            {
                if (str == null || str.Length == 0)
                {
                    w.Write("\"\"");
                    return w;
                }
                char c = '\0';
                int length = str.Length;
                w.Write('"');
                int i = 0;
                while (i < length)
                {
                    char c2 = c;
                    c = CharAtExtention.CharAt(str, i).ToCharArray()[0];
                    //c = str.CharAt(i).ToCharArray()[0];
                    char c3 = c;
                    if (c3 <= '"')
                    {
                        switch (c3)
                        {
                            case '\b':
                                w.Write("\\b");
                                break;
                            case '\t':
                                w.Write("\\t");
                                break;
                            case '\n':
                                w.Write("\\n");
                                break;
                            case '\v':
                                goto IL_FA;
                            case '\f':
                                w.Write("\\f");
                                break;
                            case '\r':
                                w.Write("\\r");
                                break;
                            default:
                                if (c3 != '"')
                                {
                                    goto IL_FA;
                                }
                                goto IL_83;
                        }
                    }
                    else if (c3 != '/')
                    {
                        if (c3 == '\\')
                        {
                            goto IL_83;
                        }
                        goto IL_FA;
                    }
                    else
                    {
                        if (c2 == '<')
                        {
                            w.Write('\\');
                        }
                        w.Write(c);
                    }
                IL_160:
                    i++;
                    continue;
                IL_83:
                    w.Write('\\');
                    w.Write(c);
                    goto IL_160;
                IL_FA:
                    if (c < ' ' || (c >= '\u0080' && c < '\u00a0') || (c >= '\u2000' && c < '℀'))
                    {
                        w.Write("\\u");
                        string text = c.ToString();
                        w.Write("0000", 0, 4 - text.Length);
                        w.Write(text);
                        goto IL_160;
                    }
                    w.Write(c);
                    goto IL_160;
                }
                w.Write('"');
                return w;
            }

            public JSONObject putOnce(string key, object value)
            {
                if (key != null && value != null)
                {
                    if (this.opt(key) != null)
                    {
                        throw new JSONException("Duplicate key \"" + key + "\"");
                    }
                    this.put(key, value);
                }
                return this;
            }

            public object opt(string key)
            {
                if (key != null)
                {
                    return this.map.get(key);
                }
                return null;
            }

            public static string nextString(char[] quote)
            {
                StringBuilder stringBuilder = new StringBuilder();
                int num = 0;
                char c2;
                while (true)
                {
                    char c = quote[num];
                    num++;
                    c2 = c;
                    if (c2 <= '\n')
                    {
                        break;
                    }
                    if (c2 == '\r')
                    {
                        goto IL_34;
                    }
                    if (c2 != '\\')
                    {
                        goto Block_5;
                    }
                    c = quote[num];
                    num++;
                    switch (c)
                    {
                        case 'b':
                            stringBuilder.Append('\b');
                            continue;
                        case 'f':
                            stringBuilder.Append('\f');
                            continue;
                        case 'n':
                            stringBuilder.Append('\n');
                            continue;
                        case 'r':
                            stringBuilder.Append('\r');
                            continue;
                        case 't':
                            stringBuilder.Append('\t');
                            continue;
                        case 'u':
                        case 'x':
                            continue;
                    }
                    stringBuilder.Append(c);
                }
                if (c2 == '\0' || c2 == '\n')
                {
                    goto IL_34;
                }
            Block_5:
                goto IL_104;
            IL_34:
                throw new Exception("Unterminated string");
            IL_104:
                return stringBuilder.ToString();
            }

            public int getInt(string key)
            {
                object obj = this.get(key);
                int result;
                try
                {
                    result = ((obj is int) ? ((int)obj) : Convert.ToInt32((string)obj));
                }
                catch (Exception)
                {
                    throw new JSONException("JSONObject[" + JSONObject.quote(key) + "] is not an int.");
                }
                return result;
            }

            public string getString(string key)
            {
                return this.get(key).ToString();
            }

            public JSONArray getJSONArray(string key)
            {
                object obj = this.get(key);
                if (obj is JSONArray)
                {
                    return (JSONArray)obj;
                }
                throw new JSONException("JSONObject[" + JSONObject.quote(key) + "] is not a JSONArray.");
            }

            public object get(string key)
            {
                object obj = this.opt(key);
                if (obj == null)
                {
                    throw new JSONException("JSONObject[" + JSONObject.quote(key) + "] not found.");
                }
                return obj;
            }

            public static object stringToValue(string s)
            {
                if (s.Equals(""))
                {
                    return s;
                }
                //char c = s.CharAt(0).ToCharArray()[0];
                char c = CharAtExtention.CharAt(s, 0).ToCharArray()[0];
                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+')
                {
                    if (c == '0')
                    {
                        if (s.Length > 2)
                        {
                            if (CharAtExtention.CharAt(s, 1).ToCharArray()[0] != 'x')
                            {
                                if (CharAtExtention.CharAt(s, 1).ToCharArray()[0] != 'X')
                                {
                                    goto IL_85;
                                }
                            }
                            try
                            {
                                object result = Convert.ToInt32(s.Substring(2), 16);
                                return result;
                            }
                            catch (Exception)
                            {
                                goto IL_97;
                            }
                        }
                    IL_85:
                        try
                        {
                            object result = Convert.ToInt32(s, 8);
                            return result;
                        }
                        catch (Exception)
                        {
                        }
                    }
                IL_97:
                    try
                    {
                        object result;
                        if (s.IndexOf('.') > -1 || s.IndexOf('e') > -1 || s.IndexOf('E') > -1)
                        {
                            result = double.Parse(s);
                            return result;
                        }
                        result = Convert.ToInt32(s);
                        return result;
                    }
                    catch (Exception)
                    {
                    }
                    return s;
                }
                return s;
            }

            public static string valueToString(object value)
            {
                if (value == null || value.Equals(null))
                {
                    return "null";
                }
                if (value is JSONString)
                {
                    object obj;
                    try
                    {
                        obj = ((JSONString)value).toJSONString();
                    }
                    catch (Exception ex)
                    {
                        throw new JSONException(ex.Message);
                    }
                    if (obj is string)
                    {
                        return (string)obj;
                    }
                    throw new JSONException("Bad value from toJSONString: " + obj);
                }
                else
                {
                    if (value is int)
                    {
                        return JSONObject.numberToString((int)value);
                    }
                    if (value is bool)
                    {
                        return value.ToString();
                    }
                    if (value is JSONObject)
                    {
                        return ((JSONObject)value).ToString();
                    }
                    if (value is JSONArray)
                    {
                        return ((JSONArray)value).ToString();
                    }
                    return JSONObject.quote(value.ToString());
                }
            }

            public static string numberToString(int n)
            {
                string text = n.ToString();
                if (text.IndexOf('.') > 0 && text.IndexOf('e') < 0 && text.IndexOf('E') < 0)
                {
                    while (text.EndsWith("0"))
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                    if (text.EndsWith("."))
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                }
                return text;
            }

            public int length()
            {
                return this.map.getSize();
            }

            public static void indent(StringWriter writer, int indent)
            {
                for (int i = 0; i < indent; i++)
                {
                    writer.Write(' ');
                }
            }

            public static StringWriter writeValue(StringWriter writer, object value, int indentFactor, int indent)
            {
                if (value == null || value.Equals(null))
                {
                    writer.Write("null");
                }
                else if (value is JSONObject)
                {
                    ((JSONObject)value).write(writer, indentFactor, indent);
                }
                else if (value is JSONArray)
                {
                    ((JSONArray)value).write(writer, indentFactor, indent);
                }
                else if (value is int)
                {
                    writer.Write(JSONObject.numberToString((int)value));
                }
                else if (value is bool)
                {
                    writer.Write(value.ToString());
                }
                else if (value is JSONString)
                {
                    object obj;
                    try
                    {
                        obj = ((JSONString)value).toJSONString();
                    }
                    catch (Exception ex)
                    {
                        throw new JSONException(ex.Message);
                    }
                    writer.Write((obj != null) ? obj.ToString() : JSONObject.quote(value.ToString()));
                }
                else
                {
                    JSONObject.quote(value.ToString(), writer);
                }
                return writer;
            }

            public StringWriter write(StringWriter writer, int indentFactor, int inden)
            {
                try
                {
                    bool flag = false;
                    int num = this.length();
                    writer.Write('{');
                    if (num == 1)
                    {
                        List<object> list = new List<object>(this.map.Values);
                        object obj = list[0];
                        writer.Write(JSONObject.quote(obj.ToString()));
                        writer.Write(':');
                        if (indentFactor > 0)
                        {
                            writer.Write(' ');
                        }
                        JSONObject.writeValue(writer, this.map.get(obj.ToString()), indentFactor, inden);
                    }
                    else if (num != 0)
                    {
                        int indent = inden + indentFactor;
                        foreach (KeyValuePair<string, object> current in this.map)
                        {
                            object key = current.Key;
                            if (flag)
                            {
                                writer.Write(',');
                            }
                            if (indentFactor > 0)
                            {
                                writer.Write('\n');
                            }
                            JSONObject.indent(writer, indent);
                            writer.Write(JSONObject.quote(key.ToString()));
                            writer.Write(':');
                            if (indentFactor > 0)
                            {
                                writer.Write(' ');
                            }
                            JSONObject.writeValue(writer, this.map.get(key.ToString()), indentFactor, indent);
                            flag = true;
                        }
                        if (indentFactor > 0)
                        {
                            writer.Write('\n');
                        }
                        JSONObject.indent(writer, inden);
                    }
                    writer.Write('}');
                }
                catch (IOException ex)
                {
                    throw new JSONException(ex.Message);
                }
                return writer;
            }

            public JSONObject put(string key, object value)
            {
                if (key == null)
                {
                    throw new JSONException("Null key.");
                }
                if (value != null)
                {
                    this.map.put(key, value);
                }
                else
                {
                    this.remove(key);
                }
                return this;
            }

            public void remove(string key)
            {
                this.map.remove(key);
            }

            public List<string> getKeys()
            {
                return this.map.getKeys();
            }

            public List<object> getValues()
            {
                return this.map.getValues();
            }

            public override string ToString()
            {
                try
                {
                    string text = "{";
                    foreach (KeyValuePair<string, object> current in this.map)
                    {
                        if (text.Length > 1)
                        {
                            text += ',';
                        }
                        text += JSONObject.quote(current.Key.ToString());
                        text += ':';
                        text += JSONObject.valueToString(this.map.get(current.Key));
                    }
                    text += '}';
                    return text;
                }
                catch (Exception)
                {
                }
                return null;
            }

        }

    }

}
