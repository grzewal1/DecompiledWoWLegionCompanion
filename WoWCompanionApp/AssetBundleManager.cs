using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace WoWCompanionApp
{
	public class AssetBundleManager : Singleton<AssetBundleManager>
	{
		private static bool s_initialized;

		private string m_assetServerIpAddress = "blzddist2-a.akamaihd.net";

		private string m_assetServerIpAddress_CN = "client02.pdl.wow.battlenet.com.cn";

		private Dictionary<AssetBundleManager.BundleName, AssetBundle> m_assetBundles = new Dictionary<AssetBundleManager.BundleName, AssetBundle>();

		private string m_assetServerURL;

		private AssetBundleManifest m_manifest;

		public Action InitializedAction;

		private UnityWebRequest m_currentWebRequest;

		private int m_numDownloads;

		private int m_numCompleteDownloads;

		private const string m_versionFile = "update.txt";

		private string m_assetBundleDirectory = "ab";

		private string m_platform = "a";

		private string m_locale;

		public string AppStoreUrl
		{
			get;
			set;
		}

		public string AppStoreUrl_CN
		{
			get;
			set;
		}

		public bool ForceUpgrade
		{
			get;
			set;
		}

		public static AssetBundle Icons
		{
			get
			{
				return Singleton<AssetBundleManager>.Instance.m_assetBundles[AssetBundleManager.BundleName.IconBundleName];
			}
		}

		public Version LatestVersion
		{
			get;
			set;
		}

		public static AssetBundle PortraitIcons
		{
			get
			{
				return Singleton<AssetBundleManager>.Instance.m_assetBundles[AssetBundleManager.BundleName.PortraitBundleName];
			}
		}

		static AssetBundleManager()
		{
		}

		public AssetBundleManager()
		{
		}

		private void DataErrorPopupDisabled()
		{
			GenericPopup.DisabledAction -= new Action(this.DataErrorPopupDisabled);
			Main.instance.OnQuitButton();
		}

		[DebuggerHidden]
		private IEnumerator FetchLatestVersion(string url)
		{
			AssetBundleManager.<FetchLatestVersion>c__Iterator2 variable = null;
			return variable;
		}

		private string GetDataErrorDescriptionText()
		{
			int num;
			string mLocale = this.m_locale;
			if (mLocale != null)
			{
				if (AssetBundleManager.<>f__switch$map6 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(11)
					{
						{ "enUS", 0 },
						{ "koKR", 1 },
						{ "frFR", 2 },
						{ "deDE", 3 },
						{ "zhCN", 4 },
						{ "zhTW", 5 },
						{ "esES", 6 },
						{ "esMX", 7 },
						{ "ruRU", 8 },
						{ "ptBR", 9 },
						{ "itIT", 10 }
					};
					AssetBundleManager.<>f__switch$map6 = strs;
				}
				if (AssetBundleManager.<>f__switch$map6.TryGetValue(mLocale, out num))
				{
					switch (num)
					{
						case 0:
						{
							return "Unable to load data from device.";
						}
						case 1:
						{
							return "기기에서 데이터를 불러올 수 없습니다.\t";
						}
						case 2:
						{
							return "Impossible de charger les données depuis l’appareil.";
						}
						case 3:
						{
							return "Gerätedaten konnten nicht geladen werden.";
						}
						case 4:
						{
							return "无法从设备中读取数据。";
						}
						case 5:
						{
							return "無法從裝置上讀取資料。";
						}
						case 6:
						{
							return "No se han podido cargar los datos del dispositivo.";
						}
						case 7:
						{
							return "No se pueden cargar los datos del dispositivo";
						}
						case 8:
						{
							return "Невозможно загрузить данные с устройства.";
						}
						case 9:
						{
							return "Não foi possível carregar os dados a partir do dispositivo.";
						}
						case 10:
						{
							return "Impossibile caricare i dati dal dispositivo.";
						}
					}
				}
			}
			return "Unable to load data from device.";
		}

		private string GetDataErrorTitleText()
		{
			int num;
			string mLocale = this.m_locale;
			if (mLocale != null)
			{
				if (AssetBundleManager.<>f__switch$map5 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(11)
					{
						{ "enUS", 0 },
						{ "koKR", 1 },
						{ "frFR", 2 },
						{ "deDE", 3 },
						{ "zhCN", 4 },
						{ "zhTW", 5 },
						{ "esES", 6 },
						{ "esMX", 7 },
						{ "ruRU", 8 },
						{ "ptBR", 9 },
						{ "itIT", 10 }
					};
					AssetBundleManager.<>f__switch$map5 = strs;
				}
				if (AssetBundleManager.<>f__switch$map5.TryGetValue(mLocale, out num))
				{
					switch (num)
					{
						case 0:
						{
							return "Data Error";
						}
						case 1:
						{
							return "데이터 오류";
						}
						case 2:
						{
							return "Erreur de données";
						}
						case 3:
						{
							return "Datenfehler";
						}
						case 4:
						{
							return "数据错误";
						}
						case 5:
						{
							return "資料錯誤";
						}
						case 6:
						{
							return "Error de datos";
						}
						case 7:
						{
							return "Error de datos";
						}
						case 8:
						{
							return "Ошибка в данных";
						}
						case 9:
						{
							return "Erro de dados";
						}
						case 10:
						{
							return "Errore di caricamento dati";
						}
					}
				}
			}
			return "Data Error";
		}

		public float GetDownloadProgress()
		{
			return ((float)this.m_numCompleteDownloads + (this.m_currentWebRequest == null ? 0f : this.m_currentWebRequest.downloadProgress)) / (float)this.m_numDownloads;
		}

		private string GetRemoteAssetPath()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("http://").Append((Singleton<Login>.instance.GetBnPortal() != "cn" ? this.m_assetServerIpAddress : this.m_assetServerIpAddress_CN)).Append("/falcon/");
			stringBuilder.Append("d").Append(string.Format("{0:D5}", 204));
			stringBuilder.Append("/").Append(this.m_assetBundleDirectory).Append("/");
			stringBuilder.Append(this.m_platform).Append("/");
			return stringBuilder.ToString();
		}

		private void InitAssetBundleManager()
		{
			if (AssetBundleManager.s_initialized)
			{
				return;
			}
			base.StartCoroutine(this.InternalInitAssetBundleManager());
		}

		[DebuggerHidden]
		private IEnumerator InternalInitAssetBundleManager()
		{
			AssetBundleManager.<InternalInitAssetBundleManager>c__Iterator0 variable = null;
			return variable;
		}

		public bool IsDevAssetBundles()
		{
			return false;
		}

		public bool IsInitialized()
		{
			return AssetBundleManager.s_initialized;
		}

		public static T LoadAsset<T>(AssetBundleManager.BundleName bundleName, string assetPath)
		where T : UnityEngine.Object
		{
			return (!Singleton<AssetBundleManager>.Instance.m_assetBundles.ContainsKey(bundleName) ? (T)null : Singleton<AssetBundleManager>.Instance.m_assetBundles[bundleName].LoadAsset<T>(assetPath));
		}

		[DebuggerHidden]
		private IEnumerator LoadAssetBundle(string fileName)
		{
			AssetBundleManager.<LoadAssetBundle>c__Iterator1 variable = null;
			return variable;
		}

		private bool ParseVersionFile(string versionText)
		{
			bool flag;
			if (versionText == null)
			{
				return false;
			}
			string[] strArrays = versionText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if ((int)strArrays.Length < 2)
			{
				return false;
			}
			try
			{
				this.LatestVersion = new Version(strArrays[0]);
				this.ForceUpgrade = Convert.ToBoolean(strArrays[1]);
				if ((int)strArrays.Length < 3)
				{
					this.AppStoreUrl = null;
				}
				else
				{
					this.AppStoreUrl = strArrays[2];
				}
				if ((int)strArrays.Length < 4)
				{
					this.AppStoreUrl_CN = null;
				}
				else
				{
					this.AppStoreUrl_CN = strArrays[3];
				}
				return true;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		private void Start()
		{
			this.LatestVersion = new Version(0, 0, 0);
			this.ForceUpgrade = false;
			this.m_locale = MobileDeviceLocale.GetBestGuessForLocale();
			this.InitAssetBundleManager();
		}

		public void UpdateVersion()
		{
			string str = string.Concat(this.GetRemoteAssetPath(), "update.txt");
			UnityEngine.Debug.Log(string.Concat("Fetching latest version of ", str));
			this.LatestVersion = new Version();
			this.ForceUpgrade = false;
			string str1 = null;
			float single = Time.timeSinceLevelLoad;
			using (WWW wWW = new WWW(str))
			{
				while (!wWW.isDone && (double)Time.timeSinceLevelLoad < (double)single + 5)
				{
				}
				if (wWW.error == null)
				{
					str1 = wWW.text;
				}
			}
			this.ParseVersionFile(str1);
		}

		public sealed class BundleName
		{
			public static AssetBundleManager.BundleName IconBundleName;

			public static AssetBundleManager.BundleName PortraitBundleName;

			public static AssetBundleManager.BundleName MainSceneBundleName;

			public static AssetBundleManager.BundleName NonLocalizedDBBundleName;

			public static AssetBundleManager.BundleName LocalizedDBBundleName;

			public static AssetBundleManager.BundleName ConfigBundleName;

			public static AssetBundleManager.BundleName[] RequiredBundleNames;

			private string m_name;

			static BundleName()
			{
				AssetBundleManager.BundleName.IconBundleName = new AssetBundleManager.BundleName("icn");
				AssetBundleManager.BundleName.PortraitBundleName = new AssetBundleManager.BundleName("picn");
				AssetBundleManager.BundleName.MainSceneBundleName = new AssetBundleManager.BundleName("main");
				AssetBundleManager.BundleName.NonLocalizedDBBundleName = new AssetBundleManager.BundleName("staticdb-common");
				AssetBundleManager.BundleName.LocalizedDBBundleName = new AssetBundleManager.BundleName("staticdb-local");
				AssetBundleManager.BundleName.ConfigBundleName = new AssetBundleManager.BundleName("config");
				AssetBundleManager.BundleName.RequiredBundleNames = new AssetBundleManager.BundleName[] { AssetBundleManager.BundleName.MainSceneBundleName, AssetBundleManager.BundleName.IconBundleName, AssetBundleManager.BundleName.PortraitBundleName, AssetBundleManager.BundleName.NonLocalizedDBBundleName, AssetBundleManager.BundleName.LocalizedDBBundleName, AssetBundleManager.BundleName.ConfigBundleName };
			}

			private BundleName(string name)
			{
				this.m_name = name;
			}

			public override bool Equals(object obj)
			{
				return (this.GetType() != obj.GetType() ? false : this.m_name == (obj as AssetBundleManager.BundleName).m_name);
			}

			public bool Equals(AssetBundleManager.BundleName bundleName)
			{
				return this.m_name == bundleName.m_name;
			}

			public override int GetHashCode()
			{
				return this.m_name.GetHashCode();
			}

			public static bool operator ==(AssetBundleManager.BundleName name1, AssetBundleManager.BundleName name2)
			{
				return name1.m_name == name2.m_name;
			}

			public static implicit operator String(AssetBundleManager.BundleName bundleName)
			{
				return bundleName.m_name;
			}

			public static implicit operator BundleName(string name)
			{
				return new AssetBundleManager.BundleName(name);
			}

			public static bool operator !=(AssetBundleManager.BundleName name1, AssetBundleManager.BundleName name2)
			{
				return name1.m_name != name2.m_name;
			}
		}
	}
}