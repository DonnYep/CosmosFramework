using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public abstract  class FSMTranslation
    {
        protected string translationName;
        public string TranslationName { get { return translationName; } }
        public FSMTranslation()
        {
            OnInit();
        }
        public abstract void OnInit();
        public abstract bool Handler();
    }
}