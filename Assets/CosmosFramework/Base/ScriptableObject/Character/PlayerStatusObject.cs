using UnityEngine;
using System.Collections;

namespace Cosmos
{
    [CreateAssetMenu(fileName ="NewPlayerStatusObject",menuName ="CosmosFramework/CharacterStatusObject/PlayerStatusObject")]
    public class PlayerStatusObject : CharacterStatusObject
    {
        public override void Reset()
        {
            statusName = "NewPlayerStatus";
            heath = 200;
            moveSpeed = 10;
        }
    }
}