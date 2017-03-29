using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;

public class GuildChatSlider : MonoBehaviour
{
	[Header("General Stuff")]
	public GameObject chatRootObj;

	[Header("Chat Stuff")]
	public GameObject m_chatViewObj;

	public Text conversationText;

	public InputField textToSend;

	public Text m_numGuildMatesOnlineText;

	[Header("Guild Member Stuff")]
	public GameObject m_guildMemberViewObj;

	public Text m_guildMemberText;

	public GuildChatSlider()
	{
	}

	public void Hide()
	{
		this.chatRootObj.GetComponent<SliderPanel>().HideSliderPanel();
	}

	public void OnReceiveText(string sender, string text)
	{
		Text text1 = this.conversationText;
		string str = text1.text;
		text1.text = string.Concat(new string[] { str, "[", sender, "]: ", text, "\n" });
	}

	public void OnSendText()
	{
		if (this.textToSend.text.Length == 0)
		{
			return;
		}
		Main.instance.SendGuildChat(this.textToSend.text);
		this.textToSend.text = string.Empty;
		this.textToSend.Select();
		this.textToSend.ActivateInputField();
	}

	public void Show()
	{
		this.chatRootObj.SetActive(true);
		this.chatRootObj.GetComponent<SliderPanel>().ShowSliderPanel();
	}

	public void ShowGuildChat()
	{
		this.m_chatViewObj.SetActive(true);
		this.m_guildMemberViewObj.SetActive(false);
	}

	public void ShowGuildMemberList()
	{
		this.m_chatViewObj.SetActive(false);
		this.m_guildMemberViewObj.SetActive(true);
	}

	private void Start()
	{
		Main.instance.SetChatScript(this);
		this.ShowGuildChat();
	}

	public void Toggle()
	{
		if (!this.chatRootObj.activeSelf)
		{
			this.Show();
		}
		else
		{
			this.Hide();
		}
	}

	private void Update()
	{
	}

	public void UpdateGuildMateRoster()
	{
		if (GuildData.guildMemberDictionary.Count != 0)
		{
			this.m_numGuildMatesOnlineText.text = string.Concat(string.Empty, GuildData.guildMemberDictionary.Count);
		}
		else
		{
			this.m_numGuildMatesOnlineText.text = string.Empty;
		}
		this.m_guildMemberText.text = string.Empty;
		foreach (KeyValuePair<string, GuildData.GuildMember> keyValuePair in GuildData.guildMemberDictionary)
		{
			Text mGuildMemberText = this.m_guildMemberText;
			mGuildMemberText.text = string.Concat(mGuildMemberText.text, keyValuePair.Value.m_mobileGuildMember.Name, "\n");
		}
	}

	public enum SLASH_CMD
	{
		SYSTEM,
		SAY,
		PARTY,
		RAID,
		GUILD,
		OFFICER,
		YELL,
		WHISPER,
		WHISPER_FOREIGN,
		WHISPER_INFORM,
		EMOTE,
		TEXT_EMOTE,
		MONSTER_SAY,
		MONSTER_PARTY,
		MONSTER_YELL,
		MONSTER_WHISPER,
		MONSTER_EMOTE,
		SEND_CHANNEL,
		JOIN_CHANNEL,
		LEAVE_CHANNEL,
		LIST_CHANNEL,
		CHANNEL_NOTICE,
		CHANNEL_NOTICE_USER,
		SEND_AFK,
		SEND_DND,
		IGNORED,
		SKILL,
		LOOT,
		MONEY,
		OPENING,
		TRADESKILLS,
		PET_INFO,
		COMBAT_MISC_INFO,
		COMBAT_MSG_XP_GAIN,
		COMBAT_MSG_HONOR_GAIN,
		COMBAT_MSG_FACTION_CHANGE,
		BG_SYSTEM_NEUTRAL,
		BG_SYSTEM_ALLIANCE,
		BG_SYSTEM_HORDE,
		RAID_LEADER,
		RAID_WARNING,
		RAID_BOSS_EMOTE,
		RAID_BOSS_WHISPER,
		SPAM_FILTER,
		RESTRICTED,
		BATTLENET,
		ACHIEVEMENT,
		GUILD_ACHIEVEMENT,
		COMBAT_MSG_ARENA_POINTS_GAIN,
		PARTY_LEADER,
		TARGET_ICONS,
		BN_WHISPER,
		BN_WHISPER_INFORM,
		BN_CONVERSATION,
		BN_CONVERSATION_NOTICE,
		BN_CONVERSATION_LIST,
		BN_INLINE_TOAST_ALERT,
		BN_INLINE_TOAST_BROADCAST,
		BN_INLINE_TOAST_BROADCAST_INFORM,
		BN_INLINE_TOAST_CONVERSATION,
		BN_WHISPER_PLAYER_OFFLINE,
		MSG_COMBAT_GUILD_XP_GAIN,
		CURRENCY,
		QUEST_BOSS_EMOTE,
		PET_BATTLE_COMBAT_LOG,
		PET_BATTLE_INFO,
		INSTANCE_CHAT,
		INSTANCE_CHAT_LEADER
	}
}