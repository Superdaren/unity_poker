using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnSelectControler : MonoBehaviour {

    public Sprite SelectSprite;
    public Sprite UnSelectSprite;
    private Image TargetGraphic;

    public bool isSelect;
    public bool Select { set { isSelect = value; } }

    // Use this for initialization
    void Start () {
        TargetGraphic = GetComponent<Image>();
        TargetGraphic.sprite = SelectSprite;
    }

    private void FixedUpdate()
    {
        if (isSelect)
        {
            TargetGraphic.sprite = SelectSprite;
        }
        else
        {
            TargetGraphic.sprite = UnSelectSprite;
        }
    }

}
