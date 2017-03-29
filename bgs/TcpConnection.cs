using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class TcpConnection
	{
		private System.Net.Sockets.Socket m_socket;

		private Queue<IPAddress> m_candidateIPAddresses;

		private IPAddress m_resolvedIPAddress;

		public Action<string> LogDebug = new Action<string>((string argument0) => {
		});

		public Action<string> LogWarning = new Action<string>((string argument1) => {
		});

		public Action OnFailure = new Action(() => {
		});

		public Action OnSuccess = new Action(() => {
		});

		public string Host
		{
			get;
			private set;
		}

		public int Port
		{
			get;
			private set;
		}

		public IPAddress ResolvedAddress
		{
			get
			{
				return this.m_resolvedIPAddress;
			}
		}

		public System.Net.Sockets.Socket Socket
		{
			get
			{
				return this.m_socket;
			}
		}

		public TcpConnection()
		{
		}

		public void Connect(string host, int port)
		{
			IPAddress pAddress;
			this.LogWarning(string.Format("TcpConnection - Connecting to host: {0}, port: {1}", host, port));
			this.Host = host;
			this.Port = port;
			this.m_candidateIPAddresses = new Queue<IPAddress>();
			if (IPAddress.TryParse(this.Host, out pAddress))
			{
				this.m_candidateIPAddresses.Enqueue(pAddress);
			}
			try
			{
				Dns.BeginGetHostByName(this.Host, new AsyncCallback(this.GetHostEntryCallback), null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LogWarning(string.Format("TcpConnection - Connect() failed, could not get host entry. ip: {0}, port: {1}, exception: {2}", this.Host, this.Port, exception.Message));
				this.OnFailure();
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			Exception exception = null;
			try
			{
				this.m_socket.EndConnect(ar);
			}
			catch (Exception exception1)
			{
				exception = exception1;
			}
			if (exception != null || !this.m_socket.Connected)
			{
				this.LogDebug(string.Format("TcpConnection - EndConnect() failed. ip: {0}, port: {1}, af: {2}, exception: {3}", new object[] { this.m_resolvedIPAddress, this.Port, this.m_resolvedIPAddress.AddressFamily, exception.Message }));
				this.ConnectInternal();
			}
			else
			{
				this.LogDebug(string.Format("TcpConnection - Connected to ip: {0}, port: {1}, af: {2}", this.m_resolvedIPAddress, this.Port, this.m_resolvedIPAddress.AddressFamily));
				this.OnSuccess();
			}
		}

		private void ConnectInternal()
		{
			this.LogDebug(string.Format("TcpConnection - ConnectInternal. address-count: {0}", this.m_candidateIPAddresses.Count));
			this.Disconnect();
			if (this.m_candidateIPAddresses.Count == 0)
			{
				this.LogWarning(string.Format("TcpConnection - Could not connect to ip: {0}, port: {1}", this.Host, this.Port));
				this.OnFailure();
				return;
			}
			this.m_resolvedIPAddress = this.m_candidateIPAddresses.Dequeue();
			IPEndPoint pEndPoint = new IPEndPoint(this.m_resolvedIPAddress, this.Port);
			this.LogDebug(string.Format("TcpConnection - Create Socket with ip: {0}, port: {1}, af: {2}", this.m_resolvedIPAddress, this.Port, this.m_resolvedIPAddress.AddressFamily));
			this.m_socket = new System.Net.Sockets.Socket(this.m_resolvedIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				this.m_socket.BeginConnect(pEndPoint, new AsyncCallback(this.ConnectCallback), null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LogDebug(string.Format("TcpConnection - BeginConnect() failed. ip: {0}, port: {1}, af: {2}, exception: {3}", new object[] { this.m_resolvedIPAddress, this.Port, this.m_resolvedIPAddress.AddressFamily, exception.Message }));
				this.ConnectInternal();
			}
		}

		public void Disconnect()
		{
			if (this.m_socket == null)
			{
				return;
			}
			if (this.m_socket.Connected)
			{
				try
				{
					this.m_socket.Shutdown(SocketShutdown.Both);
					this.m_socket.Close();
				}
				catch (SocketException socketException)
				{
					this.LogWarning(string.Format("TcpConnection.Disconnect() - SocketException: {0}", socketException.Message));
				}
			}
			this.m_socket = null;
		}

		private void GetHostEntryCallback(IAsyncResult ar)
		{
			IPHostEntry pHostEntry = Dns.EndGetHostByName(ar);
			Array.Sort<IPAddress>(pHostEntry.AddressList, (IPAddress x, IPAddress y) => {
				if (x.AddressFamily < y.AddressFamily)
				{
					return -1;
				}
				if (x.AddressFamily > y.AddressFamily)
				{
					return 1;
				}
				return 0;
			});
			IPAddress[] addressList = pHostEntry.AddressList;
			for (int i = 0; i < (int)addressList.Length; i++)
			{
				IPAddress pAddress = addressList[i];
				this.m_candidateIPAddresses.Enqueue(pAddress);
			}
			this.ConnectInternal();
		}

		public bool MatchSslCertName(IEnumerable<string> certNames)
		{
			bool flag;
			IPHostEntry hostEntry = Dns.GetHostEntry(this.Host);
			IEnumerator<string> enumerator = certNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (!current.StartsWith("::ffff:"))
					{
						continue;
					}
					string str = current.Substring("::ffff:".Length);
					IPAddress[] addressList = Dns.GetHostEntry(str).AddressList;
					for (int i = 0; i < (int)addressList.Length; i++)
					{
						IPAddress pAddress = addressList[i];
						IPAddress[] pAddressArray = hostEntry.AddressList;
						int num = 0;
						while (num < (int)pAddressArray.Length)
						{
							if (!pAddressArray[num].Equals(pAddress))
							{
								num++;
							}
							else
							{
								flag = true;
								return flag;
							}
						}
					}
				}
				goto Label0;
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return flag;
		Label0:
			string str1 = string.Format("TcpConnection - MatchSslCertName failed.", new object[0]);
			IEnumerator<string> enumerator1 = certNames.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					string current1 = enumerator1.Current;
					str1 = string.Concat(str1, string.Format("\n\t certName: {0}", current1));
				}
			}
			finally
			{
				if (enumerator1 == null)
				{
				}
				enumerator1.Dispose();
			}
			IPAddress[] addressList1 = hostEntry.AddressList;
			for (int j = 0; j < (int)addressList1.Length; j++)
			{
				IPAddress pAddress1 = addressList1[j];
				str1 = string.Concat(str1, string.Format("\n\t hostAddress: {0}", pAddress1));
			}
			this.LogWarning(str1);
			return false;
		}
	}
}