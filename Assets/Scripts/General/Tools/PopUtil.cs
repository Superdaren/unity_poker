using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using DG.Tweening;

/**
 * 通用弹窗
 * 用法:
 * 将该控件拉到要弹出提示的组件上面,然后在相应的地方调用CommonUI里面的方法
 */
public class PopUtil : MonoBehaviour
{

    static GameObject canvas
    {
        get
        {
            return GameObject.Find("Canvas");
        }
    }

    /**
     * 带有确定和取消按钮的弹窗
     */
    public static void ShowMessageBoxWithConfirmAndCancle(string title, string content, UnityAction confrimAction, UnityAction cancleAction)
    {
        GameObject LoadingView = GameObject.Find("LoadingView");
        // 加载之前先销毁
        if (LoadingView != null)
        {
            Destroy(LoadingView);
        }

        GameObject commonUIPrefab = Resources.Load("Prefabs/ConfirmAndCanclePopView") as GameObject;
        GameObject root = Instantiate(commonUIPrefab) as GameObject;
        root.transform.parent = canvas.transform;
        root.transform.localPosition = new Vector3(0, 0, 0);
        root.transform.localScale = new Vector3(1, 1, 0);

        GameObject titleObject = GameObject.Find(root.name + "/Title");
        Text titleText = titleObject.GetComponent<Text>();

        GameObject contentObject = GameObject.Find(root.name + "/Content");
        Text contentText = contentObject.GetComponent<Text>();

        GameObject comfirmObject = GameObject.Find(root.name + "/ConfirmBtn");
        Button comfirmBtn = comfirmObject.GetComponent<Button>();

        GameObject cancleObject = GameObject.Find(root.name + "/CancleBtn");
        Button cancleBtn = cancleObject.GetComponent<Button>();

        comfirmBtn.onClick.AddListener(() =>
        {
            Destroy(root);
            if (confrimAction != null)
            {
                confrimAction.Invoke();
            }
        });

        cancleBtn.onClick.AddListener(() =>
        {
            Destroy(root);
            if (cancleAction != null)
            {
                cancleAction.Invoke();
            }
        });

        titleText.text = title;
        contentText.text = content;
    }

    /**
     * 带有确定按钮的弹窗
     */
    public static void ShowMessageBoxWithConfirm(string title, string content)
    {
        ShowMessageBoxWithConfirm(title, content, null);
    }

    /**
     * 带有确定按钮的弹窗
     */
    public static void ShowMessageBoxWithConfirm(string title, string content, UnityAction confrimAction)
    {
        GameObject LoadingView = GameObject.Find("LoadingView");
        // 加载之前先销毁
        if (LoadingView != null)
        {
            Destroy(LoadingView);
        }

        GameObject commonUIPrefab = Resources.Load("Prefabs/CanclePopView") as GameObject;
        GameObject root = Instantiate(commonUIPrefab) as GameObject;
        root.transform.parent = canvas.transform;
        root.transform.localPosition = new Vector3(0, 0, 0);
        root.transform.localScale = new Vector3(1, 1, 0);

        GameObject titleObject = GameObject.Find(root.name + "/Title");
        Text titleText = titleObject.GetComponent<Text>();

        GameObject contentObject = GameObject.Find(root.name + "/Content");
        Text contentText = contentObject.GetComponent<Text>();

        GameObject confirmObject = GameObject.Find(root.name + "/ConfirmBtn");
        Button confirmBtn = confirmObject.GetComponent<Button>();

        confirmBtn.onClick.AddListener(() =>
        {
            Destroy(root);
            if (confrimAction != null)
            {
                confrimAction.Invoke();
            }
        });

        titleText.text = title;
        contentText.text = content;
    }

    /**
     * totoast
     */
    public static void ShowTotoast(string content)
    {
        GameObject LoadingView = GameObject.Find("LoadingView");
        // 加载之前先销毁
        if (LoadingView != null)
        {
            Destroy(LoadingView);
        }

        GameObject commonUIPrefab = Resources.Load("Prefabs/TotastView") as GameObject;
        GameObject root = Instantiate(commonUIPrefab) as GameObject;
        root.transform.parent = canvas.transform;
        root.transform.localPosition = new Vector3(0, 0, 0);
        root.transform.localScale = new Vector3(1, 1, 0);

        GameObject contentObject = GameObject.Find(root.name + "/Content");
        Text contentText = contentObject.GetComponent<Text>();

        GameObject backGroundImageObject = GameObject.Find(root.name + "/BackGroundImage");
        Image backGroundImage = backGroundImageObject.GetComponent<Image>();

        contentText.text = content;

        Color c = contentText.color;
        Sequence sequence = DOTween.Sequence();
        Tweener textAlpha = contentText.DOColor(new Color(c.r, c.g, c.b, 0), 0.2f);
        Tweener imageAlpha = backGroundImage.DOColor(new Color(c.r, c.g, c.b, 0), 0.2f);

        sequence.AppendInterval(1.5f);
        sequence.Append(textAlpha);
        sequence.Append(imageAlpha);
        sequence.AppendCallback(() =>
        {
            Destroy(root);
        });
    }

    /**
     * 加载界面
     */
    public static void ShowLoadingView(string content)
    {
        GameObject loadingView = GameObject.Find("LoadingView");
        // 加载之前判断是否已经存在LoadingView
        if (loadingView != null)
        {
            Text contentTextExit = loadingView.Find<Text>("LoadingView/Content/LoadingText");
            contentTextExit.text = content;
            return;
        }

        GameObject commonUIPrefab = Resources.Load("Prefabs/LoadingView") as GameObject;
        loadingView = Instantiate(commonUIPrefab) as GameObject;
        loadingView.name = "LoadingView";
        loadingView.transform.parent = canvas.transform;
        loadingView.transform.localPosition = new Vector3(0, 0, 0);
        loadingView.transform.localScale = new Vector3(1, 1, 0);
        Text contentText = loadingView.Find<Text>("LoadingView/Content/LoadingText");
        contentText.text = content;
    }

    /**
     * dismiss LoadingView
     */
    public static void DismissLoadingView()
    {
        GameObject LoadingView = GameObject.Find("LoadingView");

        Destroy(LoadingView);
    }

    /**
     * 显示签到成功界面
     */
    public static void ShowSignInSuccessView(string rewardDes)
    {
        GameObject commonUIPrefab = Resources.Load("Prefabs/SignInSuccessPopView") as GameObject;
        GameObject root = Instantiate(commonUIPrefab) as GameObject;
        root.name = "SignInSuccessPopView";
        root.transform.parent = canvas.transform;
        root.transform.localPosition = new Vector3(0, 0, 0);
        root.transform.localScale = new Vector3(1, 1, 0);

        GameObject signInImage = GameObject.Find(root.name + "/SignInImage");
        GameObject rewardDesImage = GameObject.Find(root.name + "/RewardDesImage");
        GameObject GoldImage = GameObject.Find(root.name + "/GoldImage");
        Text goldText = root.Find<Text>(root.name + "/RewardDesImage/GoldText");
        goldText.text = rewardDes;

        rewardDesImage.SetActive(false);
        GoldImage.SetActive(false);

        PKAnimateTool.signSuccesView(signInImage, () =>
        {
            rewardDesImage.SetActive(true);
            GoldImage.SetActive(true);

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(1.5f);
            sequence.AppendCallback(() =>
            {
                Destroy(root);
            });
        });
    }
}
