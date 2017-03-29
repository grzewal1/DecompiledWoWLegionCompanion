using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DustinHorne.Json.Examples
{
	public class JNSimpleObjectModel
	{
		public float FloatValue
		{
			get;
			set;
		}

		public List<int> IntList
		{
			get;
			set;
		}

		public int IntValue
		{
			get;
			set;
		}

		public JNObjectType ObjectType
		{
			get;
			set;
		}

		public string StringValue
		{
			get;
			set;
		}

		public JNSimpleObjectModel()
		{
		}
	}
}