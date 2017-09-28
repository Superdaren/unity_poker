using UnityEngine;
using System.IO;
using System;

using Google.Protobuf;

namespace NetProto
{
	public class SerializeManager
	{

		/**
		 * 序列化对象
		 *  
		 */
        public static byte[] Serialize(IMessage instance)
        {
            if (instance != null)
            {
                byte[] data = null;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
						instance.WriteTo(ms);
						data = ms.ToArray();
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
	}
}

