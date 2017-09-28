using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageLoader : Image {

	private static ImageLoader _instance = null;
	public static ImageLoader GetInstance() { return Instance; }
	public static ImageLoader Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject("ImageLoader");
				_instance = obj.AddComponent<ImageLoader>();
				DontDestroyOnLoad(obj);
				_instance.Init();
			}
			return _instance;
		}
	}

	public bool Init()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
		}
		return true;
	}

    public void setLocalImage(string path, Image image) 
    {
		if (path != "" && path != null)
		{
			Texture2D defaultTexture = (Texture2D)Resources.Load(path);
			Sprite defaultSprite = Sprite.Create(defaultTexture, new Rect(0, 0, defaultTexture.width, defaultTexture.height), new Vector2(0, 0));
			image.sprite = defaultSprite;
		}
    }

    public void SetAsyncImage(string url, Image image, string defaultImage)
	{
        //开始下载图片前，将UITexture的主图片设置为占位图
        if (defaultImage != "" && defaultImage != null){
			Texture2D defaultTexture = (Texture2D)Resources.Load(defaultImage);
			Sprite defaultSprite = Sprite.Create(defaultTexture, new Rect(0, 0, defaultTexture.width, defaultTexture.height), new Vector2(0, 0));
			image.sprite = defaultSprite;
        }

        if (url==null|| url.Equals(""))
        {
            return;
        }

		//判断是否是第一次加载这张图片
		if (!File.Exists(path + url.GetHashCode()))
		{
			//如果之前不存在缓存文件
			StartCoroutine(DownloadImage(url, image));
		}
		else
		{
			StartCoroutine(LoadLocalImage(url, image));
		}
	}

	IEnumerator DownloadImage(string url, Image image)
	{
		Debug.Log("downloading new image:" + path + url.GetHashCode());//url转换HD5作为名字
		WWW www = new WWW(url);
		yield return www;

		Texture2D tex2d = www.texture;
		//将图片保存至缓存路径
		byte[] pngData = tex2d.EncodeToPNG();
		File.WriteAllBytes(path + url.GetHashCode(), pngData);

		Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
		image.sprite = m_sprite;
	}

	IEnumerator LoadLocalImage(string url, Image image)
	{
		string filePath = "file:///" + path + url.GetHashCode();

		Debug.Log("getting local image:" + filePath);
		WWW www = new WWW(filePath);
		yield return www;

		Texture2D texture = www.texture;
        if(texture!=null){
			Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
			image.sprite = m_sprite;
        }
	}

	public string path
	{
		get
		{
			//pc,ios //android :jar:file//
			return Application.persistentDataPath + "/ImageCache/";

		}
	}
}
