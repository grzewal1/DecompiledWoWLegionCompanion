using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.JSONRealmList;

public class CharacterListView : MonoBehaviour
{
	public GameObject charListItemPrefab;

	public GameObject charListContents;

	public float m_listItemInitialEntranceDelay;

	public float m_listItemEntranceDelay;

	public JamJSONCharacterEntry m_characterEntry;

	private static string m_levelText;

	private bool m_initialized;

	static CharacterListView()
	{
	}

	public CharacterListView()
	{
	}

	public void AddCharacterButton(JamJSONCharacterEntry charData, string subRegion, string realmName, bool online)
	{
		this.m_characterEntry = charData;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.charListItemPrefab);
		gameObject.transform.SetParent(this.charListContents.transform, false);
		CharacterListButton component = gameObject.GetComponent<CharacterListButton>();
		component.SetGUID(charData.PlayerGuid);
		component.m_characterEntry = charData;
		component.m_subRegion = subRegion;
		Sprite sprite = GeneralHelpers.LoadClassIcon((int)charData.ClassID);
		if (sprite != null)
		{
			component.m_characterClassIcon.sprite = sprite;
		}
		component.m_characterName.text = charData.Name;
		bool flag = online;
		if (!charData.HasMobileAccess)
		{
			component.m_missingRequirement.text = StaticDB.GetString("REQUIRES_CLASS_HALL", null);
			component.m_missingRequirement.color = Color.red;
			flag = false;
		}
		else if (realmName != "unknown")
		{
			if (!online)
			{
				component.m_missingRequirement.text = string.Concat(realmName, " (", StaticDB.GetString("OFFLINE", null), ")");
			}
			else
			{
				component.m_missingRequirement.text = realmName;
			}
			component.m_missingRequirement.color = Color.yellow;
		}
		else
		{
			component.m_missingRequirement.text = string.Empty;
			flag = false;
		}
		component.m_missingRequirement.gameObject.SetActive(true);
		if (!flag)
		{
			gameObject.GetComponent<Button>().interactable = false;
			component.m_characterName.color = Color.grey;
			component.m_characterLevel.color = Color.grey;
		}
		int experienceLevel = charData.ExperienceLevel;
		if (experienceLevel < 1)
		{
			experienceLevel = 1;
		}
		component.m_characterLevel.text = GeneralHelpers.TextOrderString(CharacterListView.m_levelText, experienceLevel.ToString());
	}

	public void CharacterSelected()
	{
		this.ClearList();
		Main.instance.allPanels.ShowConnectingPanel();
	}

	public void ClearList()
	{
		CharacterListButton[] componentsInChildren = this.charListContents.transform.GetComponentsInChildren<CharacterListButton>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
	}

	private void OnEnable()
	{
		this.m_initialized = false;
	}

	private void SetLevelText()
	{
		if (CharacterListView.m_levelText == null)
		{
			CharacterListView.m_levelText = StaticDB.GetString("LEVEL", null);
		}
	}

	public void SortCharacterList()
	{
		CharacterListButton[] componentsInChildren = this.charListContents.transform.GetComponentsInChildren<CharacterListButton>(true);
		List<CharacterListButton> characterListButtons = new List<CharacterListButton>();
		CharacterListButton[] characterListButtonArray = componentsInChildren;
		for (int i = 0; i < (int)characterListButtonArray.Length; i++)
		{
			characterListButtons.Add(characterListButtonArray[i]);
		}
		characterListButtons.Sort(new CharacterListView.CharacterButtonComparer());
		for (int j = 0; j < characterListButtons.Count; j++)
		{
			CharacterListButton[] characterListButtonArray1 = componentsInChildren;
			int num = 0;
			while (num < (int)characterListButtonArray1.Length)
			{
				CharacterListButton characterListButton = characterListButtonArray1[num];
				if (characterListButton.m_characterEntry.PlayerGuid != characterListButtons[j].m_characterEntry.PlayerGuid)
				{
					num++;
				}
				else
				{
					characterListButton.transform.SetSiblingIndex(j);
					break;
				}
			}
		}
		for (int k = 0; k < characterListButtons.Count; k++)
		{
			FancyEntrance component = characterListButtons[k].GetComponent<FancyEntrance>();
			component.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)k;
			component.Activate();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!this.m_initialized && AssetBundleManager.instance.IsInitialized())
		{
			this.m_initialized = true;
			this.SetLevelText();
		}
	}

	private class CharacterButtonComparer : IComparer<CharacterListButton>
	{
		public CharacterButtonComparer()
		{
		}

		public int Compare(CharacterListButton char1, CharacterListButton char2)
		{
			Button component = char1.gameObject.GetComponent<Button>();
			Button button = char2.gameObject.GetComponent<Button>();
			if (component.interactable && !button.interactable)
			{
				return -1;
			}
			if (button.interactable && !component.interactable)
			{
				return 1;
			}
			if (char1.m_characterEntry.ExperienceLevel != char2.m_characterEntry.ExperienceLevel)
			{
				return char2.m_characterEntry.ExperienceLevel - char1.m_characterEntry.ExperienceLevel;
			}
			return string.Compare(char1.m_characterName.text, char2.m_characterName.text);
		}
	}
}