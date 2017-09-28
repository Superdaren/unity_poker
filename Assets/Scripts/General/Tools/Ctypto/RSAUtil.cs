using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;

public class RSAUtil
{
    
    public static string public_key 
    {
        get {
            if(URLManager.isDebug) {
                return "<RSAKeyValue><Modulus>wwuu4FKpnGK1FMZ1Y3MmkqrqS/tdh9WXf3cV19MIGLLJNW/o9lnh0YNrcLd1H0XMWy+mJSPvgGbbX793g9OsndMaU12JggPqLC8JXwtR72tNM4MihoJloKIvASlF/cakHg3ZittVHgkSnFo6uuoFOxMWYJzdDV09+cDizy4/CjSq38JT3/x9aG20XtjsPnGIgU1puLu2p8yksTJ+nuiav/QgoXQUtgzCbRJj+W9BPXdQhjpAUeaLglP5QPZ9cGmADqRfgH4pWc3tozbaAEjpsukvqBbpG2ybEtilBDVrBmbWpXuhTg2yHxxzVgagp3Agdbddm7ZeiTamYsl/GjK1Uw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            } else {
                return "";
            }
        }
    }

    public static string private_key = "<RSAKeyValue><Modulus>2S5qwQyM4dCjeL0utTxBJ+8y+QgqXBNsL8tqqF/6epbyHhQmlePbItBf2kPX0detqQPqIoZPkf9SDgR87sjeQS3EYD4NdQukxgc44UMezbDt1fL0wRs9upw6XqoYkWC1I2ULSq/EHC10rd2qHXGZR0psgFOvf0LmnxP8XEAMgEKXVO3mCdHHRqh2t5O0DW2cFmM/Z05ebT9LNBZexUzIWTcdeAt/Edp1qj8oyhhQ86mERoSRTsM4QjMZVNROd3vkqJSIc2r7nIOa9utIuzOYHB/P5N76Vi5HvkSyhF3zWvLQi2BpLb8oPw3aorCAW6c8btiJ6uQb8sllSI4MNCkIDw==</Modulus><Exponent>AQAB</Exponent><P>9KlYwbgj89+Pv5gfGVbrUX4pTbXzFetaTmKXr8hVIijiOni+n6Cj0bTwRYS8ijcWyGBjJzS+Mloy6xrxBwIYqani7nbF5m5YF4YxQg2ttqHmpTgiGlBny7QPDyitXZywoKiK7HvjHKkpEo/bxbUPS8/G3zNLujkLL6lPx3l0Bas=</P><Q>4z8L52ePfkskEh+pc3emPm+RgQ/hpWgNW+sXAdt+JLNb4CtVLdbEjeLrOrBlpsOQFBynETCETanKlguG5VMnriJVSIin1B0M4MGploCDBg92smjmnNzUDC6kyJlra48WueJYy4Mbj22xAhpRL4Cb8oJRu9jT3AAyjLPXSEmBGy0=</Q><DP>4RLRZhUCbh2iXj2RjnwZqzSGxsi1wdprj2S/6qQ/PVejFutxYeQkEI2F5E5CLFyVhuy1CqJIvoWgBg0Y4ruY+Kwi6gE1+dBVFf8LXZq7ziGOzzuek9qYQht5JxbithWpn4KtmvQ1cG5mLH9PTMhONA4HA9pqtfw4QDZgAD/I7lU=</DP><DQ>ZBvXG6cUoLdreYx3MsXn96NgvzGG3z46fF7RcOEH7hURiWythpcRWcw4gk9JSLDoXOPOeZUdA7Wj6HvFfmsmlODNHzoMbTYrASxvkDym+9l+GdR6m2nxBDjcIP7tQqHzign7whPM5V+WVe3QDBQlHy1n4k//7PAtVKy5T0AKXzU=</DQ><InverseQ>qjsJNEeQClk4+nFhDRbAOuBzhCyZdmebHP30ZsK9GobyD+iGRT0K/qAhP86eQaIqUu0gEAAw5/93/Zvxpr12pW51VPmqVCwC4V70FGn4ZKdtFhwGcZP8kmsZngUwLeiKGOs0PPVMdNC3PK9irBHWbX0yGdNeNrox0A9zwNZdJhA=</InverseQ><D>Y9jVN5+/fWngXfgL60KiaeussZUxdFdxm5+Xj4VgsM+IzI9TFgJFkwHdr4kVReI0csnb8LdzCZoqbUte7KDonWqfJ+gfY7pQrMJRQi8LkGFRVtV19PnDLXH529+zn9KoNHeV82ZPcOztAaoxNYwuffoXXAAdZRKq2GPl17uSg7xAxXNqz+/WDR4yZW9OwbWE2YRnE7XKaG0cChGMJSGpiH0q8aykuG7O+vFaZfGpzV+NKDHZgrCtA9y79ESA0WAzaHQA9qRuL6K4BGg4SllP6wsxI270PXUIPBeim0IVHM6MV7j79JSGAC2dTJ2dcX8i2rDa7wvGtwEgTelxEdJc8Q==</D></RSAKeyValue>";

	public static string RSAEncrypt(string publickey, string content)
	{
		//最大文件加密块
		int MAX_ENCRYPT_BLOCK = 116;

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		byte[] cipherbytes;
		rsa.FromXmlString(publickey);
		byte[] contentByte = Encoding.UTF8.GetBytes(content);
		int inputLen = contentByte.Length;

		int offSet = 0;
		byte[] cache;
		int i = 0;
		System.IO.MemoryStream aMS = new System.IO.MemoryStream();
		// 对数据分段加密
		while (inputLen - offSet > 0)
		{
			byte[] temp = new byte[MAX_ENCRYPT_BLOCK];
			if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
			{
				Array.Copy(contentByte, offSet, temp, 0, MAX_ENCRYPT_BLOCK);
				cache = rsa.Encrypt(temp, false);
			}
			else
			{
				Array.Copy(contentByte, offSet, temp, 0, inputLen - offSet);
				cache = rsa.Encrypt(temp, false);
			}
			aMS.Write(cache, 0, cache.Length);
			i++;
			offSet = i * MAX_ENCRYPT_BLOCK;
		}

		cipherbytes = aMS.ToArray();
		return Convert.ToBase64String(cipherbytes);
	}

	public static string RSADecrypt(string privatekey, string content)
	{
		//最大文件解密块
		int MAX_DECRYPT_BLOCK = 256;

		RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		byte[] cipherbytes;
		rsa.FromXmlString(privatekey);
		byte[] contentByte = Convert.FromBase64String(content);
		int inputLen = contentByte.Length;

		// 对数据分段解密
		int offSet = 0;
		int i = 0;
		byte[] cache;
		System.IO.MemoryStream aMS = new System.IO.MemoryStream();
		while (inputLen - offSet > 0)
		{
			byte[] temp = new byte[MAX_DECRYPT_BLOCK];
			if (inputLen - offSet > MAX_DECRYPT_BLOCK)
			{
				Array.Copy(contentByte, offSet, temp, 0, MAX_DECRYPT_BLOCK);
				cache = rsa.Decrypt(temp, false);
			}
			else
			{
				Array.Copy(contentByte, offSet, temp, 0, inputLen - offSet);
				cache = rsa.Decrypt(temp, false);
			}
			aMS.Write(cache, 0, cache.Length);
			i++;
			offSet = i * MAX_DECRYPT_BLOCK;
		}
		cipherbytes = aMS.ToArray();

		return Encoding.UTF8.GetString(cipherbytes);
	}
}
