using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mx
{
    namespace Json
    {
        public class JSONArray
        {
            private List<object> a;

            public JSONArray()
            {
                this.a = new List<object>();
            }

            public JSONArray(JSONTokener x)
                : this()
            {
                if (x.nextClean() != '[')
                {
                    throw x.syntaxError("A JSONArray text must start with '['");
                }
                if (x.nextClean() != ']')
                {
                    x.back();
                    char c;
                    while (true)
                    {
                        if (x.nextClean() == ',')
                        {
                            x.back();
                            this.a.Add(JSONObject.NULL);
                        }
                        else
                        {
                            x.back();
                            this.a.Add(x.nextValue());
                        }
                        c = x.nextClean();
                        if (c != ',')
                        {
                            break;
                        }
                        if (x.nextClean() == ']')
                        {
                            return;
                        }
                        x.back();
                    }
                    if (c != ']')
                    {
                        throw x.syntaxError("Expected a ',' or ']'");
                    }
                    return;
                }
            }

            public JSONArray(string source)

                : this(new JSONTokener(source.Replace(" ", "")))
            {
            }

            public JSONArray getJSONArray(int index)
            {
                object obj = this.get(index);
                if (obj is JSONArray)
                {
                    return (JSONArray)obj;
                }
                throw new JSONException("JSONArray[" + index + "] is not a JSONArray.");
            }

            public JSONObject getJSONObject(int index)
            {
                object obj = this.get(index);
                if (obj is JSONObject)
                {
                    return (JSONObject)obj;
                }
                throw new JSONException("JSONArray[" + index + "] is not a JSONObject.");
            }

            public string getString(int index)
            {
                object obj = this.get(index);
                if (obj is string)
                {
                    return (string)obj;
                }
                throw new JSONException("JSONArray[" + index + "] not a string.");
            }

            public bool isNull(int index)
            {
                return JSONObject.NULL.equals(this.opt(index));
            }

            public object get(int index)
            {
                object obj = this.opt(index);
                if (obj == null)
                {
                    throw new JSONException("JSONArray[" + index + "] not found.");
                }
                return obj;
            }

            public int length()
            {
                return this.a.Count;
            }

            public List<object> getList()
            {
                return this.a;
            }

            public object opt(int index)
            {
                if (index >= 0 && index < this.length())
                {
                    return this.a[index];
                }
                return null;
            }

            public JSONArray optJSONArray(int index)
            {
                object obj = this.opt(index);
                if (!(obj is JSONArray))
                {
                    return null;
                }
                return (JSONArray)obj;
            }

            public JSONObject optJSONObject(int index)
            {
                object obj = this.opt(index);
                if (!(obj is JSONObject))
                {
                    return null;
                }
                return (JSONObject)obj;
            }

            public string optString(int index)
            {
                return this.optString(index, "");
            }

            public string optString(int index, string defaultValue)
            {
                object obj = this.opt(index);
                if (!JSONObject.NULL.equals(obj))
                {
                    return obj.ToString();
                }
                return defaultValue;
            }

            public JSONArray put(bool value)
            {
                this.put(value);
                return this;
            }

            public JSONArray put(object value)
            {
                this.a.Add(value);
                return this;
            }

            public bool remove(int index)
            {
                if (index >= 0 && index < this.length())
                {
                    this.a.RemoveAt(index);
                    return true;
                }
                return false;
            }

            public string join(string separator)
            {
                int num = this.length();
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < num; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(separator);
                    }
                    stringBuilder.Append(JSONObject.valueToString(this.a[i]));
                }
                return stringBuilder.ToString();
            }

            public JSONObject toJSONObject(JSONArray names)
            {
                if (names == null || names.length() == 0 || this.length() == 0)
                {
                    return null;
                }
                JSONObject jSONObject = new JSONObject();
                for (int i = 0; i < names.length(); i++)
                {
                    jSONObject.put(names.getString(i), this.opt(i));
                }
                return jSONObject;
            }

            public string toString(int indentFactor)
            {
                StringWriter writer = new StringWriter();
                return this.write(writer, indentFactor, 0).ToString();
            }

            public StringWriter write(StringWriter writer)
            {
                return this.write(writer, 0, 0);
            }

            public StringWriter write(StringWriter writer, int indentFactor, int indent)
            {
                try
                {
                    bool flag = false;
                    int num = this.length();
                    writer.Write('[');
                    if (num == 1)
                    {
                        JSONObject.writeValue(writer, this.a[0], indentFactor, indent);
                    }
                    else if (num != 0)
                    {
                        int indent2 = indent + indentFactor;
                        for (int i = 0; i < num; i++)
                        {
                            if (flag)
                            {
                                writer.Write(',');
                            }
                            if (indentFactor > 0)
                            {
                                writer.Write('\n');
                            }
                            JSONObject.indent(writer, indent2);
                            JSONObject.writeValue(writer, this.a[i], indentFactor, indent2);
                            flag = true;
                        }
                        if (indentFactor > 0)
                        {
                            writer.Write('\n');
                        }
                        JSONObject.indent(writer, indent);
                    }
                    writer.Write(']');
                }
                catch (IOException ex)
                {
                    throw new JSONException(ex.Message);
                }
                return writer;
            }

            public override string ToString()
            {
                string result;
                try
                {
                    result = this.toString(0);
                }
                catch (Exception)
                {
                    result = null;
                }
                return result;
            }
        }

    }
 
}
