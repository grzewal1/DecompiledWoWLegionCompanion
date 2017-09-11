using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.channel
{
	public class DissolveRequest : IProtoBuf
	{
		public bool HasAgentId;

		private EntityId _AgentId;

		public bool HasReason;

		private uint _Reason;

		public EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
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

		public DissolveRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			DissolveRequest.Deserialize(stream, this);
		}

		public static DissolveRequest Deserialize(Stream stream, DissolveRequest instance)
		{
			return DissolveRequest.Deserialize(stream, instance, (long)-1);
		}

		public static DissolveRequest Deserialize(Stream stream, DissolveRequest instance, long limit)
		{
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
						else if (instance.AgentId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
						}
						else
						{
							instance.AgentId = EntityId.DeserializeLengthDelimited(stream);
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

		public static DissolveRequest DeserializeLengthDelimited(Stream stream)
		{
			DissolveRequest dissolveRequest = new DissolveRequest();
			DissolveRequest.DeserializeLengthDelimited(stream, dissolveRequest);
			return dissolveRequest;
		}

		public static DissolveRequest DeserializeLengthDelimited(Stream stream, DissolveRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return DissolveRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			DissolveRequest dissolveRequest = obj as DissolveRequest;
			if (dissolveRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != dissolveRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(dissolveRequest.AgentId))
			{
				return false;
			}
			if (this.HasReason == dissolveRequest.HasReason && (!this.HasReason || this.Reason.Equals(dissolveRequest.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode ^= this.AgentId.GetHashCode();
			}
			if (this.HasReason)
			{
				hashCode ^= this.Reason.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasReason)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Reason);
			}
			return num;
		}

		public static DissolveRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<DissolveRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			DissolveRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, DissolveRequest instance)
		{
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.HasReason)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Reason);
			}
		}

		public void SetAgentId(EntityId val)
		{
			this.AgentId = val;
		}

		public void SetReason(uint val)
		{
			this.Reason = val;
		}
	}
}