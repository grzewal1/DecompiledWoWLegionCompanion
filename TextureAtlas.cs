using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WowStaticData;

public class TextureAtlas
{
	private static TextureAtlas s_instance;

	private static bool s_initialized;

	private Dictionary<int, Dictionary<int, Sprite>> m_atlas;

	private Dictionary<string, int> m_atlasMemberIDCache;

	public static TextureAtlas instance
	{
		get
		{
			if (TextureAtlas.s_instance == null)
			{
				TextureAtlas.s_instance = new TextureAtlas();
				TextureAtlas.s_instance.InitAtlas();
			}
			return TextureAtlas.s_instance;
		}
	}

	static TextureAtlas()
	{
	}

	public TextureAtlas()
	{
	}

	public Sprite GetAtlasSprite(int memberID)
	{
		Dictionary<int, Sprite> nums;
		Sprite sprite;
		memberID = this.GetOverrideMemberID(memberID);
		UiTextureAtlasMemberRec record = StaticDB.uiTextureAtlasMemberDB.GetRecord(memberID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat("GetAtlasSprite(): Unknown member ID ", memberID));
			return null;
		}
		this.m_atlas.TryGetValue((int)record.UiTextureAtlasID, out nums);
		if (nums == null)
		{
			Debug.LogWarning(string.Concat("GetAtlasSprite(): Unknown atlas ID ", record.UiTextureAtlasID));
			return null;
		}
		nums.TryGetValue(record.ID, out sprite);
		return sprite;
	}

	private int GetOverrideMemberID(int memberID)
	{
		switch (memberID)
		{
			case 6128:
			{
				memberID = 6100;
				break;
			}
			case 6129:
			{
				memberID = 6127;
				break;
			}
			case 6130:
			{
				memberID = 6126;
				break;
			}
			case 6131:
			{
				memberID = 6095;
				break;
			}
			case 6132:
			{
				memberID = 6097;
				break;
			}
		}
		return memberID;
	}

	public static Sprite GetSprite(int memberID)
	{
		return TextureAtlas.instance.GetAtlasSprite(memberID);
	}

	public static int GetUITextureAtlasMemberID(string atlasMemberName)
	{
		int d = 0;
		TextureAtlas.instance.m_atlasMemberIDCache.TryGetValue(atlasMemberName, out d);
		if (d > 0)
		{
			return d;
		}
		StaticDB.uiTextureAtlasMemberDB.EnumRecords((UiTextureAtlasMemberRec memberRec) => {
			if (memberRec.CommittedName == null || atlasMemberName == null || !(memberRec.CommittedName.ToLower() == atlasMemberName.ToLower()))
			{
				return true;
			}
			d = memberRec.ID;
			TextureAtlas.instance.m_atlasMemberIDCache.Add(atlasMemberName, memberRec.ID);
			return false;
		});
		return d;
	}

	private void InitAtlas()
	{
		if (TextureAtlas.s_initialized)
		{
			Debug.Log("WARNING! ATTEMPTED TO INIT TEXTURE ATLAS, BUT IT IS ALREADY INITIALIZED!! IGNORING");
			return;
		}
		this.m_atlas = new Dictionary<int, Dictionary<int, Sprite>>();
		string str = (Resources.Load("TextureAtlas/AtlasDirectory") as TextAsset).ToString();
		int num = 0;
		int num1 = 0;
		do
		{
			num = str.IndexOf('\n', num1);
			if (num < 0)
			{
				continue;
			}
			string str1 = str.Substring(num1, num - num1 + 1).Trim();
			string str2 = str1.Substring(str1.Length - 10);
			int num2 = Convert.ToInt32(str2);
			Sprite[] spriteArray = Resources.LoadAll<Sprite>(string.Concat("TextureAtlas/", str1));
			if ((int)spriteArray.Length <= 0)
			{
				Debug.Log(string.Concat("Found no sprites in atlas ", str1));
				num1 = num + 1;
				throw new Exception(string.Concat("Atlas in Resources folder is missing or has no sprites: ", str1));
			}
			Dictionary<int, Sprite> nums = new Dictionary<int, Sprite>();
			Sprite[] spriteArray1 = spriteArray;
			for (int i = 0; i < (int)spriteArray1.Length; i++)
			{
				Sprite sprite = spriteArray1[i];
				nums.Add(Convert.ToInt32(sprite.name), sprite);
			}
			this.m_atlas.Add(num2, nums);
			num1 = num + 1;
		}
		while (num > 0);
		this.m_atlasMemberIDCache = new Dictionary<string, int>();
		TextureAtlas.s_initialized = true;
	}
}