using System;

public class VarKey
{
	private string m_key;

	public VarKey(string key)
	{
		this.m_key = key;
	}

	public VarKey(string key, string subKey)
	{
		this.m_key = string.Concat(key, ".", subKey);
	}

	public bool GetBool(bool def)
	{
		if (!VarsInternal.Get().Contains(this.m_key))
		{
			return def;
		}
		string str = VarsInternal.Get().Value(this.m_key);
		return GeneralUtils.ForceBool(str);
	}

	public float GetFloat(float def)
	{
		if (!VarsInternal.Get().Contains(this.m_key))
		{
			return def;
		}
		string str = VarsInternal.Get().Value(this.m_key);
		return GeneralUtils.ForceFloat(str);
	}

	public int GetInt(int def)
	{
		if (!VarsInternal.Get().Contains(this.m_key))
		{
			return def;
		}
		string str = VarsInternal.Get().Value(this.m_key);
		return GeneralUtils.ForceInt(str);
	}

	public string GetStr(string def)
	{
		if (!VarsInternal.Get().Contains(this.m_key))
		{
			return def;
		}
		return VarsInternal.Get().Value(this.m_key);
	}

	public VarKey Key(string subKey)
	{
		return new VarKey(this.m_key, subKey);
	}
}