using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.authentication
{
	public class GameAccountSelectedRequest : IProtoBuf
	{
		public bool HasGameAccount;

		private EntityId _GameAccount;

		public EntityId GameAccount
		{
			get
			{
				return this._GameAccount;
			}
			set
			{
				this._GameAccount = value;
				this.HasGameAccount = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Result
		{
			get;
			set;
		}

		public GameAccountSelectedRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameAccountSelectedRequest.Deserialize(stream, this);
		}

		public static GameAccountSelectedRequest Deserialize(Stream stream, GameAccountSelectedRequest instance)
		{
			return GameAccountSelectedRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GameAccountSelectedRequest Deserialize(Stream stream, GameAccountSelectedRequest instance, long limit)
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
					else if (num == 8)
					{
						instance.Result = ProtocolParser.ReadUInt32(stream);
					}
					else if (num != 18)
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

		public static GameAccountSelectedRequest DeserializeLengthDelimited(Stream stream)
		{
			GameAccountSelectedRequest gameAccountSelectedRequest = new GameAccountSelectedRequest();
			GameAccountSelectedRequest.DeserializeLengthDelimited(stream, gameAccountSelectedRequest);
			return gameAccountSelectedRequest;
		}

		public static GameAccountSelectedRequest DeserializeLengthDelimited(Stream stream, GameAccountSelectedRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameAccountSelectedRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameAccountSelectedRequest gameAccountSelectedRequest = obj as GameAccountSelectedRequest;
			if (gameAccountSelectedRequest == null)
			{
				return false;
			}
			if (!this.Result.Equals(gameAccountSelectedRequest.Result))
			{
				return false;
			}
			if (this.HasGameAccount == gameAccountSelectedRequest.HasGameAccount && (!this.HasGameAccount || this.GameAccount.Equals(gameAccountSelectedRequest.GameAccount)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Result.GetHashCode();
			if (this.HasGameAccount)
			{
				hashCode ^= this.GameAccount.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.Result);
			if (this.HasGameAccount)
			{
				num++;
				uint serializedSize = this.GameAccount.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			num++;
			return num;
		}

		public static GameAccountSelectedRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameAccountSelectedRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameAccountSelectedRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameAccountSelectedRequest instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.Result);
			if (instance.HasGameAccount)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccount.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccount);
			}
		}

		public void SetGameAccount(EntityId val)
		{
			this.GameAccount = val;
		}

		public void SetResult(uint val)
		{
			this.Result = val;
		}
	}
}