using UnityEngine;
using System;
using System.Collections.Generic;

namespace NetProto
{
    public class NetHandle
    {
        public Dictionary<Api.ENetMsgId, Dispatcher.MsgHandler> handlerMap = new Dictionary<Api.ENetMsgId, Dispatcher.MsgHandler>();

    }
}
