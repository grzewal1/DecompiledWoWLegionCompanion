using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DustinHorne.Json.Examples
{
	public class JNPolymorphismSample
	{
		private System.Random _rnd = new System.Random();

		public JNPolymorphismSample()
		{
		}

		private JNSimpleObjectModel GetBaseModel()
		{
			JNSimpleObjectModel jNSimpleObjectModel = new JNSimpleObjectModel()
			{
				IntValue = this._rnd.Next(),
				FloatValue = (float)this._rnd.NextDouble(),
				StringValue = Guid.NewGuid().ToString()
			};
			List<int> nums = new List<int>()
			{
				this._rnd.Next(),
				this._rnd.Next(),
				this._rnd.Next()
			};
			jNSimpleObjectModel.IntList = nums;
			jNSimpleObjectModel.ObjectType = JNObjectType.BaseClass;
			return jNSimpleObjectModel;
		}

		private JNSubClassModel GetSubClassModel()
		{
			JNSubClassModel jNSubClassModel = new JNSubClassModel()
			{
				IntValue = this._rnd.Next(),
				FloatValue = (float)this._rnd.NextDouble(),
				StringValue = Guid.NewGuid().ToString()
			};
			List<int> nums = new List<int>()
			{
				this._rnd.Next(),
				this._rnd.Next(),
				this._rnd.Next()
			};
			jNSubClassModel.IntList = nums;
			jNSubClassModel.ObjectType = JNObjectType.SubClass;
			jNSubClassModel.SubClassStringValue = "This is the subclass value.";
			return jNSubClassModel;
		}

		public void Sample()
		{
			List<JNSimpleObjectModel> jNSimpleObjectModels = new List<JNSimpleObjectModel>();
			for (int i = 0; i < 3; i++)
			{
				jNSimpleObjectModels.Add(this.GetBaseModel());
			}
			for (int j = 0; j < 2; j++)
			{
				jNSimpleObjectModels.Add(this.GetSubClassModel());
			}
			for (int k = 0; k < 3; k++)
			{
				jNSimpleObjectModels.Add(this.GetBaseModel());
			}
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.All
			};
			string str = JsonConvert.SerializeObject(jNSimpleObjectModels, Formatting.None, jsonSerializerSetting);
			List<JNSimpleObjectModel> jNSimpleObjectModels1 = JsonConvert.DeserializeObject<List<JNSimpleObjectModel>>(str, jsonSerializerSetting);
			for (int l = 0; l < jNSimpleObjectModels1.Count; l++)
			{
				JNSimpleObjectModel item = jNSimpleObjectModels1[l];
				if (item.ObjectType != JNObjectType.SubClass)
				{
					Debug.Log(item.StringValue);
				}
				else
				{
					Debug.Log((item as JNSubClassModel).SubClassStringValue);
				}
			}
		}
	}
}