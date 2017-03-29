using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

public class RegionConfirmation : MonoBehaviour
{
	public Text m_restartText;

	public Text m_sureText;

	public Text m_okayText;

	public Text m_cancelText;

	public RegionConfirmation()
	{
	}

	public void OnClickCancel()
	{
		AllPanels.instance.CancelRegionIndex();
		AllPopups.instance.HideAllPopups();
	}

	public void OnClickOkay()
	{
		AllPanels.instance.SetRegionIndex();
		AllPopups.instance.HideAllPopups();
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PopBackAction();
	}

	private void OnEnable()
	{
		this.m_restartText.text = StaticDB.GetString("RESTART_REQUIRED", "Restart Required");
		this.m_sureText.text = StaticDB.GetString("ARE_YOU_SURE", null);
		this.m_okayText.text = StaticDB.GetString("OK", null);
		this.m_cancelText.text = StaticDB.GetString("CANCEL", null);
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}
}