using MiniJSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace bgs
{
	public class SslSocket
	{
		private const int PUBKEY_MODULUS_SIZE_BITS = 2048;

		private const int PUBKEY_MODULUS_SIZE_BYTES = 256;

		private const int PUBKEY_EXP_SIZE_BYTES = 4;

		private string m_address;

		public SslCertBundleSettings m_bundleSettings;

		private static Map<SslStream, SslSocket.SslStreamValidateContext> s_streamValidationContexts;

		private static string s_magicBundleSignaturePreamble;

		private static byte[] s_standardPublicExponent;

		private static byte[] s_standardPublicModulus;

		private static byte[] s_debugPublicExponent;

		private static byte[] s_debugPublicModulus;

		private static LogThreadHelper s_log;

		private TcpConnection m_connection = new TcpConnection();

		private SslStream m_sslStream;

		private SslSocket.BeginConnectDelegate m_beginConnectDelegate;

		private static X509Certificate2 s_rootCertificate;

		public bool m_canSend = true;

		public bool Connected
		{
			get
			{
				return (this.Socket == null ? false : this.Socket.Connected);
			}
		}

		private System.Net.Sockets.Socket Socket
		{
			get
			{
				return this.m_connection.Socket;
			}
		}

		static SslSocket()
		{
			string str;
			SslSocket.s_streamValidationContexts = new Map<SslStream, SslSocket.SslStreamValidateContext>();
			SslSocket.s_magicBundleSignaturePreamble = "NGIS";
			SslSocket.s_standardPublicExponent = new byte[] { 1, 0, 1, 0 };
			SslSocket.s_standardPublicModulus = new byte[] { 53, 255, 23, 231, 51, 196, 211, 212, 240, 55, 164, 181, 124, 27, 240, 78, 49, 232, 255, 179, 12, 30, 136, 16, 77, 175, 19, 11, 88, 86, 88, 25, 88, 55, 21, 249, 235, 236, 152, 203, 157, 204, 253, 24, 241, 71, 9, 27, 227, 123, 56, 40, 158, 14, 155, 31, 159, 149, 218, 157, 97, 117, 242, 31, 160, 61, 162, 153, 189, 178, 29, 14, 105, 202, 188, 115, 27, 229, 235, 15, 231, 251, 43, 123, 178, 53, 5, 143, 245, 181, 154, 59, 18, 173, 161, 164, 140, 247, 144, 102, 136, 23, 214, 31, 147, 132, 16, 174, 242, 239, 42, 122, 95, 65, 123, 92, 128, 210, 94, 26, 253, 219, 16, 118, 147, 188, 139, 213, 230, 178, 80, 245, 81, 155, 3, 226, 83, 155, 168, 176, 177, 55, 213, 37, 102, 69, 8, 129, 32, 15, 136, 97, 174, 187, 245, 68, 245, 132, 158, 118, 39, 21, 116, 23, 198, 183, 143, 224, 45, 55, 92, 248, 82, 49, 50, 63, 250, 68, 127, 239, 36, 61, 91, 89, 249, 253, 80, 80, 202, 160, 54, 77, 98, 217, 68, 13, 105, 166, 239, 43, 206, 204, 194, 163, 188, 245, 162, 28, 238, 119, 69, 228, 51, 240, 87, 32, 191, 46, 7, 134, 43, 149, 187, 58, 252, 4, 60, 69, 63, 0, 52, 11, 54, 187, 75, 193, 15, 149, 24, 195, 217, 250, 54, 66, 202, 150, 170, 236, 122, 46, 136, 130, 60, 29, 152, 148 };
			SslSocket.s_debugPublicExponent = new byte[] { 1, 0, 1, 0 };
			SslSocket.s_debugPublicModulus = new byte[] { 133, 243, 123, 20, 90, 156, 72, 246, 79, 229, 73, 223, 100, 255, 35, 43, 111, 158, 174, 59, 13, 207, 219, 80, 164, 251, 93, 160, 119, 204, 236, 249, 106, 12, 191, 31, 19, 173, 45, 8, 208, 211, 12, 85, 187, 50, 166, 112, 178, 45, 160, 25, 10, 24, 200, 200, 69, 126, 17, 102, 65, 20, 184, 123, 229, 77, 195, 184, 109, 210, 8, 183, 31, 238, 180, 155, 156, 231, 203, 148, 186, 235, 236, 200, 155, 37, 156, 192, 152, 78, 214, 10, 155, 134, 106, 91, 112, 117, 27, 120, 179, 68, 172, 107, 64, 129, 222, 215, 98, 229, 7, 183, 89, 67, 250, 8, 12, 167, 197, 111, 29, 235, 63, 225, 128, 171, 115, 109, 166, 70, 248, 236, 219, 172, 96, 148, 28, 63, 216, 217, 4, 150, 104, 52, 146, 26, 205, 201, 168, 43, 51, 5, 143, 75, 39, 34, 64, 42, 215, 247, 190, 151, 237, 118, 104, 231, 37, 194, 144, 173, 78, 241, 231, 180, 168, 65, 84, 58, 62, 24, 172, 33, 244, 211, 67, 109, 68, 128, 64, 71, 125, 99, 4, 29, 94, 6, 139, 159, 59, 6, 43, 162, 140, 23, 48, 128, 224, 103, 197, 33, 61, 63, 126, 157, 99, 220, 246, 80, 122, 159, 175, 194, 47, 7, 10, 187, 225, 139, 48, 171, 158, 244, 252, 43, 20, 201, 5, 61, 53, 80, 92, 245, 233, 111, 46, 38, 80, 151, 17, 129, 179, 215, 43, 149, 115, 166 };
			SslSocket.s_log = new LogThreadHelper("SslSocket");
			str = (BattleNet.Client().GetMobileEnvironment() != constants.MobileEnv.PRODUCTION ? "-----BEGIN CERTIFICATE-----MIIGvTCCBKWgAwIBAgIJANOYGVoF3JlVMA0GCSqGSIb3DQEBBQUAMIGaMQswCQYDVQQGEwJVUzETMBEGA1UECBMKQ2FsaWZvcm5pYTEPMA0GA1UEBxMGSXJ2aW5lMSUwIwYDVQQKExxCbGl6emFyZCBFbnRlcnRhaW5tZW50LCBJbmMuMRMwEQYDVQQLEwpCYXR0bGUubmV0MSkwJwYDVQQDEyBCYXR0bGUubmV0IENlcnRpZmljYXRlIEF1dGhvcml0eTAeFw0xMzA4MTYxNTQzMzRaFw00MzA4MDkxNTQzMzRaMIGaMQswCQYDVQQGEwJVUzETMBEGA1UECBMKQ2FsaWZvcm5pYTEPMA0GA1UEBxMGSXJ2aW5lMSUwIwYDVQQKExxCbGl6emFyZCBFbnRlcnRhaW5tZW50LCBJbmMuMRMwEQYDVQQLEwpCYXR0bGUubmV0MSkwJwYDVQQDEyBCYXR0bGUubmV0IENlcnRpZmljYXRlIEF1dGhvcml0eTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAM9HMQkty5nBA4BjxQmQXiEuPk7FceTe82pgeZjMLRq7j2+BO100gjgfn1+rjGbsw+wDB/QlgtNOB3X42P/A2vvXfdxFGLsIAS0+f6Uv1CaEphJ/55vLhfp5l/CfWAHAi3JkVJl37hX8Y/K/UJTqyFdspKkRrRmT9ky8i2BGWfnvqJ0hEfJqVy1b04ifM/d1uq0m3q3URmzQhBAfG85VoeSewqeSuPhRrmZw0wTVJsfx09HSd842e6aECUXGXPRwwgWC1YQvXjxG9uxGo/8ZtOqzZ7L+6DwKn2OL7qmqjZMRq8KkcvFbKyPKRHaDkeC0YAs58rLG9gbYYWTPgBQtCfo23mlnFiWeUjpSIJ+OF39kShrq7jcSt5qJEv8XIfScesOHFnAJwwxvwWvpleXk2VDTgzr1uZNqQig6SixIptsbkJinXAKn+5MzM7jOGeVT9jPVoKyY8eOchkaOZGyTeZEEGwqn31jRZ8Br+bqSrX5ahyxASfUhyss/8oBBw4kJ6PPyCGG2kgTH9bvVVEqRRpwhvQWQXcg6rN37z9FsC65+aVCRVYdLIts220+XKQEmG15Q5YK3650qywQYY2qlKgGDU4QxSoBNF2dV9AwRhJNDdgGt25/tWDcLdCPYqm0sapd6OyJc2l2gwk7zbR3Ln9UFWuRXowRlEKjtiO0ToI8/AgMBAAGjggECMIH/MB0GA1UdDgQWBBSJBQiKQ3q5ckiO4UWAssWt+DldVDCBzwYDVR0jBIHHMIHEgBSJBQiKQ3q5ckiO4UWAssWt+DldVKGBoKSBnTCBmjELMAkGA1UEBhMCVVMxEzARBgNVBAgTCkNhbGlmb3JuaWExDzANBgNVBAcTBklydmluZTElMCMGA1UEChMcQmxpenphcmQgRW50ZXJ0YWlubWVudCwgSW5jLjETMBEGA1UECxMKQmF0dGxlLm5ldDEpMCcGA1UEAxMgQmF0dGxlLm5ldCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHmCCQDTmBlaBdyZVTAMBgNVHRMEBTADAQH/MA0GCSqGSIb3DQEBBQUAA4ICAQAmHUsDOLjsqc8THvQAzonRKbbKrvzJaww88LKGTH4rUwu9+YZEhjl1rvOdvWuQVOWnWozycq68WMwrUEAF0boS5g/aicJMgQPGpo+t6MxyTNT0QjKClISlInZKAIhvhpWQ5VyfZHswgjIKemhEbbgj9mJWXRS2p6x2PCckillL5qUh6+m2moTbImzEYf1By36IWrh+xUBMT2xE7TR2kq6Ac7kNgbXV7Ve/qrGDlQI9R26pOt9os+CNkrdHVtRSIAI8+CKjFA7dbGM71/scmaLXMmKA0pcuXo167LCl+MhT0ruCKA8AiV7YWq1fAiGtgw9DaTDKtAdG3tMa//J/XCvTKo4VPlOxyzd04GJIXwUIz4WuZHtsc4PRXYtY8nJCIBbRdDBSOV4MtIGz3UC2pj+mDbJJ4MrC03qAGK3nAo7Z4kkbBuTctfn6Arq/tf5VTrjMMpTAeAvB8hG2vKYBe5YMyjx80GzxNde23wu4czlmEwVc/0tCtzZcWYty2b749oydMslmez6GvVcaJ14Ln6jpinTg6XoM5x2+vcs0oG7CjuTO+GBirjk9z3asn40dz2rOdWhX0JPfR2+qnizkl/6FzzOXPPthBgjrj1CiTWLo4xtPMF370di8pwdOpoBxu7c2cbemhCdORxgt5QGKWCe8HVLIWTSvb38qcfJ7eKnRbQ==-----END CERTIFICATE-----" : "-----BEGIN CERTIFICATE-----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-----END CERTIFICATE-----");
			SslSocket.s_rootCertificate = new X509Certificate2(Encoding.ASCII.GetBytes(str));
		}

		public SslSocket()
		{
		}

		public void BeginConnect(string address, int port, SslCertBundleSettings bundleSettings, SslSocket.BeginConnectDelegate connectDelegate)
		{
			this.m_beginConnectDelegate = connectDelegate;
			this.m_bundleSettings = bundleSettings;
			this.m_connection.LogDebug = new Action<string>(SslSocket.s_log.LogDebug);
			this.m_connection.LogWarning = new Action<string>(SslSocket.s_log.LogWarning);
			this.m_connection.OnFailure = () => this.ExecuteBeginConnectDelegate(true);
			this.m_connection.OnSuccess = new Action(this.ConnectCallback);
			this.m_connection.Connect(address, port);
		}

		public void BeginReceive(byte[] buffer, int size, SslSocket.BeginReceiveDelegate beginReceiveDelegate)
		{
			try
			{
				if (this.m_sslStream == null)
				{
					throw new NullReferenceException("m_sslStream is null!");
				}
				this.m_sslStream.BeginRead(buffer, 0, size, new AsyncCallback(this.ReadCallback), beginReceiveDelegate);
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogWarning("Exception while trying to call BeginRead. {0}", new object[] { exception });
				if (beginReceiveDelegate != null)
				{
					beginReceiveDelegate(0);
				}
			}
		}

		public void BeginSend(byte[] bytes, SslSocket.BeginSendDelegate sendDelegate)
		{
			try
			{
				if (this.m_sslStream == null)
				{
					throw new NullReferenceException("m_sslStream is null!");
				}
				this.m_canSend = false;
				this.m_sslStream.BeginWrite(bytes, 0, (int)bytes.Length, new AsyncCallback(this.WriteCallback), sendDelegate);
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogWarning("Exception while trying to call BeginWrite. {0}", new object[] { exception });
				if (sendDelegate != null)
				{
					sendDelegate(false);
				}
			}
		}

		public void Close()
		{
			SslStream mSslStream = this.m_sslStream;
			this.m_sslStream = null;
			try
			{
				this.m_connection.Disconnect();
				if (mSslStream != null)
				{
					mSslStream.Close();
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void ConnectCallback()
		{
			byte[] numArray;
			try
			{
				this.ResolveSSLAddress();
				if (FileUtil.LoadFromDrive(SslSocket.GetBundleStoragePath(), out numArray))
				{
					this.m_bundleSettings.bundle = new SslCertBundle(numArray);
				}
				RemoteCertificateValidationCallback remoteCertificateValidationCallback = new RemoteCertificateValidationCallback(SslSocket.OnValidateServerCertificate);
				this.m_sslStream = new SslStream(new NetworkStream(this.Socket, true), false, remoteCertificateValidationCallback);
				SslSocket.SslStreamValidateContext sslStreamValidateContext = new SslSocket.SslStreamValidateContext()
				{
					m_sslSocket = this
				};
				SslSocket.s_streamValidationContexts.Add(this.m_sslStream, sslStreamValidateContext);
				this.m_sslStream.BeginAuthenticateAsClient(this.m_address, new AsyncCallback(this.OnAuthenticateAsClient), null);
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogError("Exception while trying to authenticate. {0}", new object[] { exception });
				this.ExecuteBeginConnectDelegate(true);
			}
		}

		private static List<SslCertBundle> DownloadCertBundles(UrlDownloaderConfig dlConfig)
		{
			List<SslCertBundle> sslCertBundles = new List<SslCertBundle>();
			List<string> strs = new List<string>();
			if (BattleNet.Client().GetMobileEnvironment() != constants.MobileEnv.PRODUCTION)
			{
				strs.Add("http://nydus-qa.web.blizzard.net/Bnet/zxx/client/bgs-key-fingerprint");
			}
			strs.Add("http://nydus.battle.net/Bnet/zxx/client/bgs-key-fingerprint");
			IUrlDownloader urlDownloader = BattleNet.Client().GetUrlDownloader();
			int count = strs.Count;
			foreach (string str in strs)
			{
				urlDownloader.Download(str, (bool success, byte[] bytes) => {
					if (success)
					{
						SslCertBundle sslCertBundle = new SslCertBundle(bytes);
						sslCertBundles.Add(sslCertBundle);
					}
					count--;
				}, dlConfig);
			}
			while (count > 0)
			{
				Thread.Sleep(15);
			}
			return sslCertBundles;
		}

		private void ExecuteBeginConnectDelegate(bool connectFailed)
		{
			this.m_bundleSettings = null;
			if (this.m_beginConnectDelegate == null)
			{
				return;
			}
			bool isEncrypted = false;
			bool isSigned = false;
			if (this.m_sslStream != null)
			{
				isEncrypted = this.m_sslStream.IsEncrypted;
				isSigned = this.m_sslStream.IsSigned;
			}
			this.m_beginConnectDelegate(connectFailed, isEncrypted, isSigned);
			this.m_beginConnectDelegate = null;
			SslSocket.s_log.LogDebug("Connected={0} isEncrypted={1} isSigned={2}", new object[] { !connectFailed, isEncrypted, isSigned });
		}

		private static bool GetBundleInfo(byte[] unsignedBundleBytes, out SslSocket.BundleInfo info)
		{
			info = new SslSocket.BundleInfo();
			info.bundleKeyHashs = new List<byte[]>();
			info.bundleUris = new List<string>();
			info.bundleCerts = new List<X509Certificate2>();
			string str = null;
			string str1 = Encoding.ASCII.GetString(unsignedBundleBytes);
			try
			{
				JsonNode jsonNode = Json.Deserialize(str1) as JsonNode;
				foreach (JsonNode item in jsonNode["PublicKeys"] as JsonList)
				{
					string item1 = (string)item["Uri"];
					string item2 = (string)item["ShaHashPublicKeyInfo"];
					byte[] numArray = null;
					SslSocket.HexStrToBytesError bytes = SslSocket.HexStrToBytes(item2, out numArray);
					if (bytes == SslSocket.HexStrToBytesError.OK)
					{
						info.bundleKeyHashs.Add(numArray);
						info.bundleUris.Add(item1);
					}
					else
					{
						str = EnumUtils.GetString<SslSocket.HexStrToBytesError>(bytes);
						break;
					}
				}
				foreach (JsonNode jsonNode1 in jsonNode["SigningCertificates"] as JsonList)
				{
					string str2 = (string)jsonNode1["RawData"];
					X509Certificate2 x509Certificate2 = new X509Certificate2(Encoding.ASCII.GetBytes(str2));
					info.bundleCerts.Add(x509Certificate2);
				}
			}
			catch (Exception exception)
			{
				str = exception.ToString();
			}
			if (str == null)
			{
				return true;
			}
			SslSocket.s_log.LogWarning("Exception while trying to parse certificate bundle. {0}", new object[] { str });
			return false;
		}

		public static string GetBundleStoragePath()
		{
			string basePersistentDataPath = BattleNet.Client().GetBasePersistentDataPath();
			if (!basePersistentDataPath.EndsWith("/"))
			{
				basePersistentDataPath = string.Concat(basePersistentDataPath, "/");
			}
			basePersistentDataPath = string.Concat(basePersistentDataPath, "dlcertbundle");
			return basePersistentDataPath;
		}

		private static List<string> GetCommonNamesFromCertSubject(string certSubject)
		{
			List<string> strs = new List<string>();
			string[] strArrays = certSubject.Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i].Trim();
				if (str.StartsWith("CN="))
				{
					strs.Add(str.Substring(3));
				}
			}
			return strs;
		}

		private static byte[] GetUnsignedBundleBytes(byte[] signedBundleBytes)
		{
			int length = (int)signedBundleBytes.Length - (SslSocket.s_magicBundleSignaturePreamble.Length + 256);
			if (length <= 0)
			{
				return null;
			}
			byte[] numArray = new byte[length];
			Array.Copy(signedBundleBytes, numArray, length);
			return numArray;
		}

		private static SslSocket.HexStrToBytesError HexStrToBytes(string hex, out byte[] outBytes)
		{
			outBytes = null;
			int length = hex.Length;
			if (length % 2 == 1)
			{
				return SslSocket.HexStrToBytesError.ODD_NUMBER_OF_DIGITS;
			}
			outBytes = new byte[length / 2];
			int num = 0;
			int num1 = 0;
			while (num < length)
			{
				string str = hex.Substring(num, 2);
				outBytes[num1] = Convert.ToByte(str, 16);
				num = num + 2;
				num1++;
			}
			return SslSocket.HexStrToBytesError.OK;
		}

		private static bool IsCertSignedByBlizzard(X509Certificate cert)
		{
			string issuer = cert.Issuer;
			string[] strArrays = issuer.Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				strArrays[i] = strArrays[i].Trim();
			}
			HashSet<string> strs = new HashSet<string>();
			strs.Add("CN=Battle.net Certificate Authority");
			string[] strArrays1 = strArrays;
			for (int j = 0; j < (int)strArrays1.Length; j++)
			{
				strs.Remove(strArrays1[j]);
			}
			return strs.Count == 0;
		}

		private static SslSocket.CertValidationResult IsServerCertificateValid(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			string str;
			SslSocket.CertValidationResult certValidationResult;
			SslStream sslStream = (SslStream)sender;
			SslSocket mSslSocket = SslSocket.s_streamValidationContexts[sslStream].m_sslSocket;
			SslCertBundleSettings mBundleSettings = mSslSocket.m_bundleSettings;
			if (mBundleSettings.bundle == null || !mBundleSettings.bundle.IsUsingCertBundle)
			{
				return SslSocket.CertValidationResult.FAILED_CERT_BUNDLE;
			}
			List<string> commonNamesFromCertSubject = SslSocket.GetCommonNamesFromCertSubject(certificate.Subject);
			SslSocket.BundleInfo bundleInfo = new SslSocket.BundleInfo();
			byte[] certBundleBytes = mBundleSettings.bundle.CertBundleBytes;
			if (mBundleSettings.bundle.isCertBundleSigned)
			{
				if (!SslSocket.VerifyBundleSignature(mBundleSettings.bundle.CertBundleBytes))
				{
					return SslSocket.CertValidationResult.FAILED_CERT_BUNDLE;
				}
				certBundleBytes = SslSocket.GetUnsignedBundleBytes(mBundleSettings.bundle.CertBundleBytes);
			}
			if (!SslSocket.GetBundleInfo(certBundleBytes, out bundleInfo))
			{
				return SslSocket.CertValidationResult.FAILED_CERT_BUNDLE;
			}
			bool flag = false;
			byte[] publicKey = certificate.GetPublicKey();
			foreach (string str1 in commonNamesFromCertSubject)
			{
				if (!SslSocket.IsWhitelistedInCertBundle(bundleInfo, str1, publicKey))
				{
					continue;
				}
				flag = true;
				break;
			}
			if (!flag)
			{
				return SslSocket.CertValidationResult.FAILED_CERT_BUNDLE;
			}
			bool flag1 = SslSocket.IsCertSignedByBlizzard(certificate);
			bool runtimeEnvironment = BattleNet.Client().GetRuntimeEnvironment() == constants.RuntimeEnvironment.Mono;
			bool flag2 = (flag1 || !runtimeEnvironment ? false : chain.ChainElements.Count == 1);
			try
			{
				if (sslPolicyErrors != SslPolicyErrors.None)
				{
					SslPolicyErrors sslPolicyError = (flag1 || flag2 ? SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateChainErrors : SslPolicyErrors.None);
					if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != SslPolicyErrors.None && mSslSocket.m_connection.MatchSslCertName(commonNamesFromCertSubject))
					{
						sslPolicyError = sslPolicyError | SslPolicyErrors.RemoteCertificateNameMismatch;
					}
					if ((sslPolicyErrors & ~sslPolicyError) != SslPolicyErrors.None)
					{
						SslSocket.s_log.LogWarning("Failed policy check. sslPolicyErrors: {0}, expectedPolicyErrors: {1}", new object[] { sslPolicyErrors, sslPolicyError });
						certValidationResult = SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE;
						return certValidationResult;
					}
				}
				if (chain.ChainElements != null)
				{
					X509ChainElementEnumerator enumerator = chain.ChainElements.GetEnumerator();
					while (enumerator.MoveNext())
					{
						X509ChainElement current = enumerator.Current;
						SslSocket.s_log.LogDebug("Certificate Thumbprint: {0}", new object[] { current.Certificate.Thumbprint });
						X509ChainStatus[] chainElementStatus = current.ChainElementStatus;
						for (int i = 0; i < (int)chainElementStatus.Length; i++)
						{
							X509ChainStatus x509ChainStatu = chainElementStatus[i];
							SslSocket.s_log.LogDebug("  Certificate Status: {0}", new object[] { x509ChainStatu.Status });
						}
					}
					bool flag3 = false;
					if (flag1)
					{
						if (chain.ChainElements.Count == 1)
						{
							chain.ChainPolicy.ExtraStore.Add(SslSocket.s_rootCertificate);
							chain.Build(new X509Certificate2(certificate));
							flag3 = true;
						}
					}
					else if (flag2 && bundleInfo.bundleCerts != null)
					{
						foreach (X509Certificate2 bundleCert in bundleInfo.bundleCerts)
						{
							chain.ChainPolicy.ExtraStore.Add(bundleCert);
						}
						chain.Build(new X509Certificate2(certificate));
						flag3 = true;
					}
					int num = 0;
					while (num < chain.ChainElements.Count)
					{
						if (chain.ChainElements[num] != null)
						{
							num++;
						}
						else
						{
							SslSocket.s_log.LogWarning(string.Concat("ChainElements[", num, "] is null"));
							certValidationResult = (!flag3 ? SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE : SslSocket.CertValidationResult.FAILED_CERT_BUNDLE);
							return certValidationResult;
						}
					}
					if (flag1)
					{
						str = (BattleNet.Client().GetMobileEnvironment() != constants.MobileEnv.PRODUCTION ? "C0805E3CF51F1A56CE9E6E35CB4F4901B68128B7" : "673D9D1072B625CAD95CB47BF0F0F512233E39FD");
						if (chain.ChainElements[1].Certificate.Thumbprint != str)
						{
							SslSocket.s_log.LogWarning("Root certificate thumb print check failure");
							SslSocket.s_log.LogWarning("  expected: {0}", new object[] { str });
							SslSocket.s_log.LogWarning("  received: {0}", new object[] { chain.ChainElements[1].Certificate.Thumbprint });
							certValidationResult = (!flag3 ? SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE : SslSocket.CertValidationResult.FAILED_CERT_BUNDLE);
							return certValidationResult;
						}
					}
					int num1 = 0;
					while (num1 < chain.ChainElements.Count)
					{
						if (DateTime.Now <= chain.ChainElements[num1].Certificate.NotAfter)
						{
							num1++;
						}
						else
						{
							SslSocket.s_log.LogWarning(string.Concat("ChainElements[", num1, "] certificate is expired."));
							certValidationResult = (!flag3 ? SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE : SslSocket.CertValidationResult.FAILED_CERT_BUNDLE);
							return certValidationResult;
						}
					}
					X509ChainElementEnumerator x509ChainElementEnumerator = chain.ChainElements.GetEnumerator();
					while (x509ChainElementEnumerator.MoveNext())
					{
						X509ChainStatus[] x509ChainStatusArray = x509ChainElementEnumerator.Current.ChainElementStatus;
						int num2 = 0;
						while (num2 < (int)x509ChainStatusArray.Length)
						{
							X509ChainStatus x509ChainStatu1 = x509ChainStatusArray[num2];
							if ((flag1 || flag3) && x509ChainStatu1.Status == X509ChainStatusFlags.UntrustedRoot)
							{
								num2++;
							}
							else
							{
								SslSocket.s_log.LogWarning("Found unexpected chain error={0}.", new object[] { x509ChainStatu1.Status });
								certValidationResult = (!flag3 ? SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE : SslSocket.CertValidationResult.FAILED_CERT_BUNDLE);
								return certValidationResult;
							}
						}
					}
					X509Certificate2 x509Certificate2 = new X509Certificate2(chain.ChainElements[0].Certificate);
					SslSocket.s_log.LogDebug("Received valid certificate from FRONT >");
					SslSocket.s_log.LogDebug("  Subject: {0}", new object[] { x509Certificate2.Subject });
					SslSocket.s_log.LogDebug("  Issuer: {0}", new object[] { x509Certificate2.Issuer });
					SslSocket.s_log.LogDebug("  Version: {0}", new object[] { x509Certificate2.Version });
					SslSocket.s_log.LogDebug("  Valid Date: {0}", new object[] { x509Certificate2.NotBefore });
					SslSocket.s_log.LogDebug("  Expiry Date: {0}", new object[] { x509Certificate2.NotAfter });
					SslSocket.s_log.LogDebug("  Thumbprint: {0}", new object[] { x509Certificate2.Thumbprint });
					SslSocket.s_log.LogDebug("  Serial Number: {0}", new object[] { x509Certificate2.SerialNumber });
					SslSocket.s_log.LogDebug("  Friendly Name: {0}", new object[] { x509Certificate2.FriendlyName });
					SslSocket.s_log.LogDebug("  Public Key Format: {0}", new object[] { x509Certificate2.PublicKey.EncodedKeyValue.Format(true) });
					SslSocket.s_log.LogDebug("  Raw Data Length: {0}", new object[] { (int)x509Certificate2.RawData.Length });
					SslSocket.s_log.LogDebug("  CN: {0}", new object[] { x509Certificate2.GetNameInfo(X509NameType.DnsName, false) });
					return SslSocket.CertValidationResult.OK;
				}
				else
				{
					SslSocket.s_log.LogWarning("ChainElements is null");
					certValidationResult = SslSocket.CertValidationResult.FAILED_SERVER_RESPONSE;
				}
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogWarning("Exception while trying to validate certificate. {0}", new object[] { exception });
				certValidationResult = SslSocket.CertValidationResult.FAILED_CERT_BUNDLE;
			}
			return certValidationResult;
		}

		private static bool IsWhitelistedInCertBundle(SslSocket.BundleInfo bundleInfo, string uri, byte[] publicKey)
		{
			byte[] numArray = SHA256.Create().ComputeHash(publicKey);
			for (int i = 0; i < bundleInfo.bundleKeyHashs.Count; i++)
			{
				if (numArray.SequenceEqual<byte>(bundleInfo.bundleKeyHashs[i]) && bundleInfo.bundleUris[i].Equals(uri))
				{
					return true;
				}
			}
			return false;
		}

		private static bool MakePKCS1SignatureBlock(byte[] hash, int hashSize, byte[] id, int idSize, byte[] signature, int signatureSize)
		{
			byte[] numArray = signature;
			int num = 3 + idSize + hashSize;
			if (num > signatureSize)
			{
				return false;
			}
			int num1 = signatureSize - num;
			int num2 = 0;
			for (int i = 0; i < hashSize; i++)
			{
				int num3 = num2;
				num2 = num3 + 1;
				numArray[num3] = hash[hashSize - i - 1];
			}
			for (int j = 0; j < idSize; j++)
			{
				int num4 = num2;
				num2 = num4 + 1;
				numArray[num4] = id[idSize - j - 1];
			}
			int num5 = num2;
			num2 = num5 + 1;
			numArray[num5] = 0;
			for (int k = 0; k < num1; k++)
			{
				int num6 = num2;
				num2 = num6 + 1;
				numArray[num6] = 255;
			}
			int num7 = num2;
			num2 = num7 + 1;
			numArray[num7] = 1;
			int num8 = num2;
			num2 = num8 + 1;
			numArray[num8] = 0;
			if (num2 != signatureSize)
			{
				return false;
			}
			return true;
		}

		private void OnAuthenticateAsClient(IAsyncResult ar)
		{
			bool flag = false;
			try
			{
				if (this.m_sslStream == null)
				{
					throw new NullReferenceException("m_sslStream is null!");
				}
				this.m_sslStream.EndAuthenticateAsClient(ar);
				SslSocket.s_log.LogDebug("Authentication completed IsEncrypted = {0}, IsSigned = {1}", new object[] { this.m_sslStream.IsEncrypted, this.m_sslStream.IsSigned });
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogError("Exception while ending client authentication. {0}", new object[] { exception });
				flag = true;
			}
			this.ExecuteBeginConnectDelegate(flag);
		}

		private static bool OnValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			SslSocket.CertValidationResult certValidationResult = SslSocket.IsServerCertificateValid(sender, certificate, chain, sslPolicyErrors);
			if (certValidationResult == SslSocket.CertValidationResult.FAILED_CERT_BUNDLE)
			{
				SslStream sslStream = (SslStream)sender;
				SslSocket mSslSocket = SslSocket.s_streamValidationContexts[sslStream].m_sslSocket;
				foreach (SslCertBundle sslCertBundle in SslSocket.DownloadCertBundles(mSslSocket.m_bundleSettings.bundleDownloadConfig))
				{
					mSslSocket.m_bundleSettings.bundle = sslCertBundle;
					certValidationResult = SslSocket.IsServerCertificateValid(sender, certificate, chain, sslPolicyErrors);
					if (certValidationResult != SslSocket.CertValidationResult.OK)
					{
						continue;
					}
					FileUtil.StoreToDrive(sslCertBundle.CertBundleBytes, SslSocket.GetBundleStoragePath(), true, true);
					break;
				}
			}
			return certValidationResult == SslSocket.CertValidationResult.OK;
		}

		public static void Process()
		{
			SslSocket.s_log.Process();
		}

		private void ReadCallback(IAsyncResult ar)
		{
			SslSocket.BeginReceiveDelegate asyncState = (SslSocket.BeginReceiveDelegate)ar.AsyncState;
			if (this.Socket == null || this.m_sslStream == null)
			{
				if (asyncState != null)
				{
					asyncState(0);
				}
				return;
			}
			try
			{
				int num = this.m_sslStream.EndRead(ar);
				if (asyncState != null)
				{
					asyncState(num);
				}
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogWarning("Exception while trying to call EndRead. {0}", new object[] { exception });
				if (asyncState != null)
				{
					asyncState(0);
				}
			}
		}

		private void ResolveSSLAddress()
		{
			string str;
			if (!UriUtils.GetHostAddressAsIp(this.m_connection.Host, out str))
			{
				this.m_address = this.m_connection.Host;
			}
			else
			{
				this.m_address = (this.m_connection.ResolvedAddress.AddressFamily != AddressFamily.InterNetworkV6 ? string.Concat("::ffff:", this.m_connection.ResolvedAddress.ToString()) : this.m_connection.ResolvedAddress.ToString());
			}
			SslSocket.s_log.LogInfo("ResolveSSLAddress address: {0}", new object[] { this.m_address });
		}

		private static bool VerifyBundleSignature(byte[] signedBundleData)
		{
			int length = (int)signedBundleData.Length - (SslSocket.s_magicBundleSignaturePreamble.Length + 256);
			if (length <= 0)
			{
				return false;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(SslSocket.s_magicBundleSignaturePreamble);
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				if (signedBundleData[length + i] != bytes[i])
				{
					return false;
				}
			}
			SHA256 sHA256 = SHA256.Create();
			sHA256.Initialize();
			sHA256.TransformBlock(signedBundleData, 0, length, null, 0);
			byte[] numArray = Encoding.ASCII.GetBytes("Blizzard Certificate Bundle");
			sHA256.TransformBlock(numArray, 0, (int)numArray.Length, null, 0);
			sHA256.TransformFinalBlock(new byte[1], 0, 0);
			byte[] hash = sHA256.Hash;
			byte[] numArray1 = new byte[256];
			Array.Copy(signedBundleData, length + SslSocket.s_magicBundleSignaturePreamble.Length, numArray1, 0, 256);
			List<RSAParameters> rSAParameters = new List<RSAParameters>();
			RSAParameters rSAParameter = new RSAParameters()
			{
				Modulus = SslSocket.s_standardPublicModulus,
				Exponent = SslSocket.s_standardPublicExponent
			};
			rSAParameters.Add(rSAParameter);
			RSAParameters rSAParameter1 = new RSAParameters()
			{
				Modulus = SslSocket.s_debugPublicModulus,
				Exponent = SslSocket.s_debugPublicExponent
			};
			rSAParameters.Add(rSAParameter1);
			bool flag = false;
			int num = 0;
			while (num < rSAParameters.Count)
			{
				if (!SslSocket.VerifySignedHash(rSAParameters[num], hash, numArray1))
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private static bool VerifySignedHash(RSAParameters key, byte[] hash, byte[] signature)
		{
			byte[] numArray = new byte[(int)key.Modulus.Length];
			byte[] numArray1 = new byte[(int)key.Exponent.Length];
			byte[] numArray2 = new byte[(int)signature.Length];
			Array.Copy(key.Modulus, numArray, (int)key.Modulus.Length);
			Array.Copy(key.Exponent, numArray1, (int)key.Exponent.Length);
			Array.Copy(signature, numArray2, (int)signature.Length);
			Array.Reverse(numArray);
			Array.Reverse(numArray1);
			Array.Reverse(numArray2);
			BigInteger bigInteger = new BigInteger(numArray);
			BigInteger bigInteger1 = new BigInteger(numArray1);
			BigInteger bigInteger2 = BigInteger.PowMod(new BigInteger(numArray2), bigInteger1, bigInteger);
			byte[] numArray3 = new byte[(int)key.Modulus.Length];
			byte[] fieldHandle = new byte[] { typeof(<PrivateImplementationDetails>).GetField("$$field-20").FieldHandle };
			if (!SslSocket.MakePKCS1SignatureBlock(hash, (int)hash.Length, fieldHandle, (int)fieldHandle.Length, numArray3, (int)key.Modulus.Length))
			{
				return false;
			}
			byte[] numArray4 = new byte[(int)numArray3.Length];
			Array.Copy(numArray3, numArray4, (int)numArray3.Length);
			Array.Reverse(numArray4);
			return (new BigInteger(numArray4)).CompareTo(bigInteger2) == 0;
		}

		private void WriteCallback(IAsyncResult ar)
		{
			SslSocket.BeginSendDelegate asyncState = (SslSocket.BeginSendDelegate)ar.AsyncState;
			if (this.Socket == null || this.m_sslStream == null)
			{
				if (asyncState != null)
				{
					asyncState(false);
				}
				return;
			}
			try
			{
				this.m_sslStream.EndWrite(ar);
				this.m_canSend = true;
				if (asyncState != null)
				{
					asyncState(true);
				}
			}
			catch (Exception exception)
			{
				SslSocket.s_log.LogWarning("Exception while trying to call EndWrite. {0}", new object[] { exception });
				if (asyncState != null)
				{
					asyncState(false);
				}
			}
		}

		public delegate void BeginConnectDelegate(bool connectFailed, bool isEncrypted, bool isSigned);

		public delegate void BeginReceiveDelegate(int bytesReceived);

		public delegate void BeginSendDelegate(bool wasSent);

		private struct BundleInfo
		{
			public List<byte[]> bundleKeyHashs;

			public List<string> bundleUris;

			public List<X509Certificate2> bundleCerts;
		}

		private enum CertValidationResult
		{
			OK,
			FAILED_SERVER_RESPONSE,
			FAILED_CERT_BUNDLE
		}

		private enum HexStrToBytesError
		{
			[Description("OK")]
			OK,
			[Description("Hex string has an odd number of digits")]
			ODD_NUMBER_OF_DIGITS,
			[Description("Unknown error parsing hex string")]
			UNKNOWN
		}

		private class SslStreamValidateContext
		{
			public SslSocket m_sslSocket;

			public SslStreamValidateContext()
			{
			}
		}
	}
}