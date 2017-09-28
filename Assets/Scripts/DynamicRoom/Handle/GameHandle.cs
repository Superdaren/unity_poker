using Google.Protobuf;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetProto
{
    public class GameHandle : NetHandle
    {
        public GameHandle()
        {
            // 通报牌桌信息
            handlerMap.Add(Api.ENetMsgId.room_get_table_ack, roomGetTableAck);
            // 玩家加入游戏
            handlerMap.Add(Api.ENetMsgId.room_player_join_ack, roomPlayerJoinAck);
            // 通报本局庄家
            handlerMap.Add(Api.ENetMsgId.room_button_ack, roomButtonAck);
            // 通报发牌
            handlerMap.Add(Api.ENetMsgId.room_deal_ack, roomDealAck);
            // 玩家下注回复
            handlerMap.Add(Api.ENetMsgId.room_player_bet_ack, roomPlayerBetAck);
            // 服务器广播离开房间的玩家
            handlerMap.Add(Api.ENetMsgId.room_player_gone_ack, roomPlayerGoneAck);
            // 通报当前下注玩家
            handlerMap.Add(Api.ENetMsgId.room_action_ack, roomActionAck);
            // 通报奖池
            handlerMap.Add(Api.ENetMsgId.room_pot_ack, roomPotAck);
            // 摊牌和比牌
            handlerMap.Add(Api.ENetMsgId.room_showdown_ack, roomShowdownAck);
            // 通报玩家站起
            handlerMap.Add(Api.ENetMsgId.room_player_standup_ack, roomPlayerStandupAck);
            // 通报玩家坐下
            handlerMap.Add(Api.ENetMsgId.room_player_sitdown_ack, roomPlayerSitdownAck);
            // 自动坐下等待玩家数通报
            handlerMap.Add(Api.ENetMsgId.room_player_auto_sitdown_ack, roomPlayerAutoSitdownAck);
            // 踢出玩家
            handlerMap.Add(Api.ENetMsgId.room_kicked_outAck, kickedOutAck);
            // 关闭牌桌，服务进行维护时通报
            handlerMap.Add(Api.ENetMsgId.room_shutdown_table_ack, roomShutdownTableAck);
            //  牌桌聊天消息回复
            handlerMap.Add(Api.ENetMsgId.room_table_chat_ack, roomTableChatAck);
        }

        #region request请求
        /**
         * 玩家下注请求
         */
        public void RoomPlayerBetReq(int betNum)
        {
            RoomPlayerBetReq roomPlayerBetReq = new RoomPlayerBetReq();
            roomPlayerBetReq.TableId = MainData.Instance().tableInfo.Id;
            roomPlayerBetReq.Bet = betNum;

            NetCore.Instance.Send(Api.ENetMsgId.room_player_bet_req, roomPlayerBetReq);
        }

        /**
         * 玩家离开牌桌请求
         */
        public void RoomPlayerGoneReq()
        {
            RoomPlayerGoneReq roomPlayerGoneReq = new RoomPlayerGoneReq();
            roomPlayerGoneReq.TableId = MainData.Instance().tableInfo.Id;

            NetCore.Instance.Send(Api.ENetMsgId.room_player_gone_req, roomPlayerGoneReq);
        }

        /**
         * 玩家站起请求
         */
        public void RoomPlayerStandupReq()
        {
            RoomPlayerStandupReq roomPlayerStandupReq = new RoomPlayerStandupReq();
            roomPlayerStandupReq.TableId = MainData.Instance().tableInfo.Id;

            NetCore.Instance.Send(Api.ENetMsgId.room_player_standup_req, roomPlayerStandupReq);
        }

        /**
         * 玩家坐下请求
         */
        public void RoomPlayerSitdownReq()
        {
            RoomPlayerSitdownReq roomPlayerSitdownReq = new RoomPlayerSitdownReq();
            roomPlayerSitdownReq.TableId = MainData.Instance().tableInfo.Id;

            NetCore.Instance.Send(Api.ENetMsgId.room_player_sitdown_req, roomPlayerSitdownReq);
        }

        /**
         * 玩家自动坐下请求
         */
        public void RoomPlayerAutoSitdownReq()
        {
            RoomPlayerAutoSitdownReq roomPlayerAutoSitdownReq = new RoomPlayerAutoSitdownReq();

            NetCore.Instance.Send(Api.ENetMsgId.room_player_auto_sitdown_req, roomPlayerAutoSitdownReq);
        }

        /**
         * 玩家换桌请求
         */
        public void RoomPlayerChangeTableReq()
        {
            RoomPlayerChangeTableReq roomPlayerChangeTableReq = new RoomPlayerChangeTableReq();

            NetCore.Instance.Send(Api.ENetMsgId.room_player_change_table_req, roomPlayerChangeTableReq);
        }

        /**
         * 请求牌桌信息
         */
        public void RoomGetTableReq()
        {
            RoomGetTableReq roomGetTableReq = new RoomGetTableReq();
            roomGetTableReq.RoomId = MainData.Instance().roomId;
            roomGetTableReq.TableId = MainData.Instance().tableInfo.Id;

            NetCore.Instance.Send(Api.ENetMsgId.room_get_table_req, roomGetTableReq);
        }

        /**
         * 牌桌发送聊天消息
         */
        public void RoomTableChatReq(byte[] body)
        {
            RoomTableChatReq roomTableChatReq = new RoomTableChatReq();
            roomTableChatReq.Id = MainData.Instance().tableInfo.Id;
            roomTableChatReq.Body = ByteString.AttachBytes(body);

            NetCore.Instance.Send(Api.ENetMsgId.room_table_chat_req, roomTableChatReq);
        }


        #endregion

        #region action 响应事件

        /**
         * 查询牌桌信息回复 (当玩家加入牌桌后，服务器会向此用户推送牌桌信息)
         */
        public object roomGetTableAck(byte[] data)
        {
            RoomGetTableAck tableAction = RoomGetTableAck.Parser.ParseFrom(data);

            return tableAction;
        }

        /**
         * 通报加入游戏的玩家
         */
        public object roomPlayerJoinAck(byte[] data)
        {
            RoomPlayerJoinAck roomPlayerJoinAction = RoomPlayerJoinAck.Parser.ParseFrom(data);

            return roomPlayerJoinAction;
        }

        /**
         * 通报本局庄家 (服务器广播此消息，代表游戏开始并确定本局庄家)
         */
        public object roomButtonAck(byte[] data)
        {
            RoomButtonAck roomButtonAction = RoomButtonAck.Parser.ParseFrom(data);

            return roomButtonAction;
        }

        /**
         * 发牌 - 共有四轮发牌，按顺序分别为：preflop (底牌), flop (翻牌), turn (转牌), river(河牌)
         */
        public object roomDealAck(byte[] data)
        {
            RoomDealAck roomDealAction = RoomDealAck.Parser.ParseFrom(data);

            return roomDealAction;
        }

        /**
         * 玩家下注回复
         */
        public object roomPlayerBetAck(byte[] data)
        {
            RoomPlayerBetAck roomPlayerBetAction = RoomPlayerBetAck.Parser.ParseFrom(data);

            return roomPlayerBetAction;
        }

        /**
         * 服务器广播离开房间的玩家
         */
        public object roomPlayerGoneAck(byte[] data)
        {
            RoomPlayerGoneAck roomPlayerGoneAction = RoomPlayerGoneAck.Parser.ParseFrom(data);

            return roomPlayerGoneAction;
        }

        /**
         * 通报当前下注玩家,轮到该玩家
         */
        public object roomActionAck(byte[] data)
        {
            RoomActionAck roomActionAction = RoomActionAck.Parser.ParseFrom(data);

            return roomActionAction;
        }

        /**
         * 通报奖池
         */
        public object roomPotAck(byte[] data)
        {
            RoomPotAck roomPotAction = RoomPotAck.Parser.ParseFrom(data);

            return roomPotAction;
        }

        /**
         *  摊牌和比牌
         */
        public object roomShowdownAck(byte[] data)
        {
            RoomShowdownAck roomShowdownAction = RoomShowdownAck.Parser.ParseFrom(data);

            return roomShowdownAction;
        }

        /**
         *  通报玩家站起
         */
        public object roomPlayerStandupAck(byte[] data)
        {
            RoomPlayerStandupAck roomPlayerStandupAction = RoomPlayerStandupAck.Parser.ParseFrom(data);

            return roomPlayerStandupAction;
        }

        /**
         *  通报玩家坐下
         */
        public object roomPlayerSitdownAck(byte[] data)
        {
            RoomPlayerSitdownAck roomPlayerSitdownAction = RoomPlayerSitdownAck.Parser.ParseFrom(data);

            return roomPlayerSitdownAction;
        }

        /**
         *  自动坐下等待玩家数通报
         */
        public object roomPlayerAutoSitdownAck(byte[] data)
        {
            RoomPlayerAutoSitdownAck roomPlayerAutoSitdownAction = RoomPlayerAutoSitdownAck.Parser.ParseFrom(data);

            return roomPlayerAutoSitdownAction;
        }

        /**
         *  踢出玩家
         */
        public object kickedOutAck(byte[] data)
        {
            KickedOutAck kickedOutAction = KickedOutAck.Parser.ParseFrom(data);

            return kickedOutAction;
        }

        /**
         *   关闭牌桌，服务进行维护时通报
         */
        public object roomShutdownTableAck(byte[] data)
        {
            RoomShutdownTableAck roomShutdownTableAction = RoomShutdownTableAck.Parser.ParseFrom(data);

            return roomShutdownTableAction;
        }

        /**
         *    牌桌聊天消息回复
         */
        public object roomTableChatAck(byte[] data)
        {
            RoomTableChatAck roomTableChatAction = RoomTableChatAck.Parser.ParseFrom(data);

            return roomTableChatAction;
        }
        #endregion

        #region Http请求

        /**
         * 请求上一局牌局数据
         */ 
        public void GetLastGame(string tableId, Action<Error, LastGameResult> action)
        {
            HttpUtil.Http.Get(URLManager.lastGameUrl(tableId)).OnSuccess((result) =>
            {
                if (result != null)
                {
                    LastGameResult resultObj = JsonMapper.ToObject<LastGameResult>(result);
                    if (resultObj != null && resultObj.ret == 1)
                    {
                        action(null, resultObj);
                    }
                    else
                    {
                        action(new Error(resultObj.ret, resultObj.msg), resultObj);
                    }
                }
            }).OnFail((result) =>
            {
                action(new Error(500, result.Message), null);
            }).GoSync();
        }

        #endregion
    }
}
