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

	// 为了减少阅读难度，变量名尽量于 C版 统一
	/*
	conv 会话ID
	mtu 最大传输单元
	mss 最大分片大小
	state 连接状态（0xFFFFFFFF表示断开连接）
	snd_una 第一个未确认的包
	snd_nxt 待发送包的序号
	rcv_nxt 待接收消息序号
	ssthresh 拥塞窗口阈值
	rx_rttvar ack接收rtt浮动值
	rx_srtt ack接收rtt静态值
	rx_rto 由ack接收延迟计算出来的复原时间
	rx_minrto 最小复原时间
	snd_wnd 发送窗口大小
	rcv_wnd 接收窗口大小
	rmt_wnd,	远端接收窗口大小
	cwnd, 拥塞窗口大小
	probe 探查变量，IKCP_ASK_TELL表示告知远端窗口大小。IKCP_ASK_SEND表示请求远端告知窗口大小
	interval    内部flush刷新间隔
	ts_flush 下次flush刷新时间戳
	nodelay 是否启动无延迟模式
	updated 是否调用过update函数的标识
	ts_probe, 下次探查窗口的时间戳
	probe_wait 探查窗口需要等待的时间
	dead_link 最大重传次数
	incr 可发送的最大数据量
	fastresend 触发快速重传的重复ack个数
	nocwnd 取消拥塞控制
	stream 是否采用流传输模式

	snd_queue 发送消息的队列
	rcv_queue 接收消息的队列
	snd_buf 发送消息的缓存
	rcv_buf 接收消息的缓存
	acklist 待发送的ack列表
	buffer 存储消息字节流的内存
	output udp发送消息的回调函数
	*/
	/// <summary>
	/// KCP网络协议类型
	/// </summary>
	public static class KcpProtocol
	{
		//public const int IKCP_RTO_NDL = 30;  // no delay min rto
		//public const int IKCP_RTO_MIN = 100; // normal min rto
		//public const int IKCP_RTO_DEF = 200;
		//public const int IKCP_RTO_MAX = 60000;
		//public const int IKCP_CMD_PUSH = 81; // cmd: push data
		//public const int IKCP_CMD_ACK = 82; // cmd: ack
		//public const int IKCP_CMD_WASK = 83; // cmd: window probe (ask)
		//public const int IKCP_CMD_WINS = 84; // cmd: window size (tell)
		//public const int IKCP_ASK_SEND = 1;  // need to send IKCP_CMD_WASK
		//public const int IKCP_ASK_TELL = 2;  // need to send IKCP_CMD_WINS
		//public const int IKCP_WND_SND = 32;
		//public const int IKCP_WND_RCV = 128; // must >= max fragment size
		//public const int IKCP_MTU_DEF = 1400;
		//public const int IKCP_ACK_FAST = 3;
		//public const int IKCP_INTERVAL = 100;
		//public const int IKCP_OVERHEAD = 24;
		//public const int IKCP_DEADLINK = 20;
		//public const int IKCP_THRESH_INIT = 2;
		//public const int IKCP_THRESH_MIN = 2;
		//public const int IKCP_PROBE_INIT = 7000;   // 7 secs to probe window size
		//public const int IKCP_PROBE_LIMIT = 120000; // up to 120 secs to probe window
		//public const int IKCP_FASTACK_LIMIT = 5;        // max times to trigger fastack
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

