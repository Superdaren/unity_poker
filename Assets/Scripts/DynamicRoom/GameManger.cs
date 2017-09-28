﻿using UnityEngine;
using NetProto;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{

    GameHandle mGameHandle;
    GameControler mGameControler;
    ButtonControler mButtonControler;
    ChatControler mChatControler;

    // 通报底池
    RoomPotAck roomPotAckAction;
    public void RoomPot()
    {
        mGameControler.PutChip(roomPotAckAction.Pot);
    }

    // Use this for initialization
    void Start()
    {
        /* 注册请求 */
        mGameHandle = new GameHandle();
        NetCore.Instance.registHandle(mGameHandle);

        mGameControler = GetComponent<GameControler>();
        mGameControler.mGameHandle = mGameHandle;
        mButtonControler = GetComponent<ButtonControler>();
        mButtonControler.mGameHandle = mGameHandle;
        mChatControler = GetComponent<ChatControler>();
        mChatControler.mGameHandle = mGameHandle;

        // 初始化房间信息
        InitRoomInfo();

        // 通知更新牌桌(换桌的时候会通知一次)
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_get_table_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_get_table_ack, (roomGetTableAck) =>
        {
            NetProto.RoomGetTableAck roomGetTableAction = (NetProto.RoomGetTableAck)roomGetTableAck;
            MainData.Instance().tableInfo = roomGetTableAction.Table;
            foreach (var playerInfo in roomGetTableAction.Table.Players)
            {
                if (UserManager.Instance().authModel.user_id == playerInfo.Id)
                {
                    MainData.Instance().selfInfo = playerInfo;
                    break;
                }
            }
            // 重置UI
            mGameControler.ResetUIAfterChangeTable();
            // 初始化房间信息
            InitRoomInfo();
            // 隐藏换桌加载组件
            mGameControler.SetChangeTableLoadingActive(false);
        });

        // 玩家加入游戏
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_join_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_join_ack, (roomPlayerJoinAck) =>
        {
            RoomPlayerJoinAck roomPlayerJoinAction = (RoomPlayerJoinAck)roomPlayerJoinAck;
            // 更新玩家信息
            mGameControler.UpdateAfterPlayerJoin(roomPlayerJoinAction.Player);
        });

        // 玩家离开游戏
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_gone_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_gone_ack, (roomPlayerGoneAck) =>
        {
            RoomPlayerGoneAck roomPlayerGoneAction = (RoomPlayerGoneAck)roomPlayerGoneAck;
            // 更新玩家信息
            mGameControler.UpdateAfterPlayerGone(roomPlayerGoneAction.Player);
        });

        // 通报庄家
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_button_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_button_ack, (roomButtonAck) =>
        {
            RoomButtonAck roomButtonAction = (RoomButtonAck)roomButtonAck;
            // 设置庄家
            mGameControler.SetDealer(roomButtonAction.ButtonPos);
        });

        // 发牌
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_deal_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_deal_ack, (roomDealAck) =>
        {
            RoomDealAck roomDealAction = (RoomDealAck)roomDealAck;
            string cardType = CardInfoUtil.GetCardType(roomDealAction.HandLevel);
            switch (roomDealAction.Action)
            {
                case CardInfoUtil.ACTION_PREFLOP:
                    mGameControler.DealHandCard(roomDealAction.Cards, cardType);
                    break;
                case CardInfoUtil.ACTION_FLOP:
                    mGameControler.DealPublicCard(roomDealAction.Cards, cardType);
                    break;
                case CardInfoUtil.ACTION_TURN:
                    mGameControler.DealPublicCard(roomDealAction.Cards, cardType);
                    break;
                case CardInfoUtil.ACTION_RIVER:
                    mGameControler.DealPublicCard(roomDealAction.Cards, cardType);
                    break;
                default:
                    break;
            }
        });

        // 通报当前下注玩家
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_action_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_action_ack, (roomActionAck) =>
        {
            RoomActionAck roomActionAction = (RoomActionAck)roomActionAck;
            mGameControler.TurnNext(roomActionAction.Pos, roomActionAction.BaseBet);
        });

        // 玩家押注
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_bet_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_bet_ack, (roomPlayerBetAck) =>
        {
            RoomPlayerBetAck roomPlayerBetAction = (RoomPlayerBetAck)roomPlayerBetAck;
            if (roomPlayerBetAction.Action.Equals(PlayerInfoUtil.ACTION_FLOD))
            {
                mGameControler.Discard(roomPlayerBetAction.Pos);
            }
            else
            {
                mGameControler.PlayerBetting(roomPlayerBetAction);
            }
        });

        // 通报底池
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_pot_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_pot_ack, (roomPotAck) =>
        {
            roomPotAckAction = (RoomPotAck)roomPotAck;
            Invoke("RoomPot", 0.5f);
        });

        // 摊牌比牌
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_showdown_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_showdown_ack, (roomShowdownAck) =>
        {
            RoomShowdownAck roomShowdownAction = (RoomShowdownAck)roomShowdownAck;
            mGameControler.GameOver(roomShowdownAction);
        });

        // 通报玩家站起
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_standup_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_standup_ack, (roomPlayerStandupAck) =>
        {
            RoomPlayerStandupAck roomPlayerStandupAction = (RoomPlayerStandupAck)roomPlayerStandupAck;
            // 更新玩家信息
            mGameControler.UpdateAfterPlayerStandUp(roomPlayerStandupAction);
        });

        // 通报玩家坐下
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_sitdown_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_sitdown_ack, (roomPlayerSitdownAck) =>
        {
            RoomPlayerSitdownAck roomPlayerSitdownAction = (RoomPlayerSitdownAck)roomPlayerSitdownAck;
            // 更新玩家信息
            mGameControler.UpdateAfterPlayerSitDown(roomPlayerSitdownAction.Player);
        });

        // 自动坐下等待玩家数通报
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_auto_sitdown_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_auto_sitdown_ack, (roomPlayerAutoSitdownAck) =>
        {
            RoomPlayerAutoSitdownAck roomPlayerAutoSitdownAction = (RoomPlayerAutoSitdownAck)roomPlayerAutoSitdownAck;
            mButtonControler.UpdateWaitCount(roomPlayerAutoSitdownAction.Num);
        });

        // 踢出玩家
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_kicked_outAck);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_kicked_outAck, (kickedOutAck) =>
        {
            KickedOutAck kickedOutAction = (KickedOutAck)kickedOutAck;
        });

        // 关闭牌桌，服务进行维护时通报
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_shutdown_table_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_shutdown_table_ack, (roomShutdownTableAck) =>
        {
            Application.LoadLevel("main");
        });

        // 牌桌聊天消息回复
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_table_chat_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_table_chat_ack, (roomTableChatAck) =>
        {
            RoomTableChatAck roomTableChatAction = (RoomTableChatAck)roomTableChatAck;
            mChatControler.AddMessage(roomTableChatAction.Body.ToByteArray());
        });
    }

    // 初始化房间信息
    public void InitRoomInfo()
    {
        if (MainData.Instance() == null || MainData.Instance().tableInfo == null)
        {
            return;
        }
        PlayerInfo selfInfo = MainData.Instance().selfInfo;
        TableInfo tableInfo = MainData.Instance().tableInfo;

        // 初始化Info
        mGameControler.InitInfo(selfInfo, tableInfo);

        // 初始化玩家信息
        mGameControler.InitPlayerInfo(tableInfo.Players);

        // 当前玩家数大于3（玩家数中包括自己）且牌局还未结束，初始化牌桌信息
        if (tableInfo.N >= 2 && tableInfo.Status == 1)
        {
            // 初始化公共牌信息
            mGameControler.InitPublicCardInfo(tableInfo.Cards);

            // 初始化底池
            mGameControler.InitPotInfo(tableInfo.Pot);

            // 初始化玩家手牌信息
            mGameControler.InitPlayerHandCard(tableInfo.Players);

            // 初始化已下注筹码
            mGameControler.InitPlayerChips(tableInfo.Players);

            // 光标转到当前玩家
            mGameControler.TrunCurrentPalyer(tableInfo.Players, tableInfo.Bet);

            // 设置庄家位置
            mGameControler.SetDealer(tableInfo.Button);
        }
        else
        {
            mGameControler.SetPlayerReadyAction(tableInfo.Players);
        }
    }
}
