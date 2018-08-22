using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public class DialogFactory : Singleton<DialogFactory>
	{
		private readonly static string MainCanvasName;

		private readonly static string GameCanvasName;

		private readonly static string Level2CanvasName;

		private readonly static string Level3CanvasName;

		private Canvas _mainCanvas;

		private Canvas _gameCanvas;

		private Canvas _level2Canvas;

		private Canvas _level3Canvas;

		public BaseDialog m_baseDialogPrefab;

		public OKCancelDialog m_okCancelDialogPrefab;

		public MissionDialog m_missionDialogPrefab;

		private MissionDialog m_missionDialog;

		public ActivationConfirmationDialog m_activationConfirmationDialogPrefab;

		public DeactivationConfirmationDialog m_deactivationConfirmationDialogPrefab;

		public MissionTypeDialog m_missionTypeDialogPrefab;

		public AppSettingsDialog m_appSettingsDialogPrefab;

		public CharacterViewPanel m_characterViewPanelPrefab;

		private Canvas GameCanvas
		{
			get
			{
				if (this._gameCanvas == null)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag(DialogFactory.GameCanvasName);
					if (gameObject != null)
					{
						this._gameCanvas = gameObject.GetComponent<Canvas>();
					}
				}
				return this._gameCanvas;
			}
		}

		private Canvas Level2Canvas
		{
			get
			{
				if (this._level2Canvas == null)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag(DialogFactory.Level2CanvasName);
					if (gameObject != null)
					{
						this._level2Canvas = gameObject.GetComponent<Canvas>();
					}
				}
				return this._level2Canvas;
			}
		}

		private Canvas Level3Canvas
		{
			get
			{
				if (this._level3Canvas == null)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag(DialogFactory.Level3CanvasName);
					if (gameObject != null)
					{
						this._level3Canvas = gameObject.GetComponent<Canvas>();
					}
				}
				return this._level3Canvas;
			}
		}

		private Canvas MainCanvas
		{
			get
			{
				if (this._mainCanvas == null)
				{
					this._mainCanvas = GameObject.FindGameObjectWithTag(DialogFactory.MainCanvasName).GetComponent<Canvas>();
				}
				return this._mainCanvas;
			}
		}

		static DialogFactory()
		{
			DialogFactory.MainCanvasName = "MainCanvas";
			DialogFactory.GameCanvasName = "GameCanvas";
			DialogFactory.Level2CanvasName = "Level2Canvas";
			DialogFactory.Level3CanvasName = "Level3Canvas";
		}

		public DialogFactory()
		{
		}

		public void CloseMissionDialog()
		{
			if (this.m_missionDialog == null)
			{
				return;
			}
			this.m_missionDialog.gameObject.SetActive(false);
		}

		public AppSettingsDialog CreateAppSettingsDialog()
		{
			Canvas level3Canvas = this.Level3Canvas ?? this.MainCanvas;
			AppSettingsDialog appSettingsDialog = UnityEngine.Object.Instantiate<AppSettingsDialog>(this.m_appSettingsDialogPrefab, level3Canvas.transform, false);
			appSettingsDialog.gameObject.name = "AppSettingsDialog";
			appSettingsDialog.gameObject.SetActive(true);
			TiledRandomTexture[] componentsInChildren = this.m_appSettingsDialogPrefab.GetComponentsInChildren<TiledRandomTexture>();
			TiledRandomTexture[] tiledRandomTextureArray = appSettingsDialog.GetComponentsInChildren<TiledRandomTexture>();
			for (int i = 0; i < (int)tiledRandomTextureArray.Length; i++)
			{
				TiledRandomTexture tiledRandomTexture = tiledRandomTextureArray[i];
				RectTransform rectTransform = tiledRandomTexture.transform as RectTransform;
				Rect rect = rectTransform.rect;
				TiledRandomTexture tiledRandomTexture1 = componentsInChildren.FirstOrDefault<TiledRandomTexture>((TiledRandomTexture tex) => tex.gameObject.name == tiledRandomTexture.gameObject.name);
				if (tiledRandomTexture1 != null)
				{
					RectTransform rectTransform1 = tiledRandomTexture1.transform as RectTransform;
					rectTransform.localScale = rectTransform1.localScale;
					rectTransform.anchoredPosition = rectTransform1.anchoredPosition;
				}
			}
			return appSettingsDialog;
		}

		public ActivationConfirmationDialog CreateChampionActivationConfirmationDialog(FollowerDetailView followerDetailView)
		{
			ActivationConfirmationDialog activationConfirmationDialog = UnityEngine.Object.Instantiate<ActivationConfirmationDialog>(this.m_activationConfirmationDialogPrefab, this.GameCanvas.transform, false);
			activationConfirmationDialog.gameObject.name = "ActivationConfirmationDialog";
			activationConfirmationDialog.Show(followerDetailView);
			return activationConfirmationDialog;
		}

		public DeactivationConfirmationDialog CreateChampionDeactivationConfirmationDialog(FollowerDetailView followerDetailView)
		{
			DeactivationConfirmationDialog deactivationConfirmationDialog = UnityEngine.Object.Instantiate<DeactivationConfirmationDialog>(this.m_deactivationConfirmationDialogPrefab, this.GameCanvas.transform, false);
			deactivationConfirmationDialog.gameObject.name = "DeactivationConfirmationDialog";
			deactivationConfirmationDialog.Show(followerDetailView);
			return deactivationConfirmationDialog;
		}

		public CharacterViewPanel CreateCharacterViewPanel()
		{
			CharacterViewPanel characterViewPanel = UnityEngine.Object.Instantiate<CharacterViewPanel>(this.m_characterViewPanelPrefab, this.Level3Canvas.transform, false);
			return characterViewPanel;
		}

		public MissionDialog CreateMissionDialog(int missionID)
		{
			if (this.m_missionDialog == null)
			{
				this.m_missionDialog = UnityEngine.Object.Instantiate<MissionDialog>(this.m_missionDialogPrefab, this.GameCanvas.GetComponentInChildren<GamePanel>().m_missionListPanel.gameObject.transform, false);
				this.m_missionDialog.gameObject.name = "MissionDialog";
			}
			this.m_missionDialog.gameObject.SetActive(true);
			this.m_missionDialog.SetMission(missionID);
			return this.m_missionDialog;
		}

		public MissionTypeDialog CreateMissionTypeDialog()
		{
			MissionTypeDialog missionTypeDialog = UnityEngine.Object.Instantiate<MissionTypeDialog>(this.m_missionTypeDialogPrefab, this.Level3Canvas.transform, false);
			missionTypeDialog.gameObject.name = "MissionTypeDialog";
			return missionTypeDialog;
		}

		public OKCancelDialog CreateOKCancelDialog(string titleKey, string messageKey, Action onOK, Action onCancel = null)
		{
			Canvas level3Canvas = this.Level3Canvas ?? this.MainCanvas;
			OKCancelDialog oKCancelDialog = UnityEngine.Object.Instantiate<OKCancelDialog>(this.m_okCancelDialogPrefab, level3Canvas.transform, false);
			oKCancelDialog.gameObject.name = "OKCancelDialog";
			oKCancelDialog.SetText(titleKey, messageKey);
			oKCancelDialog.onOK += onOK;
			if (onCancel != null)
			{
				oKCancelDialog.onCancel += onCancel;
			}
			return oKCancelDialog;
		}
	}
}