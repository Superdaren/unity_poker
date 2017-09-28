using DG.Tweening;
using Google.Protobuf.Collections;
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolControler : MonoBehaviour
{

    public int curPoolCount = 0;// 当前底池个数
    public float padding = 10;//间距
    public List<float> widthList = new List<float>();// 每个底池的宽度集合
    public List<float> targetList = new List<float>();// 目标位置
    public List<GameObject> poolList = new List<GameObject>();// 底池对象集合

    // Use this for initialization
    void Start()
    {

    }

    // 创建底池
    // flag--表示是否执行动画
    public void CreatePool(RepeatedField<int> pot, bool flag)
    {
        if (pot == null)
        {
            return;
        }
        if (curPoolCount == pot.Count && pot.Count > 0)
        {
            poolList[poolList.Count - 1].GetComponent<PoolChipControler>().ChangeChip(pot[pot.Count - 1]);
            return;
        }
        // 创建底池
        for (int i = curPoolCount; i < pot.Count; i++)
        {
            GameObject poolChip = GameObject.Instantiate(Resources.Load("Prefabs/PoolChip"), transform) as GameObject;
            poolChip.name = "poolChip" + i;
            poolChip.transform.position = transform.position;
            poolChip.GetComponent<PoolChipControler>().ChangeChip(pot[i]);
            poolList.Add(poolChip);
            RectTransform R = poolChip.GetComponent<RectTransform>();
            widthList.Add(poolChip.GetComponent<RectTransform>().sizeDelta.x);// 添加width
        }
        curPoolCount = pot.Count;
        // 计算总和
        float total = GetTotalWidth();
        // 计算目标位置
        InitTagetPosition(total);
        if (flag)
        {
            // 设置当前位置
            SetTagetPosition();
        }
        else
        {
            // 移动到当前位置
            MoveToTaget();
        }
    }

    // 计算总和
    private float GetTotalWidth()
    {
        float total = 0;
        for (int i = 0; i < widthList.Count; i++)
        {
            float width = widthList[i];
            if (i == 0 || i == widthList.Count - 1)
            {
                total += (width / 2);
            }
            else
            {
                total += width;
            }
        }
        total += (padding * (widthList.Count - 1));
        return total;
    }

    // 计算目标位置
    private void InitTagetPosition(float total)
    {
        targetList.Clear();
        for (int i = 0; i < widthList.Count; i++)
        {
            float targetX = 0;
            if (i == 0)
            {
                targetX = -total / 2;
            }
            else
            {
                float pre = targetList[targetList.Count - 1];
                targetX = pre + padding + widthList[i] / 2 + widthList[i - 1] / 2;//preX-padding-(preWidth/2)-(curWidth/2)
            }
            targetList.Add(targetX);
        }
    }

    // 移动
    private void MoveToTaget()
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            GameObject poolChip = poolList[i];
            Sequence s = DOTween.Sequence();
            s.AppendInterval(0.01f);
            s.Append(poolChip.transform.DOLocalMoveX(targetList[i], 0.5f));
        }
    }

    // 设置当前位置（重置房间时候调用）
    private void SetTagetPosition()
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            GameObject poolChip = poolList[i];
            //poolChip.transform.DOLocalMoveX(targetList[i], 1);
            Vector3 vect = poolChip.transform.position;
            vect.x = targetList[i];
            poolChip.transform.localPosition = vect;
        }
    }

    // 发放筹码
    public void GrantChips(RepeatedField<int> chips, RepeatedField<PotInfo> potInfos, List<GameObject> players, Func<int, int> GetPlayerPos)
    {
        if (potInfos != null)
        {
            for (int i = 0; i < potInfos.Count; i++)
            {
                PotInfo potInfo = potInfos[i];
                if (potInfos[i].Pot > 0)
                {
                    GameObject pool = poolList[i];
                    pool.GetComponent<PoolChipControler>().MoveToPlayer(chips, potInfo.Ps, players, GetPlayerPos);
                }
            }
        }
    }

    // 回合结束,重置
    public void ResetUI()
    {
        foreach (var pool in poolList)
        {
            Destroy(pool);
        }
        poolList.Clear();
        targetList.Clear();
        widthList.Clear();
        curPoolCount = 0;
    }

}
