using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;

namespace NetProto
{
    /**
     * 该类用于注册对应消息类型的方法和action
     */
    public class Dispatcher
    {
        public delegate object MsgHandler(byte[] data);

        class Group
        {
            public MsgHandler handler;
            // action注册一次，执行一次，它在handler之后被执行
            // action的输入为handler的返回值
            public System.Action<object> action;
			// 判断action是否只能执行一次,默认是
			public bool isOnce = true;
        }

        Dictionary<Api.ENetMsgId, Group> msgMap = new Dictionary<Api.ENetMsgId, Group>();

        public Dispatcher()
        {
        }

		/**
         * 把NetHandle里面添加的方法注册进来
         */
		public void Register(NetHandle handle)
        {
            foreach(var v in handle.handlerMap)
            {
                RegisterHandler(v.Key, v.Value);
            }
        }

		/**
         * 注册handler 此处的id是ack类型的ID
         */
		public bool RegisterHandler(Api.ENetMsgId id, MsgHandler handler)
        {
            if (msgMap.ContainsKey(id))
            {
                Debug.LogWarning(id + " is already registered");
                return false;
            }
            Group g = new Group();
            g.handler = new MsgHandler(handler);
            g.action = null;
            msgMap.Add(id, g);

            return true;
        }

        /**
         * 注册Action 此处的id是ack类型的ID
         */
        public bool RegisterAction(Api.ENetMsgId id, System.Action<object> act)
        {
            if (!msgMap.ContainsKey(id))
            {
                Debug.LogError("register handler of " + id + " first");
                return false;
            }

            msgMap[id].action = act;

            return true;
        }

        /**
         * 设置某个action不销毁
         */
        public void setActionForever(Api.ENetMsgId id)
		{
			if (!msgMap.ContainsKey(id))
			{
				Debug.LogError("register handler of " + id + " first");
			}

            msgMap[id].isOnce = false;
		}

        /**
         * 处理Handler,Action 此处的id是ack类型的ID
         */
        public bool InvokeHandler(Api.ENetMsgId id, byte[] data)
        {
            if (!msgMap.ContainsKey(id))
            {
                Debug.LogWarning(id + " is not registered");
                return false;
            }

            Group g = msgMap[id];

            object ret = g.handler(data);

            if (g.action != null)
            {
                g.action.Invoke(ret);
                if(g.isOnce) {
                    g.action = null;
                }
            }

            return true;
        }
    }
}
