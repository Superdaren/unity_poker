using NetProto;
using System;
using UnityEngine;

public class ReConnectManager : MonoBehaviour
{
    private ReConnectHandle reConnectHandle;

    private void Start()
    {
        reConnectHandle = new ReConnectHandle();
        NetCore.Instance.registHandle(reConnectHandle);
        // 设置断开连接的回调方法
        NetCore.Instance.DisConnectAction = DisConnect;
        // 设置重连成功的回调方法（只有在没有登录的情况调用）
        NetCore.Instance.ReConnectSuccessAction = ReconnectSuccess;
        // 设置重连失败的回调方法
        NetCore.Instance.ReConnectFailAction = ReconnectFail;

        // 玩家断线重连 牌桌信息回复
        NetCore.Instance.setActionForever(NetProto.Api.ENetMsgId.room_player_reconnect_ack);
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_player_reconnect_ack, (roomPlayerReconnectAck) =>
        {
            NetProto.RoomPlayerReconnectAck roomPlayerReconnectAction = (NetProto.RoomPlayerReconnectAck)roomPlayerReconnectAck;
            // 表示玩家不在牌桌上，留在当前界面
            if (roomPlayerReconnectAction.BaseAck.Ret == 0)
            {
                this.isShow = false;
                if (gameObject.scene.name.Equals("dynamic_room"))
                {
                    // 表示玩家不在牌桌上，返回首页
                    Application.LoadLevel("main");
                }
            }
            else
            {
                MainData.Instance().tableInfo = roomPlayerReconnectAction.Table;
                bool isUpdateSelfInfo = false;
                for (int i = 0; i < roomPlayerReconnectAction.Table.Players.Count; i++)
                {
                    NetProto.PlayerInfo info = roomPlayerReconnectAction.Table.Players[i];
                    if (UserManager.Instance().authModel.user_id == info.Id)
                    {
                        isUpdateSelfInfo = true;
                        MainData.Instance().selfInfo = info;
                        break;
                    }
                }
                if (isUpdateSelfInfo)
                {
                    Application.LoadLevel("dynamic_room");
                }
                else
                {
                    // 表示玩家不在牌桌上，返回首页
                    Application.LoadLevel("main");
                }
            }
        });
    }

    private void Update()
    {
        ShowLoading(isShow);
    }

    private bool isShow = false;
    private GameObject loadingObj;

    // 断开网络时调用
    public void DisConnect()
    {
        this.isShow = true;
    }

    // 重连成功时调用
    public void ReconnectSuccess()
    {
        this.isShow = false;
    }

    // 重连失败时调用
    public void ReconnectFail()
    {

    }

    // 显示加载界面
    public void ShowLoading(bool active)
    {
        if (loadingObj == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            loadingObj = Instantiate(Resources.Load("Prefabs/Loading"), canvas.transform) as GameObject;
            loadingObj.name = "loading";
        }
        loadingObj.SetActive(active);
    }

}
