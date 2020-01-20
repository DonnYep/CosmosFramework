using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public abstract  class FSMTranslation
    {
        public FSMTranslation()
        {
            OnInit();
        }
        public abstract void OnInit();
        public abstract bool Handler();
    }
}