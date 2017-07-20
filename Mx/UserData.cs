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
            /// <summary>
            /// 注册
            /// </summary>
            /// <param name="name"></param>
            /// <param name="password"></param>
            /// <param name="phoneNumber"></param>
            /// <returns></returns>
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

            /// <summary>
            /// 登陆
            /// </summary>
            /// <param name="name"></param>
            /// <param name="password"></param>
            /// <param name="phoneNumber"></param>
            /// <returns></returns>
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

            /// <summary>
            /// 下载文件
            /// </summary>
            /// <param name="state"></param>
            /// <param name="name"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public string DownloadFile(string state, string name, string number)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_FILE_DOWNLOAD));

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
            /// 获得vip时间
            /// </summary>
            /// <param name="state"></param>
            /// <param name="name"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public string GetUserVipTime(string username, string tp)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_ORBBEC_USER_VIP_TIME));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("username", username);
                jsonObject.put("state", tp);

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }

            /// <summary>
            /// 获得产品信息
            /// </summary>
            /// <param name="state"></param>
            /// <param name="name"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public string GetProducData(string producname)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_PRODUCT));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("producname", producname);
                jsonObject.put("state", "1");

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }

            /// <summary>
            /// 获得服务器签名
            /// </summary>
            /// <param name="username"></param>
            /// <param name="token"></param>
            /// <returns></returns>
            public string GetSign(string username,string token)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_ORBBEC_SIGN));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("username", username);
                jsonObject.put("token", token);

                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();

            }

            /// <summary>
            /// 获得VIP数据
            /// </summary>
            /// <param name="viptype"></param>
            /// <returns></returns>
            public string GetVipData(string viptype)
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_ORBBEC_VIP));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("viptype", viptype);
                jsonObject.put("ty1", "asd");
                JSONArray jsonArray = new JSONArray();
                jsonArray.put(label);
                jsonArray.put(jsonObject);

                return jsonArray.ToString();
            }


            /// <summary>
            /// 获得订单信息
            /// </summary>
            /// <param name="username"></param>
            /// <param name="viptype"></param>
            /// <returns></returns>
            public string GetOrderData(string username, string viptype) 
            {
                JSONObject label = GetLabel(Enum.GetName(typeof(RequestType), RequestType.REQUEST_GET), Enum.GetName(typeof(GetDataType), GetDataType.GET_DATA_ORBBEC_ORDER));

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("username",username);
                jsonObject.put("viptype", viptype);
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
