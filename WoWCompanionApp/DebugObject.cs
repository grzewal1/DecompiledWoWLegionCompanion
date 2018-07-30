using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class DebugObject : MonoBehaviour
	{
		public DebugObject()
		{
		}

		private void Awake()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}