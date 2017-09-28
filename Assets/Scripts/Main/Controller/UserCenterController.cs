using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using System;
using DG.Tweening;

public class UserCenterController : MonoBehaviour
{
    MainHandle mainHandle;
	// 用户中心界面
	public GameObject userCenterView;
	// 用户中心弹层
	public GameObject userCenter;
	// 自定义头像弹层
	public GameObject defineAvatar;
    public bool isEdit = false;
	// 钻石购买界面
	public GameObject diamondBuyView;
	// 商品购买页面
	public GameObject shopingView;
    // 背包界面
    public GameObject bag;
	// 牌局界面
	public GameObject pokerRecord;
	// Use this for initialization
	void Start()
	{
		//Debug.Log("123456:"+UserManager.getUserInfoFromUserDefault().avatar);
		mainHandle = new MainHandle();
		NetCore.Instance.registHandle(mainHandle);
        // 默认编辑图像
        Image editImg1 = userCenter.Find<Image>(userCenter.name + "/UserInfo/Name/Edit");
        editImg1.SetLocalImage("Textures/main/main_edit");

		GameObject.Find(userCenter.name + "/UserInfo/Name/UserName").SetActive(true);
		GameObject.Find(userCenter.name + "/UserInfo/Name/NameField").SetActive(false);
        initVal();

	}

