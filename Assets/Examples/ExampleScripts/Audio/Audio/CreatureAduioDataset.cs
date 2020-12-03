using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewCreatureAduioDataSet", menuName = "CosmosFramework/AudioDataSet/CreatureAduio")]
    public class CreatureAduioDataset : AudioDataset
    {
        [SerializeField] AudioClip aduioCilp;
        public override AudioClip AudioClip { get { return aduioCilp; } }
        public override void Reset()
        {
           objectName = "NewCreatureAduio";
            mute = false;
            playOnAwake = false;
            loop = false;
            volume = 1;
            spatialBlend = 0;
            speed = 1;
            aduioCilp = null;
        }
    }
}