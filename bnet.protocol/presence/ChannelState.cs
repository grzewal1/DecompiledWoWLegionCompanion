using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.presence
{
	public class ChannelState : IProtoBuf
	{
		public bool HasEntityId;

		private bnet.protocol.EntityId _EntityId;

		private List<bnet.protocol.presence.FieldOperation> _FieldOperation = new List<bnet.protocol.presence.FieldOperation>();

		public bool HasHealing;

		private bool _Healing;

		public bnet.protocol.EntityId EntityId
		{
			get
			{
				return this._EntityId;
			}
			set
			{
				this._EntityId = value;
				this.HasEntityId = value != null;
			}
		}

		public List<bnet.protocol.presence.FieldOperation> FieldOperation
		{
			get
			{
				return this._FieldOperation;
			}
			set
			{
				this._FieldOperation = value;
			}
		}

		public int FieldOperationCount
		{
			get
			{
				return this._FieldOperation.Count;
			}
		}

		public List<bnet.protocol.presence.FieldOperation> FieldOperationList
		{
			get
			{
				return this._FieldOperation;
			}
		}

		public bool Healing
		{
			get
			{
				return this._Healing;
			}
			set
			{
				this._Healing = value;
				this.HasHealing = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChannelState()
		{
		}

		public void AddFieldOperation(bnet.protocol.presence.FieldOperation val)
		{
			this._FieldOperation.Add(val);
		}

		public void ClearFieldOperation()
		{
			this._FieldOperation.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ChannelState.Deserialize(stream, this);
		}

		public static ChannelState Deserialize(Stream stream, ChannelState instance)
		{
			return ChannelState.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelState Deserialize(Stream stream, ChannelState instance, long limit)
		{
			if (instance.FieldOperation == null)
			{
				instance.FieldOperation = new List<bnet.protocol.presence.FieldOperation>();
			}
			instance.Healing = false;
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
						if (instance.EntityId != null)
						{
							bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
						}
						else
						{
							instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						instance.FieldOperation.Add(bnet.protocol.presence.FieldOperation.DeserializeLengthDelimited(stream));
					}
					else if (num == 24)
					{
						instance.Healing = ProtocolParser.ReadBool(stream);
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

		public static ChannelState DeserializeLengthDelimited(Stream stream)
		{
			ChannelState channelState = new ChannelState();
			ChannelState.DeserializeLengthDelimited(stream, channelState);
			return channelState;
		}

		public static ChannelState DeserializeLengthDelimited(Stream stream, ChannelState instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelState.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelState channelState = obj as ChannelState;
			if (channelState == null)
			{
				return false;
			}
			if (this.HasEntityId != channelState.HasEntityId || this.HasEntityId && !this.EntityId.Equals(channelState.EntityId))
			{
				return false;
			}
			if (this.FieldOperation.Count != channelState.FieldOperation.Count)
			{
				return false;
			}
			for (int i = 0; i < this.FieldOperation.Count; i++)
			{
				if (!this.FieldOperation[i].Equals(channelState.FieldOperation[i]))
				{
					return false;
				}
			}
			if (this.HasHealing == channelState.HasHealing && (!this.HasHealing || this.Healing.Equals(channelState.Healing)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasEntityId)
			{
				hashCode ^= this.EntityId.GetHashCode();
			}
			foreach (bnet.protocol.presence.FieldOperation fieldOperation in this.FieldOperation)
			{
				hashCode ^= fieldOperation.GetHashCode();
			}
			if (this.HasHealing)
			{
				hashCode ^= this.Healing.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasEntityId)
			{
				num++;
				uint serializedSize = this.EntityId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.FieldOperation.Count > 0)
			{
				foreach (bnet.protocol.presence.FieldOperation fieldOperation in this.FieldOperation)
				{
					num++;
					uint serializedSize1 = fieldOperation.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasHealing)
			{
				num++;
				num++;
			}
			return num;
		}

		public static ChannelState ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelState>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelState.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelState instance)
		{
			if (instance.HasEntityId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
				bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			}
			if (instance.FieldOperation.Count > 0)
			{
				foreach (bnet.protocol.presence.FieldOperation fieldOperation in instance.FieldOperation)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, fieldOperation.GetSerializedSize());
					bnet.protocol.presence.FieldOperation.Serialize(stream, fieldOperation);
				}
			}
			if (instance.HasHealing)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.Healing);
			}
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetFieldOperation(List<bnet.protocol.presence.FieldOperation> val)
		{
			this.FieldOperation = val;
		}

		public void SetHealing(bool val)
		{
			this.Healing = val;
		}
	}
}