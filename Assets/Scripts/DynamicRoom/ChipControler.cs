using UnityEngine;
using UnityEngine.UI;

public class ChipControler : MonoBehaviour {

    public bool is_left;// 是否是左边的筹码

    private GameObject chipCount;// 显示筹码数量的组件
    private GameObject chipIcon;// 显示筹码图标的组件
    private string formatLeft = "   {0}";// 左边筹码的format
    private string formatRight = "{0}   ";// 右边筹码的format

    public GameObject GetChipIcon()
    {
        if (chipIcon == null)
        {
            chipIcon = GameObject.Find(name + "/chipIcon");
        }
        return chipIcon;
    }

    // Use this for initialization
    void Start () {
        ChangeChip(100);
    }

    // 修改筹码
    public void ChangeChip(int price)
    {
        if (chipCount == null)
        {
            chipCount = GameObject.Find(name + "/chipCount");
        }
        if (chipIcon == null)
        {
            chipIcon = GameObject.Find(name + "/chipIcon");
        }
        string format = is_left ? formatLeft : formatRight;
        string stringChip = StringUtil.GetStringChip(price);
        //GetComponent<Text>().text = string.Format(format, stringChip);
        chipCount.GetComponent<Text>().text = string.Format(format, stringChip);
    }
}
