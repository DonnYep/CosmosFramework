using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Mono;
using Cosmos.Event;
using Cosmos.Input;
namespace Cosmos.Test
{
    public class InputDispatcher : MonoBehaviour
    {
        public string DispatcherName { get { return gameObject.name; } }
        InputEventArgs inputHandler = new InputEventArgs();
        private void Start()
        {
            InputManager.Instance.OnInitialization();
        }
    }
}