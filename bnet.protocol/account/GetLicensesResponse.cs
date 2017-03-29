using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GetLicensesResponse : IProtoBuf
	{
		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<AccountLicense> Licenses
		{
			get
			{
				return this._Licenses;
			}
			set
			{
				this._Licenses = value;
			}
		}

		public int LicensesCount
		{
			get
			{
				return this._Licenses.Count;
			}
		}

		public List<AccountLicense> LicensesList
		{
			get
			{
				return this._Licenses;
			}
		}

		public GetLicensesResponse()
		{
		}

		public void AddLicenses(AccountLicense val)
		{
			this._Licenses.Add(val);
		}

		public void ClearLicenses()
		{
			this._Licenses.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetLicensesResponse.Deserialize(stream, this);
		}

		public static GetLicensesResponse Deserialize(Stream stream, GetLicensesResponse instance)
		{
			return GetLicensesResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetLicensesResponse Deserialize(Stream stream, GetLicensesResponse instance, long limit)
		{
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
					else if (num == 10)
					{
						instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
				}
				else
				{
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static GetLicensesResponse DeserializeLengthDelimited(Stream stream)
		{
			GetLicensesResponse getLicensesResponse = new GetLicensesResponse();
			GetLicensesResponse.DeserializeLengthDelimited(stream, getLicensesResponse);
			return getLicensesResponse;
		}

		public static GetLicensesResponse DeserializeLengthDelimited(Stream stream, GetLicensesResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetLicensesResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetLicensesResponse getLicensesResponse = obj as GetLicensesResponse;
			if (getLicensesResponse == null)
			{
				return false;
			}
			if (this.Licenses.Count != getLicensesResponse.Licenses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Licenses.Count; i++)
			{
				if (!this.Licenses[i].Equals(getLicensesResponse.Licenses[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode = hashCode ^ license.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num++;
					uint serializedSize = license.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GetLicensesResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetLicensesResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetLicensesResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetLicensesResponse instance)
		{
			if (instance.Licenses.Count > 0)
			{
				foreach (AccountLicense license in instance.Licenses)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, license.GetSerializedSize());
					AccountLicense.Serialize(stream, license);
				}
			}
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}
	}
}