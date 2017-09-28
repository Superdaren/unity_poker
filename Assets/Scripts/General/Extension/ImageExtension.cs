using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * 图片扩展类
 */
public static class ImageExtension
{
    /**
     * 设置远程图片
     */
    public static void SetWebImage(this Image image,string url, string defaultImage) {
        ImageLoader.Instance.SetAsyncImage(url, image, defaultImage);
    }

    /**
     * 设置本地图片
     */
	public static void SetLocalImage(this Image image, string path)
	{
        ImageLoader.Instance.setLocalImage(path, image);
	}
}
