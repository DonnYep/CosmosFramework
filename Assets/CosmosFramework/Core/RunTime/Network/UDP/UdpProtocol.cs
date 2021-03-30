using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
	/// <summary>
	/// https://github.com/skywind3000/kcp/wiki/Network-Layer
	/// <para>外部buffer ----拆分拷贝----等待列表 -----移动----发送列表----拷贝----发送buffer---output</para>
	/// https://github.com/skywind3000/kcp/issues/118#issuecomment-338133930
	/// </summary>

	/// <summary>
	/// Udp网络协议类型
	/// </summary>
	public static class UdpProtocol
	{
		/// <summary>
		/// 无状态
		/// </summary>
		public const byte NIL = 0;
		/// <summary>
		/// 同步标志
		/// </summary>
		public const byte SYN = 1;
		/// <summary>
		/// 确认标志
		/// </summary>
		public const byte ACK = 2;
		/// <summary>
		/// 结束标志
		/// </summary>
		public const byte FIN = 3;
		/// <summary>
		///  业务标志
		/// </summary>
		public const byte MSG = 4;
	}
}

