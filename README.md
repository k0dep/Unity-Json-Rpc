UnityJsonRpc
=========

Json-Rpc server for unity as upm package.

Sample
------

1. Create `Server.cs` mono behaviour file:

```csharp
using System.Collections.Concurrent;
using UnityEngine;
using UnityJsonRpc;
using Random = UnityEngine.Random;

public class Server : MonoBehaviour
{
    public int ListenPort = 8080;

    private JsonRpcServer server;
    private ConcurrentQueue<JsonRpcRequest> requests;
    
    public void Awake()
    {
        server = new JsonRpcServer();
        requests = server.Start("http://localhost:" + ListenPort + "/");
    }

    void Update()
    {
        if (requests.IsEmpty)
        {
            return;
        }

        if (requests.TryDequeue(out var request))
        {
            if (Random.value > 0.5)
            {
                request.Error(-121276);
            }
            else
            {
                request.Respond("ok");
            }
        }
    }
    
    public void OnDestroy()
    { 
        server.Stop();
    }
}
```

2. Add `Server` component to some GameObject at scene
3. Play
4. Request(by Postman or another app) `http://localhost:8080/` with payload:
```json
{
	"method": "method"
}
```
5. Take response

Using
-----

For start using this package add lines into `./Packages/manifest.json` like next sample:  
```json
{
  "dependencies": {
    "unity-json-rpc": "https://github.com/k0dep/unity-json-rpc.git"
  }
}
```

Or use it as dependency from another packages and auto include by [Originer](https://github.com/k0dep/Originer) package
