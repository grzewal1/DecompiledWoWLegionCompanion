using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class RedCloseButton : MonoBehaviour
	{
		public RedCloseButton()
		{
		}

		public void PlayClickSound()
		{
			Main.instance.m_UISound.Play_ButtonRedClick();
		}
	}
}