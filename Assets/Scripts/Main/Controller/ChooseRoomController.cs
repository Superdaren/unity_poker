﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using DG.Tweening;

public class ChooseRoomController : MonoBehaviour {

    MainHandle mainHandle;
    public GameObject mainController;

    public void toRoom(int roomId) {
        NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.room_get_table_ack, (tableAction) =>
		{
            RoomGetTableAck tableAck = (RoomGetTableAck)tableAction;
            MainData.Instance().roomId = roomId;
            MainData.Instance().tableInfo = tableAck.Table;
            for (int i = 0; i < tableAck.Table.Players.Count; i++){
                PlayerInfo info = tableAck.Table.Players[i];
                if (UserManager.Instance().authModel.user_id == info.Id){
                    MainData.Instance().selfInfo = info;
                    break;
                }
            }
            Debug.Log("牌桌id: " + MainData.Instance().tableInfo.Id);
            Application.LoadLevel("dynamic_room");
        });
        mainHandle.roomPlayerJoinReq(roomId);
    }

	/**
     * 注册loginHandle
     */
	void registHandle()
	{
        mainHandle = new MainHandle();
		NetCore.Instance.registHandle(mainHandle);
	}

	// Use this for initialization
	void Start () {

        registHandle();

		mainHandle.roomList((error, result) =>
		{
			if (error == null)
			{
                for (int i = 0; i < result.list.Count; i ++)
				{
                    Room room = result.list[i];
                    GameObject.Find("Button-" + (i + 1) + "/Max").GetComponent<Text>().text = room.max_carry.ToString();
                    GameObject.Find("Button-" + (i + 1) + "/Blind").GetComponent<Text>().text = room.small_blind.ToString() + "/" + room.big_blind.ToString();
					GameObject.Find("Button-" + (i + 1)).GetComponent<Button>().onClick.AddListener(() =>
					{
                        toRoom(room.id);
					});
				}
			}
		});

	}
	
	/**
	 * =========================点击事件=======================
	 */

    /**
     * 点击返回按钮
     */
    public void backBtnClick() {
		var position = gameObject.GetComponent<RectTransform>().position;
        gameObject.transform.DOLocalMoveX(position.x + mainController.GetComponent<RectTransform>().rect.width, 0.5f, false);
        mainController.transform.DOLocalMoveX(0 , 0.5f, false);
    }
}
