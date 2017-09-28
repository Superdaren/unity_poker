using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using NetProto;

public class RegistManager : MonoBehaviour
{
    
	public InputField countField;
	public InputField passwordField;
    public GameObject registView;

    public LoginHandle loginHandle;

    RegistHandle registHandle;

    public void closeBtnClick() {
        Tweener tweener = registView.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
        tweener.onComplete = (() => {
            registView.SetActive(false);
        });
    }

    public void registBtnClick(){
        var count = countField.text;
        var password = passwordField.text;

		if (count == null || count == "")
		{
            PopUtil.ShowTotoast("帐号不能为空");
			return;
		}

		if (password == null || password == "")
		{
            PopUtil.ShowTotoast("密码不能为空");
			return;
		}

        PopUtil.ShowLoadingView("注册中...");
        registHandle.registAction(count, password,(error, result) => {
			if (error == null)
			{
                PopUtil.ShowLoadingView("登录中...");
				NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.user_login_ack, (loginAction) =>
				{
					UserLoginAck userLoginAck = (UserLoginAck)loginAction;
					if (userLoginAck.BaseAck.Ret == 1)
					{
                        Application.LoadLevel("main");
                    }
				});
				loginHandle.loginReq();
			}
			else
			{
				if (error.code == 500)
				{
					PopUtil.ShowTotoast("网络有问题,或者联系: JX");
				}
				else
				{
					PopUtil.ShowTotoast(error.msg);
				}
			}
        });
    }
    
	// Use this for initialization
	void Start()
	{
        registHandle = new RegistHandle();
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
