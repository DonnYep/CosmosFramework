using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.FSM
{
    /// <summary>
    /// 状态机容器
    /// </summary>
    internal class FSMGroup 
    {
        #region Properties
        List<FSMBase> fsmList = new List<FSMBase>();
        public List<FSMBase> FSMList { get { return fsmList; } }
        public bool IsPause { get;private set; }
        public int RefreshInterval { get; private set; }
        /// <summary>
        /// 上一次刷新时间
        /// </summary>
        long latestTime = 0;
        #endregion

        #region Methods
        public void OnPause(){IsPause = true;}
        public void OnUnPause(){ IsPause = false; }
        public void AddFSM(FSMBase fsm)
        {
            if (!fsmList.Contains(fsm))
                fsmList.Add(fsm);
            else
                throw new ArgumentException(" FSM is exists" + fsm.OwnerType.ToString());
        }
        public void DestoryFSM(Predicate<FSMBase>predicate)
        {
            var fsm= fsmList.Find(predicate);
            if (fsm == null)
                throw new ArgumentNullException("FSM not  exists" + predicate.ToString());
            fsmList.Remove(fsm);
            fsm.Shutdown();
        }
        public void DestoryFSM(FSMBase fsm)
        {
            fsmList.Remove(fsm);
            fsm.Shutdown();
        }
        public void SetRefreshInterval(int interval)
        {
            if (interval <= 0)
            {
                this.RefreshInterval = 0;
                return;
            }
            this.RefreshInterval = interval;
            latestTime = Utility.Time.MillisecondNow() + RefreshInterval;
        }
        public void OnRefresh()
        {
            if (IsPause)
                return;
            var msNow = Utility.Time.MillisecondNow();
            if (latestTime <= msNow)
            {
                latestTime = msNow + RefreshInterval;
                int length = fsmList.Count;
                for (int i = 0; i < length; i++)
                {
                    fsmList[i].OnRefresh();
                }
            }
        }
        public bool HasFSM(Predicate<FSMBase> predicate)
        {
            return fsmList.Find(predicate) == null;
        }
        public bool HasFSM(FSMBase fsm)
        {
            return fsmList.Contains(fsm);
        }
        public FSMBase GetFSM(Predicate<FSMBase> predicate)
        {
            return fsmList.Find(predicate);
        }
        public int GetFSMCount()
        {
            return fsmList.Count;
        }
        public void DestoryAllFSM()
        {
            int length = fsmList.Count;
            for (int i = 0; i < length; i++)
            {
                fsmList[i].Shutdown();
            }
            fsmList.Clear();
        }
        #endregion
    }
}
