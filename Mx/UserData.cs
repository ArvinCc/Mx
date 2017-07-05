using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mx.Json;

namespace Mx
{
    namespace Server
    {
        public class User
        {
            public string name;

            public string password;

            public string phoneNumber;
        }


        public class UserData : DataHandler
        {
            private static volatile UserData instance;
            private static readonly object syncRoot = new object();
            private UserData() { }

            public static UserData Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (syncRoot)
                        {
                            if (instance == null)
                            {
                                instance = new UserData();
                            }
                        }
                    }
                    return instance;
                }
            }

            public string SignUp(string name, string password, string phoneNumber)
            {

                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_SIGN_UP));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("name", name);
                jsonObject.put("password", password);
                jsonObject.put("phoneNumber", phoneNumber);

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }


            public string SignIn(string name, string password, string phoneNumber)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_SIGN_IN));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("name", name);
                jsonObject.put("password", password);
                jsonObject.put("phoneNumber", phoneNumber);

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }

            public string DownloadFile(string state, string name, string number)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_PRODUCT));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("state", state);
                jsonObject.put("name", name);
                jsonObject.put("number", number);

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }

            /// <summary>
            /// 封装信息包
            /// </summary>
            /// <param name="pkgId"></param>
            /// <param name="content"></param>
            /// <returns></returns>
            public byte[] MsgPkg(string pkgId, byte[] content)
            {
                byte[] id = Encoding.UTF8.GetBytes(pkgId);

                string length = "" + content.Length;
                length = length.PadRight(8);

                byte[] contentLength = Encoding.UTF8.GetBytes(length);


                byte[] bytes = new byte[id.Length + contentLength.Length + content.Length];

                Buffer.BlockCopy(id, 0, bytes, 0, id.Length);
                Buffer.BlockCopy(contentLength, 0, bytes, id.Length, contentLength.Length);
                Buffer.BlockCopy(content, 0, bytes, id.Length + contentLength.Length, content.Length);

                return bytes;
            }
        }
    }
}
