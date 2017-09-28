using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using System;
using DG.Tweening;

public class BagItemAdapter : BaseAdapterItem<BagItem> {

    public Text name;
    public Text desc;
    public Text num;
    public Image pic;

    public override void SetData(BagItem data, params object[] objs)
    {
        pic.SetWebImage(data.image, "");
        name.text = data.name;
        desc.text = data.goods_describe;
        num.text = data.number.ToString()+"个";
    }

   
}
