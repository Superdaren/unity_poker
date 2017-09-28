using UnityEngine;
using UnityEngine.UI;

public abstract class BaseAdapterItem<T> : MonoBehaviour
{
    private BaseAdapter<T> adapter;
    private int index;

    public BaseAdapter<T> Adapter
    {
        set { adapter = value; }
    }

    public int Index
    {
        get { return index; }
        set
        {
            index = value;
            transform.localPosition = adapter.GetPosition(index);
            gameObject.name = "Item" + (index < 10 ? "0" + index : index.ToString());
        }
    }

    // 初始化数据
    public abstract void SetData(T data, params object[] objs);
}
