using bgs;
using JamLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class MobileNetwork
{
	public byte[] m_byteArray;

	public BufferedStream m_bufferedStream;

	private TcpConnection m_connection = new TcpConnection();

	private const int BUFFER_SIZE = 524288;

	public uint m_messageLength;

	public int m_messageCount;

	private static LogThreadHelper s_log;

	public bool IsConnected
	{
		get
		{
			return (this.m_connection.Socket == null ? false : this.m_connection.Socket.Connected);
		}
	}

	static MobileNetwork()
	{
		MobileNetwork.s_log = new LogThreadHelper("MobileNetwork");
	}

	public MobileNetwork()
	{
	}

	private void AddOutput(string text)
	{
		UnityEngine.Debug.Log(text);
	}

	private void AddOutput(object obj)
	{
		UnityEngine.Debug.Log(obj.ToString());
	}

	public bool ConnectAsync(string serverAddress, int serverPort)
	{
		this.m_connection.LogDebug = new Action<string>(MobileNetwork.s_log.LogDebug);
		this.m_connection.LogWarning = new Action<string>(MobileNetwork.s_log.LogWarning);
		this.m_connection.OnFailure = new Action(this.ConnectFailedCallback);
		this.m_connection.OnSuccess = new Action(this.ConnectCallback);
		this.m_connection.Connect(serverAddress, serverPort);
		this.InitSocketReadState();
		return true;
	}

	private void ConnectCallback()
	{
		this.SocketStartReceiving();
		this.OnConnectionStateChanged();
	}

	private void ConnectFailedCallback()
	{
		this.OnConnectionStateChanged();
	}

	private void ConnectionLost()
	{
		if (this.m_connection.Socket != null)
		{
			this.m_connection.Disconnect();
			this.SocketStopReceiving();
			if (this.ServerConnectionLostEventHandler != null)
			{
				this.ServerConnectionLostEventHandler(this, EventArgs.Empty);
			}
			this.OnConnectionStateChanged();
		}
	}

	public ulong ConvertDateTimeToTimeT(DateTime dt)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		if (dt <= dateTime)
		{
			return (ulong)0;
		}
		return (ulong)Math.Floor((dt - dateTime).TotalSeconds);
	}

	public DateTime ConvertTimeTToDateTime(ulong timet)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		if (timet <= (long)0)
		{
			return dateTime;
		}
		return dateTime.AddSeconds((double)((float)timet));
	}

	public object Deserialize(string msg)
	{
		object obj;
		try
		{
			obj = MessageFactory.Deserialize(msg);
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			this.AddOutput("~~~~~~~~~~~~~~~~~~~~~~~~~");
			this.AddOutput("FAILED TO PARSE JSON CODE");
			this.AddOutput("~~~~~~~~~~~~~~~~~~~~~~~~~");
			this.AddOutput(msg);
			this.AddOutput("~~~~~~~~~~~~~~~~~~~~~~~~~");
			this.AddOutput(exception);
			this.AddOutput("~~~~~~~~~~~~~~~~~~~~~~~~~");
			obj = null;
		}
		return obj;
	}

	public bool Disconnect()
	{
		if (this.m_connection.Socket == null)
		{
			return false;
		}
		this.m_connection.Disconnect();
		this.SocketStopReceiving();
		this.OnConnectionStateChanged();
		return true;
	}

	private void InitSocketReadState()
	{
		if (this.m_byteArray == null)
		{
			this.m_byteArray = new byte[524288];
		}
		if (this.m_bufferedStream == null)
		{
			this.m_bufferedStream = new BufferedStream(new MemoryStream());
		}
	}

	public object InputFactory(Type type, object oldMessage)
	{
		if (type == null)
		{
			return null;
		}
		object obj = type.GetConstructor(Type.EmptyTypes).Invoke(null);
		if (oldMessage == null)
		{
			return obj;
		}
		return obj;
	}

	protected void OnConnectionStateChanged()
	{
		if (this.ConnectionStateChanged != null)
		{
			this.ConnectionStateChanged(this, EventArgs.Empty);
		}
	}

	protected void OnMessageReceived(object msg, int count)
	{
		if (this.MessageReceivedEventHandler != null)
		{
			MobileNetwork.MobileNetworkEventArgs mobileNetworkEventArg = new MobileNetwork.MobileNetworkEventArgs()
			{
				Count = count
			};
			this.MessageReceivedEventHandler(msg, mobileNetworkEventArg);
		}
	}

	protected void OnUnknownMessageReceived(string msg)
	{
		if (this.UnknownMessageReceivedEventHandler != null)
		{
			this.UnknownMessageReceivedEventHandler(msg, EventArgs.Empty);
		}
	}

	public static void Process()
	{
		MobileNetwork.s_log.Process();
	}

	private void ProcessSocketData()
	{
		unsafe
		{
			while (this.m_bufferedStream.Length > (long)4)
			{
				if (this.m_messageLength == 0)
				{
					long position = this.m_bufferedStream.Position;
					byte[] numArray = new byte[4];
					this.m_bufferedStream.Seek((long)0, SeekOrigin.Begin);
					this.m_bufferedStream.Read(numArray, 0, 4);
					this.m_messageLength = (uint)(numArray[0] << 24 | numArray[1] << 16 | numArray[2] << 8 | numArray[3]);
					this.m_bufferedStream.Seek(position, SeekOrigin.Begin);
				}
				if (this.m_messageLength <= 0 || this.m_bufferedStream.Length < (ulong)(this.m_messageLength + 4))
				{
					break;
				}
				else
				{
					this.m_bufferedStream.Seek((long)4, SeekOrigin.Begin);
					byte[] numArray1 = new byte[this.m_messageLength];
					this.m_bufferedStream.Read(numArray1, 0, (Int32)this.m_messageLength);
					string str = Encoding.UTF8.GetString(numArray1);
					object obj = this.Deserialize(str);
					if (obj == null)
					{
						this.OnUnknownMessageReceived(str);
					}
					else
					{
						this.m_messageCount++;
						this.OnMessageReceived(obj, this.m_messageCount);
					}
					BufferedStream bufferedStream = new BufferedStream(new MemoryStream());
					while (this.m_bufferedStream.Position < this.m_bufferedStream.Length)
					{
						int num = this.m_bufferedStream.Read(this.m_byteArray, 0, (int)this.m_byteArray.Length);
						bufferedStream.Write(this.m_byteArray, 0, num);
					}
					this.m_bufferedStream = bufferedStream;
					this.m_messageLength = 0;
				}
			}
		}
	}

	public void SendMessage(object obj)
	{
		this.SendStringMessage(MessageFactory.Serialize(obj));
	}

	public void SendStringMessage(string message)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			byte[] length = new byte[] { (byte)((int)bytes.Length >> 24 & 255), (byte)((int)bytes.Length >> 16 & 255), (byte)((int)bytes.Length >> 8 & 255), (byte)((int)bytes.Length & 255) };
			if (!this.IsConnected)
			{
				UnityEngine.Debug.Log("SendStringMessage(): Connection lost.");
				this.ConnectionLost();
			}
			else
			{
				this.m_connection.Socket.Send(length);
				this.m_connection.Socket.Send(bytes);
			}
		}
		catch (SocketException socketException1)
		{
			SocketException socketException = socketException1;
			UnityEngine.Debug.Log(string.Concat("SendStringMessage() exception: ", socketException.ToString()));
			if (socketException.ErrorCode != 10058)
			{
				UnityEngine.Debug.Log("SendStringMessage(): Connection lost in exception.");
				this.ConnectionLost();
			}
			else
			{
				this.ServerDisconnected();
			}
		}
	}

	private void ServerDisconnected()
	{
		if (this.m_connection.Socket == null)
		{
			return;
		}
		this.m_connection.Disconnect();
		this.SocketStopReceiving();
		if (this.ServerDisconnectedEventHandler != null)
		{
			this.ServerDisconnectedEventHandler(this, EventArgs.Empty);
		}
	}

	private void SocketReadCallback(IAsyncResult asyncResult)
	{
		int num = this.m_connection.Socket.EndReceive(asyncResult);
		this.m_bufferedStream.Write(this.m_byteArray, 0, num);
		this.ProcessSocketData();
		this.SocketStartReceiving();
	}

	private void SocketStartReceiving()
	{
		this.m_connection.Socket.BeginReceive(this.m_byteArray, 0, 524288, SocketFlags.None, new AsyncCallback(this.SocketReadCallback), null);
	}

	private void SocketStopReceiving()
	{
		this.m_bufferedStream.Seek((long)0, SeekOrigin.Begin);
	}

	public event EventHandler<EventArgs> ConnectionStateChanged
	{
		add
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> connectionStateChanged = this.ConnectionStateChanged;
			do
			{
				eventHandler = connectionStateChanged;
				connectionStateChanged = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ConnectionStateChanged, (EventHandler<EventArgs>)Delegate.Combine(eventHandler, value), connectionStateChanged);
			}
			while ((object)connectionStateChanged != (object)eventHandler);
		}
		remove
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> connectionStateChanged = this.ConnectionStateChanged;
			do
			{
				eventHandler = connectionStateChanged;
				connectionStateChanged = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ConnectionStateChanged, (EventHandler<EventArgs>)Delegate.Remove(eventHandler, value), connectionStateChanged);
			}
			while ((object)connectionStateChanged != (object)eventHandler);
		}
	}

	public event EventHandler<MobileNetwork.MobileNetworkEventArgs> MessageReceivedEventHandler
	{
		add
		{
			EventHandler<MobileNetwork.MobileNetworkEventArgs> eventHandler;
			EventHandler<MobileNetwork.MobileNetworkEventArgs> messageReceivedEventHandler = this.MessageReceivedEventHandler;
			do
			{
				eventHandler = messageReceivedEventHandler;
				messageReceivedEventHandler = Interlocked.CompareExchange<EventHandler<MobileNetwork.MobileNetworkEventArgs>>(ref this.MessageReceivedEventHandler, (EventHandler<MobileNetwork.MobileNetworkEventArgs>)Delegate.Combine(eventHandler, value), messageReceivedEventHandler);
			}
			while ((object)messageReceivedEventHandler != (object)eventHandler);
		}
		remove
		{
			EventHandler<MobileNetwork.MobileNetworkEventArgs> eventHandler;
			EventHandler<MobileNetwork.MobileNetworkEventArgs> messageReceivedEventHandler = this.MessageReceivedEventHandler;
			do
			{
				eventHandler = messageReceivedEventHandler;
				messageReceivedEventHandler = Interlocked.CompareExchange<EventHandler<MobileNetwork.MobileNetworkEventArgs>>(ref this.MessageReceivedEventHandler, (EventHandler<MobileNetwork.MobileNetworkEventArgs>)Delegate.Remove(eventHandler, value), messageReceivedEventHandler);
			}
			while ((object)messageReceivedEventHandler != (object)eventHandler);
		}
	}

	public event EventHandler<EventArgs> ServerConnectionLostEventHandler
	{
		add
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> serverConnectionLostEventHandler = this.ServerConnectionLostEventHandler;
			do
			{
				eventHandler = serverConnectionLostEventHandler;
				serverConnectionLostEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ServerConnectionLostEventHandler, (EventHandler<EventArgs>)Delegate.Combine(eventHandler, value), serverConnectionLostEventHandler);
			}
			while ((object)serverConnectionLostEventHandler != (object)eventHandler);
		}
		remove
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> serverConnectionLostEventHandler = this.ServerConnectionLostEventHandler;
			do
			{
				eventHandler = serverConnectionLostEventHandler;
				serverConnectionLostEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ServerConnectionLostEventHandler, (EventHandler<EventArgs>)Delegate.Remove(eventHandler, value), serverConnectionLostEventHandler);
			}
			while ((object)serverConnectionLostEventHandler != (object)eventHandler);
		}
	}

	public event EventHandler<EventArgs> ServerDisconnectedEventHandler
	{
		add
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> serverDisconnectedEventHandler = this.ServerDisconnectedEventHandler;
			do
			{
				eventHandler = serverDisconnectedEventHandler;
				serverDisconnectedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ServerDisconnectedEventHandler, (EventHandler<EventArgs>)Delegate.Combine(eventHandler, value), serverDisconnectedEventHandler);
			}
			while ((object)serverDisconnectedEventHandler != (object)eventHandler);
		}
		remove
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> serverDisconnectedEventHandler = this.ServerDisconnectedEventHandler;
			do
			{
				eventHandler = serverDisconnectedEventHandler;
				serverDisconnectedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.ServerDisconnectedEventHandler, (EventHandler<EventArgs>)Delegate.Remove(eventHandler, value), serverDisconnectedEventHandler);
			}
			while ((object)serverDisconnectedEventHandler != (object)eventHandler);
		}
	}

	public event EventHandler<EventArgs> UnknownMessageReceivedEventHandler
	{
		add
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> unknownMessageReceivedEventHandler = this.UnknownMessageReceivedEventHandler;
			do
			{
				eventHandler = unknownMessageReceivedEventHandler;
				unknownMessageReceivedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.UnknownMessageReceivedEventHandler, (EventHandler<EventArgs>)Delegate.Combine(eventHandler, value), unknownMessageReceivedEventHandler);
			}
			while ((object)unknownMessageReceivedEventHandler != (object)eventHandler);
		}
		remove
		{
			EventHandler<EventArgs> eventHandler;
			EventHandler<EventArgs> unknownMessageReceivedEventHandler = this.UnknownMessageReceivedEventHandler;
			do
			{
				eventHandler = unknownMessageReceivedEventHandler;
				unknownMessageReceivedEventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.UnknownMessageReceivedEventHandler, (EventHandler<EventArgs>)Delegate.Remove(eventHandler, value), unknownMessageReceivedEventHandler);
			}
			while ((object)unknownMessageReceivedEventHandler != (object)eventHandler);
		}
	}

	public class MobileNetworkEventArgs : EventArgs
	{
		public int Count
		{
			get;
			set;
		}

		public MobileNetworkEventArgs()
		{
			this.Count = 0;
		}
	}
}