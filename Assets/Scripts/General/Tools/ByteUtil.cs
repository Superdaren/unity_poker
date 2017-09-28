using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ByteUtil
{
    public static byte[] ToBytes(string message)
    {
        return Encoding.UTF8.GetBytes(message);
    }

    public static string ToString(byte[] body)
    {
        return Encoding.UTF8.GetString(body);
    }
}
