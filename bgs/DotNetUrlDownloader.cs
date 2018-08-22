using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace bgs
{
	public class DotNetUrlDownloader : IUrlDownloader
	{
		private List<DotNetUrlDownloader.DownloadResult> m_completedDownloads = new List<DotNetUrlDownloader.DownloadResult>();

		public DotNetUrlDownloader()
		{
		}

		public void Download(string url, UrlDownloadCompletedCallback cb)
		{
			this.Download(url, cb, new UrlDownloaderConfig());
		}

		public void Download(string url, UrlDownloadCompletedCallback cb, UrlDownloaderConfig config)
		{
			WebRequest webRequest = WebRequest.Create(url);
			DotNetUrlDownloader.DownloadResult downloadResult = new DotNetUrlDownloader.DownloadResult()
			{
				callback = cb
			};
			DotNetUrlDownloader.DownloadState downloadState = new DotNetUrlDownloader.DownloadState()
			{
				downloader = this,
				host = url,
				downloadResult = downloadResult,
				request = webRequest,
				numRetriesLeft = config.numRetries,
				timeoutMs = config.timeoutMs
			};
			DotNetUrlDownloader.Download(downloadState);
		}

		private static void Download(DotNetUrlDownloader.DownloadState state)
		{
			try
			{
				IAsyncResult asyncResult = state.request.BeginGetResponse(new AsyncCallback(DotNetUrlDownloader.ResponseCallback), state);
				int num = state.timeoutMs;
				if (num < 0)
				{
					num = -1;
				}
				state.timeoutWatchHandle = asyncResult.AsyncWaitHandle;
				state.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(state.timeoutWatchHandle, new WaitOrTimerCallback(DotNetUrlDownloader.TimeoutCallback), state, num, true);
			}
			catch (Exception exception)
			{
				DotNetUrlDownloader.FinishDownload(state);
			}
		}

		private static void FinishDownload(DotNetUrlDownloader.DownloadState state)
		{
			// 
			// Current member / type: System.Void bgs.DotNetUrlDownloader::FinishDownload(bgs.DotNetUrlDownloader/DownloadState)
			// File path: C:\apktool\WoW Companion_2.0.27325\assets\bin\Data\Managed\Assembly-CSharp.dll
			// 
			// Product version: 2018.2.803.0
			// Exception in: System.Void FinishDownload(bgs.DotNetUrlDownloader/DownloadState)
			// 
			// La référence d'objet n'est pas définie à une instance d'un objet.
			//    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 93
			//    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 24
			//    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			//    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 19
			//    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			//    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			//    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			//    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			//    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void Process()
		{
			object mCompletedDownloads = this.m_completedDownloads;
			Monitor.Enter(mCompletedDownloads);
			try
			{
				foreach (DotNetUrlDownloader.DownloadResult mCompletedDownload in this.m_completedDownloads)
				{
					mCompletedDownload.callback(mCompletedDownload.succeeded, mCompletedDownload.downloadData);
				}
				this.m_completedDownloads.Clear();
			}
			finally
			{
				Monitor.Exit(mCompletedDownloads);
			}
		}

		private static void ReadCallback(IAsyncResult ar)
		{
			DotNetUrlDownloader.DownloadState asyncState = (DotNetUrlDownloader.DownloadState)ar.AsyncState;
			bool flag = true;
			try
			{
				Stream stream = asyncState.responseStream;
				int num = stream.EndRead(ar);
				if (num > 0)
				{
					flag = false;
					Array.Copy(asyncState.readBuffer, 0, asyncState.downloadResult.downloadData, asyncState.readPos, num);
					asyncState.readPos += num;
					stream.BeginRead(asyncState.readBuffer, 0, (int)asyncState.readBuffer.Length, new AsyncCallback(DotNetUrlDownloader.ReadCallback), asyncState);
				}
				else if (num == 0)
				{
					asyncState.downloadResult.succeeded = true;
				}
			}
			catch (Exception exception)
			{
			}
			if (flag)
			{
				DotNetUrlDownloader.FinishDownload(asyncState);
			}
		}

		private static void ResponseCallback(IAsyncResult ar)
		{
			DotNetUrlDownloader.DownloadState asyncState = (DotNetUrlDownloader.DownloadState)ar.AsyncState;
			try
			{
				WebResponse webResponse = asyncState.request.EndGetResponse(ar);
				Stream responseStream = webResponse.GetResponseStream();
				asyncState.responseStream = responseStream;
				asyncState.downloadResult.downloadData = new byte[checked((IntPtr)webResponse.ContentLength)];
				responseStream.BeginRead(asyncState.readBuffer, 0, (int)asyncState.readBuffer.Length, new AsyncCallback(DotNetUrlDownloader.ReadCallback), asyncState);
			}
			catch (Exception exception)
			{
				DotNetUrlDownloader.FinishDownload(asyncState);
			}
		}

		private static void TimeoutCallback(object context, bool timedOut)
		{
			DotNetUrlDownloader.DownloadState downloadState = (DotNetUrlDownloader.DownloadState)context;
			downloadState.UnregisterTimeout();
			if (timedOut)
			{
				downloadState.request.Abort();
			}
		}

		internal class DownloadResult
		{
			public UrlDownloadCompletedCallback callback;

			public byte[] downloadData;

			public bool succeeded;

			public DownloadResult()
			{
			}
		}

		internal class DownloadState
		{
			public DotNetUrlDownloader downloader;

			public string host;

			public WebRequest request;

			public Stream responseStream;

			public int numRetriesLeft;

			public int timeoutMs;

			public RegisteredWaitHandle timeoutWaitHandle;

			public WaitHandle timeoutWatchHandle;

			private const int bufferSize = 1024;

			public byte[] readBuffer;

			public int readPos;

			public DotNetUrlDownloader.DownloadResult downloadResult;

			public DownloadState()
			{
				this.readBuffer = new byte[1024];
			}

			public bool UnregisterTimeout()
			{
				bool flag = false;
				if (this.timeoutWaitHandle != null && this.timeoutWatchHandle != null)
				{
					flag = this.timeoutWaitHandle.Unregister(this.timeoutWatchHandle);
					this.timeoutWaitHandle = null;
					this.timeoutWatchHandle = null;
				}
				return flag;
			}
		}
	}
}