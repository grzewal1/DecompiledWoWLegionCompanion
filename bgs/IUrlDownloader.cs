using System;

namespace bgs
{
	public interface IUrlDownloader
	{
		void Download(string url, UrlDownloadCompletedCallback cb);

		void Download(string url, UrlDownloadCompletedCallback cb, UrlDownloaderConfig config);

		void Process();
	}
}