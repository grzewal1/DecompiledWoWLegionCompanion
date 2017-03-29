using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_utilities
{
	public class PresenceChannelCreatedRequest : IProtoBuf
	{
		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public bool HasBnetAccountId;

		private EntityId _BnetAccountId;

		public bool HasHost;

		private ProcessId _Host;

		public EntityId BnetAccountId
		{
			get
			{
				return this._BnetAccountId;
			}
			set
			{
				this._BnetAccountId = value;
				this.HasBnetAccountId = value != null;
			}
		}

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

		public ProcessId Host
		{
			get
			{
				return this._Host;
			}
			set
			{
				this._Host = value;
				this.HasHost = value != null;
			}
		}

		public EntityId Id
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

		public PresenceChannelCreatedRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			PresenceChannelCreatedRequest.Deserialize(stream, this);
		}

		public static PresenceChannelCreatedRequest Deserialize(Stream stream, PresenceChannelCreatedRequest instance)
		{
			return PresenceChannelCreatedRequest.Deserialize(stream, instance, (long)-1);
		}

		public static PresenceChannelCreatedRequest Deserialize(Stream stream, PresenceChannelCreatedRequest instance, long limit)
		{
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
							if (instance.Id != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.Id);
							}
							else
							{
								instance.Id = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 26)
						{
							if (instance.GameAccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
							}
							else
							{
								instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 34)
						{
							if (instance.BnetAccountId != null)
							{
								EntityId.DeserializeLengthDelimited(stream, instance.BnetAccountId);
							}
							else
							{
								instance.BnetAccountId = EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 != 42)
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							if (key.Field == 0)
							{
								throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
							}
							ProtocolParser.SkipKey(stream, key);
						}
						else if (instance.Host != null)
						{
							ProcessId.DeserializeLengthDelimited(stream, instance.Host);
						}
						else
						{
							instance.Host = ProcessId.DeserializeLengthDelimited(stream);
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

		public static PresenceChannelCreatedRequest DeserializeLengthDelimited(Stream stream)
		{
			PresenceChannelCreatedRequest presenceChannelCreatedRequest = new PresenceChannelCreatedRequest();
			PresenceChannelCreatedRequest.DeserializeLengthDelimited(stream, presenceChannelCreatedRequest);
			return presenceChannelCreatedRequest;
		}

		public static PresenceChannelCreatedRequest DeserializeLengthDelimited(Stream stream, PresenceChannelCreatedRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return PresenceChannelCreatedRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			PresenceChannelCreatedRequest presenceChannelCreatedRequest = obj as PresenceChannelCreatedRequest;
			if (presenceChannelCreatedRequest == null)
			{
				return false;
			}
			if (!this.Id.Equals(presenceChannelCreatedRequest.Id))
			{
				return false;
			}
			if (this.HasGameAccountId != presenceChannelCreatedRequest.HasGameAccountId || this.HasGameAccountId && !this.GameAccountId.Equals(presenceChannelCreatedRequest.GameAccountId))
			{
				return false;
			}
			if (this.HasBnetAccountId != presenceChannelCreatedRequest.HasBnetAccountId || this.HasBnetAccountId && !this.BnetAccountId.Equals(presenceChannelCreatedRequest.BnetAccountId))
			{
				return false;
			}
			if (this.HasHost == presenceChannelCreatedRequest.HasHost && (!this.HasHost || this.Host.Equals(presenceChannelCreatedRequest.Host)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Id.GetHashCode();
			if (this.HasGameAccountId)
			{
				hashCode = hashCode ^ this.GameAccountId.GetHashCode();
			}
			if (this.HasBnetAccountId)
			{
				hashCode = hashCode ^ this.BnetAccountId.GetHashCode();
			}
			if (this.HasHost)
			{
				hashCode = hashCode ^ this.Host.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Id.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize1 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.HasBnetAccountId)
			{
				num++;
				uint num1 = this.BnetAccountId.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			if (this.HasHost)
			{
				num++;
				uint serializedSize2 = this.Host.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			num++;
			return num;
		}

		public static PresenceChannelCreatedRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PresenceChannelCreatedRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PresenceChannelCreatedRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PresenceChannelCreatedRequest instance)
		{
			if (instance.Id == null)
			{
				throw new ArgumentNullException("Id", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Id.GetSerializedSize());
			EntityId.Serialize(stream, instance.Id);
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
			if (instance.HasBnetAccountId)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.BnetAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.BnetAccountId);
			}
			if (instance.HasHost)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
				ProcessId.Serialize(stream, instance.Host);
			}
		}

		public void SetBnetAccountId(EntityId val)
		{
			this.BnetAccountId = val;
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}

		public void SetId(EntityId val)
		{
			this.Id = val;
		}
	}
}