using bnet.protocol.profanity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bgs
{
	public class ProfanityAPI : BattleNetAPI
	{
		private Random m_rand;

		private readonly static char[] REPLACEMENT_CHARS;

		private string m_localeName;

		private WordFilters m_wordFilters;

		static ProfanityAPI()
		{
			ProfanityAPI.REPLACEMENT_CHARS = new char[] { '!', '@', '#', '$', '%', '\u005E', '&', '*' };
		}

		public ProfanityAPI(BattleNetCSharp battlenet) : base(battlenet, "Profanity")
		{
			this.m_rand = new Random();
		}

		private void DownloadCompletedCallback(byte[] data, object userContext)
		{
			if (data == null)
			{
				base.ApiLog.LogWarning("Downloading of profanity data from depot failed!");
				return;
			}
			base.ApiLog.LogDebug("Downloading of profanity data completed");
			try
			{
				this.m_wordFilters = WordFilters.ParseFrom(data);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.ApiLog.LogWarning("Failed to parse received data into protocol buffer. Ex  = {0}", new object[] { exception.ToString() });
			}
			if (this.m_wordFilters != null && this.m_wordFilters.IsInitialized)
			{
				return;
			}
			base.ApiLog.LogWarning("WordFilters failed to initialize");
		}

		public string FilterProfanity(string unfiltered)
		{
			if (this.m_wordFilters == null)
			{
				return unfiltered;
			}
			string str = unfiltered;
			foreach (WordFilter filtersList in this.m_wordFilters.FiltersList)
			{
				if (filtersList.Type == "bad")
				{
					Regex regex = new Regex(filtersList.Regex, RegexOptions.IgnoreCase);
					MatchCollection matchCollections = regex.Matches(str);
					if (matchCollections.Count != 0)
					{
						IEnumerator enumerator = matchCollections.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								Match current = (Match)enumerator.Current;
								if (current.Success)
								{
									char[] charArray = str.ToCharArray();
									if (current.Index <= (int)charArray.Length)
									{
										int length = current.Length;
										if (current.Index + current.Length > (int)charArray.Length)
										{
											length = (int)charArray.Length - current.Index;
										}
										for (int i = 0; i < length; i++)
										{
											charArray[current.Index + i] = this.GetReplacementChar();
										}
										str = new string(charArray);
									}
								}
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable == null)
							{
							}
							disposable.Dispose();
						}
					}
				}
			}
			return str;
		}

		private char GetReplacementChar()
		{
			int num = this.m_rand.Next((int)ProfanityAPI.REPLACEMENT_CHARS.Length);
			return ProfanityAPI.REPLACEMENT_CHARS[num];
		}

		public override void Initialize()
		{
			this.m_wordFilters = null;
			ResourcesAPI resources = this.m_battleNet.Resources;
			if (resources == null)
			{
				base.ApiLog.LogWarning("ResourcesAPI is not initialized! Unable to proceed.");
				return;
			}
			this.m_localeName = BattleNet.Client().GetLocaleName();
			if (string.IsNullOrEmpty(this.m_localeName))
			{
				base.ApiLog.LogWarning("Unable to get Locale from Localization class");
				this.m_localeName = "enUS";
			}
			base.ApiLog.LogDebug("Locale is {0}", new object[] { this.m_localeName });
			resources.LookupResource(new FourCC("BN"), new FourCC("apft"), new FourCC(this.m_localeName), new ResourcesAPI.ResourceLookupCallback(this.ResouceLookupCallback), null);
		}

		private void ResouceLookupCallback(ContentHandle contentHandle, object userContext)
		{
			if (contentHandle == null)
			{
				base.ApiLog.LogWarning("BN resource look up failed unable to proceed");
				return;
			}
			base.ApiLog.LogDebug("Lookup done Region={0} Usage={1} SHA256={2}", new object[] { contentHandle.Region, contentHandle.Usage, contentHandle.Sha256Digest });
			this.m_battleNet.LocalStorage.GetFile(contentHandle, new LocalStorageAPI.DownloadCompletedCallback(this.DownloadCompletedCallback), null);
		}
	}
}