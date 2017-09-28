using UnityEngine;
using System.Collections.Generic;

public abstract class BaseAdapter<T> : MonoBehaviour
{
    public enum Arrangement { Horizontal, Vertical, }
    public Arrangement movement = Arrangement.Horizontal;
    //Item之间的距离
    [Range(0, 20)]
    public int cellPadiding = 5;
    //Item的宽高
    public int cellWidth = 500;
    public int cellHeight = 100;
    //默认加载的Item个数，一般比可显示个数大2~3个
    [Range(0, 20)]
    public int viewCount = 6;
    public GameObject itemPrefab;
    public RectTransform content;

    // 数据
    protected List<T> datas = new List<T>();
    // 其他参数
    protected object[] otherParam;

    private int position = -1;
    private List<BaseAdapterItem<T>> itemList = new List<BaseAdapterItem<T>>();
    private int dataCount;

    //将未显示出来的Item存入未使用队列里面，等待需要使用的时候直接取出
    private Queue<BaseAdapterItem<T>> unUsedQueue = new Queue<BaseAdapterItem<T>>();

    // 设置数据
    public void SetDatas(List<T> datas, params object[] otherParam)
    {
        this.datas = datas;
        this.otherParam = otherParam;
        position = -1;
        DataCount = datas.Count;
        OnValueChange(Vector3.zero);
    }

    public void OnValueChange(Vector2 pos)
    {
        int index = GetPosIndex();
        if (position != index && index > -1)
        {
            position = index;
            for (int i = itemList.Count; i > 0; i--)
            {
                BaseAdapterItem<T> item = itemList[i - 1];
                if (item.Index < index || (item.Index >= index + viewCount))
                {
                    itemList.Remove(item);
                    unUsedQueue.Enqueue(item);
                }
            }
            for (int i = position; i < position + viewCount; i++)
            {
                if (i < 0) continue;
                if (i > dataCount - 1) continue;
                bool isOk = false;
                foreach (BaseAdapterItem<T> item in itemList)
                {
                    if (item.Index == i)
                    {
                        isOk = true;
                        UpdateItem(i, item);
                    }
                }
                if (isOk) continue;
                CreateItem(i);
            }
        }
    }

    private void UpdateItem(int index, BaseAdapterItem<T> itemBase)
    {
        itemBase.Index = index;
        itemBase.SetData(datas[index], otherParam);
    }

    private void CreateItem(int index)
    {
        BaseAdapterItem<T> itemBase;
        if (unUsedQueue.Count > 0)
        {
            itemBase = unUsedQueue.Dequeue();
        }
        else
        {
            itemBase = AddChild(content, itemPrefab).GetComponent<BaseAdapterItem<T>>();
        }

        itemBase.Adapter = this;
        itemBase.Index = index;
        itemBase.SetData(datas[index], otherParam);
        itemList.Add(itemBase);
    }

    private int GetPosIndex()
    {
        switch (movement)
        {
            case Arrangement.Horizontal:
                return Mathf.FloorToInt(content.anchoredPosition.x / -(cellWidth + cellPadiding));
            case Arrangement.Vertical:
                return Mathf.FloorToInt(content.anchoredPosition.y / (cellHeight + cellPadiding));
        }
        return 0;
    }

    public Vector3 GetPosition(int i)
    {
        switch (movement)
        {
            case Arrangement.Horizontal:
                return new Vector3(i * (cellWidth + cellPadiding), 0f, 0f);
            case Arrangement.Vertical:
                return new Vector3(0f, i * -(cellHeight + cellPadiding), 0f);
        }
        return Vector3.zero;
    }

    public int DataCount
    {
        get { return dataCount; }
        set
        {
            dataCount = value;
            UpdateTotalWidth();
        }
    }

    private void UpdateTotalWidth()
    {
        switch (movement)
        {
            case Arrangement.Horizontal:
                content.sizeDelta = new Vector2(cellWidth * dataCount + cellPadiding * (dataCount - 1), content.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                content.sizeDelta = new Vector2(content.sizeDelta.x, cellHeight * dataCount + cellPadiding * (dataCount - 1));
                break;
        }
    }

    public GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }
}
