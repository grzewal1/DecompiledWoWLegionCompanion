using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WoWCompanionApp
{
	public class OnRedButtonClick : MonoBehaviour
	{
		public GameObject redButtonGameObject;

		private Vector3 initialScale;

		public OnRedButtonClick()
		{
		}

		public void ResizeRedButton(BaseEventData e)
		{
			RectTransform vector3 = this.redButtonGameObject.transform as RectTransform;
			float single = vector3.localScale.x;
			float single1 = vector3.localScale.y;
			Vector3 vector31 = vector3.localScale;
			this.initialScale = new Vector3(single, single1, vector31.z);
			Vector3 vector32 = vector3.localScale;
			Vector3 vector33 = vector3.localScale;
			vector3.localScale = new Vector3(vector32.x * 0.8f, vector33.y * 0.8f, 0f);
		}

		public void RevertRedButtonSize(BaseEventData e)
		{
			RectTransform vector3 = this.redButtonGameObject.transform as RectTransform;
			vector3.localScale = new Vector3(this.initialScale.x, this.initialScale.y, this.initialScale.z);
		}
	}
}