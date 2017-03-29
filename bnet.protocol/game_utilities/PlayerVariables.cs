using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_utilities
{
	public class PlayerVariables : IProtoBuf
	{
		public bool HasRating;

		private double _Rating;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public bnet.protocol.Identity Identity
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

		public double Rating
		{
			get
			{
				return this._Rating;
			}
			set
			{
				this._Rating = value;
				this.HasRating = true;
			}
		}

		public PlayerVariables()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void Deserialize(Stream stream)
		{
			PlayerVariables.Deserialize(stream, this);
		}

		public static PlayerVariables Deserialize(Stream stream, PlayerVariables instance)
		{
			return PlayerVariables.Deserialize(stream, instance, (long)-1);
		}

		public static PlayerVariables Deserialize(Stream stream, PlayerVariables instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
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
							if (instance.Identity != null)
							{
								bnet.protocol.Identity.DeserializeLengthDelimited(stream, instance.Identity);
							}
							else
							{
								instance.Identity = bnet.protocol.Identity.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 17)
						{
							instance.Rating = binaryReader.ReadDouble();
						}
						else if (num1 == 26)
						{
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
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

		public static PlayerVariables DeserializeLengthDelimited(Stream stream)
		{
			PlayerVariables playerVariable = new PlayerVariables();
			PlayerVariables.DeserializeLengthDelimited(stream, playerVariable);
			return playerVariable;
		}

		public static PlayerVariables DeserializeLengthDelimited(Stream stream, PlayerVariables instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return PlayerVariables.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			PlayerVariables playerVariable = obj as PlayerVariables;
			if (playerVariable == null)
			{
				return false;
			}
			if (!this.Identity.Equals(playerVariable.Identity))
			{
				return false;
			}
			if (this.HasRating != playerVariable.HasRating || this.HasRating && !this.Rating.Equals(playerVariable.Rating))
			{
				return false;
			}
			if (this.Attribute.Count != playerVariable.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(playerVariable.Attribute[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Identity.GetHashCode();
			if (this.HasRating)
			{
				hashCode = hashCode ^ this.Rating.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Identity.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasRating)
			{
				num++;
				num = num + 8;
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static PlayerVariables ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PlayerVariables>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PlayerVariables.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PlayerVariables instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Identity == null)
			{
				throw new ArgumentNullException("Identity", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Identity.GetSerializedSize());
			bnet.protocol.Identity.Serialize(stream, instance.Identity);
			if (instance.HasRating)
			{
				stream.WriteByte(17);
				binaryWriter.Write(instance.Rating);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetIdentity(bnet.protocol.Identity val)
		{
			this.Identity = val;
		}

		public void SetRating(double val)
		{
			this.Rating = val;
		}
	}
}