using System;
using UnityEngine;

public class TransformUtil
{

    private const float SW = 1920f;
    private const float SH = 1080f;

    /**
     * 重置锚点
     */
    public static void ResetAnchor(GameObject go, Vector3 localPosition)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        Vector2 minV = new Vector2(0.5f, 0.5f);
        Vector2 maxV = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = minV;
        rectTransform.anchorMax = maxV;
        go.transform.localPosition = localPosition;
    }

    /**
     *  设置包裹组件的锚点的值
     */
    public static void SetWarpAnchor(GameObject go)
    {
        Vector3 localPosition = go.transform.localPosition;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        float posX = localPosition.x;
        float posY = localPosition.y;
        float halfW = rectTransform.sizeDelta.x / 2;
        float halfH = rectTransform.sizeDelta.y / 2;

        float minX = ((posX - halfW) / SW) + 0.5f;
        float minY = ((posY - halfH) / SH) + 0.5f;
        Vector2 minV = new Vector2(minX, minY);

        float maxX = ((posX + halfW) / SW) + 0.5f;
        float maxY = ((posY + halfH) / SH) + 0.5f;
        Vector2 maxV = new Vector2(maxX, maxY);

        rectTransform.anchorMin = minV;
        rectTransform.anchorMax = maxV;
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }

    /**
     *  重新设置包裹组件的锚点的值
     */
    public static void ResetWarpAnchor(GameObject go)
    {
        SetAnchor(go, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        Vector3 localPosition = go.transform.localPosition;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        float posX = localPosition.x;
        float posY = localPosition.y;
        float halfW = rectTransform.sizeDelta.x / 2;
        float halfH = rectTransform.sizeDelta.y / 2;

        float minX = ((posX - halfW) / SW) + 0.5f;
        float minY = ((posY - halfH) / SH) + 0.5f;
        Vector2 minV = new Vector2(minX, minY);

        float maxX = ((posX + halfW) / SW) + 0.5f;
        float maxY = ((posY + halfH) / SH) + 0.5f;
        Vector2 maxV = new Vector2(maxX, maxY);

        rectTransform.anchorMin = minV;
        rectTransform.anchorMax = maxV;
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }

    /**
     *  设置锚点最小值
     */
    public static void SetMinAnchor(GameObject go, Vector2 minVector)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchorMin = minVector;
    }

    /**
     *  设置锚点最大值
     */
    public static void SetMaxAnchor(GameObject go, Vector2 maxVector)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchorMin = maxVector;
    }

    /**
     *  设置锚点
     */
    public static void SetAnchor(GameObject go, Vector2 minVector, Vector2 maxVector)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchorMin = minVector;
        rectTransform.anchorMin = maxVector;
    }


}
