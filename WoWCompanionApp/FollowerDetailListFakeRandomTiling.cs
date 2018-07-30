using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class FollowerDetailListFakeRandomTiling : MonoBehaviour
	{
		public FollowerDetailListFakeRandomTiling()
		{
		}

		private void Start()
		{
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			component.offsetMin = new Vector2(UnityEngine.Random.Range(-300f, -2000f), component.offsetMin.y);
			component.offsetMax = new Vector2(-component.offsetMin.x, component.offsetMax.y);
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				float single = -1f * component.localScale.x;
				float single1 = component.localScale.y;
				Vector3 vector3 = component.localScale;
				component.localScale = new Vector3(single, single1, vector3.z);
			}
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				float single2 = component.localScale.x;
				float single3 = -1f * component.localScale.y;
				Vector3 vector31 = component.localScale;
				component.localScale = new Vector3(single2, single3, vector31.z);
			}
		}
	}
}