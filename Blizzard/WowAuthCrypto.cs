using System;
using System.Security.Cryptography;

namespace Blizzard
{
	public static class WowAuthCrypto
	{
		public const int CHALLENGE_SIZE = 16;

		public const int PROOF_SIZE = 24;

		public const int SECRET_SIZE = 32;

		public const int SESSION_KEY_SIZE = 40;

		private static byte[] REALM_JOIN_TAG;

		private static byte[] CONTINUE_SESSION_TAG;

		private static byte[] MAKE_SESSION_KEY_TAG;

		static WowAuthCrypto()
		{
			WowAuthCrypto.REALM_JOIN_TAG = new byte[] { 197, 198, 152, 149, 118, 63, 29, 205, 182, 161, 55, 40, 179, 18, 255, 138 };
			WowAuthCrypto.CONTINUE_SESSION_TAG = new byte[] { 22, 173, 12, 212, 70, 249, 79, 178, 239, 125, 234, 42, 23, 102, 77, 47 };
			WowAuthCrypto.MAKE_SESSION_KEY_TAG = new byte[] { 88, 203, 207, 64, 254, 46, 206, 166, 90, 144, 184, 1, 104, 108, 40, 11 };
		}

		private static bool CompareBytes(byte[] left, byte[] right)
		{
			if (left == null || right == null || (int)left.Length != (int)right.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)left.Length; i++)
			{
				if (left[i] != right[i])
				{
					return false;
				}
			}
			return true;
		}

		private static byte[] Concatenate(params byte[][] arrays)
		{
			int length = 0;
			for (int i = 0; i < (int)arrays.Length; i++)
			{
				length = checked(length + (int)arrays[i].Length);
			}
			byte[] numArray = new byte[length];
			int num = 0;
			for (int j = 0; j < (int)arrays.Length; j++)
			{
				Array.Copy(arrays[j], 0, numArray, num, (int)arrays[j].Length);
				num += (int)arrays[j].Length;
			}
			return numArray;
		}

		private static string FormatHex(byte[] data)
		{
			if (data == null || (int)data.Length == 0)
			{
				return string.Empty;
			}
			return string.Concat("0x", BitConverter.ToString(data).Replace("-", ", 0x"), ",");
		}

		public static byte[] GenerateChallenge()
		{
			byte[] numArray = new byte[16];
			(new RNGCryptoServiceProvider()).GetBytes(numArray);
			return numArray;
		}

		public static byte[] GenerateSecret()
		{
			byte[] numArray = new byte[32];
			(new RNGCryptoServiceProvider()).GetBytes(numArray);
			return numArray;
		}

		private static void Main(string[] args)
		{
			byte[] numArray = new byte[] { 176, 78, 211, 218, 114, 8, 0, 13, 123, 221, 218, 157, 108, 99, 149, 198, 119, 9, 57, 63, 0, 71, 192, 27, 250, 64, 179, 242, 236, 206, 83, 13 };
			byte[] numArray1 = new byte[] { 125, 73, 96, 253, 110, 64, 35, 206, 201, 11, 222, 183, 70, 22, 93, 32, 137, 184, 228, 228, 142, 16, 50, 68, 42, 99, 74, 105, 38, 131, 42, 69 };
			byte[] numArray2 = new byte[] { 215, 45, 27, 195, 145, 121, 49, 207, 52, 62, 232, 194, 241, 226, 8, 189 };
			byte[] numArray3 = new byte[] { 216, 246, 86, 53, 124, 144, 185, 179, 47, 229, 128, 162, 102, 108, 178, 29 };
			byte[] numArray4 = new byte[] { 78, 227, 176, 41, 143, 104, 175, 7, 56, 183, 255, 18, 255, 55, 78, 109 };
			ulong num = 5793138720705260506L;
			byte[] numArray5 = new byte[] { 73, 217, 13, 12, 31, 228, 141, 202, 216, 215, 67, 157, 130, 132, 127, 190, 140, 67, 2, 225, 154, 29, 230, 39 };
			byte[] numArray6 = new byte[] { 74, 43, 54, 227, 47, 242, 173, 15, 166, 230, 105, 27, 126, 94, 167, 172, 173, 109, 25, 146, 229, 113, 125, 45, 125, 168, 233, 176, 201, 155, 71, 12, 199, 110, 211, 48, 92, 160, 231, 41 };
			byte[] numArray7 = new byte[] { 122, 17, 5, 194, 174, 42, 175, 11, 175, 153, 229, 247, 188, 184, 17, 100, 32, 127, 229, 167, 236, 74, 96, 132 };
			byte[] numArray8 = WowAuthCrypto.ProveRealmJoinChallenge(numArray, numArray1, numArray2, numArray3);
			Console.WriteLine(WowAuthCrypto.CompareBytes(numArray5, numArray8));
			Console.WriteLine(WowAuthCrypto.VerifyRealmJoinChallenge(numArray5, numArray, numArray1, numArray2, numArray3));
			byte[] numArray9 = WowAuthCrypto.MakeSessionKey(numArray, numArray1, numArray2, numArray4);
			Console.WriteLine(WowAuthCrypto.CompareBytes(numArray6, numArray9));
			byte[] numArray10 = WowAuthCrypto.ProveContinueSessionChallenge(numArray9, numArray2, numArray3, num);
			Console.WriteLine(WowAuthCrypto.CompareBytes(numArray7, numArray10));
			Console.WriteLine(WowAuthCrypto.VerifyContinueSessionChallenge(numArray10, numArray9, numArray2, numArray3, num));
		}

