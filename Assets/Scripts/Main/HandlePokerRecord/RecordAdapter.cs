using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetProto;

public class RecordAdapter : BaseAdapter<RecordItem>
{
	MainHandle mainHandle;
	// 背包界面
	public GameObject record;
	public GameObject none;

	void Start()
	{
		mainHandle = new MainHandle();
		NetCore.Instance.registHandle(mainHandle);
        none = GameObject.Find(record.name + "/Nothing");
		none.SetActive(false);
		loadData();
	}

	public void loadData()
	{
		mainHandle.pokerRecordList((error, result) =>
		{
			if (error == null)
			{
				if (result.ret == 1 && result.list != null)
				{
                    for (int i = result.count; i > result.count-10;i--){
                        result.list[result.count-i].order_id = i;
                    }
					SetDatas(result.list);
				}
				else
				{
					none.SetActive(true);
				}

			}
		});
	}
}
