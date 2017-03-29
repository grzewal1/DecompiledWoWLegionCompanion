using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels
{
	public class SampleChild : SampleBase
	{
		public Dictionary<int, SimpleClassObject> ObjectDictionary
		{
			get;
			set;
		}

		public List<SimpleClassObject> ObjectList
		{
			get;
			set;
		}

		public SampleChild()
		{
		}
	}
}