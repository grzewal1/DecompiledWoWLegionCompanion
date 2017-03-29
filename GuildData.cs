using System;
using System.Collections.Generic;
using WowJamMessages.MobileClientJSON;

public class GuildData
{
	private static GuildData s_instance;

	private Dictionary<string, GuildData.GuildMember> m_guildMemberDictionary;

	public static Dictionary<string, GuildData.GuildMember> guildMemberDictionary
	{
		get
		{
			return GuildData.instance.m_guildMemberDictionary;
		}
	}

	public static GuildData instance
	{
		get
		{
			if (GuildData.s_instance == null)
			{
				GuildData.s_instance = new GuildData()
				{
					m_guildMemberDictionary = new Dictionary<string, GuildData.GuildMember>()
				};
			}
			return GuildData.s_instance;
		}
	}

	static GuildData()
	{
	}

	public GuildData()
	{
	}

	public static void AddGuildMember(MobileGuildMember mobileGuildMember)
	{
		if (!GuildData.instance.m_guildMemberDictionary.ContainsKey(mobileGuildMember.Guid))
		{
			GuildData.GuildMember guildMember = new GuildData.GuildMember()
			{
				m_mobileGuildMember = mobileGuildMember,
				m_isLoggedIn = true
			};
			GuildData.instance.m_guildMemberDictionary.Add(mobileGuildMember.Guid, guildMember);
		}
	}

	public static void ClearData()
	{
		GuildData.instance.m_guildMemberDictionary.Clear();
	}

	public static void RemoveGuildMember(string mobileGuildMemberGUID)
	{
		if (GuildData.instance.m_guildMemberDictionary.ContainsKey(mobileGuildMemberGUID))
		{
			GuildData.instance.m_guildMemberDictionary.Remove(mobileGuildMemberGUID);
		}
	}

	public class GuildMember
	{
		public MobileGuildMember m_mobileGuildMember;

		public bool m_isLoggedIn;

		public GuildMember()
		{
		}
	}
}