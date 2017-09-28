using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RecordItemAdapter : BaseAdapterItem<RecordItem>
{
    public Text order;
    public Text num;
    public Image First;
    public Image Second;
    public Text Date;
	public override void SetData(RecordItem data, params object[] objs)
	{
        order.text = "第" + data.order_id + "手牌";
        num.text = data.players[0].win.ToString();
        First.SetLocalImage("Textures/cards/card_" + data.players[0].cards[0].suit + data.players[0].cards[0].value);
        Second.SetLocalImage("Textures/cards/card_" + data.players[0].cards[1].suit + data.players[0].cards[1].value);
        Date.text = DateStringFromTimestamp.DateStringFromNow(data.start.ToString());
	}

}
