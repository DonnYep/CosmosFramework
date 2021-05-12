using UnityEngine;
using System.Collections;
namespace Cosmos.UI
{
    /// <summary>
    /// 临时UI
    /// </summary>
    public abstract class UITemporaryForm :UIFormBase
    {
        public override bool IsTemporaryForm { get; protected set; } = true;
        public sealed override void ShowUIForm(){}
        public sealed override void HideUIForm(){}
    }
}