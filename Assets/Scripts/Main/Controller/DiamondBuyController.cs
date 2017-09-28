using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DiamondBuyController : MonoBehaviour
{
	// 钻石商品物体集合
	List<GameObject> diamondObjectList;

	// Use this for initialization
	void Start()
	{
		ShoppingHandle.getDiamondList((error, resultList) =>
		{
			if (error == null)
			{
                MainData.Instance().diamondList = resultList.list;
				loadGoldView();
			}
		});
	}

	void loadGoldView()
	{
		GameObject commonUIPrefab = Resources.Load("Prefabs/DiamondItem") as GameObject;
		diamondObjectList = new List<GameObject>();
        for (int i = 0; i < MainData.Instance().diamondList.Length; i++)
		{
			GameObject diamond = Instantiate(commonUIPrefab) as GameObject;
            float width =  diamond.GetComponent<RectTransform>().sizeDelta.x;
            diamond.transform.parent = GameObject.Find(gameObject.name + "/BackGround").transform;
			diamond.name = "" + i;
			diamond.transform.localPosition = new Vector3(-300 + i * width, -70, 0);
			diamond.transform.localScale = new Vector3(1, 1, 0);

            Diamond goods = MainData.Instance().diamondList[i];
            Text DiamondNum = gameObject.Find<Text>(diamond.name + "/DiamondNum");
            Text money = gameObject.Find<Text>(diamond.name + "/BuyBtn/Money");
            DiamondNum.text = goods.number.ToString() + " 钻";
            money.text = "¥ " + goods.price.ToString();

            Button buyBtn = gameObject.Find<Button>(diamond.name + "/BuyBtn");
            buyBtn.onClick.AddListener(() => {
                PurchaseToIOS.purchaseProductWithID(gameObject.name, goods.charge_goods_id);
            });

			diamondObjectList.Add(diamond);
		}
	}

	/**
     * ========================点击事件=============================
     */
	public void closeBtnClick()
	{
        PKAnimateTool.closePopUpView(gameObject);
	}

}
