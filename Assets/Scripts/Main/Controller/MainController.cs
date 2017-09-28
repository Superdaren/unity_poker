using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MainController : MonoBehaviour
{
    // 右上角的用户信息模块
    public GameObject userInfoView;
    // 钻石购买界面
    public GameObject diamondBuyView;
    // 商品购买页面
    public GameObject shopingView;
    // 选择房间界面
    public GameObject chooseRoomView;
    // 用户中心界面
    public GameObject userCenterView;
    // 用户中心弹层
    public GameObject userCenter;
	// 自定义头像弹层
	public GameObject defineAvatar;
    // 签到界面
    public GameObject signInView;
    // 任务界面
    public GameObject taskView;
	// 设置界面
	public GameObject settingView;

	void Start()
	{
        // 进入主界面, 刷新右上角用户信息
        UserInfoRefreshManager.refreshUserInfo(null);
        initUserInfoView();
        initUIPosition();

        // 检查当前是否有签到,没有签到择弹出签到窗口
        checkIsSignIn();
	}

    /**
     * 初始化我的信息
     */
    public void initUserInfoView() {
        Button diamondBtn = userInfoView.Find<Button>(userInfoView.name + "/UserdiamondBtn");
        diamondBtn.onClick.AddListener(() => {
            PKAnimateTool.popUpView(diamondBuyView);
        });
        Button goldBtn = userInfoView.Find<Button>(userInfoView.name + "/UserGoldBtn");
        goldBtn.onClick.AddListener(() => {
            shopingView.SetActive(true);
        });
    }

    /**
     * 检查当天是否已经签到
     */
    void checkIsSignIn() {
        if(UserManager.Instance().userInfo.is_checkin == 0) {
            PKAnimateTool.popUpView(signInView);
        }
    }

    /**
     * 初始化UI的位置
     */
    void initUIPosition() {
        var position = chooseRoomView.GetComponent<RectTransform>().localPosition;
        chooseRoomView.transform.localPosition = new Vector3(position.x + gameObject.GetComponent<RectTransform>().rect.width, position.y,position.z);
    }

    /**
     * ============================点击事件===================================
     */

    /**
     * 点击购买金币
     */
    public void goldBuyBtnClick() {
        shopingView.SetActive(true);
    }

    /**
     * 点击选择房间
     */
    public void chooseRoomBtnClick() {
        var position = chooseRoomView.GetComponent<RectTransform>().localPosition;
        gameObject.transform.DOLocalMoveX(-gameObject.GetComponent<RectTransform>().rect.width, 0.5f, false);
        chooseRoomView.transform.DOLocalMoveX(0, 0.5f, false);
    }
    /**
     * 点击底部菜单
     */
    public void bottomMenuItemClick(int index)
    {
        switch (index)
        {
            case 1: // 消息
                {

                    break;
                }
            case 2: // 活动
                {
                    PKAnimateTool.popUpView(taskView);
                    break;
                }
            case 3: // 反馈
                {

                    break;
                }
            case 4: // 设置
                {
                    openSettings();
                    break;
                }
        }
    }

    /**
     * 点击左部菜单
     */
    public void leftMenuItemClick(int index)
	{
		switch (index)
		{
			case 1: // 签到
				{
                    PKAnimateTool.popUpView(signInView);
					break;
				}
			case 2: // 训练营
				{
					
					break;
				}
			case 3: // 邀请用户
				{

					break;
				}
		}
	}
	/**
     * 点击大厅头像 
     */
    public void openUserCenter() {
		userCenter.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
		userCenterView.SetActive(true);
		userCenter.SetActive(true);
		userCenter.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
    }
	/**
	 * 点击设置按钮
	 */
	public void openSettings()
	{
		settingView.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
		settingView.SetActive(true);
		settingView.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
	}

}
