using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mx.Json;

namespace Mx
{
    namespace Server
    {
        public enum GetDataType
        {
            GET_SIGN_IN,
            GET_SIGN_UP,
            GET_DATA_THEME,
            GET_DATA_USER,
            GET_DATA_PRODUCT,
            GET_DATA_ORDER,
            GET_DATA_ORDER_DETAILS,
            GET_FILE_DOWNLOAD,
            GET_DATA_ORBBEC_SIGN,
            GET_DATA_ORBBEC_ORDER,
            GET_DATA_ORBBEC_VIP

        }

        public enum PostDataType
        {
            POST_DATA_THEME,
            POST_DATA_USER,
            POST_DATA_PRODUCT,
            POST_DATA_ORDER,
            POST_DATA_ORDER_DETAILS,
            POST_FILE_UPLOAD
        }

        public enum RequestType
        {
            REQUEST_GET,
            REQUEST_POST
        }

        public interface IData
        {
            //获得标签
            string GetDataPark(string label, string msg);

            string GetDataPark(string label, List<string> msg);

            bool IsResult(string msg);
        }

        public class DataHandler : IData
        {

            public JSONObject GetLabel(string requestType, string getDataType)
            {

                JSONObject data = new JSONObject();

                data.put("requestType", requestType);
                data.put("msgType", getDataType);

                return data;
            }

            /// <summary>
            /// 封装信息
            /// </summary>
            /// <param name="label">标签</param>
            /// <param name="msg">信息</param>
            /// <returns></returns>
            public string GetDataPark(string label, string msg)
            {
                List<string> park = new List<string>();
                park.Add(label);
                park.Add(msg);

                return park.ToString();
            }

            /// <summary>
            /// 封装  信息list
            /// </summary>
            /// <param name="label">标签</param>
            /// <param name="msg">信息集合</param>
            /// <returns></returns>
            public string GetDataPark(string label, List<string> msg)
            {

                msg.Insert(0, label);

                return msg.ToString();
            }

            /// <summary>
            /// 返回结果情况
            /// </summary>
            /// <param name="msg">返回信息</param>
            /// <returns></returns>
            public bool IsResult(string msg)
            {
                object json = new JSONTokener(msg).nextValue();

                if (json is JSONArray)
                {

                    JSONArray jsonArray = (JSONArray)json;

                    JSONObject jsonObject = jsonArray.getJSONObject(0);

                    return (bool)jsonObject.get("返回结果");
                }
                else if (json is JSONObject)
                {
                    JSONObject jsonObject = (JSONObject)json;

                    return (bool)jsonObject.get("返回结果");
                }
                return false;
            }


            public string IsResult(string msg, string key)
            {
                object json = new JSONTokener(msg).nextValue();

                if (json is JSONArray)
                {

                    JSONArray jsonArray = (JSONArray)json;

                    JSONObject jsonObject = jsonArray.getJSONObject(0);

                    return jsonObject.getString(key);
                }
                else if (json is JSONObject)
                {
                    JSONObject jsonObject = (JSONObject)json;

                    return jsonObject.getString(key);
                }
                return null;
            }
        }
    }

}
