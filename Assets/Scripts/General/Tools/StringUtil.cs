using System;

public class StringUtil
{
    // 获取字符串筹码
    public static string GetStringChip(int price)
    {
        float p = price / 10000f;
        if (p >= 1)
        {
            return p + "w";
        }
        return price.ToString();
    }

}
