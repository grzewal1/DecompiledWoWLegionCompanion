using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class OKCancelDialog : MonoBehaviour
	{
		public Text m_titleText;

		public Text m_messageText;

		public OKCancelDialog()
		{
		}

		private void Awake()
		{
			base.transform.Find("PopupView").localScale = new Vector3(0.8f, 0.8f, 1f);
		}

		public void OnClickCancel()
		{
			this.onCancel();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void OnClickOkay()
		{
			this.onOK();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PushBackAction(new BackButtonManager.BackAction(this.OnClickCancel));
		}

		public void SetText(string titleKey, string messageKey = null)
		{
			this.m_titleText.text = StaticDB.GetString(titleKey, null);
			if (messageKey == null)
			{
				this.m_messageText.text = string.Empty;
			}
			else
			{
				this.m_messageText.text = StaticDB.GetString(messageKey, null);
			}
		}

		public event Action onCancel
		{
			add
			{
				Action action;
				Action action1 = this.onCancel;
				do
				{
					action = action1;
					action1 = Interlocked.CompareExchange<Action>(ref this.onCancel, (Action)Delegate.Combine(action, value), action1);
				}
				while ((object)action1 != (object)action);
			}
			remove
			{
				Action action;
				Action action1 = this.onCancel;
				do
				{
					action = action1;
					action1 = Interlocked.CompareExchange<Action>(ref this.onCancel, (Action)Delegate.Remove(action, value), action1);
				}
				while ((object)action1 != (object)action);
			}
		}

		public event Action onOK
		{
			add
			{
				Action action;
				Action action1 = this.onOK;
				do
				{
					action = action1;
					action1 = Interlocked.CompareExchange<Action>(ref this.onOK, (Action)Delegate.Combine(action, value), action1);
				}
				while ((object)action1 != (object)action);
			}
			remove
			{
				Action action;
				Action action1 = this.onOK;
				do
				{
					action = action1;
					action1 = Interlocked.CompareExchange<Action>(ref this.onOK, (Action)Delegate.Remove(action, value), action1);
				}
				while ((object)action1 != (object)action);
			}
		}
	}
}