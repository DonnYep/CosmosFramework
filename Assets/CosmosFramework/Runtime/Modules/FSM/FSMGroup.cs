using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.FSM
{
    /// <summary>
    /// 状态机容器
    /// </summary>
    internal class FSMGroup 
    {
        #region Properties
        List<FSMBase> fsmSet = new List<FSMBase>();
        public List<FSMBase> FSMList { get { return fsmSet; } }
        public bool IsPause { get;private set; }
        public float RefreshInterval { get; private set; }
        float coolTime = 0;
        #endregion

        #region Methods
        public void OnPause(){IsPause = true;}
        public void OnUnPause(){ IsPause = false; }
        public void AddFSM(FSMBase fsm)
        {
            if (!fsmSet.Contains(fsm))
                fsmSet.Add(fsm);
            else
                throw new ArgumentException(" FSM is exists" + fsm.OwnerType.ToString());
        }
        public void DestoryFSM(Predicate<FSMBase>predicate)
        {
            var fsm= fsmSet.Find(predicate);
            if (fsm == null)
                throw new ArgumentNullException("FSM not  exists" + predicate.ToString());
            fsmSet.Remove(fsm);
            fsm.Shutdown();
        }
        public void DestoryFSM(FSMBase fsm)
        {
            fsmSet.Remove(fsm);
            fsm.Shutdown();
        }
        public void SetRefreshInterval(float interval)
        {
            if (interval <= 0)
            {
                Utility.Debug.LogError("FSM Refresh interval less than Zero, use Zero instead");
                this.RefreshInterval = 0;
                return;
            }
            this.RefreshInterval = interval;
        }
        public void OnRefresh()
        {
            if (IsPause)
                return;
            coolTime += Time.deltaTime;
            if(coolTime>= RefreshInterval)
            {
                int length = fsmSet.Count;
                for (int i = 0; i < length; i++)
                {
                    fsmSet[i].OnRefresh();
                }
                coolTime = 0;
            }
        }
        public bool HasFSM(Predicate<FSMBase> predicate)
        {
            return fsmSet.Find(predicate) == null;
        }
        public bool HasFSM(FSMBase fsm)
        {
            return fsmSet.Contains(fsm);
        }
        public FSMBase GetFSM(Predicate<FSMBase> predicate)
        {
            return fsmSet.Find(predicate);
        }
        public int GetFSMCount()
        {
            return fsmSet.Count;
        }
        public void DestoryAllFSM()
        {
            int length = fsmSet.Count;
            for (int i = 0; i < length; i++)
            {
                fsmSet[i].Shutdown();
            }
            fsmSet.Clear();
        }
        #endregion
    }
}
