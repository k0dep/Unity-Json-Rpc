using System;

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