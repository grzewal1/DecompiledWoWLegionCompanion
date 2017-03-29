using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SecurePlayerPrefs
{
	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(SecurePlayerPrefs.GenerateMD5(key));
	}

	private static string GenerateMD5(string text)
	{
		MD5 mD5 = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		byte[] numArray = mD5.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < (int)numArray.Length; i++)
		{
			stringBuilder.Append(numArray[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public static string GetString(string key, string password)
	{
		string str;
		string str1 = SecurePlayerPrefs.GenerateMD5(key);
		if (!PlayerPrefs.HasKey(str1))
		{
			return string.Empty;
		}
		DESEncryption dESEncryption = new DESEncryption();
		string str2 = PlayerPrefs.GetString(str1);
		dESEncryption.TryDecrypt(str2, password, out str);
		return str;
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(SecurePlayerPrefs.GenerateMD5(key));
	}

	public static void SetString(string key, string value, string password)
	{
		DESEncryption dESEncryption = new DESEncryption();
		string str = SecurePlayerPrefs.GenerateMD5(key);
		PlayerPrefs.SetString(str, dESEncryption.Encrypt(value, password));
	}
}