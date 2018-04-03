using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class DownloadingPanel : MonoBehaviour
{
	public Image m_progressBarFillImage;

	public Text m_downloadText;

	public DownloadingPanel()
	{
	}

	private void Start()
	{
		int num;
		this.m_downloadText.font = GeneralHelpers.LoadFancyFont();
		string locale = Main.instance.GetLocale();
		if (locale != null)
		{
			if (DownloadingPanel.<>f__switch$map1 == null)
			{
				Dictionary<string, int> strs = new Dictionary<string, int>(10)
				{
					{ "koKR", 0 },
					{ "frFR", 1 },
					{ "deDE", 2 },
					{ "zhCN", 3 },
					{ "zhTW", 4 },
					{ "esES", 5 },
					{ "esMX", 6 },
					{ "ruRU", 7 },
					{ "ptBR", 8 },
					{ "itIT", 9 }
				};
				DownloadingPanel.<>f__switch$map1 = strs;
			}
			if (DownloadingPanel.<>f__switch$map1.TryGetValue(locale, out num))
			{
				switch (num)
				{
					case 0:
					{
						this.m_downloadText.text = "다운로드 중...";
						break;
					}
					case 1:
					{
						this.m_downloadText.text = "Téléchargement…";
						break;
					}
					case 2:
					{
						this.m_downloadText.text = "Lade herunter...";
						break;
					}
					case 3:
					{
						this.m_downloadText.text = "下载中……";
						break;
					}
					case 4:
					{
						this.m_downloadText.text = "下載中...";
						break;
					}
					case 5:
					{
						this.m_downloadText.text = "Descargando...";
						break;
					}
					case 6:
					{
						this.m_downloadText.text = "Descargando...";
						break;
					}
					case 7:
					{
						this.m_downloadText.text = "Загрузка...";
						break;
					}
					case 8:
					{
						this.m_downloadText.text = "Baixando...";
						break;
					}
					case 9:
					{
						this.m_downloadText.text = "Download...";
						break;
					}
				}
			}
		}
	}

	private void Update()
	{
		this.m_progressBarFillImage.fillAmount = AssetBundleManager.instance.GetDownloadProgress();
	}
}