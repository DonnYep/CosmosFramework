using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Cosmos.Test
{
    public class MultiplayerYBotView : MonoBehaviour
    {
        Dictionary<int, NetworkTransform> netTransDict = new Dictionary<int, NetworkTransform>();
        MultiplayerYBotCamera  multiplayerYBotCamera;
        NetworkTransform authorityTrans;

        private void Start()
        {
            MultiplayerManager.Instance.OnConnect += OnConnectHandler;
            MultiplayerManager.Instance.OnPlayerEnter += OnPlayerEnterHandler;
            MultiplayerManager.Instance.OnPlayerExit += OnPlayerExitHandler;
            MultiplayerManager.Instance.OnDisconnect += OnDisconnectHandler;
            MultiplayerManager.Instance.OnPlayerInput += OnPlayerInputHandler;
        }
        void OnConnectHandler()
        {
            multiplayerYBotCamera = GameObject.Find("YBotCamera").AddComponent<MultiplayerYBotCamera>();
            var go = GameObject.Instantiate(MultiplayerManager.Instance.LocalPlayerPrefab);
            authorityTrans = go.AddComponent<NetworkTransform>();
           var ctrlComp= go.AddComponent<MultiplayerYBotController>();
            authorityTrans.NetId = MultiplayerManager.Instance.AuthorityConv;
            authorityTrans.IsAuthority = true;
            multiplayerYBotCamera.SetCameraTarget(ctrlComp.CameraTarget);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MultiplayerManager.Instance.RemotePlayerPrefab);
            var comp = go.AddComponent<NetworkTransform>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netTransDict.Add(comp.NetId, comp);
        }
        void OnPlayerExitHandler(int conv)
        {
            if (netTransDict.Remove(conv, out var networkIndentity))
            {
                MonoGameManager.KillObject(networkIndentity.gameObject);
            }
        }
        /// <summary>
        /// int表示Conv，string表示FixTransform的json
        /// </summary>
        void OnPlayerInputHandler(Dictionary<int, string> inputData)
        {
            foreach (var iptData in inputData)
            {
                if (netTransDict.TryGetValue(iptData.Key, out var netTrans))
                {
                    netTrans.DeserializeNetworkTransform(Utility.Json.ToObject<FixTransform>(iptData.Value));
                }
            }
        }
        void OnDisconnectHandler()
        {
            try
            {
                foreach (var netObj in netTransDict.Values)
                {
                    MonoGameManager.KillObject(netObj.gameObject);
                }
                MonoGameManager.KillObject(authorityTrans.gameObject);
                authorityTrans = null;
                netTransDict.Clear();
                multiplayerYBotCamera.ReleaseTarget();
                MonoGameManager.KillObject(multiplayerYBotCamera);
            }
            catch { }
        }
    }
}