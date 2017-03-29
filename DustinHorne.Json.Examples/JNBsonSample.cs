using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DustinHorne.Json.Examples
{
	public class JNBsonSample
	{
		public JNBsonSample()
		{
		}

		public void Sample()
		{
			JNSimpleObjectModel jNSimpleObjectModel;
			JNSimpleObjectModel jNSimpleObjectModel1 = new JNSimpleObjectModel()
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
			JNSimpleObjectModel jNSimpleObjectModel2 = jNSimpleObjectModel1;
			byte[] array = new byte[0];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BsonWriter bsonWriter = new BsonWriter(memoryStream))
				{
					(new JsonSerializer()).Serialize(bsonWriter, jNSimpleObjectModel2);
				}
				array = memoryStream.ToArray();
				Debug.Log(Convert.ToBase64String(array));
			}
			using (MemoryStream memoryStream1 = new MemoryStream(array))
			{
				using (BsonReader bsonReader = new BsonReader(memoryStream1))
				{
					jNSimpleObjectModel = (new JsonSerializer()).Deserialize<JNSimpleObjectModel>(bsonReader);
				}
			}
			if (jNSimpleObjectModel != null)
			{
				Debug.Log(jNSimpleObjectModel.StringValue);
			}
		}
	}
}