using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoWCompanionApp
{
	public static class UnityExtensions
	{
		public static T AddAsChildObject<T>(this GameObject gameObj, T prefab)
		where T : Component
		{
			T t = UnityEngine.Object.Instantiate<T>(prefab);
			t.transform.SetParent(gameObj.transform);
			t.transform.localScale = prefab.transform.localScale;
			t.transform.SetAsLastSibling();
			return t;
		}

		public static GameObject AddAsChildObject(this GameObject gameObj, GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			gameObject.transform.SetParent(gameObj.transform);
			gameObject.transform.localScale = prefab.transform.localScale;
			gameObject.transform.SetAsLastSibling();
			return gameObject;
		}

		public static void DetachAllChildren(this GameObject gameObj)
		{
			for (int i = gameObj.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(gameObj.transform.GetChild(i).gameObject);
			}
			gameObj.transform.DetachChildren();
		}

		public static string GetCanonicalName(this GameObject gameObj, string fieldName = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; gameObj.transform != gameObj.transform.root && i < 10; i++)
			{
				stringBuilder.Insert(0, ".").Insert(0, gameObj.name);
				gameObj = gameObj.transform.parent.gameObject;
			}
			stringBuilder.Insert(0, ".").Insert(0, gameObj.name);
			stringBuilder.Insert(0, "::").Insert(0, gameObj.scene.name ?? "<Prefab>");
			if (string.IsNullOrEmpty(fieldName))
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			else
			{
				stringBuilder.Append(fieldName);
			}
			return stringBuilder.ToString();
		}
	}
}