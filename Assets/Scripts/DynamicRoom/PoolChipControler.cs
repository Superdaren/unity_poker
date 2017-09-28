using DG.Tweening;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolChipControler : MonoBehaviour
{

    private GameObject chipCountObj;   // 显示筹码数量的组件
    private GameObject chipGroupObj;   // 显示筹码的垂直布局组件
    private string format = "   {0}";  // 左边筹码的format
    private int chipCount;             // 当前奖池的筹码数

    private List<Sprite> chipList = new List<Sprite>();         // 下注的筹码图标
    private List<GameObject> chipFabs = new List<GameObject>(); // 下注的筹码组件

    //十、百、1千、5千、10万、50万
    private string[] chipArray = {
        "Textures/room/chips_yellow_img",
        "Textures/room/chips_green_img",
        "Textures/room/chips_orange_img",
        "Textures/room/chips_purple_img",
        "Textures/room/chips_red_img",
        "Textures/room/chips_blue_img",
        "Textures/room/chips_black_img"
    };

    // Use this for initialization
    void Start()
    {
    }

    // 修改筹码
    public void ChangeChip(int count)
    {
        if (chipCountObj == null)
        {
            chipCountObj = GameObject.Find(name + "/chipCount");
        }
        if (chipGroupObj == null)
        {
            chipGroupObj = GameObject.Find(name + "/chipGroup");
        }
        chipCount = count;
        string stringChip = StringUtil.GetStringChip(count);
        chipCountObj.GetComponent<Text>().text = string.Format(format, stringChip);
        CreateChips(count);
    }

    // 创建筹码
    private void CreateChips(int count)
    {
        // 每次更新底池筹码的时候需要清空之前的筹码
        ClearPoolChips();
        // 初始化chipList
        InitChipList(count);
        int unit = count / chipList.Count;
        for (int i = 0; i < chipList.Count; i++)
        {
            GameObject item = GameObject.Instantiate((Resources.Load<GameObject>("Prefabs/PoolChipItem")), chipGroupObj.transform);
            item.name = "chipItem" + i;
            item.GetComponent<Image>().sprite = chipList[i];
            chipFabs.Add(item);
        }
    } 
    
    // 更新筹码
    private void UpdateChips(int count)
    {
        chipCount = count;
        // 每次更新底池筹码的时候需要清空之前的筹码
        chipFabs.Clear();
        // 初始化chipList
        InitChipList(count);
        int unit = count / chipList.Count;
        for (int i = 0; i < chipList.Count; i++)
        {
            GameObject item = GameObject.Instantiate((Resources.Load<GameObject>("Prefabs/PoolChipItem")), chipGroupObj.transform);
            item.name = "chipItem" + i;
            item.GetComponent<Image>().sprite = chipList[i];
            chipFabs.Add(item);
        }
    }

    // 初始化筹码Icon列表
    public void InitChipList(int count)
    {
        chipList.Clear();
        int time = count.ToString().Length - 1; // 获取10的n次方
        if (time < 1)
        {
            Sprite sprite = Resources.Load(chipArray[0], typeof(Sprite)) as Sprite;
            chipList.Add(sprite);
        }
        else
        {
            for (int i = time; i > 0; i--)
            {
                if (chipList.Count > 5)
                {
                    break;
                }
                int digit = (int)(count / Math.Pow(10, i));
                count = (int)(count % Math.Pow(10, i));

                Sprite sprite = Resources.Load(chipArray[i - 1 < chipArray.Length ? i - 1 : chipArray.Length - 1], typeof(Sprite)) as Sprite;

                for (int j = 0; j < digit; j++)
                {
                    chipList.Add(sprite);
                }
            }
        }
    }

    // 清空筹码
    public void ClearPoolChips()
    {
        foreach (var chipFab in chipFabs)
        {
            Destroy(chipFab);
        }
        chipFabs.Clear();
        chipList.Clear();
    }

    // 将筹码移动到对应的玩家上
    public void MoveToPlayer(RepeatedField<int> chips, RepeatedField<int> ps, List<GameObject> playerObjs, Func<int, int> GetPlayerPos)
    {
        bool isWinForSelf = false;
        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        chipCountObj.SetActive(false);
        for (int i = 0; i < ps.Count; i++)
        {
            if (ps[i] > 0)
            {
                int chip = ps[i];
                GameObject playerObj = playerObjs[GetPlayerPos(i)];
                for (int j = 0; j < chipFabs.Count; j++)
                {
                    GameObject chipFab = chipFabs[j];
                    Sequence s = DOTween.Sequence();
                    s.AppendInterval(j * 0.1f);
                    s.Append(chipFab.transform.DOMove(playerObj.transform.position, 0.5f));
                    s.AppendCallback(() =>
                    {
                        Destroy(chipFab);
                    });
                }
                if (chipCount - chip > 0)
                {
                    UpdateChips(chipCount - chip);
                }
                playerObj.GetComponent<PlayerControler>().Win(chips[i]);
                if (playerObj.GetComponent<PlayerControler>().PlayerInfo.Id == UserManager.Instance().userInfo.id)
                {
                    isWinForSelf = true;
                }
            }
        }
        // 播放音乐
        AudioUtil.Play(isWinForSelf?AudioUtil.Win: AudioUtil.Lose);
    }
}
