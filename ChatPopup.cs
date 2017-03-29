using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopup : MonoBehaviour
{
	public Text conversationText;

	public InputField textToSend;

	public ChatPopup()
	{
	}

	public void OnReceiveText(string sender, string text)
	{
		Text text1 = this.conversationText;
		string str = text1.text;
		text1.text = string.Concat(new string[] { str, "[", sender, "]: ", text, "\n" });
	}

	public void OnSendText()
	{
		if (this.textToSend.text.Length == 0)
		{
			return;
		}
		Main.instance.SendGuildChat(this.textToSend.text);
		this.textToSend.text = string.Empty;
		this.textToSend.Select();
		this.textToSend.ActivateInputField();
	}

	private void Start()
	{
		TouchScreenKeyboard.hideInput = true;
	}

	private void Update()
	{
		Vector3 vector3;
		if (!TouchScreenKeyboard.visible)
		{
			base.transform.localPosition = Vector3.zero;
		}
		else
		{
			RectTransform component = this.textToSend.gameObject.GetComponent<RectTransform>();
			Rect rect = TouchScreenKeyboard.area;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(component, rect.max, null, out vector3);
			Transform transforms = base.transform;
			float single = base.transform.position.x;
			float single1 = vector3.y;
			Vector3 vector31 = base.transform.position;
			transforms.position = new Vector3(single, single1, vector31.z);
		}
		TouchScreenKeyboard.hideInput = true;
	}
}