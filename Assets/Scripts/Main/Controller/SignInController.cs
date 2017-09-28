using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SignInController : MonoBehaviour
{
    // 签到列表
    public SignInResult signInResult;
    // 签到天数view集合
    public List<GameObject> signInObjectList;

    // 领取日常签到奖励按钮遮罩
    GameObject receiveMask;

    GameObject receiveSevenMask;
    
	// Use this for initialization
	void Start()
	{
        // 获取签到列表
        requestSignInList();
	}

    /**
     * 获取签到列表
     */
    void requestSignInList() {
        SignInHandle.requestSignInList((error, result) => {
            if(error == null) {
                if(result.list.Length < 9) {
                    PopUtil.ShowMessageBoxWithConfirm("提示", "后台数据错误!");
                    return;
                }
                signInResult = result;
                loadSignInView();
            }
        });
    }

    /**
     * 加载签到列表
     */
    void loadSignInView() {
		GameObject singDayItemPrefab = Resources.Load("Prefabs/SingDayItem") as GameObject;
        GameObject contentView = GameObject.Find("SignInView/ContentView/GridView");

        // 如果用户签到大于7天,将第八天的奖励替换掉第七天的奖励
        if(signInResult.already_checkin >= 7) {
            signInResult.list[6] = signInResult.list[7];
        }

        // 设置正常签到的item
        for (int i = 0; i < signInResult.list.Length - 2; i++)
		{
            SignIn signIn = signInResult.list[i];
			GameObject signDayItem = Instantiate(singDayItemPrefab) as GameObject;
            float width = signDayItem.GetComponent<RectTransform>().sizeDelta.x;
            float heigh = signDayItem.GetComponent<RectTransform>().sizeDelta.y;

			signDayItem.transform.parent = contentView.transform;
            signDayItem.name = "SingDayItem" + signIn.days;
            signDayItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            signDayItem.GetComponent<RectTransform>().localPosition = new Vector3(width * (i % 4) , -(i / 4 * (heigh + 10)), 0);

			setSignInView(signDayItem, signIn);
			signInObjectList.Add(signDayItem);
		}

        // 设置超过七天的item
        GameObject signSevenReward = GameObject.Find("SignInView/ContentView/SignSevenReward");
        setSevenSignView(signSevenReward, signInResult.list[8]);

        // 领取日常签到奖励
        receiveMask = GameObject.Find("SignInView/ContentView/ReceiveBtn/Mask");
        Button receiveBtn = contentView.Find<Button>("SignInView/ContentView/ReceiveBtn/ReceiveBtn");
        if(UserManager.Instance().userInfo.is_checkin == 1) {
            receiveMask.SetActive(true);
        }else {
            receiveMask.SetActive(false);
        }
        // 点击领取日常签到奖励
        receiveBtn.onClick.AddListener(() => {
            receiveSignInReward(0);
        });
    }

    /**
     * 设置日常签到item的内容
     */
    void setSignInView(GameObject signDayItem, SignIn signIn) {
        Text title = signDayItem.Find<Text>(signDayItem.name + "/DayText");
        Image rewardImage = signDayItem.Find<Image>(signDayItem.name + "/RewardImage");
        GameObject signInedImage = GameObject.Find(signDayItem.name + "/SignInedImage");
        Text rewardText = signDayItem.Find<Text>(signDayItem.name + "/RewardText");
        GameObject mask = GameObject.Find(signDayItem.name + "/Mask");

        title.text = DayUtil.getChineseDayWithInt(signIn.days);
        rewardText.text = signIn.image_describe;

        float width = signDayItem.GetComponent<RectTransform>().sizeDelta.x;

        if(signIn.days == 7 || signIn.days == 8) {
            signDayItem.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 2, 180);
        }

        // 已经签到的显示打勾
        if(signIn.days <= signInResult.already_checkin){ // 当前天数小于已经签到的天数
            if(signInResult.already_checkin >= 7 && (signIn.days == 7 || signIn.days == 8)) {    // 已经签到的天数大于7等于7天
                if(UserManager.Instance().userInfo.is_checkin == 1){ // 今天已经签到过了
					signInedImage.SetActive(true);
					mask.SetActive(true);
                } else {
					signInedImage.SetActive(false);
					mask.SetActive(false);
                }
            } else {
				signInedImage.SetActive(true);
				mask.SetActive(true);
            }
        } else {
            signInedImage.SetActive(false);
            mask.SetActive(false);
        }
    }

	/**
     * 设置超过7天签到item的内容
     */
	void setSevenSignView(GameObject signDayItem, SignIn sign){
		Text topTitle = signDayItem.Find<Text>("SignInView/ContentView/SignSevenReward/TopTitle");
		Text GoldNumText = signDayItem.Find<Text>("SignInView/ContentView/SignSevenReward/GoldNumText");

        topTitle.text = "已连续签到" + signInResult.already_checkin + "天";
        GoldNumText.text = sign.image_describe;

		// 领取签到超过7天奖励
		Button receiveSevenBtn = signDayItem.Find<Button>("SignInView/ContentView/SignSevenReward/ReceiveSevenBtn/ReceiveSevenBtn");
        receiveSevenMask = GameObject.Find("SignInView/ContentView/SignSevenReward/ReceiveSevenBtn/Mask");
        if(signInResult.already_checkin < 7 || signInResult.is_more == 1){
            receiveSevenMask.SetActive(true);
        } else {
            receiveSevenMask.SetActive(false);
        }
        // 点击领取超过7天的额外奖励
		receiveSevenBtn.onClick.AddListener(() =>
		{
            receiveSignInReward(1);
		});
    }

    /**
     * 领取签到奖励请求
     */
    void receiveSignInReward(int isMore) {
		SignInHandle.receiveSignInReward(isMore, (error, result) =>
		{
			if (error == null)
			{
				if (result.ret == 1)
				{
                    if(isMore == 1){
                        receiveSevenMask.SetActive(true);
                        signInResult.is_more = 1;

						SignIn signIn = signInResult.list[8];
						PopUtil.ShowSignInSuccessView(signIn.image_describe);
                    } else {
						int currenItem = result.checkin_days > 7 ? 8 : result.checkin_days;

						GameObject signInedImage = GameObject.Find("SignInView/ContentView/GridView/SingDayItem" + currenItem + "/SignInedImage");
						GameObject mask = GameObject.Find("SignInView/ContentView/GridView/SingDayItem" + currenItem + "/Mask");
						signInedImage.SetActive(true);
						mask.SetActive(true);
						receiveMask.SetActive(true);
						UserManager.Instance().userInfo.is_checkin = 1;

                        // 更新额外奖励的
                        GameObject signSevenReward = GameObject.Find("SignInView/ContentView/SignSevenReward");
						Text topTitle = signSevenReward.Find<Text>("SignInView/ContentView/SignSevenReward/TopTitle");
                        signInResult.already_checkin = signInResult.already_checkin + 1;
						topTitle.text = "已连续签到" + signInResult.already_checkin + "天";

						SignIn signIn = signInResult.list[currenItem - 1];
						PopUtil.ShowSignInSuccessView(signIn.image_describe);
                    }
				}
				else
				{
					PopUtil.ShowMessageBoxWithConfirm("提示", "签到失败");
				}
			}
			else
			{
				PopUtil.ShowMessageBoxWithConfirm("提示", "签到失败");
			}
		});
    }

    /**
     * 点击关闭按钮
     */
    public void closeBtnClick() {
        PKAnimateTool.closePopUpView(gameObject);
    }
}
