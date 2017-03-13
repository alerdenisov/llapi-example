using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LlapiExample
{
    public interface INetworkLogic
    {
        void Setup();
        void Update();
    }

    public interface ICommonLogic { }
    public interface IServerLogic { }
    public interface IClientLogic { }
}