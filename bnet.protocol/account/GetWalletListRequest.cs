using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.account
{
	public class GetWalletListRequest : IProtoBuf
	{
		public bool HasRefresh;

		private bool _Refresh;

		public bnet.protocol.account.AccountId AccountId
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool Refresh
		{
			get
			{
				return this._Refresh;
			}
			set
			{
				this._Refresh = value;
				this.HasRefresh = true;
			}
		}

		public GetWalletListRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetWalletListRequest.Deserialize(stream, this);
		}

		public static GetWalletListRequest Deserialize(Stream stream, GetWalletListRequest instance)
		{
			return GetWalletListRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetWalletListRequest Deserialize(Stream stream, GetWalletListRequest instance, long limit)
		{
			instance.Refresh = false;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 16)
							{
								instance.Refresh = ProtocolParser.ReadBool(stream);
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
						else if (instance.AccountId != null)
						{
							bnet.protocol.account.AccountId.DeserializeLengthDelimited(stream, instance.AccountId);
						}
						else
						{
							instance.AccountId = bnet.protocol.account.AccountId.DeserializeLengthDelimited(stream);
						}
					}
					else
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
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

		public static GetWalletListRequest DeserializeLengthDelimited(Stream stream)
		{
			GetWalletListRequest getWalletListRequest = new GetWalletListRequest();
			GetWalletListRequest.DeserializeLengthDelimited(stream, getWalletListRequest);
			return getWalletListRequest;
		}

		public static GetWalletListRequest DeserializeLengthDelimited(Stream stream, GetWalletListRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetWalletListRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetWalletListRequest getWalletListRequest = obj as GetWalletListRequest;
			if (getWalletListRequest == null)
			{
				return false;
			}
			if (!this.AccountId.Equals(getWalletListRequest.AccountId))
			{
				return false;
			}
			if (this.HasRefresh == getWalletListRequest.HasRefresh && (!this.HasRefresh || this.Refresh.Equals(getWalletListRequest.Refresh)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.AccountId.GetHashCode();
			if (this.HasRefresh)
			{
				hashCode = hashCode ^ this.Refresh.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.AccountId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasRefresh)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static GetWalletListRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetWalletListRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetWalletListRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetWalletListRequest instance)
		{
			if (instance.AccountId == null)
			{
				throw new ArgumentNullException("AccountId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.AccountId.GetSerializedSize());
			bnet.protocol.account.AccountId.Serialize(stream, instance.AccountId);
			if (instance.HasRefresh)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Refresh);
			}
		}

		public void SetAccountId(bnet.protocol.account.AccountId val)
		{
			this.AccountId = val;
		}

		public void SetRefresh(bool val)
		{
			this.Refresh = val;
		}
	}
}