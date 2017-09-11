using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class SelectGameAccountRequest : IProtoBuf
	{
		public EntityId GameAccount
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

		public SelectGameAccountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			SelectGameAccountRequest.Deserialize(stream, this);
		}

		public static SelectGameAccountRequest Deserialize(Stream stream, SelectGameAccountRequest instance)
		{
			return SelectGameAccountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SelectGameAccountRequest Deserialize(Stream stream, SelectGameAccountRequest instance, long limit)
		{
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.GameAccount != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.GameAccount);
					}
					else
					{
						instance.GameAccount = EntityId.DeserializeLengthDelimited(stream);
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

		public static SelectGameAccountRequest DeserializeLengthDelimited(Stream stream)
		{
			SelectGameAccountRequest selectGameAccountRequest = new SelectGameAccountRequest();
			SelectGameAccountRequest.DeserializeLengthDelimited(stream, selectGameAccountRequest);
			return selectGameAccountRequest;
		}

		public static SelectGameAccountRequest DeserializeLengthDelimited(Stream stream, SelectGameAccountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SelectGameAccountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SelectGameAccountRequest selectGameAccountRequest = obj as SelectGameAccountRequest;
			if (selectGameAccountRequest == null)
			{
				return false;
			}
			if (!this.GameAccount.Equals(selectGameAccountRequest.GameAccount))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.GameAccount.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameAccount.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static SelectGameAccountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SelectGameAccountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SelectGameAccountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SelectGameAccountRequest instance)
		{
			if (instance.GameAccount == null)
			{
				throw new ArgumentNullException("GameAccount", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameAccount.GetSerializedSize());
			EntityId.Serialize(stream, instance.GameAccount);
		}

		public void SetGameAccount(EntityId val)
		{
			this.GameAccount = val;
		}
	}
}