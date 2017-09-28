using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastGameAdapter : BaseAdapter<Player> {

    public void SetData(LastGame lastGame)
    {
        if (lastGame == null || lastGame.players == null)
        {
            return;
        }
        List<Player> list = new List<Player>(lastGame.players);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
                i--;
            }
        }
        SetDatas(list, lastGame);
    }
}
