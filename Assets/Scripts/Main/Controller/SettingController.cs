using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using System;
using DG.Tweening;

public class SettingController : MonoBehaviour
{
	MainHandle mainHandle;
	// 用户中心弹层
	public GameObject settingView;

	// Use this for initialization
	void Start()
	{
        initVal();
	}

	// Update is called once per frame
	void Update()
	{

	}
    protected internal void initVal()
    {
		// ID
		Text IDText = settingView.Find<Text>("ID");
		IDText.text = UserManager.getUserInfoFromUserDefault().id.ToString();

		// Music Status
        int MusicStatus = PrefsUtil.GetInt("setting_music") > 0 ? PrefsUtil.GetInt("setting_music") : 0;
		Text MusicStatusText = settingView.Find<Text>("Option_Music/Status");
        MusicStatusText.text = MusicStatus > 0 ? "开" : "关";
        Image MusicStatusImg = settingView.Find<Image>("Option_Music/Button");
        if (MusicStatus > 0)
        {
            MusicStatusImg.SetLocalImage("Textures/main/setting_btn_open");
        }
        else
        {
            MusicStatusImg.SetLocalImage("Textures/main/setting_btn_close");
        }

		// Sound Status
        int SoundStatus = PrefsUtil.GetInt("setting_sound") > 0 ? PrefsUtil.GetInt("setting_sound") : 0;
		Text SoundStatusText = settingView.Find<Text>("Option_Sound/Status");
		SoundStatusText.text = SoundStatus > 0 ? "开" : "关";
		Image SoundStatusImg = settingView.Find<Image>("Option_Sound/Button");
		if (SoundStatus > 0)
		{
			SoundStatusImg.SetLocalImage("Textures/main/setting_btn_open");
		}
		else
		{
			SoundStatusImg.SetLocalImage("Textures/main/setting_btn_close");
		}

		// Vibrate Status
        int VibrateStatus = PrefsUtil.GetInt("setting_vibrate") > 0 ? PrefsUtil.GetInt("setting_vibrate") : 0;
		Text VibrateStatusText = settingView.Find<Text>("Option_Vibrate/Status");
		VibrateStatusText.text = VibrateStatus > 0 ? "开" : "关";
		Image VibrateStatusImg = settingView.Find<Image>("Option_Vibrate/Button");
		if (VibrateStatus > 0)
		{
			VibrateStatusImg.SetLocalImage("Textures/main/setting_btn_open");
		}
		else
		{
			VibrateStatusImg.SetLocalImage("Textures/main/setting_btn_close");
		}
	}
	/**
     * ========================点击事件=============================
     */
	// 关闭按钮
	public void closeBtnClick()
	{
		Tweener tweener = settingView.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
	    tweener.onComplete = (() =>
		{
		 settingView.SetActive(false);
		});
	}
	/**
     * 开关按钮
     */
	public void optionItemClick(int index)
	{
		switch (index)
		{
			case 1: // Music
				{
                    Text MusicStatusText = settingView.Find<Text>("Option_Music/Status");
                    Image MusicStatusImg = settingView.Find<Image>("Option_Music/Button");
                    if (PrefsUtil.GetInt("setting_music") > 0)
                    {
                        PrefsUtil.Set("setting_music", 0);
                        MusicStatusText.text = "关";
                        MusicStatusImg.SetLocalImage("Textures/main/setting_btn_close");
                    }
                    else
                    {
                        PrefsUtil.Set("setting_music", 1);
                        MusicStatusText.text = "开";
                        MusicStatusImg.SetLocalImage("Textures/main/setting_btn_open");
                    }
					break;
				}
			case 2: // Sound
				{
					Text SoundStatusText = settingView.Find<Text>("Option_Sound/Status");
                    Image SoundStatusImg = settingView.Find<Image>("Option_Sound/Button");
					if (PrefsUtil.GetInt("setting_sound") > 0)
                    {
						PrefsUtil.Set("setting_sound", 0);
                        SoundStatusText.text = "关";
                        SoundStatusImg.SetLocalImage("Textures/main/setting_btn_close");
					}
                    else
                    {
						PrefsUtil.Set("setting_sound", 1);
                        SoundStatusText.text = "开";
                        SoundStatusImg.SetLocalImage("Textures/main/setting_btn_open");
					}
					break;
				}
			case 3: // Vibrate
				{
                    Text VibrateStatusText = settingView.Find<Text>("Option_Vibrate/Status");
                    Image VibrateStatusImg = settingView.Find<Image>("Option_Vibrate/Button");
					if (PrefsUtil.GetInt("setting_vibrate") > 0)
                    {
						PrefsUtil.Set("setting_vibrate", 0);
                        VibrateStatusText.text = "关";
                        VibrateStatusImg.SetLocalImage("Textures/main/setting_btn_close");
					}
                    else
                    {
						PrefsUtil.Set("setting_vibrate", 1);
						VibrateStatusText.text = "开";
                        VibrateStatusImg.SetLocalImage("Textures/main/setting_btn_open");
					}
					break;
				}
		}
	}
	// 退出按钮
	public void logoutBtnClick()
	{
        mainHandle = new MainHandle();
		UserManager.logout();
		mainHandle.roomList((error, result) =>
		{
        });
        NetCore.Instance.Close();
        Application.LoadLevel("login");
	}
}
