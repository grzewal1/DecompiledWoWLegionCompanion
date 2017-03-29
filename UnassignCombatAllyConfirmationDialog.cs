using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

public class UnassignCombatAllyConfirmationDialog : MonoBehaviour
{
	public Text m_areYouSureLabel;

	public Text m_cancelButtonLabel;

	public Text m_okButtonLabel;

	public CombatAllyListItem m_combatAllyListItem;

	public UnassignCombatAllyConfirmationDialog()
	{
	}

	public void ConfirmUnassign()
	{
		this.m_combatAllyListItem.UnassignCombatAlly();
		base.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PopBackAction();
	}

	private void OnEnable()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		this.m_areYouSureLabel.text = StaticDB.GetString("ARE_YOU_SURE", null);
		this.m_cancelButtonLabel.text = StaticDB.GetString("NO", null);
		this.m_okButtonLabel.text = StaticDB.GetString("YES_UNASSIGN", "Yes, Unassign!");
	}

	private void Start()
	{
		this.m_areYouSureLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_cancelButtonLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_okButtonLabel.font = GeneralHelpers.LoadStandardFont();
	}
}