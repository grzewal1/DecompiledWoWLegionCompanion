using System;
using UnityEngine;

public class BuildNum : MonoBehaviour
{
	private const int s_buildNum = 125;

	private const int s_dataBuildNum = 61;

	public static int CodeBuildNum
	{
		get
		{
			return 125;
		}
	}

	public static int DataBuildNum
	{
		get
		{
			return 61;
		}
	}

	public BuildNum()
	{
	}
}