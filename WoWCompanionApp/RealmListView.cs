using bnet.protocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class RealmListView : MonoBehaviour
	{
		public GameObject loginListItemPrefab;

		public GameObject bnLoginListItemPrefab;

		public GameObject gameAccountButtonPrefab;

		public GameObject loginListContents;

		public float m_listItemInitialEntranceDelay;

		public float m_listItemEntranceDelay;

		public Text m_titleText;

		public Text m_cancelText;

		public RealmListView()
		{
		}

		public void AddBnLoginButton(string realmName, ulong realmAddress, string subRegion, int characterCount, bool online)
		{
			BnLoginButton[] componentsInChildren = this.loginListContents.GetComponentsInChildren<BnLoginButton>();
			int num = (componentsInChildren == null ? 0 : (int)componentsInChildren.Length);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bnLoginListItemPrefab);
			gameObject.transform.SetParent(this.loginListContents.transform, false);
			gameObject.GetComponent<BnLoginButton>().SetInfo(realmAddress, realmName, subRegion, characterCount, online);
			FancyEntrance component = gameObject.GetComponent<FancyEntrance>();
			component.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)num;
			component.Activate();
		}

		public void AddGameAccountButton(EntityId gameAccount, string name, bool isBanned, bool isSuspended)
		{
			BnLoginButton[] componentsInChildren = this.loginListContents.GetComponentsInChildren<BnLoginButton>();
			int num = (componentsInChildren == null ? 0 : (int)componentsInChildren.Length);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameAccountButtonPrefab);
			gameObject.transform.SetParent(this.loginListContents.transform, false);
			gameObject.GetComponent<BnGameAccountButton>().SetInfo(gameAccount, name, isBanned, isSuspended);
			FancyEntrance component = gameObject.GetComponent<FancyEntrance>();
			component.m_timeToDelayEntrance = this.m_listItemInitialEntranceDelay + this.m_listItemEntranceDelay * (float)num;
			component.Activate();
		}

		public bool BnLoginButtonExists(ulong realmAddress)
		{
			BnLoginButton[] componentsInChildren = this.loginListContents.GetComponentsInChildren<BnLoginButton>();
			if (componentsInChildren == null)
			{
				return false;
			}
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetRealmAddress() == realmAddress)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearBnRealmList()
		{
			BnLoginButton[] componentsInChildren = this.loginListContents.transform.GetComponentsInChildren<BnLoginButton>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			BnGameAccountButton[] bnGameAccountButtonArray = this.loginListContents.transform.GetComponentsInChildren<BnGameAccountButton>(true);
			for (int j = 0; j < (int)bnGameAccountButtonArray.Length; j++)
			{
				UnityEngine.Object.Destroy(bnGameAccountButtonArray[j].gameObject);
			}
		}

		private void OnEnable()
		{
			!Main.instance;
		}

		public void SetGameAccountTitle()
		{
			this.m_titleText.text = StaticDB.GetString("ACCOUNT_SELECTION", null);
		}

		public void SetRealmListTitle()
		{
			this.m_titleText.text = StaticDB.GetString("REALM_SELECTION", null);
		}

		private void Start()
		{
			this.m_titleText.font = GeneralHelpers.LoadFancyFont();
			this.m_titleText.text = StaticDB.GetString("REALM_SELECTION", null);
			this.m_cancelText.font = GeneralHelpers.LoadStandardFont();
			this.m_cancelText.text = StaticDB.GetString("CANCEL", null);
		}

		private void Update()
		{
		}

		public void UpdateLoginButton(ulong realmAddress, bool online)
		{
			BnLoginButton[] componentsInChildren = this.loginListContents.GetComponentsInChildren<BnLoginButton>();
			if (componentsInChildren == null)
			{
				return;
			}
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetRealmAddress() == realmAddress)
				{
					componentsInChildren[i].SetOnline(online);
					return;
				}
			}
		}
	}
}