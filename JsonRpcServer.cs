using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace UnityJsonRpc
{
    public class JsonRpcServer
    {
        private HttpListener server;

        public void Stop()
        {
            server.Stop();
            server.Close();
        }

        public ConcurrentQueue<JsonRpcRequest> Start(string address)
        {
            server = new HttpListener();
            var queue = new ConcurrentQueue<JsonRpcRequest>();
            
            server.Prefixes.Add(address);

            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException();
            }

            server.Start();

            new Thread(() =>
            {
                while (server.IsListening)
                {
                    var context = server.GetContext();

                    var data = "{}";

                    using (var stream = new StreamReader(context.Request.InputStream))
                    {
                        data = stream.ReadToEnd();
                    }

                    JsonRpcRequestData jrpcData = null;
                    try
                    {
                        jrpcData = JsonConvert.DeserializeObject<JsonRpcRequestData>(data);
                    }
                    catch
                    {
                    }

                    var jsonRpcRequest = new JsonRpcRequest(context.Response, jrpcData);

                    if (jrpcData == null || string.IsNullOrEmpty(jrpcData.method))
                    {
                        jsonRpcRequest.Error("bad request");
                    }

                    queue.Enqueue(new JsonRpcRequest(context.Response, jrpcData));
                }
            }).Start();

            return queue;
        }
    }
}