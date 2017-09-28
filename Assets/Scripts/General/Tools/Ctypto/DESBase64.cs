using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

/**
 * DES加解密
 */
public class DESBase64
{
    /** 本地加密密钥 **/
    static string localkey = "dezhoupoker";
	/** 购买加密密钥 **/
    public static string purchaseKey = "fun*&fm%";

    public static string DesEncrypt(string encryptString)
    {
        return DesEncryptWithKey(encryptString, localkey);
    }

    public static string DesDecrypt(string decryptString) 
    {
        return DesDecryptWithKey(decryptString, localkey);
    }

	/**
     * DES加密
     */
	public static string DesEncryptWithKey(string source, string sourceKey) {
        byte[] keyBytes = GetKeyMd5Hash(sourceKey);
        return DesEncrypt(source, keyBytes);
    }

	/**
     * DES解密
     */
	public static string DesDecryptWithKey(string source, string sourceKey)
	{
		byte[] keyBytes = GetKeyMd5Hash(sourceKey);
		return DesDecrypt(source, keyBytes);
	}

	public static string DesEncrypt(string toEncrypt, byte[] privateKey)
	{
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

		TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
		{
			Key = privateKey,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		};

		ICryptoTransform cTransform = tdes.CreateEncryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		tdes.Clear();

		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	public static string DesDecrypt(string toDecrypt, byte[] privateKey)
	{
		//先base64解密 因为加密的时候最后走了一道base64加密
		byte[] enBytes = Convert.FromBase64String(toDecrypt);

		TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
		{
			Key = privateKey,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		};

		ICryptoTransform cTransform = tdes.CreateDecryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(enBytes, 0, enBytes.Length);
		tdes.Clear();

		return Encoding.UTF8.GetString(resultArray);

	}

	public static byte[] GetKeyMd5Hash(string key)
	{
		MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
		byte[] keyBytes = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
		hashmd5.Clear();

		return keyBytes;
	}
}
