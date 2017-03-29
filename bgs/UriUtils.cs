using System;
using System.Net;

namespace bgs
{
	public class UriUtils
	{
		public UriUtils()
		{
		}

		public static bool GetHostAddress(string hostName, out string address)
		{
			bool flag;
			if (UriUtils.GetHostAddressAsIp(hostName, out address))
			{
				return true;
			}
			try
			{
				if (!UriUtils.GetHostAddressByDns(hostName, out address))
				{
					return false;
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}

		public static bool GetHostAddressAsIp(string hostName, out string address)
		{
			IPAddress pAddress;
			address = string.Empty;
			if (!IPAddress.TryParse(hostName, out pAddress))
			{
				return false;
			}
			address = pAddress.ToString();
			return true;
		}

		public static bool GetHostAddressByDns(string hostName, out string address)
		{
			bool flag;
			address = string.Empty;
			try
			{
				IPAddress[] addressList = Dns.GetHostEntry(hostName).AddressList;
				int num = 0;
				if (num < (int)addressList.Length)
				{
					address = addressList[num].ToString();
					flag = true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}
	}
}