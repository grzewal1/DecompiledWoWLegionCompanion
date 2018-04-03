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
	}
}