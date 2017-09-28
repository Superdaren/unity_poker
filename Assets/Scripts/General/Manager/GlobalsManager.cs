using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        NetCore.Instance.Connect(NetProto.Config.address, NetProto.Config.port);

#if UNITY_IOS
		// 初始化iOS SDK
		SDKToIOS.Instance.InitSDK();
        PurchaseToIOS.resendAppStoreRequestReceipt();
#elif UNITY_ANDROID
        SDKToAndroid.Instance.InitSDK();
#endif
    }

}
