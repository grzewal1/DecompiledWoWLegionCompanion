using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobilePlayerJSON;
using WowStaticData;

public class TalentTreePanel : MonoBehaviour
{
	public Image m_classBG;

	public GameObject m_talentTreeItemRoot;

	public GameObject m_talentTreeItemPrefab;

	public GameObject m_romanNumeralRoot;

	private Vector2 m_multiPanelViewSizeDelta;

	private bool m_needsFullInit = true;

	private List<TalentTreeItem> m_talentTreeItems;

	public GameObject m_resourcesDisplay;

	public TalentTreePanel()
	{
	}

	public bool AnyTalentIsResearching()
	{
		TalentTreeItem[] componentsInChildren = this.m_talentTreeItemRoot.GetComponentsInChildren<TalentTreeItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			TalentTreeItem talentTreeItem = componentsInChildren[i];
			if (talentTreeItem.m_talentButtonLeft.IsResearching() || talentTreeItem.m_talentButtonRight.IsResearching())
			{
				return true;
			}
		}
		return false;
	}

	private void HandleEnteredWorld()
	{
		this.m_needsFullInit = true;
	}

	private void HandleGarrisonDataResetFinished()
	{
		if (!this.m_needsFullInit)
		{
			TalentTreeItem[] componentsInChildren = this.m_talentTreeItemRoot.GetComponentsInChildren<TalentTreeItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				TalentTreeItem talentTreeItem = componentsInChildren[i];
				talentTreeItem.HandleGarrisonDataResetFinished();
				MobilePlayerCanResearchGarrisonTalent mobilePlayerCanResearchGarrisonTalent = new MobilePlayerCanResearchGarrisonTalent()
				{
					GarrTalentID = talentTreeItem.m_talentButtonLeft.GetTalentID()
				};
				Login.instance.SendToMobileServer(mobilePlayerCanResearchGarrisonTalent);
				MobilePlayerCanResearchGarrisonTalent mobilePlayerCanResearchGarrisonTalent1 = new MobilePlayerCanResearchGarrisonTalent()
				{
					GarrTalentID = talentTreeItem.m_talentButtonRight.GetTalentID()
				};
				Login.instance.SendToMobileServer(mobilePlayerCanResearchGarrisonTalent1);
			}
		}
		else
		{
			this.InitTalentTree();
		}
	}

	private void InitTalentTree()
	{
		this.m_needsFullInit = false;
		Sprite sprite = this.LoadTalengBGForClass(GarrisonStatus.CharacterClassID());
		if (sprite != null)
		{
			this.m_classBG.sprite = sprite;
		}
		TalentTreeItem[] componentsInChildren = this.m_talentTreeItemRoot.GetComponentsInChildren<TalentTreeItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		Image[] imageArray = this.m_romanNumeralRoot.GetComponentsInChildren<Image>(true);
		for (int j = 0; j < (int)imageArray.Length; j++)
		{
			UnityEngine.Object.DestroyImmediate(imageArray[j].gameObject);
		}
		GarrTalentTreeRec garrTalentTreeRec1 = null;
		StaticDB.garrTalentTreeDB.EnumRecords((GarrTalentTreeRec garrTalentTreeRec) => {
			if (garrTalentTreeRec.ClassID != GarrisonStatus.CharacterClassID())
			{
				return true;
			}
			garrTalentTreeRec1 = garrTalentTreeRec;
			return false;
		});
		if (garrTalentTreeRec1 == null)
		{
			Debug.LogError(string.Concat("No GarrTalentTree record found for class ", GarrisonStatus.CharacterClassID()));
			return;
		}
		this.m_talentTreeItems = new List<TalentTreeItem>();
		for (int k = 0; k < garrTalentTreeRec1.MaxTiers; k++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_talentTreeItemPrefab);
			gameObject.transform.SetParent(this.m_talentTreeItemRoot.transform, false);
			TalentTreeItem component = gameObject.GetComponent<TalentTreeItem>();
			this.m_talentTreeItems.Add(component);
			switch (k)
			{
				case 0:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-One");
					break;
				}
				case 1:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Two");
					break;
				}
				case 2:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Three");
					break;
				}
				case 3:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Four");
					break;
				}
				case 4:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Five");
					break;
				}
				case 5:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Six");
					break;
				}
				case 6:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Seven");
					break;
				}
				case 7:
				{
					component.m_talentTier.sprite = Resources.Load<Sprite>("OrderAdvancement/Number-Eight");
					break;
				}
			}
		}
		StaticDB.garrTalentDB.EnumRecordsByParentID(garrTalentTreeRec1.ID, (GarrTalentRec garrTalentRec) => {
			this.m_talentTreeItems[garrTalentRec.Tier].SetTalent(garrTalentRec);
			MobilePlayerCanResearchGarrisonTalent mobilePlayerCanResearchGarrisonTalent = new MobilePlayerCanResearchGarrisonTalent()
			{
				GarrTalentID = garrTalentRec.ID
			};
			Login.instance.SendToMobileServer(mobilePlayerCanResearchGarrisonTalent);
			return true;
		});
		foreach (TalentTreeItem mTalentTreeItem in this.m_talentTreeItems)
		{
			mTalentTreeItem.UpdateVisualStates();
		}
	}

	private Sprite LoadTalengBGForClass(int classID)
	{
		Sprite sprite = null;
		switch (classID)
		{
			case 1:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-WarriorBG");
				break;
			}
			case 2:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-PaladinBG");
				break;
			}
			case 3:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-HunterBG");
				break;
			}
			case 4:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-RogueBG");
				break;
			}
			case 5:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-PriestBG");
				break;
			}
			case 6:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-DKBG");
				break;
			}
			case 7:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-ShamanBG");
				break;
			}
			case 8:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-MageBG");
				break;
			}
			case 9:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-WarlockBG");
				break;
			}
			case 10:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-MonkBG");
				break;
			}
			case 11:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-DruidBG");
				break;
			}
			case 12:
			{
				sprite = Resources.Load<Sprite>("OrderAdvancement/OrderAdvancement-DHBG");
				break;
			}
		}
		return sprite;
	}

	private void OnDisable()
	{
		Main.instance.GarrisonDataResetFinishedAction -= new Action(this.HandleGarrisonDataResetFinished);
	}

	private void OnEnable()
	{
		Main.instance.GarrisonDataResetFinishedAction += new Action(this.HandleGarrisonDataResetFinished);
		this.HandleEnteredWorld();
		this.InitTalentTree();
	}

	public void SetNeedsFullInit()
	{
		this.m_needsFullInit = true;
	}

	private void Start()
	{
	}

	public bool TalentIsReadyToPlayGreenCheckAnim()
	{
		bool flag;
		List<TalentTreeItem>.Enumerator enumerator = this.m_talentTreeItems.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TalentTreeItem current = enumerator.Current;
				if (!current.m_talentButtonLeft.IsReadyToShowGreenCheckAnim() && !current.m_talentButtonRight.IsReadyToShowGreenCheckAnim() && !current.m_talentButtonSolo.IsReadyToShowGreenCheckAnim())
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	private void Update()
	{
	}
}