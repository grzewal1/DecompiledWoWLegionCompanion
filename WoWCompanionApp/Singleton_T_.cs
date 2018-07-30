using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public class Singleton<T> : MonoBehaviour
	where T : MonoBehaviour
	{
		private static T s_instance;

		public static T instance
		{
			get
			{
				return Singleton<T>.Instance;
			}
		}

		public static T Instance
		{
			get
			{
				if (Singleton<T>.s_instance == null)
				{
					Singleton<T>.s_instance = UnityEngine.Object.FindObjectOfType<T>();
					if (Singleton<T>.s_instance == null)
					{
						Singleton<T>.s_instance = (new GameObject(typeof(T).Name)).AddComponent<T>();
					}
				}
				return Singleton<T>.s_instance;
			}
		}

		protected bool IsCloneGettingRemoved
		{
			get;
			private set;
		}

		public Singleton()
		{
		}

		private void Awake()
		{
			if (!(Singleton<T>.s_instance != null) || !(Singleton<T>.s_instance != this))
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				Singleton<T>.s_instance = base.gameObject.GetComponent<T>();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
				this.IsCloneGettingRemoved = true;
			}
		}
	}
}