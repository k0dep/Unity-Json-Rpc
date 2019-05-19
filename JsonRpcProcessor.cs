using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityJsonRpc
{
    public class JsonRpcProcessor
    {
        private readonly Dictionary<string, Delegate> _methods;

        public JsonRpcProcessor()
        {
            _methods = new Dictionary<string, Delegate>();
            SetMethod("help", GetHelp);
        }

        public void SetMethod(string name, Action handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TRes>(string name, Func<TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TArg, TRes>(string name, Func<TArg, TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TArg, TArg1, TRes>(string name, Func<TArg, TArg1, TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TArg, TArg1, TArg2, TRes>(string name, Func<TArg, TArg1, TArg2, TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TArg, TArg1, TArg2, TArg3, TRes>(string name, Func<TArg, TArg1, TArg2, TArg3, TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        public void SetMethod<TArg, TArg1, TArg2, TArg3, TArg4, TRes>(string name, Func<TArg, TArg1, TArg2, TArg3, TArg4, TRes> handler)
        {
            SetMethod(name, (Delegate)handler);
        }
        
        private void SetMethod(string name, Delegate handler)
        {
            var methodName = FormatMethodName(name, handler.Method.GetParameters().Select(p => p.ParameterType));
            _methods[methodName] = handler;
        }

        public void Handle(JsonRpcRequest request)
        {
            var requestData = request.RequestData;
            var requestParams = requestData.@params == null ? new object[0] : requestData.@params;
            var methodName = FormatMethodName(requestData.method, requestParams.Select(t => t.GetType()));

            Delegate method = null;
            if (!_methods.TryGetValue(methodName, out method))
            {
                request.Error("method with signature `" + methodName + "` not found");
                return;
            }

            if (method == null)
            {
                request.Error("System error. Method is broken");
                return;
            }

            object result = null;
            try
            {
                result = method.DynamicInvoke(requestData.@params);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                request.Error(e.ToString());
                return;
            }
            
            request.Respond(result);
        }

        private string FormatMethodName(string name, IEnumerable<Type> parameters)
        {
            var parameterTypeNames = parameters != null ? parameters.Select(p => p.FullName).ToArray() : new string[0];
            var methodName = string.Format("{0}({1})", name, string.Join(",", parameterTypeNames));
            return methodName;
        }

        private object[] GetHelp()
        {
            return _methods.Select(row => new
            {
                humanName = row.Key,
                method = row.Key.Substring(0, row.Key.IndexOf('(')),
                arguments = row.Value.Method.GetParameters().Select(p => new {
                    name = p.Name,
                    type = p.ParameterType.FullName
                })
            })
            .ToArray();
        }
    }
}