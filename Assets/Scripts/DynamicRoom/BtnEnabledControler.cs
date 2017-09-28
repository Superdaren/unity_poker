using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnEnabledControler : MonoBehaviour {

    public void SetEnabled(bool enabled)
    {
        GetComponent<Button>().interactable = enabled;
        GameObject.Find(name + "/Text").GetComponent<Text>().color =
            enabled ? Color.white : GetComponent<Button>().colors.disabledColor;
    }

	// Use this for initialization
	void Start () {
		
	}
	
}
