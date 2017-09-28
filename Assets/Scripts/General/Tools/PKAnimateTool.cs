using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PKAnimateTool : MonoBehaviour
{
    /**
     * 显示一半大小 -> 伸缩到正常大小
     */
    public static void popUpView(GameObject animateObject) {
		animateObject.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
		animateObject.SetActive(true);
		animateObject.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
    }

    public static void closePopUpView(GameObject animateObject) {
		Tweener tweener = animateObject.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
		tweener.onComplete = (() =>
		{
			animateObject.SetActive(false);
		});
    }

	/**
     * 伸缩到原来的2倍 -> 伸缩到正常大小
     */
	public static void signSuccesView(GameObject naimateObject, Action action){
		Sequence sequence = DOTween.Sequence();
        sequence.Append(naimateObject.transform.DOScale(new Vector3(2, 2, 2), 0.5f));
		sequence.Append(naimateObject.transform.DOScale(new Vector3(1, 1, 1), 0.3f));
        sequence.Append(naimateObject.transform.DOShakePosition(0.1f, 20, 100, 90, false, false));
		sequence.AppendCallback(() =>
		{
            if(action != null) action.Invoke();
		});
    }
}
