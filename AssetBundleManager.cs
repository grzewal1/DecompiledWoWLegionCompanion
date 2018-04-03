using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private const int HASH_LENGTH = 32;

	private static AssetBundleManager s_instance;

	private static bool s_initialized;

	private string m_assetServerIpAddress = "blzddist2-a.akamaihd.net";

	private string m_assetServerIpAddress_CN = "client02.pdl.wow.battlenet.com.cn";

	private AssetBundle m_portraitIconsBundle;

	private AssetBundle m_iconsBundle;

	public string m_devAssetServerURL;

	private string m_assetServerURL;

	private Dictionary<string, string> m_manifest;

	public Action InitializedAction;

	private WWW m_currentWWW;

	private float m_priorProgress;

	private float m_progressMultiplier;

	private float m_progressStartTime;

	private const string m_versionFile = "update.txt";

	private string m_assetBundleDirectory = "ab";

	private string m_platform = "a";

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
			return AssetBundleManager.instance.m_iconsBundle;
		}
	}

	public static AssetBundleManager instance
	{
		get
		{
			return AssetBundleManager.s_instance;
		}
	}

	public int LatestVersion
	{
		get;
		set;
	}

	public static AssetBundle portraitIcons
	{
		get
		{
			return AssetBundleManager.instance.m_portraitIconsBundle;
		}
	}

	static AssetBundleManager()
	{
	}

	public AssetBundleManager()
	{
	}

	private void Awake()
	{
		if (AssetBundleManager.s_instance == null)
		{
			AssetBundleManager.s_instance = this;
			this.m_manifest = new Dictionary<string, string>();
		}
		this.LatestVersion = 0;
		this.ForceUpgrade = false;
	}

	private void BuildManifest(string manifestText)
	{
		int num = 0;
		int num1 = 0;
		do
		{
			num = manifestText.IndexOf('\n', num1);
			if (num < 0)
			{
				continue;
			}
			string str = manifestText.Substring(num1, num - num1 + 1).Trim();
			this.ParseManifestLine(str);
			num1 = num + 1;
		}
		while (num > 0);
	}

	private void DataErrorPopupDisabled()
	{
		GenericPopup.DisabledAction -= new Action(this.DataErrorPopupDisabled);
		Main.instance.OnQuitButton();
	}

	[DebuggerHidden]
	public IEnumerator FetchLatestVersion(string url)
	{
		AssetBundleManager.<FetchLatestVersion>c__Iterator2 variable = null;
		return variable;
	}

	private string GetBundleFileName(string fileIdentifier)
	{
		string str;
		if (this.m_manifest.TryGetValue(fileIdentifier, out str))
		{
			return str;
		}
		return null;
	}

	private string GetDataErrorDescriptionText()
	{
		int num;
		string locale = Main.instance.GetLocale();
		if (locale != null)
		{
			if (AssetBundleManager.<>f__switch$map3 == null)
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
				AssetBundleManager.<>f__switch$map3 = strs;
			}
			if (AssetBundleManager.<>f__switch$map3.TryGetValue(locale, out num))
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
		string locale = Main.instance.GetLocale();
		if (locale != null)
		{
			if (AssetBundleManager.<>f__switch$map2 == null)
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
				AssetBundleManager.<>f__switch$map2 = strs;
			}
			if (AssetBundleManager.<>f__switch$map2.TryGetValue(locale, out num))
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
		float mPriorProgress = this.m_priorProgress;
		if (this.m_currentWWW != null && Time.timeSinceLevelLoad > this.m_progressStartTime + 1f)
		{
			mPriorProgress = mPriorProgress + this.m_currentWWW.progress * this.m_progressMultiplier;
		}
		return mPriorProgress;
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

	[DebuggerHidden]
	public IEnumerator LoadAssetBundle(string fileIdentifier, Action<AssetBundle> resultCallback)
	{
		AssetBundleManager.<LoadAssetBundle>c__Iterator1 variable = null;
		return variable;
	}

	private void ParseManifestLine(string lineText)
	{
		int num = 0;
		int num1 = 0;
		do
		{
			string str = "Name: ";
			num = lineText.IndexOf(str, num1);
			if (num >= 0)
			{
				int length = num + str.Length;
				string str1 = lineText.Substring(length, lineText.Length - length).Trim();
				string str2 = str1.Substring(0, str1.Length - 33);
				this.m_manifest.Add(str2, str1);
			}
			num1 = num + 1;
		}
		while (num > 0);
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
			this.LatestVersion = Convert.ToInt32(strArrays[0]);
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
		this.InitAssetBundleManager();
	}

	public void UpdateVersion()
	{
		string mAssetServerIpAddress = this.m_assetServerIpAddress;
		if (Login.instance.GetBnPortal() == "cn")
		{
			mAssetServerIpAddress = this.m_assetServerIpAddress_CN;
		}
		string str = string.Concat(new string[] { "http://", mAssetServerIpAddress, "/falcon/d", string.Format("{0:D5}", BuildNum.DataBuildNum), "/", this.m_assetBundleDirectory, "/" });
		str = string.Concat(str, this.m_platform, "/update.txt");
		this.LatestVersion = 0;
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
}