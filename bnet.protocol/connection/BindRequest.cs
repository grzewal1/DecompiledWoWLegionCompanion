using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.connection
{
	public class BindRequest : IProtoBuf
	{
		private List<uint> _ImportedServiceHash = new List<uint>();

		private List<BoundService> _ExportedService = new List<BoundService>();

		public List<BoundService> ExportedService
		{
			get
			{
				return this._ExportedService;
			}
			set
			{
				this._ExportedService = value;
			}
		}

		public int ExportedServiceCount
		{
			get
			{
				return this._ExportedService.Count;
			}
		}

		public List<BoundService> ExportedServiceList
		{
			get
			{
				return this._ExportedService;
			}
		}

		public List<uint> ImportedServiceHash
		{
			get
			{
				return this._ImportedServiceHash;
			}
			set
			{
				this._ImportedServiceHash = value;
			}
		}

		public int ImportedServiceHashCount
		{
			get
			{
				return this._ImportedServiceHash.Count;
			}
		}

		public List<uint> ImportedServiceHashList
		{
			get
			{
				return this._ImportedServiceHash;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public BindRequest()
		{
		}

		public void AddExportedService(BoundService val)
		{
			this._ExportedService.Add(val);
		}

		public void AddImportedServiceHash(uint val)
		{
			this._ImportedServiceHash.Add(val);
		}

		public void ClearExportedService()
		{
			this._ExportedService.Clear();
		}

		public void ClearImportedServiceHash()
		{
			this._ImportedServiceHash.Clear();
		}

		public void Deserialize(Stream stream)
		{
			BindRequest.Deserialize(stream, this);
		}

		public static BindRequest Deserialize(Stream stream, BindRequest instance)
		{
			return BindRequest.Deserialize(stream, instance, (long)-1);
		}

		public static BindRequest Deserialize(Stream stream, BindRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.ImportedServiceHash == null)
			{
				instance.ImportedServiceHash = new List<uint>();
			}
			if (instance.ExportedService == null)
			{
				instance.ExportedService = new List<BoundService>();
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
							long position = (long)ProtocolParser.ReadUInt32(stream);
							position += stream.Position;
							while (stream.Position < position)
							{
								instance.ImportedServiceHash.Add(binaryReader.ReadUInt32());
							}
							if (stream.Position != position)
							{
								throw new ProtocolBufferException("Read too many bytes in packed data");
							}
						}
						else if (num1 == 18)
						{
							instance.ExportedService.Add(BoundService.DeserializeLengthDelimited(stream));
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

		public static BindRequest DeserializeLengthDelimited(Stream stream)
		{
			BindRequest bindRequest = new BindRequest();
			BindRequest.DeserializeLengthDelimited(stream, bindRequest);
			return bindRequest;
		}

		public static BindRequest DeserializeLengthDelimited(Stream stream, BindRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return BindRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			BindRequest bindRequest = obj as BindRequest;
			if (bindRequest == null)
			{
				return false;
			}
			if (this.ImportedServiceHash.Count != bindRequest.ImportedServiceHash.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ImportedServiceHash.Count; i++)
			{
				if (!this.ImportedServiceHash[i].Equals(bindRequest.ImportedServiceHash[i]))
				{
					return false;
				}
			}
			if (this.ExportedService.Count != bindRequest.ExportedService.Count)
			{
				return false;
			}
			for (int j = 0; j < this.ExportedService.Count; j++)
			{
				if (!this.ExportedService[j].Equals(bindRequest.ExportedService[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (uint importedServiceHash in this.ImportedServiceHash)
			{
				hashCode ^= importedServiceHash.GetHashCode();
			}
			foreach (BoundService exportedService in this.ExportedService)
			{
				hashCode ^= exportedService.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.ImportedServiceHash.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint importedServiceHash in this.ImportedServiceHash)
				{
					num += 4;
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			if (this.ExportedService.Count > 0)
			{
				foreach (BoundService exportedService in this.ExportedService)
				{
					num++;
					uint serializedSize = exportedService.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static BindRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<BindRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			BindRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, BindRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.ImportedServiceHash.Count > 0)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, (uint)(4 * instance.ImportedServiceHash.Count));
				foreach (uint importedServiceHash in instance.ImportedServiceHash)
				{
					binaryWriter.Write(importedServiceHash);
				}
			}
			if (instance.ExportedService.Count > 0)
			{
				foreach (BoundService exportedService in instance.ExportedService)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, exportedService.GetSerializedSize());
					BoundService.Serialize(stream, exportedService);
				}
			}
		}

		public void SetExportedService(List<BoundService> val)
		{
			this.ExportedService = val;
		}

		public void SetImportedServiceHash(List<uint> val)
		{
			this.ImportedServiceHash = val;
		}
	}
}