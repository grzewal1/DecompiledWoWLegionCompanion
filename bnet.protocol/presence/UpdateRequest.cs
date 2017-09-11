using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class UpdateRequest : IProtoBuf
	{
		private List<bnet.protocol.presence.FieldOperation> _FieldOperation = new List<bnet.protocol.presence.FieldOperation>();

		public bnet.protocol.EntityId EntityId
		{
			get;
			set;
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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public UpdateRequest()
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
			UpdateRequest.Deserialize(stream, this);
		}

		public static UpdateRequest Deserialize(Stream stream, UpdateRequest instance)
		{
			return UpdateRequest.Deserialize(stream, instance, (long)-1);
		}

		public static UpdateRequest Deserialize(Stream stream, UpdateRequest instance, long limit)
		{
			if (instance.FieldOperation == null)
			{
				instance.FieldOperation = new List<bnet.protocol.presence.FieldOperation>();
			}
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
							if (num1 == 18)
							{
								instance.FieldOperation.Add(bnet.protocol.presence.FieldOperation.DeserializeLengthDelimited(stream));
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

		public static UpdateRequest DeserializeLengthDelimited(Stream stream)
		{
			UpdateRequest updateRequest = new UpdateRequest();
			UpdateRequest.DeserializeLengthDelimited(stream, updateRequest);
			return updateRequest;
		}

		public static UpdateRequest DeserializeLengthDelimited(Stream stream, UpdateRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return UpdateRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			UpdateRequest updateRequest = obj as UpdateRequest;
			if (updateRequest == null)
			{
				return false;
			}
			if (!this.EntityId.Equals(updateRequest.EntityId))
			{
				return false;
			}
			if (this.FieldOperation.Count != updateRequest.FieldOperation.Count)
			{
				return false;
			}
			for (int i = 0; i < this.FieldOperation.Count; i++)
			{
				if (!this.FieldOperation[i].Equals(updateRequest.FieldOperation[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.EntityId.GetHashCode();
			foreach (bnet.protocol.presence.FieldOperation fieldOperation in this.FieldOperation)
			{
				hashCode ^= fieldOperation.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.EntityId.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.FieldOperation.Count > 0)
			{
				foreach (bnet.protocol.presence.FieldOperation fieldOperation in this.FieldOperation)
				{
					num++;
					uint serializedSize1 = fieldOperation.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static UpdateRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<UpdateRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			UpdateRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, UpdateRequest instance)
		{
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			if (instance.FieldOperation.Count > 0)
			{
				foreach (bnet.protocol.presence.FieldOperation fieldOperation in instance.FieldOperation)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, fieldOperation.GetSerializedSize());
					bnet.protocol.presence.FieldOperation.Serialize(stream, fieldOperation);
				}
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
	}
}