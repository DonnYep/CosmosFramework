using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public abstract class CharacterInputController:MonoBehaviour
    {
        protected abstract void Handler(object sender, Event.GameEventArgs arg);
       
    }
}