		public static byte[] MakeSessionKey(byte[] clientSecret, byte[] joinSecret, byte[] clientChallenge, byte[] userRouterChallenge)
		{
			SHA256 sHA256CryptoServiceProvider = new SHA256CryptoServiceProvider();
			byte[] numArray = sHA256CryptoServiceProvider.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { clientSecret, joinSecret }));
			HMACSHA256 hMACSHA256 = new HMACSHA256(numArray);
			byte[] numArray1 = hMACSHA256.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { userRouterChallenge, clientChallenge, WowAuthCrypto.MAKE_SESSION_KEY_TAG }));
			return (new WowAuthCrypto.CryptoRandom(numArray1)).Read(40);
		}

		public static byte[] ProveContinueSessionChallenge(byte[] sessionKey, byte[] clientChallenge, byte[] serverChallenge, ulong connectionKey)
		{
			WowAuthCrypto.ValidateParameter(sessionKey, 40, "sessionKey");
			WowAuthCrypto.ValidateParameter(clientChallenge, 16, "clientChallenge");
			WowAuthCrypto.ValidateParameter(serverChallenge, 16, "serverChallenge");
			byte[] bytes = BitConverter.GetBytes(connectionKey);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			HMACSHA256 hMACSHA256 = new HMACSHA256(sessionKey);
			byte[] numArray = hMACSHA256.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { bytes, clientChallenge, serverChallenge, WowAuthCrypto.CONTINUE_SESSION_TAG }));
			byte[] numArray1 = new byte[24];
			Array.Copy(numArray, numArray1, (int)numArray1.Length);
			return numArray1;
		}

		public static byte[] ProveRealmJoinChallenge(byte[] clientSecret, byte[] joinSecret, byte[] clientChallenge, byte[] serverChallenge)
		{
			WowAuthCrypto.ValidateParameter(clientSecret, 32, "clientSecret");
			WowAuthCrypto.ValidateParameter(joinSecret, 32, "joinSecret");
			WowAuthCrypto.ValidateParameter(clientChallenge, 16, "clientChallenge");
			WowAuthCrypto.ValidateParameter(serverChallenge, 16, "serverChallenge");
			SHA256 sHA256CryptoServiceProvider = new SHA256CryptoServiceProvider();
			byte[] numArray = sHA256CryptoServiceProvider.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { clientSecret, joinSecret }));
			HMACSHA256 hMACSHA256 = new HMACSHA256(numArray);
			byte[] numArray1 = hMACSHA256.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { clientChallenge, serverChallenge, WowAuthCrypto.REALM_JOIN_TAG }));
			byte[] numArray2 = new byte[24];
			Array.Copy(numArray1, numArray2, (int)numArray2.Length);
			return numArray2;
		}

		private static void ValidateParameter(byte[] param, int expectedSize, string paramName)
		{
			if (param == null || (int)param.Length != expectedSize)
			{
				throw new ArgumentException("Improper size of contained array", paramName);
			}
		}

		public static bool VerifyContinueSessionChallenge(byte[] proof, byte[] sessionKey, byte[] clientChallenge, byte[] serverChallenge, ulong connectionKey)
		{
			WowAuthCrypto.ValidateParameter(proof, 24, "proof");
			byte[] numArray = WowAuthCrypto.ProveContinueSessionChallenge(sessionKey, clientChallenge, serverChallenge, connectionKey);
			return WowAuthCrypto.CompareBytes(proof, numArray);
		}

		public static bool VerifyRealmJoinChallenge(byte[] proof, byte[] clientSecret, byte[] joinSecret, byte[] clientChallenge, byte[] serverChallenge)
		{
			WowAuthCrypto.ValidateParameter(proof, 24, "proof");
			byte[] numArray = WowAuthCrypto.ProveRealmJoinChallenge(clientSecret, joinSecret, clientChallenge, serverChallenge);
			return WowAuthCrypto.CompareBytes(proof, numArray);
		}

		private class CryptoRandom
		{
			private SHA256 sha256;

			private int used;

			private byte[] data;

			private byte[] key0;

			private byte[] key1;

			public CryptoRandom(byte[] seed)
			{
				byte[] numArray = new byte[(int)seed.Length / 2];
				byte[] numArray1 = new byte[(int)seed.Length - (int)numArray.Length];
				Array.Copy(seed, 0, numArray, 0, (int)numArray.Length);
				Array.Copy(seed, (int)numArray.Length, numArray1, 0, (int)numArray1.Length);
				this.sha256 = new SHA256CryptoServiceProvider();
				this.used = 0;
				this.key0 = this.sha256.ComputeHash(numArray);
				this.key1 = this.sha256.ComputeHash(numArray1);
				this.data = new byte[(int)this.key0.Length];
				this.Process();
			}

			private void Process()
			{
				this.data = this.sha256.ComputeHash(WowAuthCrypto.Concatenate(new byte[][] { this.key0, this.data, this.key1 }));
				this.used = 0;
			}

			public byte[] Read(int size)
			{
				byte[] numArray = new byte[size];
				for (int i = 0; i < size; i++)
				{
					if (this.used == (int)this.data.Length)
					{
						this.Process();
					}
					byte[] numArray1 = this.data;
					WowAuthCrypto.CryptoRandom cryptoRandom = this;
					int num = cryptoRandom.used;
					int num1 = num;
					cryptoRandom.used = num + 1;
					numArray[i] = numArray1[num1];
				}
				return numArray;
			}
		}
	}
}