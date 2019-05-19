using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace UnityJsonRpc
{
    public class JsonRpcRequest
    {
        private HttpListenerResponse _response;
        
        public JsonRpcRequestData RequestData { get; set; }

        public JsonRpcRequest(HttpListenerResponse response, JsonRpcRequestData data)
        {
            _response = response;
            RequestData = data;
        }

        public void Respond(object result)
        {
            Write(new JsonRpcResponseData()
            {
                result = result,
                id = RequestData != null ? RequestData.id : null
            });
        }

        public void Error(object error)
        {
            Write(new JsonRpcResponseData()
            {
                error = error,
                id = RequestData != null ? RequestData.id : null
            });
        }

        private void Write(object data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { 
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            }));
            _response.ContentLength64 = buffer.Length;
            _response.Headers.Add("Content-Type", "application/json");
            _response.Headers.Add("Access-Control-Allow-Origin", "*");
            _response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            _response.Headers.Add("Access-Control-Allow-Credentials", "true");
            _response.Headers.Add("Access-Control-Allow-Headers",
                "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
            
            var output = _response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}