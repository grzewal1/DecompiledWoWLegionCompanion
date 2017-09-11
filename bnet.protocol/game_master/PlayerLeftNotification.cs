using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class PlayerLeftNotification : IProtoBuf
	{
		public bool HasReason;

		private uint _Reason;

		public bnet.protocol.game_master.GameHandle GameHandle
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

		public EntityId MemberId
		{
			get;
			set;
		}

		public uint Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
				this.HasReason = true;
			}
		}

		public PlayerLeftNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			PlayerLeftNotification.Deserialize(stream, this);
		}

		public static PlayerLeftNotification Deserialize(Stream stream, PlayerLeftNotification instance)
		{
			return PlayerLeftNotification.Deserialize(stream, instance, (long)-1);
		}

		public static PlayerLeftNotification Deserialize(Stream stream, PlayerLeftNotification instance, long limit)
		{
			instance.Reason = 1;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							if (instance.GameHandle != null)
							{
								bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream, instance.GameHandle);
							}
							else
							{
								instance.GameHandle = bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 18)
						{
							if (num1 == 24)
							{
								instance.Reason = ProtocolParser.ReadUInt32(stream);
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
						else if (instance.MemberId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.MemberId);
						}
						else
						{
							instance.MemberId = EntityId.DeserializeLengthDelimited(stream);
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

		public static PlayerLeftNotification DeserializeLengthDelimited(Stream stream)
		{
			PlayerLeftNotification playerLeftNotification = new PlayerLeftNotification();
			PlayerLeftNotification.DeserializeLengthDelimited(stream, playerLeftNotification);
			return playerLeftNotification;
		}

		public static PlayerLeftNotification DeserializeLengthDelimited(Stream stream, PlayerLeftNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return PlayerLeftNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			PlayerLeftNotification playerLeftNotification = obj as PlayerLeftNotification;
			if (playerLeftNotification == null)
			{
				return false;
			}
			if (!this.GameHandle.Equals(playerLeftNotification.GameHandle))
			{
				return false;
			}
			if (!this.MemberId.Equals(playerLeftNotification.MemberId))
			{
				return false;
			}
			if (this.HasReason == playerLeftNotification.HasReason && (!this.HasReason || this.Reason.Equals(playerLeftNotification.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.GameHandle.GetHashCode();
			hashCode ^= this.MemberId.GetHashCode();
			if (this.HasReason)
			{
				hashCode ^= this.Reason.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameHandle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			uint serializedSize1 = this.MemberId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			num += 2;
			return num;
		}

		public static PlayerLeftNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PlayerLeftNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PlayerLeftNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PlayerLeftNotification instance)
		{
			if (instance.GameHandle == null)
			{
				throw new ArgumentNullException("GameHandle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameHandle.GetSerializedSize());
			bnet.protocol.game_master.GameHandle.Serialize(stream, instance.GameHandle);
			if (instance.MemberId == null)
			{
				throw new ArgumentNullException("MemberId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.MemberId.GetSerializedSize());
			EntityId.Serialize(stream, instance.MemberId);
			if (instance.HasReason)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetGameHandle(bnet.protocol.game_master.GameHandle val)
		{
			this.GameHandle = val;
		}

		public void SetMemberId(EntityId val)
		{
			this.MemberId = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}
	}
}