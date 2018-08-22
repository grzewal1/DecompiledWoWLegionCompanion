using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class BackButtonManager : MonoBehaviour
	{
		private Stack<BackButtonManager.BackAction> m_backActionStack;

		public BackButtonManager()
		{
		}

		private void Awake()
		{
			this.m_backActionStack = new Stack<BackButtonManager.BackAction>(10);
		}

		public BackButtonManager.BackAction PopBackAction()
		{
			return this.m_backActionStack.Pop();
		}

		public void PushBackAction(BackActionType backActionType, GameObject backActionTarget = null)
		{
			BackButtonManager.BackAction backAction = () => {
			};
			switch (backActionType)
			{
				case BackActionType.hideAllPopups:
				{
					backAction = () => {
						if (AllPopups.instance != null)
						{
							AllPopups.instance.HideAllPopups();
						}
					};
					break;
				}
				case BackActionType.hideSliderPanel:
				{
					backAction = () => {
						if (backActionTarget != null)
						{
							SliderPanel component = backActionTarget.GetComponent<SliderPanel>();
							if (component != null)
							{
								component.HideSliderPanel();
							}
						}
					};
					break;
				}
				case BackActionType.hideMissionResults:
				{
					break;
				}
				case BackActionType.hideMissionDialog:
				{
					backAction = new BackButtonManager.BackAction(Singleton<DialogFactory>.Instance.CloseMissionDialog);
					break;
				}
				case BackActionType.hideHamburgerMenu:
				{
					backAction = () => AllPopups.instance.HideHamburgerMenu();
					break;
				}
				default:
				{
					goto case BackActionType.hideMissionResults;
				}
			}
			this.PushBackAction(backAction);
		}

		public void PushBackAction(BackButtonManager.BackAction action)
		{
			this.m_backActionStack.Push(action);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (this.m_backActionStack.Count == 0)
				{
					return;
				}
				this.m_backActionStack.Peek()();
			}
		}

		public delegate void BackAction();
	}
}