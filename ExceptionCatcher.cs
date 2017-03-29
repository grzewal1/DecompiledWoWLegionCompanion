using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class ExceptionCatcher : MonoBehaviour
{
	private const int WoWCompanionProjectID = 292;

	public ExceptionPanel exceptionPanel;

	private readonly static HashSet<string> sentReports;

	private IPAddress unknownAddress = new IPAddress(new byte[4]);

	private IPAddress ipAddress
	{
		get
		{
			try
			{
				IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
				int num = 0;
				while (num < (int)addressList.Length)
				{
					IPAddress pAddress = addressList[num];
					if (pAddress.AddressFamily != AddressFamily.InterNetwork)
					{
						num++;
					}
					else
					{
						return pAddress;
					}
				}
			}
			catch (SocketException socketException)
			{
			}
			catch (ArgumentException argumentException)
			{
			}
			return this.unknownAddress;
		}
	}

	private static string localTime
	{
		get
		{
			DateTime now = DateTime.Now;
			return now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
		}
	}

	private string SubmitURL
	{
		get
		{
			return string.Concat("http://iir.blizzard.com:3724/submit/", 292);
		}
	}

	static ExceptionCatcher()
	{
		ExceptionCatcher.sentReports = new HashSet<string>();
	}

	public ExceptionCatcher()
	{
	}

	private static bool AlreadySent(string hash)
	{
		return ExceptionCatcher.sentReports.Contains(hash);
	}

	private void Awake()
	{
	}

	private static string BuildMarkup(string title, string stackTrace, string hashBlock)
	{
		string str = ExceptionCatcher.CreateEscapedSGML(stackTrace);
		return string.Concat(new object[] { "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<ReportedIssue xmlns=\"http://schemas.datacontract.org/2004/07/Inspector.Models\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\n\t<Summary>", title, "</Summary>\n\t<Assertion>", str, "</Assertion>\n\t<HashBlock>", hashBlock, "</HashBlock>\n\t<BuildNumber>", BuildNum.CodeBuildNum, "</BuildNumber>\n\t<Module>WoW Legion Companion</Module>\n\t<EnteredBy>0</EnteredBy>\n\t<IssueType>Exception</IssueType>\n\t<ProjectId>", 292, "</ProjectId>\n\t<Metadata><NameValuePairs>\n\t\t<NameValuePair><Name>Build</Name><Value>", BuildNum.CodeBuildNum, "</Value></NameValuePair>\n\t\t<NameValuePair><Name>OS.Platform</Name><Value>", Application.platform, "</Value></NameValuePair>\n\t\t<NameValuePair><Name>Unity.Version</Name><Value>", Application.unityVersion, "</Value></NameValuePair>\n\t\t<NameValuePair><Name>Unity.Genuine</Name><Value>", Application.genuine, "</Value></NameValuePair>\n\t\t<NameValuePair><Name>Locale</Name><Value>", Main.instance.GetLocale(), "</Value></NameValuePair>\n\t</NameValuePairs></Metadata>\n</ReportedIssue>\n" });
	}

	private static string CreateEscapedSGML(string blob)
	{
		XmlElement xmlElement = (new XmlDocument()).CreateElement("root");
		xmlElement.InnerText = blob;
		return xmlElement.InnerXml;
	}

	private static string CreateHash(string blob)
	{
		return ExceptionCatcher.GetMd5Hash(MD5.Create(), blob);
	}

	private void ExceptionReporterCallback(string message, string stackTrace, LogType logType)
	{
		if (logType == LogType.Exception)
		{
			string str = ExceptionCatcher.CreateHash(string.Concat(message, stackTrace));
			if (!ExceptionCatcher.AlreadySent(str))
			{
				base.StartCoroutine(this.SendExceptionReport(message, stackTrace, str));
				ExceptionCatcher.sentReports.Add(str);
				return;
			}
		}
	}

	private static string GetMd5Hash(MD5 md5Hash, string input)
	{
		byte[] numArray = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < (int)numArray.Length; i++)
		{
			stringBuilder.Append(numArray[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private void OnApplicationQuit()
	{
		this.UnregisterExceptionReporter();
	}

	private void OnDisable()
	{
		this.UnregisterExceptionReporter();
	}

	private void OnEnable()
	{
		this.RegisterExceptionReporter();
	}

	public void RegisterExceptionReporter()
	{
		Application.logMessageReceived += new Application.LogCallback(this.ExceptionReporterCallback);
	}

	[DebuggerHidden]
	private IEnumerator SendExceptionReport(string message, string stackTrace, string hash)
	{
		ExceptionCatcher.<SendExceptionReport>c__IteratorC variable = null;
		return variable;
	}

	private void ShowCurrentException(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			this.exceptionPanel.gameObject.SetActive(true);
			UnityEngine.Debug.Log(string.Concat(condition, "\n", stackTrace));
			this.exceptionPanel.m_exceptionText.text = string.Concat(condition, "\n", stackTrace);
		}
	}

	private void Start()
	{
		this.RegisterExceptionReporter();
	}

	public void UnregisterExceptionReporter()
	{
		Application.logMessageReceived -= new Application.LogCallback(this.ExceptionReporterCallback);
	}

	private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
	{
		string str = ExceptionCatcher.GetMd5Hash(md5Hash, input);
		if (StringComparer.OrdinalIgnoreCase.Compare(str, hash) == 0)
		{
			return true;
		}
		return false;
	}
}