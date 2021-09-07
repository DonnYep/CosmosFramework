using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    public interface ISceneInfo
    {
        string SceneName { get; }
        bool Additive { get;  }
    }
}
