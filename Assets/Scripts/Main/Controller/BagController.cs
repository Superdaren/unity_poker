using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using System;
using DG.Tweening;

public class BagController : MonoBehaviour
{
    // 背包界面
    public GameObject bag;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public void BtnClose(){
        bag.SetActive(false);
    }
}
