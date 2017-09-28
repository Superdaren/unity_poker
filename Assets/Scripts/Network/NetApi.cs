
/**
 * 消息类型
 */
namespace NetProto.Api
{
	public enum ENetMsgId
    {
        heart_beat_req                  = 0,            // 心跳包..
        heart_beat_ack                  = 1,            // 心跳包回复
        user_login_req                  = 10,           // 登陆请求
        user_login_ack                  = 11,           // 登陆回复
        client_error_ack                = 13,           // 客户端错误
        get_seed_req                    = 30,           // socket通信加密使用
        get_seed_ack                    = 31,           // socket通信加密使用
        proto_ping_req                  = 1001,         // ping
        proto_ping_ack                  = 1002,         // ping回复

        room_kicked_outAck              = 40,           // 踢出玩家
        room_set_table_req              = 2003,         // 创建牌桌
        room_set_table_ack              = 2004,         // 创建牌桌回复
		room_player_join_req            = 2101,         // 玩家加入游戏
        room_get_table_req              = 2005,         // 查询牌桌信息
		room_get_table_ack              = 2006,         // 查询牌桌信息回复 (当玩家加入牌桌后，服务器会向此用户推送牌桌信息)
        room_player_join_ack            = 2102,         // 通报加入游戏的玩家
        room_player_gone_req            = 2103,         // 玩家离开牌桌
        room_player_gone_ack            = 2104,         // 服务器广播离开房间的玩家
        room_player_bet_req             = 2105,         // 玩家下注
        room_player_bet_ack             = 2106,         // 玩家下注回复
        room_button_ack                 = 2107,         // 通报本局庄家
        room_deal_ack                   = 2108,         // 发牌
        room_pot_ack                    = 2109,         // 通报奖池
        room_action_ack                 = 2110,         // 通报当前下注玩家
        room_showdown_ack               = 2111,         // 摊牌和比牌
        room_player_standup_req         = 2112,         // 玩家站起
        room_player_standup_ack         = 2113,         // 玩家站起通报
        room_player_sitdown_req         = 2114,         // 玩家坐下
        room_player_sitdown_ack         = 2115,         // 玩家坐下通报
        room_player_change_table_req    = 2116,         // 玩家换桌
        room_shutdown_table_ack         = 2117,         // 关闭牌桌，服务进行维护时通报
        room_player_logout_req          = 2118,         // 玩家退出游戏
        room_player_reconnect_ack       = 2119,         // 玩家断线重连 牌桌信息回复
        room_table_chat_ack             = 2120,         // 牌桌聊天消息回复
        room_table_chat_req             = 2121,         // 牌桌发送聊天消息
        room_player_auto_sitdown_ack    = 2122,         // 自动坐下等待玩家数通报
        room_player_auto_sitdown_req    = 2123,         // 玩家加入自动坐下队列
    };
}
