using Assets.DustinHorne.JsonDotNetUnity.TestCases;
using Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels;
using Newtonsoft.Json;
using SampleClassLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JsonTestScript
{
	private const string BAD_RESULT_MESSAGE = "Incorrect Deserialized Result";

	private TextMesh _text;

	public JsonTestScript(TextMesh text)
	{
		this._text = text;
	}

	public void DictionaryObjectKeySerialization()
	{
		this.LogStart("Dictionary (Object As Key)");
		try
		{
			Dictionary<SampleBase, int> sampleBases = new Dictionary<SampleBase, int>();
			for (int i = 0; i < 4; i++)
			{
				sampleBases.Add(TestCaseUtils.GetSampleBase(), i);
			}
			string str = JsonConvert.SerializeObject(sampleBases);
			this.LogSerialized(str);
			this._text.text = str;
			Dictionary<SampleBase, int> sampleBases1 = JsonConvert.DeserializeObject<Dictionary<SampleBase, int>>(str);
			List<SampleBase> sampleBases2 = new List<SampleBase>();
			List<SampleBase> sampleBases3 = new List<SampleBase>();
			foreach (SampleBase key in sampleBases.Keys)
			{
				sampleBases2.Add(key);
			}
			foreach (SampleBase sampleBase in sampleBases1.Keys)
			{
				sampleBases3.Add(sampleBase);
			}
			this.LogResult(sampleBases2[1].TextValue, sampleBases3[1].TextValue);
			if (sampleBases2[1].TextValue == sampleBases3[1].TextValue)
			{
				this.DisplaySuccess("Dictionary (Object As Key)");
			}
			else
			{
				this.DisplayFail("Dictionary (Object As Key)", "Incorrect Deserialized Result");
			}
		}
		catch (Exception exception)
		{
			this.DisplayFail("Dictionary (Object As Key)", exception.Message);
			throw;
		}
	}

	public void DictionaryObjectValueSerialization()
	{
		this.LogStart("Dictionary (Object Value)");
		try
		{
			Dictionary<int, SampleBase> nums = new Dictionary<int, SampleBase>();
			for (int i = 0; i < 4; i++)
			{
				nums.Add(i, TestCaseUtils.GetSampleBase());
			}
			string str = JsonConvert.SerializeObject(nums);
			this.LogSerialized(str);
			Dictionary<int, SampleBase> nums1 = JsonConvert.DeserializeObject<Dictionary<int, SampleBase>>(str);
			this.LogResult(nums[1].TextValue, nums1[1].TextValue);
			if (nums[1].TextValue == nums1[1].TextValue)
			{
				this.DisplaySuccess("Dictionary (Object Value)");
			}
			else
			{
				this.DisplayFail("Dictionary (Object Value)", "Incorrect Deserialized Result");
			}
		}
		catch (Exception exception)
		{
			this.DisplayFail("Dictionary (Object Value)", exception.Message);
			throw;
		}
	}

	public void DictionarySerialization()
	{
		this.LogStart("Dictionary & Other DLL");
		try
		{
			SampleExternalClass sampleExternalClass = new SampleExternalClass()
			{
				SampleString = Guid.NewGuid().ToString()
			};
			SampleExternalClass sampleExternalClass1 = sampleExternalClass;
			sampleExternalClass1.SampleDictionary.Add(1, "A");
			sampleExternalClass1.SampleDictionary.Add(2, "B");
			sampleExternalClass1.SampleDictionary.Add(3, "C");
			sampleExternalClass1.SampleDictionary.Add(4, "D");
			string str = JsonConvert.SerializeObject(sampleExternalClass1);
			this.LogSerialized(str);
			SampleExternalClass sampleExternalClass2 = JsonConvert.DeserializeObject<SampleExternalClass>(str);
			this.LogResult(sampleExternalClass1.SampleString, sampleExternalClass2.SampleString);
			int count = sampleExternalClass1.SampleDictionary.Count;
			this.LogResult(count.ToString(), sampleExternalClass2.SampleDictionary.Count);
			StringBuilder stringBuilder = new StringBuilder(4);
			StringBuilder stringBuilder1 = new StringBuilder(4);
			foreach (KeyValuePair<int, string> sampleDictionary in sampleExternalClass1.SampleDictionary)
			{
				stringBuilder.Append(sampleDictionary.Key.ToString());
				stringBuilder1.Append(sampleDictionary.Value);
			}
			this.LogResult("1234", stringBuilder.ToString());
			this.LogResult("ABCD", stringBuilder1.ToString());
			if (sampleExternalClass1.SampleString != sampleExternalClass2.SampleString || sampleExternalClass1.SampleDictionary.Count != sampleExternalClass2.SampleDictionary.Count || stringBuilder.ToString() != "1234" || stringBuilder1.ToString() != "ABCD")
			{
				this.DisplayFail("Dictionary & Other DLL", "Incorrect Deserialized Result");
			}
			else
			{
				this.DisplaySuccess("Dictionary & Other DLL");
			}
		}
		catch (Exception exception)
		{
			this.DisplayFail("Dictionary & Other DLL", exception.Message);
			throw;
		}
	}

	private void DisplayFail(string testName, string reason)
	{
		try
		{
			this._text.text = string.Concat(testName, "\r\nFailed :( \r\n", reason) ?? string.Empty;
		}
		catch
		{
			Debug.Log(string.Concat("%%%%%%%%%%%", testName));
		}
	}

	private void DisplaySuccess(string testName)
	{
		this._text.text = string.Concat(testName, "\r\nSuccessful");
	}

	public void GenericListSerialization()
	{
		this.LogStart("List<T> Serialization");
		try
		{
			List<SimpleClassObject> simpleClassObjects = new List<SimpleClassObject>();
			for (int i = 0; i < 4; i++)
			{
				simpleClassObjects.Add(TestCaseUtils.GetSimpleClassObject());
			}
			string str = JsonConvert.SerializeObject(simpleClassObjects);
			this.LogSerialized(str);
			List<SimpleClassObject> simpleClassObjects1 = JsonConvert.DeserializeObject<List<SimpleClassObject>>(str);
			int count = simpleClassObjects.Count;
			this.LogResult(count.ToString(), simpleClassObjects1.Count);
			this.LogResult(simpleClassObjects[2].TextValue, simpleClassObjects1[2].TextValue);
			if (simpleClassObjects.Count != simpleClassObjects1.Count || simpleClassObjects[3].TextValue != simpleClassObjects1[3].TextValue)
			{
				this.DisplayFail("List<T> Serialization", "Incorrect Deserialized Result");
				Debug.LogError("Deserialized List<T> has incorrect count or wrong item value");
			}
			else
			{
				this.DisplaySuccess("List<T> Serialization");
			}
		}
		catch (Exception exception)
		{
			this.DisplayFail("List<T> Serialization", exception.Message);
			throw;
		}
		this.LogEnd(2);
	}

	private void Log(object message)
	{
		Debug.Log(message);
	}

	private void LogEnd(int testNum)
	{
	}

	private void LogResult(string shouldEqual, object actual)
	{
		this.Log("--------------------");
		this.Log(string.Format("*** Original Test value: {0}", shouldEqual));
		this.Log(string.Format("*** Deserialized Test Value: {0}", actual));
		this.Log("--------------------");
	}

	private void LogSerialized(string message)
	{
		Debug.Log(string.Format("#### Serialized Object: {0}", message));
	}

	private void LogStart(string testName)
	{
		this.Log(string.Empty);
		this.Log(string.Format("======= SERIALIZATION TEST: {0} ==========", testName));
	}

	public void PolymorphicSerialization()
	{
		this.LogStart("Polymorphic Serialization");
		try
		{
			List<SampleBase> sampleBases = new List<SampleBase>();
			for (int i = 0; i < 4; i++)
			{
				sampleBases.Add(TestCaseUtils.GetSampleChid());
			}
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.All
			};
			string str = JsonConvert.SerializeObject(sampleBases, Formatting.None, jsonSerializerSetting);
			this.LogSerialized(str);
			jsonSerializerSetting = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.All
			};
			List<SampleBase> sampleBases1 = JsonConvert.DeserializeObject<List<SampleBase>>(str, jsonSerializerSetting);
			if (sampleBases1[2] is SampleChild)
			{
				this.LogResult(sampleBases[2].TextValue, sampleBases1[2].TextValue);
				if (sampleBases[2].TextValue == sampleBases1[2].TextValue)
				{
					this.DisplaySuccess("Polymorphic Serialization");
				}
				else
				{
					this.DisplayFail("Polymorphic Serialization", "Incorrect Deserialized Result");
				}
			}
			else
			{
				this.DisplayFail("Polymorphic Serialization", "Incorrect Deserialized Result");
			}
		}
		catch (Exception exception)
		{
			this.DisplayFail("Polymorphic Serialization", exception.Message);
			throw;
		}
		this.LogEnd(3);
	}

	public void SerializeVector3()
	{
		this.LogStart("Vector3 Serialization");
		try
		{
			Vector3 vector3 = new Vector3(2f, 4f, 6f);
			JsonConverter[] vector3Converter = new JsonConverter[] { new Vector3Converter() };
			string str = JsonConvert.SerializeObject(vector3, Formatting.None, vector3Converter);
			this.LogSerialized(str);
			Vector3 vector31 = JsonConvert.DeserializeObject<Vector3>(str);
			this.LogResult("4", vector31.y);
			if (vector31.y != vector3.y)
			{
				this.DisplayFail("Vector3 Serialization", "Incorrect Deserialized Result");
			}
			this.DisplaySuccess("Vector3 Serialization");
		}
		catch (Exception exception)
		{
			this.DisplayFail("Vector3 Serialization", exception.Message);
		}
		this.LogEnd(1);
	}
}