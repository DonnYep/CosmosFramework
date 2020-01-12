using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test{
public class ModuleTester: MonoBehaviour {
        int id=-1;
        List<int> monoIDList = new List<int>();
        [SerializeField]
        bool debugMonoMudule;
        [SerializeField]
         int maxCount = 50;
        int count = 0;
        private void Start()
        {
            Facade.Instance.InitAllModule();
            if (debugMonoMudule)
            {
                Facade.Instance.AddMonoListener(UpdateTest, Mono.UpdateType.Update,
                    (id) => { if (!monoIDList.Contains(id)) monoIDList.Add(id); });
                ++count;
                Utility.DebugLog("updateTest" + count);
            }
        }
        private void UpdateTest()
        {
            if (count >= maxCount)
                return;
            Facade.Instance.AddMonoListener(AddListenerTest, Mono.UpdateType.Update,
                 (id) => { if (!monoIDList.Contains(id)) monoIDList.Add(id); });
            ++count;
            Utility.DebugLog("updateTest" + count);
        }
        void AddListenerTest()
        {
            if (count >= maxCount)
                return;
            Facade.Instance.AddMonoListener(UpdateTest, Mono.UpdateType.Update,
                 (id) => { if (!monoIDList.Contains(id)) monoIDList.Add(id); });
            ++count;
            Utility.DebugLog("AddListenerTestupdateTest" + count);
        }
    }
}