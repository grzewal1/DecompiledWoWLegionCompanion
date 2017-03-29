using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DESEncryption : IEncryption
{
	private const int Iterations = 1000;

	public DESEncryption()
	{
	}

	public string Encrypt(string plainText, string password)
	{
		string base64String;
		if (plainText == null)
		{
			throw new ArgumentNullException("plainText");
		}
		if (string.IsNullOrEmpty(password))
		{
			throw new ArgumentNullException("password");
		}
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.GenerateIV();
		Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(password, dESCryptoServiceProvider.IV, 1000);
		byte[] bytes = rfc2898DeriveByte.GetBytes(8);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, dESCryptoServiceProvider.IV), CryptoStreamMode.Write))
			{
				memoryStream.Write(dESCryptoServiceProvider.IV, 0, (int)dESCryptoServiceProvider.IV.Length);
				byte[] numArray = Encoding.UTF8.GetBytes(plainText);
				cryptoStream.Write(numArray, 0, (int)numArray.Length);
				cryptoStream.FlushFinalBlock();
				base64String = Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		return base64String;
	}

	public bool TryDecrypt(string cipherText, string password, out string plainText)
	{
		bool flag;
		if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(password))
		{
			plainText = string.Empty;
			return false;
		}
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
				byte[] numArray = new byte[8];
				memoryStream.Read(numArray, 0, (int)numArray.Length);
				byte[] bytes = (new Rfc2898DeriveBytes(password, numArray, 1000)).GetBytes(8);
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(bytes, numArray), CryptoStreamMode.Read))
				{
					using (StreamReader streamReader = new StreamReader(cryptoStream))
					{
						plainText = streamReader.ReadToEnd();
						flag = true;
					}
				}
			}
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			plainText = string.Empty;
			flag = false;
		}
		return flag;
	}
}