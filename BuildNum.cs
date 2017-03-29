using System;
using UnityEngine;

public class BuildNum : MonoBehaviour
{
	private const int s_buildNum = 91;

	private const int s_dataBuildNum = 42;

	public static int CodeBuildNum
	{
		get
		{
			return 91;
		}
	}

	public static int DataBuildNum
	{
		get
		{
			return 42;
		}
	}

	public BuildNum()
	{
	}
}