using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol
{
	public class Path : IProtoBuf
	{
		private List<uint> _Ordinal = new List<uint>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<uint> Ordinal
		{
			get
			{
				return this._Ordinal;
			}
			set
			{
				this._Ordinal = value;
			}
		}

		public int OrdinalCount
		{
			get
			{
				return this._Ordinal.Count;
			}
		}

		public List<uint> OrdinalList
		{
			get
			{
				return this._Ordinal;
			}
		}

		public Path()
		{
		}

		public void AddOrdinal(uint val)
		{
			this._Ordinal.Add(val);
		}

		public void ClearOrdinal()
		{
			this._Ordinal.Clear();
		}

		public void Deserialize(Stream stream)
		{
			bnet.protocol.Path.Deserialize(stream, this);
		}

		public static bnet.protocol.Path Deserialize(Stream stream, bnet.protocol.Path instance)
		{
			return bnet.protocol.Path.Deserialize(stream, instance, (long)-1);
		}

		public static bnet.protocol.Path Deserialize(Stream stream, bnet.protocol.Path instance, long limit)
		{
			if (instance.Ordinal == null)
			{
				instance.Ordinal = new List<uint>();
			}
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
						long position = (long)ProtocolParser.ReadUInt32(stream);
						position += stream.Position;
						while (stream.Position < position)
						{
							instance.Ordinal.Add(ProtocolParser.ReadUInt32(stream));
						}
						if (stream.Position != position)
						{
							throw new ProtocolBufferException("Read too many bytes in packed data");
						}
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

		public static bnet.protocol.Path DeserializeLengthDelimited(Stream stream)
		{
			bnet.protocol.Path path = new bnet.protocol.Path();
			bnet.protocol.Path.DeserializeLengthDelimited(stream, path);
			return path;
		}

		public static bnet.protocol.Path DeserializeLengthDelimited(Stream stream, bnet.protocol.Path instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return bnet.protocol.Path.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			bnet.protocol.Path path = obj as bnet.protocol.Path;
			if (path == null)
			{
				return false;
			}
			if (this.Ordinal.Count != path.Ordinal.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Ordinal.Count; i++)
			{
				if (!this.Ordinal[i].Equals(path.Ordinal[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (uint ordinal in this.Ordinal)
			{
				hashCode ^= ordinal.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Ordinal.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint ordinal in this.Ordinal)
				{
					num += ProtocolParser.SizeOfUInt32(ordinal);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			return num;
		}

		public static bnet.protocol.Path ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<bnet.protocol.Path>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			bnet.protocol.Path.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, bnet.protocol.Path instance)
		{
			if (instance.Ordinal.Count > 0)
			{
				stream.WriteByte(10);
				uint num = 0;
				foreach (uint ordinal in instance.Ordinal)
				{
					num += ProtocolParser.SizeOfUInt32(ordinal);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint ordinal1 in instance.Ordinal)
				{
					ProtocolParser.WriteUInt32(stream, ordinal1);
				}
			}
		}

		public void SetOrdinal(List<uint> val)
		{
			this.Ordinal = val;
		}
	}
}