using System;
using UnityEngine;

public class BuildNum : MonoBehaviour
{
	private const int s_buildNum = 114;

	private const int s_dataBuildNum = 57;

	public static int CodeBuildNum
	{
		get
		{
			return 114;
		}
	}

	public static int DataBuildNum
	{
		get
		{
			return 57;
		}
	}

	public BuildNum()
	{
	}
}