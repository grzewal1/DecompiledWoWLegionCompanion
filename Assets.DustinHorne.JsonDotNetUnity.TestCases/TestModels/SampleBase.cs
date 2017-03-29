using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels
{
	public class SampleBase
	{
		public int NumberValue
		{
			get;
			set;
		}

		public string TextValue
		{
			get;
			set;
		}

		public Vector3 VectorValue
		{
			get;
			set;
		}

		public SampleBase()
		{
		}
	}
}