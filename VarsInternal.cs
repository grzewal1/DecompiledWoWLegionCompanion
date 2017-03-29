using System;
using System.Collections.Generic;

internal class VarsInternal
{
	private static VarsInternal s_instance;

	private Dictionary<string, string> m_vars = new Dictionary<string, string>();

	static VarsInternal()
	{
		VarsInternal.s_instance = new VarsInternal();
	}

	private VarsInternal()
	{
		this.LoadConfig(Vars.GetClientConfigPath());
	}

	public bool Contains(string key)
	{
		return this.m_vars.ContainsKey(key);
	}

	public static VarsInternal Get()
	{
		return VarsInternal.s_instance;
	}

	private bool LoadConfig(string path)
	{
		ConfigFile configFile = new ConfigFile();
		if (!configFile.LightLoad(path))
		{
			return false;
		}
		foreach (ConfigFile.Line line in configFile.GetLines())
		{
			this.m_vars[line.m_fullKey] = line.m_value;
		}
		return true;
	}

	public static void RefreshVars()
	{
		VarsInternal.s_instance = new VarsInternal();
	}

	public string Value(string key)
	{
		return this.m_vars[key];
	}
}