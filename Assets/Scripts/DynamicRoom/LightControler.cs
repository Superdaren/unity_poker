using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControler : MonoBehaviour {

    private const float DEFAULT_ANGLE = 90;// 光标默认角度

    public List<GameObject> mPlayerObjects = new List<GameObject>();// 所有PlayerObject集合
    public List<float> spaces = new List<float>();                  // 每个座位之间的角度差
    private List<float> angles = new List<float>();                 // 每个座位相对于（0，0，0）的角度
    private List<float> scales = new List<float>();                 // 光标对应不同座位的不同缩放比例

    // 设置PlayerObjects
    public void SetPalyerObjects(List<GameObject> mPlayerObjects)
    {
        this.mPlayerObjects = mPlayerObjects;
        //sizeDelta.x--宽
        //sizeDelta.y--高
        mOriginWidth = GetComponent<RectTransform>().sizeDelta.x;
        InitAngle();
    }

    private float mOriginWidth = 0;             //光标初始大小
    private int currentPosition = -1;           // 光标当前位置
    private float currentAngle = DEFAULT_ANGLE; // 当前旋转的角度

    // Use this for initialization
    void Start () {
        
    }

    // 获取旋转角度
    private void InitAngle()
    {
        if (mPlayerObjects.Count <= 0)
        {
            return;
        }
        foreach (GameObject player in mPlayerObjects)
        {
            float x = player.transform.localPosition.x;
            float y = player.transform.localPosition.y;
            float angle = Mathf.Atan2(y, x) * 180 / Mathf.PI;
            angles.Add(angle);
            float width = (Mathf.Sqrt(x * x + y * y));
            float scale = width / mOriginWidth;
            scales.Add(scale);
        }

        // 初始化两两之间的角度差
        for (int i = 0; i <= angles.Count; i++)
        {
            float space = 0;
            if (i==0)
            {
                space = angles[i] - DEFAULT_ANGLE;
            }
            else if (i == angles.Count)
            {
                space = angles[0] - angles[i-1];
            }
            else
            {
                if (angles[i]> angles[i - 1])
                {
                    space = angles[i] - (angles[i - 1] + 360);
                }
                else
                {
                    space = angles[i] - angles[i - 1];
                }
            }
            spaces.Add(-Mathf.Abs(space));
        }
    }

    // 计算旋转角度
    public float GetAngle(int targetPosition)
    {
        if (currentPosition==-1)
        {
            for (int i = 0; i <= targetPosition; i++)
            {
                currentAngle += spaces[i];
            }
        }
        else
        {
            if (currentPosition < targetPosition)
            {
                for (int i = currentPosition + 1; i <= targetPosition; i++)
                {
                    currentAngle += spaces[i];
                }
            }
            else
            {
                for (int i = currentPosition + 1; i < spaces.Count; i++)
                {
                    currentAngle += spaces[i];
                }
                for (int i = 1; i <= targetPosition; i++)
                {
                    currentAngle += spaces[i];
                }
            }
        }
        currentPosition = targetPosition;
        return currentAngle;
    }

    // 旋转
    public void Rotate(int position)
    {
        if (position >= mPlayerObjects.Count || position < 0)
        {
            return;
        }
        float angle = GetAngle(position);
        float scale = scales[position];
        Rotate(angle, scale, 0.5f);
    }

    // 旋转
    public void Rotate(float angle, float scale, float duration)
    {
        transform.DOScaleX(scale, duration);
        Quaternion quaternion = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.DOLocalRotateQuaternion(quaternion, duration);
    }

    // 重置角度
    public void ResetAngle()
    {
        currentAngle = DEFAULT_ANGLE;
        currentPosition = -1;
        gameObject.SetActive(false);
        transform.DOLocalRotate(new Vector3(0, 0, 90), 1, RotateMode.FastBeyond360);
    }

}
