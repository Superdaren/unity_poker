using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;

public class CardControler : MonoBehaviour {

    public bool is_left;        // 是否是左边的手牌
    private bool is_display;     // 是否显示手牌/亮牌
    public bool IsDisplay
    {
        set
        {
            is_display = value;
            if (is_display)
            {
                is_display = !is_display;
                StartFlipScale(is_left);
            }
        }
    }

    public GameObject player;   // 拥有手牌的玩家
    public GameObject positive; // 手牌的正面
    public GameObject negative; // 手牌的反面

    private void Start()
    {
    }

    // 初始化手牌（positive）
    public void SetCardValue(CardInfo info)
    {
        string format = "Textures/cards/card_" + info.Suit + info.Val;
        positive.GetComponent<Image>().sprite = Resources.Load(format, typeof(Sprite)) as Sprite;
    }

    // 显示牌的正面
    public void ShowCard()
    {
        positive.SetActive(true);
        negative.SetActive(false);
    }

    // 公共牌翻牌动画
    public Sequence Flip()
    {
        positive.SetActive(false);
        negative.SetActive(true);
        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f));
        s.InsertCallback(0.3f, () =>
        {
            positive.SetActive(true);
            negative.SetActive(false);
        });
        s.Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
        return s;
    }

    // 其他玩家看牌动画
    public void OtherFlip() {
        if (is_left)
        {
            transform.DORotate(new Vector3(0, 0, 12), 1);
            transform.DOLocalMoveX(transform.localPosition.x - 20, 1);
        }
        else
        {
            transform.DORotate(new Vector3(0, 0, -2), 1);
        }
    }

    // 玩家自己看牌动画
    public void SelfFlip()
    {
        positive.SetActive(false);
        negative.SetActive(true);
        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(new Vector3(0, 90, 0), 0.4f));
        s.InsertCallback(0.2f, () =>
        {
            positive.SetActive(true);
            negative.SetActive(false);
        });
        s.Append(transform.DORotate(new Vector3(0, 0, 0), 0.4f));
        s.AppendCallback(()=>
        {
            if (is_left)
            {
                transform.DORotate(new Vector3(0, 0, 10), 1);
                transform.DOLocalMoveX(transform.localPosition.x + 30, 1);
                transform.DOLocalMoveY(transform.localPosition.y + 40, 1);
            }
            else
            {
                transform.DOLocalMoveX(transform.localPosition.x + 80, 1);
                transform.DOLocalMoveY(transform.localPosition.y + 20, 1);
                transform.DORotate(new Vector3(0, 0, -8), 1);
            }
        });
        transform.DOScale(new Vector3(2, 2, 0), 1);
    }

    // 亮牌的动画
    public void StartFlipScale(bool is_left)
    {
        positive.SetActive(false);
        negative.SetActive(true);

        transform.DORotate(new Vector3(0, 0, 0), 0.1f);
        Sequence s = DOTween.Sequence();
        s.Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f));
        s.InsertCallback(0.3f, () =>
        {
            positive.SetActive(true);
            negative.SetActive(false);
        });
        s.Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f));
        transform.DOScale(new Vector3(2, 2, 0), 1);
        if (is_left)
        {
            transform.DOMove(GameObject.Find(player.name + "/leftCard").transform.position, 1);
        }
        else
        {
            transform.DOMove(GameObject.Find(player.name + "/rightCard").transform.position, 1);
        }
    }

    // 设置牌的位置（只有在进房间的时候会执行）
    public void SetCardPosition(Vector3 position)
    {
        transform.position = position;
        if (is_left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 10);
            Vector3 localPosition = transform.localPosition;
            localPosition.x -= 10;
            localPosition.y += 5;
            transform.localPosition = localPosition;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -12);
        }
    }

    // 设置牌的位置（只有在进房间的时候会执行）
    public void SetSelfCardPosition(Vector3 position)
    {
        transform.position = position;
        SelfFlip();
    }

}
