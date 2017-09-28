using UnityEngine;
using System.Collections;

/**
 * 心跳类
 */
namespace NetProto
{
	public class HeartBeatHandle : NetHandle
	{
		// 要回调的方法在这边注册
		public HeartBeatHandle()
		{
			handlerMap.Add(Api.ENetMsgId.heart_beat_ack, HeartBeatAck);
		}

		// 请求心跳
		public void HeartBeatReq()
		{
            AutoId autoId = new AutoId();
            autoId.Id = 1;
            //Debug.Log("---------------------请求心跳");
			NetCore.Instance.Send(Api.ENetMsgId.heart_beat_req, autoId);
		}

		// 响应心跳
        public object HeartBeatAck(byte[] data)
		{
            AutoId autoID = AutoId.Parser.ParseFrom(data);

			return null;
		}
	}
}

