using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetProto;

public class BagAdapter : BaseAdapter<BagItem> {
    MainHandle mainHandle;
	// 背包界面
	public GameObject bag;
    public GameObject none;

     void Start()
    {
		mainHandle = new MainHandle();
		NetCore.Instance.registHandle(mainHandle);
        none = GameObject.Find(bag.name + "/Nothing");
        none.SetActive(false);
        loadData();
    }

    public void loadData(){
        mainHandle.bagList((error, result) => 
        {
           if (error == null)
            {
                if(result.ret == 1 && result.list != null){
                    SetDatas(result.list);
                }else{
                    none.SetActive(true);
                }
                
            }
        });
    }
}
