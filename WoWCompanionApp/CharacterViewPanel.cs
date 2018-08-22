using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CharacterViewPanel : MonoBehaviour
	{
		public GameObject spinner;

		public Sprite backArrowSprite;

		public Image background;

		public string errorKey;

		public int timeoutSeconds;

		private CharacterWebView webView;

		private CompanionMultiPanel companionMultiPanel;

		private Button hamburgerMenuButton;

		private GameObject backButton;

		private float startTime;

		private bool isLoading;

		public int TopMargin
		{
			get;
			private set;
		}

		public CharacterViewPanel()
		{
		}

		public void DestroyPanel()
		{
			Main.instance.m_backButtonManager.PopBackAction();
			UnityEngine.Object.Destroy(base.gameObject);
			UnityEngine.Object.Destroy(this.backButton.gameObject);
			this.isLoading = true;
		}

		private void OnDestroy()
		{
			this.backButton.SetActive(false);
			this.hamburgerMenuButton.gameObject.SetActive(true);
			this.hamburgerMenuButton.interactable = true;
		}

		public void OnWebViewLoaded(bool success)
		{
			this.webView.SetWebViewVisible(success);
			if (!success)
			{
				this.DestroyPanel();
				Singleton<Login>.Instance.LoginUI.ShowGenericPopupFull(StaticDB.GetString(this.errorKey, "Error loading character"));
			}
			else
			{
				this.backButton.SetActive(true);
				this.hamburgerMenuButton.gameObject.SetActive(false);
			}
			this.SetSpinnerActive(false);
			this.isLoading = false;
		}

		public void SetSpinnerActive(bool active)
		{
			if (this.background != null)
			{
				this.background.gameObject.SetActive(active);
			}
			if (this.spinner != null)
			{
				this.spinner.SetActive(active);
			}
		}

		public void ShowWebView()
		{
			this.SetSpinnerActive(true);
			this.hamburgerMenuButton.interactable = false;
			this.hamburgerMenuButton.gameObject.SetActive(false);
			this.startTime = Time.timeSinceLevelLoad;
			this.isLoading = true;
		}

		private void Start()
		{
			this.webView = base.GetComponentInChildren<CharacterWebView>(true);
			this.companionMultiPanel = UnityEngine.Object.FindObjectOfType<CompanionMultiPanel>();
			Transform transforms = this.companionMultiPanel.transform.Find("CompanionNavigationBar");
			float single = 0f;
			Rect rect = (transforms.Find("CharacterImagePanel").transform as RectTransform).rect;
			single += rect.height;
			this.TopMargin = Mathf.FloorToInt(single * transforms.GetComponentInParent<Canvas>().scaleFactor);
			this.hamburgerMenuButton = transforms.Find("HamburgerButtonHolder").Find("HamburgerMenuButton").GetComponent<Button>();
			this.backButton = UnityEngine.Object.Instantiate<GameObject>(this.hamburgerMenuButton.gameObject, this.hamburgerMenuButton.gameObject.transform.parent, false);
			(this.backButton.GetComponent<Button>().targetGraphic as Image).sprite = this.backArrowSprite;
			this.backButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
			this.backButton.GetComponent<Button>().onClick.AddListener(new UnityAction(this.DestroyPanel));
			this.backButton.SetActive(false);
			this.ShowWebView();
		}

		private void Update()
		{
			if (Time.timeSinceLevelLoad > this.startTime + (float)this.timeoutSeconds && this.isLoading)
			{
				this.OnWebViewLoaded(false);
			}
		}
	}
}