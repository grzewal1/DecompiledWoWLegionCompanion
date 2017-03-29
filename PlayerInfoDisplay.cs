using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour
{
	public Text m_characterName;

	public Text m_characterClassName;

	public Text m_characterListButton;

	public Image m_classIcon;

	public GameObject m_recentCharactersPanel;

	public PlayerInfoDisplay()
	{
	}

	private void HandlePlayerLeveledUp(int newLevel)
	{
		this.InitPlayerDisplay(newLevel);
	}

	public void HideRecentCharacterPanel()
	{
		this.m_recentCharactersPanel.SetActive(false);
	}

	private void InitPlayerDisplay(int playerLevel)
	{
		this.m_characterName.text = GarrisonStatus.CharacterName();
		if (Main.instance.GetLocale() != "frFR")
		{
			this.m_characterClassName.text = string.Concat(GeneralHelpers.TextOrderString(StaticDB.GetString("LEVEL", null), playerLevel.ToString()), " ", GarrisonStatus.CharacterClassName());
		}
		else
		{
			this.m_characterClassName.text = string.Concat(new string[] { GarrisonStatus.CharacterClassName(), " ", StaticDB.GetString("LEVEL", null), " ", playerLevel.ToString() });
		}
		this.m_characterListButton.text = StaticDB.GetString("CHARACTER_LIST", null);
		Sprite sprite = GeneralHelpers.LoadClassIcon(GarrisonStatus.CharacterClassID());
		if (sprite != null)
		{
			this.m_classIcon.sprite = sprite;
		}
	}

	private void OnDisable()
	{
		Main.instance.PlayerLeveledUpAction -= new Action<int>(this.HandlePlayerLeveledUp);
	}

	private void OnEnable()
	{
		this.InitPlayerDisplay(GarrisonStatus.CharacterLevel());
		Main.instance.PlayerLeveledUpAction += new Action<int>(this.HandlePlayerLeveledUp);
	}

	public void ToggleRecentCharacterPanel()
	{
		Main.instance.m_UISound.Play_ButtonBlackClick();
		this.m_recentCharactersPanel.SetActive(!this.m_recentCharactersPanel.activeSelf);
	}
}