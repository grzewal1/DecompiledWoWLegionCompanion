using Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DustinHorne.JsonDotNetUnity.TestCases
{
	public static class TestCaseUtils
	{
		private static System.Random _rnd;

		static TestCaseUtils()
		{
			TestCaseUtils._rnd = new System.Random();
		}

		public static SampleBase GetSampleBase()
		{
			SampleBase sampleBase = new SampleBase()
			{
				TextValue = Guid.NewGuid().ToString(),
				NumberValue = TestCaseUtils._rnd.Next()
			};
			int num = TestCaseUtils._rnd.Next();
			int num1 = TestCaseUtils._rnd.Next();
			int num2 = TestCaseUtils._rnd.Next();
			sampleBase.VectorValue = new Vector3((float)num, (float)num1, (float)num2);
			return sampleBase;
		}

		public static SampleChild GetSampleChid()
		{
			SampleChild sampleChild = new SampleChild()
			{
				TextValue = Guid.NewGuid().ToString(),
				NumberValue = TestCaseUtils._rnd.Next()
			};
			int num = TestCaseUtils._rnd.Next();
			int num1 = TestCaseUtils._rnd.Next();
			int num2 = TestCaseUtils._rnd.Next();
			sampleChild.VectorValue = new Vector3((float)num, (float)num1, (float)num2);
			sampleChild.ObjectDictionary = new Dictionary<int, SimpleClassObject>();
			for (int i = 0; i < 4; i++)
			{
				SimpleClassObject simpleClassObject = TestCaseUtils.GetSimpleClassObject();
				sampleChild.ObjectDictionary.Add(i, simpleClassObject);
			}
			sampleChild.ObjectList = new List<SimpleClassObject>();
			for (int j = 0; j < 4; j++)
			{
				SimpleClassObject simpleClassObject1 = TestCaseUtils.GetSimpleClassObject();
				sampleChild.ObjectList.Add(simpleClassObject1);
			}
			return sampleChild;
		}

		public static SimpleClassObject GetSimpleClassObject()
		{
			SimpleClassObject simpleClassObject = new SimpleClassObject()
			{
				TextValue = Guid.NewGuid().ToString(),
				NumberValue = TestCaseUtils._rnd.Next()
			};
			int num = TestCaseUtils._rnd.Next();
			int num1 = TestCaseUtils._rnd.Next();
			int num2 = TestCaseUtils._rnd.Next();
			simpleClassObject.VectorValue = new Vector3((float)num, (float)num1, (float)num2);
			return simpleClassObject;
		}
	}
}