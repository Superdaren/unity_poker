using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class GameObjectExtension
{
    public static UI Find<UI>(this GameObject gameObject, string path)
	{
		GameObject findObject = GameObject.Find(path);
        UI findUI = findObject.GetComponent<UI>();
        return findUI;
	}
}
