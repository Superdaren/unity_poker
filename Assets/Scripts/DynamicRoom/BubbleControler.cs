using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleControler : MonoBehaviour
{

    GameObject contentObj;

    // Use this for initialization
    void Start()
    {
        contentObj = GameObject.Find(name + "/Text");
    }

    // 设置文本
    public void SetText(string message)
    {
        if (contentObj == null)
        {
            contentObj = GameObject.Find(name + "/Text");
        }
        contentObj.GetComponent<Text>().text = message;
        Roll();
    }

    // 滚动文本
    private void Roll()
    {
        Sequence s = DOTween.Sequence();
        float posy = contentObj.transform.localPosition.y;
        float h = contentObj.GetComponent<RectTransform>().sizeDelta.y;
        float h1 = contentObj.GetComponent<Text>().preferredHeight;
        int count = (int)(h1 / h) - 1;
        float h3 = h * count + posy;
        s.AppendInterval(1f);
        s.Append(contentObj.transform.DOLocalMoveY(h3, count));
        s.AppendInterval(1f);
        s.AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }

}
