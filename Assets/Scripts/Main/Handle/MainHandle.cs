using System.Collections.Generic;
using System;
using LitJson;

namespace NetProto
{
	public class MainHandle : NetHandle
	{
		public MainHandle()
		{
            handlerMap.Add(Api.ENetMsgId.room_get_table_ack, roomGetTableAck);
		}

		#region request
		/**
         * 玩家请求加入游戏
         */
		public void roomPlayerJoinReq(int roomId)
		{
			BaseReq baseReq = new BaseReq();
            baseReq.AppFrom = "1";
            baseReq.AppVer = 2;
            baseReq.AppChannel = "3";

			RoomPlayerJoinReq joinReq = new RoomPlayerJoinReq();
            joinReq.BaseReq = baseReq;
            joinReq.RoomId = roomId;

			NetCore.Instance.Send(Api.ENetMsgId.room_player_join_req, joinReq);
		}
		#endregion

		#region action
		/**
         * 查询牌桌信息回复 (当玩家加入牌桌后，服务器会向此用户推送牌桌信息)
         */
        public object roomGetTableAck(byte[] data)
		{
			RoomGetTableAck tableAction = RoomGetTableAck.Parser.ParseFrom(data);

            return tableAction;
		}
		#endregion

		#region httpRequest
		/**
         * 获取房间列表
         */
		public void roomList(Action<Error, RoomList> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			HttpUtil.Http.Get(URLManager.roomListUrl).OnSuccess(result =>
			{
				if (result != null)
				{
					RoomList roomList = JsonMapper.ToObject<RoomList>(result);
					action(null, roomList);
				}
			}).OnFail(result =>
			{
				action(new Error(500, null), null);
            }).Go();
		}

		/**
         * 获取用户详细信息
         */
		public void userDetail(Action<Error, UserDetail> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			HttpUtil.Http.Get(URLManager.userDetail()).OnSuccess(result =>
			{
				if (result != null)
				{
					UserDetail userDetail = JsonMapper.ToObject<UserDetail>(result);
					action(null, userDetail);
				}
			}).OnFail(result =>
			{
				action(new Error(500, null), null);
            }).Go();
		}

		/**
         * 更新昵称 
         */
        public void nickname(string nickname, Action<Error, string> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("nickname", nickname);

			HttpUtil.Http.Post(URLManager.postNickname()).Form(dic).OnSuccess(result =>
			{
				if (result != null)
				{
					action(null, nickname);
				}
            }).OnFail(result =>
			{
				if (action != null)
					action(new Error(500, null), null);
            }).Go();
		}

		/**
         * 退出登录
         */
		public void logout(Action<Error, string> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();

            HttpUtil.Http.Post(URLManager.logoutUrl()).Form(dic).OnSuccess(result =>
			{
				if (result != null)
				{
					action(null, null);
				}
			}).OnFail(result =>
			{
				if (action != null)
					action(new Error(500, null), null);
            }).Go();
		}
        /**
         *  获取用户背包列表
         */
        public void bagList(Action<Error, UserBagList> action)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
			HttpUtil.Http.Get(URLManager.bagList()).OnSuccess(result =>
			{
                if (result != null)
				{
					UserBagList userBagList = JsonMapper.ToObject<UserBagList>(result);
					action(null, userBagList);
                }
			}).OnFail(result =>
			{
				action(new Error(500, null), null);
            }).Go();
        }
		/**
         *  获取用户牌局记录
         */
		public void pokerRecordList(Action<Error, UserPokerRecord> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			HttpUtil.Http.Get(URLManager.recordList("10","1")).OnSuccess(result =>
			{
				if (result != null)
				{
					UserPokerRecord userPokerRecordList = JsonMapper.ToObject<UserPokerRecord>(result);
					action(null, userPokerRecordList);
				}
			}).OnFail(result =>
			{
				action(new Error(500, null), null);
			}).GoSync();
		}
		#endregion
	}
}

