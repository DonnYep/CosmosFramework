using System;
namespace Cosmos.DataTable
{
    public abstract class DataTableBase
    {
        protected readonly string name;
        protected Action onReadSuccess;
        protected Action onReadFailure;
        public string Name { get { return name; } }
        public event Action OnReadSuccess
        {
            add { onReadSuccess += value; }
            remove { onReadSuccess -= value; }
        }
        public event Action OnReadFailure
        {
            add { onReadFailure += value; }
            remove { onReadFailure -= value; }
        }
        internal DataTableAssetInfo DataTableAssetInfo { get; set; }
        public DataTableBase(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// 读取数据表资源；
        /// </summary>
        /// <param name="dataTableBytes">资源表数据</param>
        public abstract void ReadDataTable(byte[] dataTableBytes);
        internal void OnReadDataSuccess()
        {
            onReadSuccess?.Invoke();
        }
        internal void OnReadDataFailure()
        {
            onReadFailure?.Invoke();
            DataTableAssetInfo = null;
        }
    }
}
