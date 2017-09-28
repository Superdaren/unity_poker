using System;
using UnityEngine;
using Google.Protobuf;

/**
 * 断线重连
 */
namespace NetProto
{
    public class ReConnectHandle : NetHandle
    {
        public ReConnectHandle()
        {
            handlerMap.Add(Api.ENetMsgId.user_login_ack, userLoginAck);

             //玩家断线重连 牌桌信息回复
            handlerMap.Add(Api.ENetMsgId.room_player_reconnect_ack, roomPlayerReconnectAck);
        }

        #region request

        /**
         * 用户重连
         */
        public void ReConnect(Action reConnectSuccessAction)
        {
            if (UserManager.Instance().authModel != null)
            {
                UserLoginReq loginReq = new UserLoginReq();
                loginReq.UserId = UserManager.Instance().authModel.user_id;
                loginReq.UniqueId = "3";
                loginReq.Token = UserManager.Instance().authModel.token;
                loginReq.ConnectTo = PrefsUtil.GetString(PrefsUtil.ServiceId);
                loginReq.IsReconnect = 0;

                NetCore.Instance.Send(Api.ENetMsgId.user_login_req, loginReq);
            }
            else
            {
                if (reConnectSuccessAction != null)
                {
                    // 没有登录时候调用
                    reConnectSuccessAction();
                }
            }
        }

        #endregion

        #region action
        /**
         *   用户登录回复
         */
        public object userLoginAck(byte[] data)
        {
            UserLoginAck loginAction = UserLoginAck.Parser.ParseFrom(data);

            PrefsUtil.Set(PrefsUtil.ServiceId, loginAction.ServiceId);

            Debug.Log("登录成功!");

            return loginAction;
        }

        /**
         *   玩家断线重连 牌桌信息回复
         */
        public object roomPlayerReconnectAck(byte[] data)
        {
            RoomPlayerReconnectAck roomPlayerReconnectAction = RoomPlayerReconnectAck.Parser.ParseFrom(data);

            return roomPlayerReconnectAction;
        }

        #endregion

    }
}
