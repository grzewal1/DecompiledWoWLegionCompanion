using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStatConstants;

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
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		bool flag = true;
		IEnumerator enumerator = PersistentArmamentData.armamentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileFollowerArmamentExt current = (MobileFollowerArmamentExt)enumerator.Current;
				FollowerInventoryListItem followerInventoryListItem = UnityEngine.Object.Instantiate<FollowerInventoryListItem>(this.m_armamentListItemPrefab);
				followerInventoryListItem.transform.SetParent(this.m_armamentListContent.transform, false);
				followerInventoryListItem.SetArmament(current, followerDetailView);
				flag = false;
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
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.ArmamentInventoryChangedAction += new Action(this.HandleArmamentsChanged);
		MobilePlayerFollowerArmamentsExtendedRequest mobilePlayerFollowerArmamentsExtendedRequest = new MobilePlayerFollowerArmamentsExtendedRequest()
		{
			GarrFollowerTypeID = 4
		};
		Login.instance.SendToMobileServer(mobilePlayerFollowerArmamentsExtendedRequest);
	}
}