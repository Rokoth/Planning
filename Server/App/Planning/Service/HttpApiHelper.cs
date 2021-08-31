using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Planning.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planning.Service
{
    public static class HttpApiHelper
    {
        public static StringContent SerializeRequest<TReq>(this TReq entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            return data;
        }

        public static async Task<TResp> ParseResponse<TResp>(this HttpResponseMessage result) where TResp : class
        {
            if (result != null && result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return JObject.Parse(response).ToObject<TResp>();
            }
            return null;
        }

        public static async Task<Response<TResp>> ParseResponseExt<TResp>(this HttpResponseMessage result) where TResp : class
        {
            if (result != null && result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return new Response<TResp>()
                {
                    ResponseCode = ResponseEnum.OK,
                    ResponseBody = JObject.Parse(response).ToObject<TResp>()
                };
            }
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new Response<TResp>()
                {
                    ResponseCode = ResponseEnum.NeedAuth
                };
            }
            return new Response<TResp>()
            {
                ResponseCode = ResponseEnum.Error
            };
        }

        public static async Task<Response<IEnumerable<T>>> ParseResponseArray<T>(this HttpResponseMessage result) where T : class
        {
            if (result != null && result.IsSuccessStatusCode)
            {
                var ret = new List<T>();
                var response = await result.Content.ReadAsStringAsync();
                foreach (var item in JArray.Parse(response))
                {
                    ret.Add(item.ToObject<T>());
                }
                return new Response<IEnumerable<T>>()
                {
                    ResponseCode = ResponseEnum.OK,
                    ResponseBody = ret
                };
            }
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new Response<IEnumerable<T>>()
                {
                    ResponseCode = ResponseEnum.NeedAuth
                };
            }
            return new Response<IEnumerable<T>>()
            {
                ResponseCode = ResponseEnum.Error
            };
        }
    }
}
