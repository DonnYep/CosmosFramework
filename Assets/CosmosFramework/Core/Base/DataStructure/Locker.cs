using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmos
{
	/// <summary>
	/// 提供同步访问对象的机制。
	/// Lock锁与Monitor区别：
	/// https://www.cnblogs.com/chenwolong/p/7503977.html
	/// </summary>
	public class Locker : IDisposable
    {
		private object locked;
		public bool HasLock { get; private set; }
        public Locker(object obj)
		{
			if (!Monitor.TryEnter(obj))
			{
				return;
			}
			this.HasLock = true;
			this.locked = obj;
		}
		public void Dispose()
		{
			if (!this.HasLock)
			{
				return;
			}

			Monitor.Exit(this.locked);
			this.locked = null;
			this.HasLock = false;
		}
	}
}
