using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class ArmamentDialog : MonoBehaviour
	{
		public FollowerInventoryListItem m_armamentListItemPrefab;

		public GameObject m_armamentListContent;

		public Text m_titleText;

		public Text m_emptyMessage;

		private FollowerDetailView m_currentFollowerDetailView;

		public ArmamentDialog()
		{
		}

		private void Awake()
		{
			this.m_titleText.font = GeneralHelpers.LoadFancyFont();
			this.m_titleText.text = StaticDB.GetString("CHAMPION_ENHANCEMENT", null);
			this.m_emptyMessage.font = GeneralHelpers.LoadStandardFont();
			this.m_emptyMessage.text = StaticDB.GetString("NO_ARMAMENTS2", "You do not have any armaments to equip.");
		}

		private void HandleArmamentsChanged()
		{
			if (this.m_currentFollowerDetailView != null)
			{
				this.Init(this.m_currentFollowerDetailView);
			}
		}

		public void Init(FollowerDetailView followerDetailView)
		{
			this.m_currentFollowerDetailView = followerDetailView;
			FollowerInventoryListItem[] componentsInChildren = this.m_armamentListContent.GetComponentsInChildren<FollowerInventoryListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			bool flag = true;
			foreach (WrapperFollowerArmamentExt value in PersistentArmamentData.armamentDictionary.Values)
			{
				FollowerInventoryListItem followerInventoryListItem = UnityEngine.Object.Instantiate<FollowerInventoryListItem>(this.m_armamentListItemPrefab);
				followerInventoryListItem.transform.SetParent(this.m_armamentListContent.transform, false);
				followerInventoryListItem.SetArmament(value, followerDetailView);
				flag = false;
			}
			this.m_emptyMessage.gameObject.SetActive(flag);
		}

		private void OnDisable()
		{
			Main.instance.m_backButtonManager.PopBackAction();
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.ArmamentInventoryChangedAction -= new Action(this.HandleArmamentsChanged);
			this.m_currentFollowerDetailView = null;
		}

		public void OnEnable()
		{
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
			Main.instance.m_UISound.Play_ShowGenericTooltip();
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.ArmamentInventoryChangedAction += new Action(this.HandleArmamentsChanged);
			LegionCompanionWrapper.RequestFollowerArmamentsExtended((int)GarrisonStatus.GarrisonFollowerType);
		}
	}
}