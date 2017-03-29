using System;
using UnityEngine;

public class Vars
{
	public const string CONFIG_FILE_NAME = "client.config";

	public Vars()
	{
	}

	public static string GetClientConfigPath()
	{
		return string.Format("{0}/{1}", Application.persistentDataPath, "client.config");
	}

	public static VarKey Key(string key)
	{
		return new VarKey(key);
	}

	public static void RefreshVars()
	{
		VarsInternal.RefreshVars();
	}
}