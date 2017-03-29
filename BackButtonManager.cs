using System;
using System.Collections.Generic;
using UnityEngine;
using WowStatConstants;

public class BackButtonManager : MonoBehaviour
{
	private Stack<BackActionData> m_backActionStack;

	public BackButtonManager()
	{
	}

	private void Awake()
	{
		this.m_backActionStack = new Stack<BackActionData>(10);
	}

	public BackActionData PopBackAction()
	{
		return this.m_backActionStack.Pop();
	}

	public void PushBackAction(BackAction backAction, GameObject backActionTarget = null)
	{
		BackActionData backActionDatum = new BackActionData()
		{
			m_backAction = backAction,
			m_backActionTarget = backActionTarget
		};
		this.m_backActionStack.Push(backActionDatum);
	}

	private void Update()
	{
		if (Input.GetKeyDown("escape"))
		{
			if (this.m_backActionStack.Count == 0)
			{
				return;
			}
			BackActionData backActionDatum = this.m_backActionStack.Peek();
			switch (backActionDatum.m_backAction)
			{
				case BackAction.hideAllPopups:
				{
					AllPopups.instance.HideAllPopups();
					break;
				}
				case BackAction.hideSliderPanel:
				{
					if (backActionDatum.m_backActionTarget != null)
					{
						SliderPanel component = backActionDatum.m_backActionTarget.GetComponent<SliderPanel>();
						if (component != null)
						{
							component.HideSliderPanel();
						}
					}
					break;
				}
				case BackAction.hideMissionResults:
				{
					AllPanels.instance.m_missionResultsPanel.HideMissionResults();
					break;
				}
				case BackAction.hideMissionDialog:
				{
					AllPopups.instance.m_missionDialog.m_missionDetailView.HideMissionDetailView();
					break;
				}
			}
		}
	}
}