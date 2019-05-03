using System;

namespace UnityJsonRpc
{
    [Serializable]
    public class JsonRpcResponseData
    {
        public object result;
        public object error;
        public object id;
    }
}