	// Update is called once per frame
	void Update()
	{

	}
    protected internal void initVal()
    {
		// 头像
        Image avatarImg = userCenter.Find<Image>(userCenter.name + "/UserAvatar");
        avatarImg.SetWebImage(UserManager.getUserInfoFromUserDefault().avatar,"");

		// ID
        Text IDText = userCenter.Find<Text>(userCenter.name + "/UserInfo/ID/Text");
        IDText.text = UserManager.getUserInfoFromUserDefault().id.ToString();

        // 性别
        if(UserManager.getUserInfoFromUserDefault().gender == 2){
            Image sexImg = userCenter.Find<Image>(userCenter.name + "/UserInfo/Name/Sex");
			sexImg.SetLocalImage("Textures/main/default_avatar_girl");
        }

		// 昵称
        Text nameText = userCenter.Find<Text>(userCenter.name + "/UserInfo/Name/UserName");
        nameText.text = UserManager.getUserInfoFromUserDefault().nick_name;

		// 金币
        Text coinText = userCenter.Find<Text>(userCenter.name + "/UserInfo/Coin/Text");
        coinText.text = UserManager.getUserInfoFromUserDefault().balance.ToString();

		// 钻石
        Text diamondText = userCenter.Find<Text>(userCenter.name + "/UserInfo/Diamond/Text");
        diamondText.text = UserManager.getUserInfoFromUserDefault().diamond_balance.ToString();

		// 抓取数据接口
		mainHandle.userDetail((error, result) =>
		{
			if (error == null)
			{
				// 胜率
                Text winRateText = userCenter.Find<Text>(userCenter.name + "/PlayInfo/WinRate/Text");
				winRateText.text = (result.win_rate)*100 + "%";

				// 入局率
                Text inRateText = userCenter.Find<Text>(userCenter.name + "/PlayInfo/InRate/Text");
				inRateText.text = (result.inbound_rate) * 100 + "%";

				// 最大赢取
                Text maxWinText = userCenter.Find<Text>(userCenter.name + "/PlayInfo/MaxWin/Text");
                maxWinText.text = result.best_winner.ToString();

				// 总局数
                Text totalText = userCenter.Find<Text>(userCenter.name + "/PlayInfo/TotalPlay/Text");
				totalText.text = result.total_game.ToString();

                GameObject noMax = GameObject.Find(userCenter.name + "/MaxPoket/NoMax");
                // 最大牌型
                if(result.cards!=null && result.cards.Count > 0){
                    noMax.SetActive(false);
					for (int i = 0; i < result.cards.Count; i++)
					{
						//Debug.Log("Textures/cards/card_" + result.cards[i].suit + result.cards[i].value);
						Image cardImg = userCenter.Find<Image>(userCenter.name + "/MaxPoket/card_" + i);
                        cardImg.SetLocalImage("Textures/cards/card_"+result.cards[i].suit + result.cards[i].value);
					}
                }else{
                    noMax.SetActive(true);
                }

			}
		});

	}
	/**
     * ========================点击事件=============================
     */
	// 自定义头像面板
	public void avatarClick()
    {
		defineAvatar.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
		defineAvatar.SetActive(true);
		defineAvatar.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
	}
	// 关闭按钮
	public void closeBtnClick()
	{
		Tweener tweener = userCenter.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
	    tweener.onComplete = (() =>
		{
		 userCenter.SetActive(false);
		 userCenterView.SetActive(false);
		});
	}
	// 取消按钮
	public void cancelBtnClick()
    {
		Tweener tweener = defineAvatar.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
		  tweener.onComplete = (() =>
		  {
		      defineAvatar.SetActive(false);
		  });

	}
    // 佩戴道具
    public void wearBtnClick()
    {
        //CommonUI.showTotoast(gameObject, "wear tool!");
        Debug.Log("wear tool!");
    }
	// 编辑昵称
	public void editBtnClick()
    {
		Image editImg = userCenter.Find<Image>(userCenter.name + "/UserInfo/Name/Edit");
        Text nameText = userCenter.Find<Text>(userCenter.name + "/UserInfo/Name/UserName");
        InputField nameFieldText = userCenter.Find<InputField>(userCenter.name + "/UserInfo/Name/NameField");
        if(isEdit){
            if (nameFieldText.text == null || nameFieldText.text == "")
			{
				PopUtil.ShowTotoast("昵称不能为空");
				return;
            }else{
				isEdit = false;
				editImg.SetLocalImage("Textures/main/main_edit");
				mainHandle.nickname(nameFieldText.text, (error, result) =>
				 {
					 if (error == null)
					 {
						 //UserManager.Instance().userInfo.nick_name = result;
						 //UserManager.saveUserInfoToUserDefault(UserManager.Instance().userInfo);
						 Text nicknameText = userCenter.Find<Text>(userCenter.name + "/UserInfo/Name/UserName");
						 nicknameText.text = result;
                        UserInfoRefreshManager.refreshUserInfo(null);
					 }
					 else
					 {
						 PopUtil.ShowTotoast("昵称修改失败");
						 return;
					 }
				 });
				GameObject.Find(userCenter.name + "/UserInfo/Name/UserName").SetActive(true);
				GameObject.Find(userCenter.name + "/UserInfo/Name/NameField").SetActive(false);
            }
		
        }else{
            isEdit = true;
            editImg.SetLocalImage("Textures/main/main_checkin");
            nameFieldText.Select();
			GameObject.Find(userCenter.name + "/UserInfo/Name/NameField").SetActive(true);
            nameFieldText.text = nameText.text;
            GameObject.Find(userCenter.name + "/UserInfo/Name/UserName").SetActive(false);
        }
    }

    // 调起拍照功能
    public void takePhotoBtn()
    {
        Debug.Log("taking photos~");
    }

	// 调起本地图片功能
	public void localPicBtn()
	{
		Debug.Log("local pictures~");
	}
	// 背包
    public void bagBtnClick()
    {
        //Debug.Log("getting bag~");
        bag.SetActive(true);
    }
	// 成就
    public void  achieveBtnClick()
    {
        Debug.Log("getting achieve~");
    }
	// 牌局
    public void poketBtnClick()
    {
        //Debug.Log("getting poket~");
        pokerRecord.SetActive(true);
    }
	// 购买金币
    public void coinBuyClick()
    {
        shopingView.SetActive(true);
        userCenterView.SetActive(false);
    }
	// 购买钻石
	public void diamondBuyClick()
	{
		diamondBuyView.SetActive(true);
		userCenterView.SetActive(false);
	}
}
