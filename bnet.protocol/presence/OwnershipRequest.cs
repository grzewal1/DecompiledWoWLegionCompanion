using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class OwnershipRequest : IProtoBuf
	{
		public bool HasReleaseOwnership;

		private bool _ReleaseOwnership;

		public bnet.protocol.EntityId EntityId
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

		public bool ReleaseOwnership
		{
			get
			{
				return this._ReleaseOwnership;
			}
			set
			{
				this._ReleaseOwnership = value;
				this.HasReleaseOwnership = true;
			}
		}

		public OwnershipRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			OwnershipRequest.Deserialize(stream, this);
		}

		public static OwnershipRequest Deserialize(Stream stream, OwnershipRequest instance)
		{
			return OwnershipRequest.Deserialize(stream, instance, (long)-1);
		}

		public static OwnershipRequest Deserialize(Stream stream, OwnershipRequest instance, long limit)
		{
			instance.ReleaseOwnership = false;
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
						if (num == 16)
						{
							instance.ReleaseOwnership = ProtocolParser.ReadBool(stream);
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
					else if (instance.EntityId != null)
					{
						bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
					}
					else
					{
						instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
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

		public static OwnershipRequest DeserializeLengthDelimited(Stream stream)
		{
			OwnershipRequest ownershipRequest = new OwnershipRequest();
			OwnershipRequest.DeserializeLengthDelimited(stream, ownershipRequest);
			return ownershipRequest;
		}

		public static OwnershipRequest DeserializeLengthDelimited(Stream stream, OwnershipRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return OwnershipRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			OwnershipRequest ownershipRequest = obj as OwnershipRequest;
			if (ownershipRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(ownershipRequest.EntityId))
			{
				return false;
			}
			if (this.HasReleaseOwnership == ownershipRequest.HasReleaseOwnership && (!this.HasReleaseOwnership || this.ReleaseOwnership.Equals(ownershipRequest.ReleaseOwnership)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.EntityId.GetHashCode();
			if (this.HasReleaseOwnership)
			{
				hashCode ^= this.ReleaseOwnership.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.EntityId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasReleaseOwnership)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static OwnershipRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<OwnershipRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			OwnershipRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, OwnershipRequest instance)
		{
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			if (instance.HasReleaseOwnership)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.ReleaseOwnership);
			}
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetReleaseOwnership(bool val)
		{
			this.ReleaseOwnership = val;
		}
	}
}