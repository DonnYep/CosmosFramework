using System.Collections.Generic;
using MessagePack;
using Protocol;
using UnityEngine;
namespace Cosmos.Lockstep
{
    public class MultiplayEntitiesAgent
    {
        Dictionary<int, MultiplayLogicComponent> compDict = new Dictionary<int, MultiplayLogicComponent>();
        public void OnMulitplayConnected()
        {
            var authorityConv = GameEntry.MultiplayManager.AuthorityConv;
            var localGo = MultiplayConfig.Instance.LocalPlayerPrefab;
            var go = GameObject.Instantiate(localGo, GameEntry.MultiplayManager.InstanceObject().transform);
            var comp = go.AddComponent<MultiplayLogicComponent>();
            comp.Init(authorityConv, true);
            compDict.TryAdd(authorityConv, comp);
            go.AddComponent<MultiplayInputComponent>();
            if (GameEntry.ControllerManager.GetController("LockstepCamera", out var controller))
            {
                controller.Handle.CastTo<LockstepCamera>().SetTransformTarget(comp.transform);
            }
        }
        public void OnMultiplayInput(Dictionary<int, List<byte[]>> inputs)
        {
            foreach (var ipt in inputs)
            {
                var comp = compDict[ipt.Key];
                var length = ipt.Value.Count;
                for (int i = 0; i < length; i++)
                {
                    comp.OnTickInput(MessagePackSerializer.Deserialize<OpInput>(ipt.Value[i]));
                }
            }
        }
        public void OnMulitplayPlayerEnter(int conv)
        {
            var remoteGo = MultiplayConfig.Instance.RemotePlayerPrefab;
            var go = GameObject.Instantiate(remoteGo, GameEntry.MultiplayManager.InstanceObject().transform);
            var comp = go.AddComponent<MultiplayLogicComponent>();
            comp.Init(conv);
            compDict.TryAdd(conv, comp);
        }
        public void OnMulitplayPlayerExit(int conv)
        {
            if (compDict.TryRemove(conv, out var comp))
            {
                GameObject.Destroy(comp.gameObject);
            }
        }
        public void OnMulitplayDisconnected()
        {
            if (GameEntry.ControllerManager.GetController("Camera", out var controller))
            {
                controller.Handle.CastTo<LockstepCamera>().Clear();
            }
            foreach (var comp in compDict)
            {
                GameObject.Destroy(comp.Value.gameObject);
            }
            compDict.Clear();
        }
    }
}
