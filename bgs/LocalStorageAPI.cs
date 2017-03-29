using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace bgs
{
	public class LocalStorageAPI : BattleNetAPI
	{
		private static int m_depotport;

		private static List<LocalStorageFileState> m_completedDownloads;

		private readonly static int m_bufferSize;

		private static LogThreadHelper s_log;

		private static int m_downloadId;

		public int DepotPort
		{
			get
			{
				return LocalStorageAPI.m_depotport;
			}
			set
			{
				LocalStorageAPI.m_depotport = value;
			}
		}

		static LocalStorageAPI()
		{
			LocalStorageAPI.m_depotport = 1119;
			LocalStorageAPI.m_completedDownloads = new List<LocalStorageFileState>();
			LocalStorageAPI.m_bufferSize = 1024;
			LocalStorageAPI.s_log = new LogThreadHelper("LocalStorage");
			LocalStorageAPI.m_downloadId = 0;
		}

		public LocalStorageAPI(BattleNetCSharp battlenet) : base(battlenet, "LocalStorage")
		{
		}

		private static void CompleteDownload(LocalStorageFileState state)
		{
			bool flag = true;
			LocalStorageAPI.s_log.LogDebug("Download completed for State={0}", new object[] { state });
			HTTPHeader hTTPHeader = LocalStorageAPI.ParseHTTPHeader(state.FileData);
			if (hTTPHeader != null)
			{
				byte[] numArray = new byte[hTTPHeader.ContentLength];
				Array.Copy(state.FileData, hTTPHeader.ContentStart, numArray, 0, hTTPHeader.ContentLength);
				if (LocalStorageAPI.ComputeSHA256(numArray) == state.CH.Sha256Digest)
				{
					flag = false;
					LocalStorageAPI.DecompressStateIfPossible(state, numArray);
				}
				else
				{
					LocalStorageAPI.s_log.LogWarning("Integrity check failed for State={0}", new object[] { state });
				}
			}
			else
			{
				LocalStorageAPI.s_log.LogWarning("Parsinig of HTTP header failed for State={0}", new object[] { state });
			}
			if (flag || state.FileData == null)
			{
				LocalStorageAPI.ExecuteFailedDownload(state);
			}
			else
			{
				LocalStorageAPI.ExecuteSucessfulDownload(state);
			}
		}

		private static string ComputeSHA256(byte[] bytes)
		{
			SHA256 sHA256 = SHA256.Create();
			byte[] numArray = sHA256.ComputeHash(bytes, 0, (int)bytes.Length);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] numArray1 = numArray;
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				byte num = numArray1[i];
				stringBuilder.Append(num.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private static void ConnectCallback(LocalStorageFileState state)
		{
			try
			{
				LocalStorageAPI.s_log.LogDebug("ConnectCallback called for State={0}", new object[] { state });
				state.ReceiveBuffer = new byte[LocalStorageAPI.m_bufferSize];
				state.Socket.BeginReceive(state.ReceiveBuffer, 0, LocalStorageAPI.m_bufferSize, SocketFlags.None, new AsyncCallback(LocalStorageAPI.ReceiveCallback), state);
				string str = string.Format("GET /{0}.{1} HTTP/1.1\r\n", state.CH.Sha256Digest, state.CH.Usage);
				str = string.Concat(str, "Host: ", state.Connection.Host, "\r\n");
				str = string.Concat(str, "User-Agent: HearthStone\r\n");
				str = string.Concat(str, "Connection: close\r\n");
				str = string.Concat(str, "\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes(str);
				state.Socket.Send(bytes, 0, (int)bytes.Length, SocketFlags.None);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION: {0}", new object[] { exception.Message });
			}
		}

		private static void DecompressStateIfPossible(LocalStorageFileState state, byte[] data)
		{
			ulong num;
			if (!LocalStorageAPI.IsCompressedStream(data, out num))
			{
				state.FileData = data;
			}
			else
			{
				state.CompressedData = data;
				MemoryStream memoryStream = new MemoryStream(data, 16, (int)data.Length - 16);
				state.FileData = LocalStorageAPI.Inflate(memoryStream, (int)num);
			}
		}

		private void DownloadFromDepot(LocalStorageFileState state)
		{
			string str = string.Format("{0}.depot.battle.net", state.CH.Region);
			LocalStorageAPI.s_log.LogDebug("Starting download from {0}", new object[] { str });
			state.Connection.LogDebug = new Action<string>(LocalStorageAPI.s_log.LogDebug);
			state.Connection.LogWarning = new Action<string>(LocalStorageAPI.s_log.LogWarning);
			state.Connection.OnFailure = () => LocalStorageAPI.ExecuteFailedDownload(state);
			state.Connection.OnSuccess = () => LocalStorageAPI.ConnectCallback(state);
			state.Connection.Connect(str, this.DepotPort);
		}

		private static void ExecuteFailedDownload(LocalStorageFileState state)
		{
			state.FileData = null;
			LocalStorageAPI.FinalizeDownload(state);
		}

		private static void ExecuteSucessfulDownload(LocalStorageFileState state)
		{
			LocalStorageAPI.StoreStateToDrive(state);
			LocalStorageAPI.FinalizeDownload(state);
		}

		private static void FinalizeDownload(LocalStorageFileState state)
		{
			state.Connection.Disconnect();
			state.ReceiveBuffer = null;
			LocalStorageAPI.FinalizeState(state);
		}

		private static void FinalizeState(LocalStorageFileState state)
		{
			List<LocalStorageFileState> mCompletedDownloads = LocalStorageAPI.m_completedDownloads;
			Monitor.Enter(mCompletedDownloads);
			try
			{
				LocalStorageAPI.m_completedDownloads.Add(state);
			}
			finally
			{
				Monitor.Exit(mCompletedDownloads);
			}
		}

		public bool GetFile(ContentHandle ch, LocalStorageAPI.DownloadCompletedCallback cb, object userContext = null)
		{
			bool flag;
			try
			{
				LocalStorageFileState localStorageFileState = new LocalStorageFileState(LocalStorageAPI.m_downloadId)
				{
					CH = ch,
					Callback = cb,
					UserContext = userContext
				};
				LocalStorageAPI.s_log.LogDebug("Starting GetFile State={0}", new object[] { localStorageFileState });
				if (!this.LoadStateFromDrive(localStorageFileState))
				{
					LocalStorageAPI.s_log.LogDebug("Unable to load file from disk, starting a download. State={0}", new object[] { localStorageFileState });
					this.DownloadFromDepot(localStorageFileState);
				}
				return true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION (GetFile): {0}", new object[] { exception.Message });
				flag = false;
			}
			return flag;
		}

		private static byte[] Inflate(MemoryStream ms, int length)
		{
			byte[] numArray;
			ms.Seek((long)0, SeekOrigin.Begin);
			InflaterInputStream inflaterInputStream = new InflaterInputStream(ms, new Inflater(false));
			byte[] numArray1 = new byte[length];
			int num = 0;
			int num1 = (int)numArray1.Length;
			try
			{
				while (true)
				{
					int num2 = inflaterInputStream.Read(numArray1, num, num1);
					if (num2 <= 0)
					{
						break;
					}
					num = num + num2;
					num1 = num1 - num2;
				}
				if (num == length)
				{
					return numArray1;
				}
				LocalStorageAPI.s_log.LogWarning("Decompressed size does not equal expected size.");
				return null;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION (Inflate): {0}", new object[] { exception.Message });
				numArray = null;
			}
			return numArray;
		}

		private static bool IsCompressedStream(byte[] data, out ulong decompressedLength)
		{
			bool flag;
			decompressedLength = (ulong)0;
			try
			{
				if ((int)data.Length < 16)
				{
					flag = false;
				}
				else if (BitConverter.ToUInt32(data, 0) != 1131245658)
				{
					flag = false;
				}
				else if (BitConverter.ToUInt32(data, 4) == 0)
				{
					decompressedLength = BitConverter.ToUInt64(data, 8);
					return true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION: {0}", new object[] { exception.Message });
				flag = false;
			}
			return flag;
		}

		private bool LoadStateFromDrive(LocalStorageFileState state)
		{
			bool flag;
			try
			{
				LocalStorageAPI.s_log.LogDebug("LoadState State={0}", new object[] { state });
				string str = LocalStorageAPI.MakeFullPathFromState(state);
				LocalStorageAPI.s_log.LogDebug("Attempting to load {0}", new object[] { str });
				if (File.Exists(str))
				{
					FileStream fileStream = File.OpenRead(str);
					byte[] numArray = new byte[checked((IntPtr)fileStream.Length)];
					fileStream.Read(numArray, 0, (int)numArray.Length);
					fileStream.Close();
					if (LocalStorageAPI.ComputeSHA256(numArray) == state.CH.Sha256Digest)
					{
						LocalStorageAPI.DecompressStateIfPossible(state, numArray);
						LocalStorageAPI.s_log.LogDebug("Loading completed");
						LocalStorageAPI.FinalizeState(state);
						return true;
					}
					else
					{
						LocalStorageAPI.s_log.LogDebug("File was loaded but integrity check failed, attempting to delete ...");
						File.Delete(str);
						flag = false;
					}
				}
				else
				{
					LocalStorageAPI.s_log.LogDebug("File does not exist, unable to load from disk.");
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION: {0}", new object[] { exception.Message });
				flag = false;
			}
			return flag;
		}

		private static string MakeFullPathFromState(LocalStorageFileState state)
		{
			string temporaryCachePath = BattleNet.Client().GetTemporaryCachePath();
			string str = string.Concat(state.CH.Sha256Digest, ".", state.CH.Usage);
			return Path.Combine(temporaryCachePath, str);
		}

		private static HTTPHeader ParseHTTPHeader(byte[] data)
		{
			HTTPHeader hTTPHeader;
			try
			{
				int num = LocalStorageAPI.SearchForBytePattern(data, new byte[] { 13, 10, 13, 10 });
				if (num != -1)
				{
					int num1 = num + 1;
					if (num1 < (int)data.Length)
					{
						string str = Encoding.ASCII.GetString(data, 0, num);
						if (str.IndexOf("200 OK") != -1)
						{
							Match match = (new Regex("(?<=Content-Length:\\s)\\d+", RegexOptions.IgnoreCase)).Match(str);
							if (match.Success)
							{
								int num2 = (int)uint.Parse(match.Value);
								if (num2 == (int)data.Length - num1)
								{
									HTTPHeader hTTPHeader1 = new HTTPHeader()
									{
										ContentLength = num2,
										ContentStart = num1
									};
									hTTPHeader = hTTPHeader1;
								}
								else
								{
									hTTPHeader = null;
								}
							}
							else
							{
								hTTPHeader = null;
							}
						}
						else
						{
							hTTPHeader = null;
						}
					}
					else
					{
						hTTPHeader = null;
					}
				}
				else
				{
					hTTPHeader = null;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION (ParseHTTPHeader): {0}", new object[] { exception.Message });
				return null;
			}
			return hTTPHeader;
		}

		public override void Process()
		{
			List<LocalStorageFileState> mCompletedDownloads = LocalStorageAPI.m_completedDownloads;
			Monitor.Enter(mCompletedDownloads);
			try
			{
				foreach (LocalStorageFileState mCompletedDownload in LocalStorageAPI.m_completedDownloads)
				{
					if (mCompletedDownload.FileData == null)
					{
						LocalStorageAPI.s_log.LogWarning("Request failed State={0}", new object[] { mCompletedDownload });
					}
					else
					{
						LocalStorageAPI.s_log.LogDebug("Request completed State={0}", new object[] { mCompletedDownload });
					}
					LocalStorageAPI.s_log.Process();
					mCompletedDownload.Callback(mCompletedDownload.FileData, mCompletedDownload.UserContext);
				}
				LocalStorageAPI.m_completedDownloads.Clear();
			}
			finally
			{
				Monitor.Exit(mCompletedDownloads);
			}
		}

		private static void ReceiveCallback(IAsyncResult ar)
		{
			LocalStorageFileState asyncState = (LocalStorageFileState)ar.AsyncState;
			try
			{
				LocalStorageAPI.s_log.LogDebug("ReceiveCallback called for State={0}", new object[] { asyncState });
				int num = asyncState.Socket.EndReceive(ar);
				if (num <= 0)
				{
					LocalStorageAPI.CompleteDownload(asyncState);
				}
				else
				{
					int num1 = (asyncState.FileData != null ? (int)asyncState.FileData.Length : 0) + num;
					MemoryStream memoryStream = new MemoryStream(new byte[num1], 0, num1, true, true);
					if (asyncState.FileData != null)
					{
						memoryStream.Write(asyncState.FileData, 0, (int)asyncState.FileData.Length);
					}
					memoryStream.Write(asyncState.ReceiveBuffer, 0, num);
					asyncState.FileData = memoryStream.GetBuffer();
					asyncState.Socket.BeginReceive(asyncState.ReceiveBuffer, 0, LocalStorageAPI.m_bufferSize, SocketFlags.None, new AsyncCallback(LocalStorageAPI.ReceiveCallback), asyncState);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION: {0}", new object[] { exception.Message });
				asyncState.FailureMessage = exception.Message;
				LocalStorageAPI.ExecuteFailedDownload(asyncState);
			}
		}

		private static int SearchForBytePattern(byte[] source, byte[] pattern)
		{
			for (int i = 0; i < (int)source.Length; i++)
			{
				if (pattern[0] == source[i] && (int)source.Length - i >= (int)pattern.Length)
				{
					bool flag = true;
					for (int j = 1; j < (int)pattern.Length && flag; j++)
					{
						if (source[i + j] != pattern[j])
						{
							flag = false;
						}
					}
					if (flag)
					{
						i = i + ((int)pattern.Length - 1);
						return i;
					}
				}
			}
			return -1;
		}

		private static void StoreStateToDrive(LocalStorageFileState state)
		{
			try
			{
				LocalStorageAPI.s_log.LogDebug("StoreState State={0}", new object[] { state });
				string str = LocalStorageAPI.MakeFullPathFromState(state);
				LocalStorageAPI.s_log.LogDebug("Attempting to save {0}", new object[] { str });
				if (!File.Exists(str))
				{
					FileStream fileStream = File.Create(str, (int)state.FileData.Length);
					if (state.CompressedData != null)
					{
						LocalStorageAPI.s_log.LogDebug("Writing compressed file to disk");
						fileStream.Write(state.CompressedData, 0, (int)state.CompressedData.Length);
					}
					else
					{
						LocalStorageAPI.s_log.LogDebug("Writing uncompressed file to disk");
						fileStream.Write(state.FileData, 0, (int)state.FileData.Length);
					}
					fileStream.Flush();
					fileStream.Close();
					LocalStorageAPI.s_log.LogDebug("Writing completed");
				}
				else
				{
					LocalStorageAPI.s_log.LogDebug("Unable to save the file, it already exists");
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LocalStorageAPI.s_log.LogWarning("EXCEPTION (StoreStateToDrive): {0}", new object[] { exception.Message });
			}
		}

		public delegate void DownloadCompletedCallback(byte[] data, object userContext);
	}
}