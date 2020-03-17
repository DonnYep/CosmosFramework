using UnityEngine;
using System.Collections;

namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewPlayerStatusDataSet", menuName = "CosmosFramework/CharacterStatusDataSet/PlayerStatus")]
    public class PlayerStatusDataSet : CharacterStatusDataSet
    {
        public override void Reset()
        {
            statusName = "NewPlayerStatus";
            heath = 200;
            moveSpeed = 10;
        }
    }
}