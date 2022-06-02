using System;
namespace Cosmos.DataTable
{
    public abstract class DataTableBase
    {
        protected readonly string name;
        protected Action<DataTableBase> onReadSuccess;
        protected Action<DataTableBase> onReadFailure;
        public string Name { get { return name; } }
        public event Action<DataTableBase> OnReadSuccess
        {
            add { onReadSuccess += value; }
            remove { onReadSuccess -= value; }
        }
        public event Action<DataTableBase> OnReadFailure
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
        internal abstract void ReadDataTable(byte[] dataTableBytes);
        /// <summary>
        /// 当此表被释放；
        /// </summary>
        internal abstract void OnRelease();
        internal void OnReadDataSuccess()
        {
            onReadSuccess?.Invoke(this);
            onReadSuccess = null;
        }
        internal void OnReadDataFailure()
        {
            onReadFailure?.Invoke(this);
            DataTableAssetInfo = null;
            onReadFailure = null;
        }
    }
}
