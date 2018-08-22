using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class BountySite : MonoBehaviour
	{
		public Image m_emissaryIcon;

		public Text m_invalidFileDataID;

		public Image m_errorImage;

		private WrapperWorldQuestBounty m_bounty;

		public BountySite()
		{
		}

		private void Awake()
		{
		}

		public void OnTap()
		{
			UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 3f, 0f);
			AllPopups.instance.ShowBountyInfoTooltip(this.m_bounty);
			Main.instance.m_UISound.Play_SelectWorldQuest();
		}

		public void SetBounty(WrapperWorldQuestBounty bounty)
		{
			this.m_bounty = bounty;
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, bounty.IconFileDataID);
			if (sprite == null)
			{
				this.m_invalidFileDataID.gameObject.SetActive(true);
				this.m_invalidFileDataID.text = string.Concat(string.Empty, bounty.IconFileDataID);
			}
			else
			{
				this.m_invalidFileDataID.gameObject.SetActive(false);
				this.m_emissaryIcon.sprite = sprite;
			}
		}

		private void Update()
		{
		}
	}
}