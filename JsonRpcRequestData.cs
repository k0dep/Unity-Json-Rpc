using System;
using Newtonsoft.Json;

namespace UnityJsonRpc
{
    [Serializable]
    public class JsonRpcRequestData
    {
        public string method;
        public object[] @params;
        public object id;
    }
}