using System;
using System.Collections;
using UnityEngine;

public class IEnumeratorProvider
{
    // 延迟处理
    public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}
