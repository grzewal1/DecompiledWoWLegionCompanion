using System;
using UnityEngine;
using WowJamMessages;

public class RecentCharacterArea : MonoBehaviour
{
	public RecentCharacterButton[] m_charButtons;

	public RecentCharacterArea()
	{
	}

	public void SetRecentCharacter(int index, RecentCharacter recentChar)
	{
		if (index < 0 || index >= (int)this.m_charButtons.Length)
		{
			Debug.Log(string.Concat("SetRecentCharacter: invalid index ", index));
			return;
		}
		if (recentChar == null)
		{
			this.m_charButtons[index].gameObject.SetActive(false);
		}
		else
		{
			this.m_charButtons[index].gameObject.SetActive(true);
			this.m_charButtons[index].SetRecentCharacter(recentChar);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}