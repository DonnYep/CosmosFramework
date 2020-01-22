using UnityEngine;
using System.Collections;
namespace Cosmos.FSM
{
    public abstract  class FSMTranslation<T>
        where T:class
    {
        protected string translationName;
        public string TranslationName { get { return translationName; } }
        public FSMTranslation()
        {
            OnInit();
        }
        public abstract void OnInit();
        public abstract bool Handler(IFSM<T> fsm);
    }
}