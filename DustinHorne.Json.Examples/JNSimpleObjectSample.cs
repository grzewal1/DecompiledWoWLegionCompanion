using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DustinHorne.Json.Examples
{
	public class JNSimpleObjectSample
	{
		public JNSimpleObjectSample()
		{
		}

		public void Sample()
		{
			JNSimpleObjectModel jNSimpleObjectModel = new JNSimpleObjectModel()
			{
				IntValue = 5,
				FloatValue = 4.98f,
				StringValue = "Simple Object",
				IntList = new List<int>()
				{
					4,
					7,
					25,
					34
				},
				ObjectType = JNObjectType.BaseClass
			};
			JNSimpleObjectModel jNSimpleObjectModel1 = JsonConvert.DeserializeObject<JNSimpleObjectModel>(JsonConvert.SerializeObject(jNSimpleObjectModel));
			Debug.Log(jNSimpleObjectModel1.IntList.Count);
		}
	}
}