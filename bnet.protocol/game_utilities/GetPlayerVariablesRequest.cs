using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_utilities
{
	public class GetPlayerVariablesRequest : IProtoBuf
	{
		private List<bnet.protocol.game_utilities.PlayerVariables> _PlayerVariables = new List<bnet.protocol.game_utilities.PlayerVariables>();

		public bool HasHost;

		private ProcessId _Host;

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<bnet.protocol.game_utilities.PlayerVariables> PlayerVariables
		{
			get
			{
				return this._PlayerVariables;
			}
			set
			{
				this._PlayerVariables = value;
			}
		}

		public int PlayerVariablesCount
		{
			get
			{
				return this._PlayerVariables.Count;
			}
		}

		public List<bnet.protocol.game_utilities.PlayerVariables> PlayerVariablesList
		{
			get
			{
				return this._PlayerVariables;
			}
		}

		public GetPlayerVariablesRequest()
		{
		}

		public void AddPlayerVariables(bnet.protocol.game_utilities.PlayerVariables val)
		{
			this._PlayerVariables.Add(val);
		}

		public void ClearPlayerVariables()
		{
			this._PlayerVariables.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetPlayerVariablesRequest.Deserialize(stream, this);
		}

		public static GetPlayerVariablesRequest Deserialize(Stream stream, GetPlayerVariablesRequest instance)
		{
			return GetPlayerVariablesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetPlayerVariablesRequest Deserialize(Stream stream, GetPlayerVariablesRequest instance, long limit)
		{
			if (instance.PlayerVariables == null)
			{
				instance.PlayerVariables = new List<bnet.protocol.game_utilities.PlayerVariables>();
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
							instance.PlayerVariables.Add(bnet.protocol.game_utilities.PlayerVariables.DeserializeLengthDelimited(stream));
						}
						else if (num1 != 18)
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

		public static GetPlayerVariablesRequest DeserializeLengthDelimited(Stream stream)
		{
			GetPlayerVariablesRequest getPlayerVariablesRequest = new GetPlayerVariablesRequest();
			GetPlayerVariablesRequest.DeserializeLengthDelimited(stream, getPlayerVariablesRequest);
			return getPlayerVariablesRequest;
		}

		public static GetPlayerVariablesRequest DeserializeLengthDelimited(Stream stream, GetPlayerVariablesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetPlayerVariablesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetPlayerVariablesRequest getPlayerVariablesRequest = obj as GetPlayerVariablesRequest;
			if (getPlayerVariablesRequest == null)
			{
				return false;
			}
			if (this.PlayerVariables.Count != getPlayerVariablesRequest.PlayerVariables.Count)
			{
				return false;
			}
			for (int i = 0; i < this.PlayerVariables.Count; i++)
			{
				if (!this.PlayerVariables[i].Equals(getPlayerVariablesRequest.PlayerVariables[i]))
				{
					return false;
				}
			}
			if (this.HasHost == getPlayerVariablesRequest.HasHost && (!this.HasHost || this.Host.Equals(getPlayerVariablesRequest.Host)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.game_utilities.PlayerVariables playerVariable in this.PlayerVariables)
			{
				hashCode = hashCode ^ playerVariable.GetHashCode();
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
			if (this.PlayerVariables.Count > 0)
			{
				foreach (bnet.protocol.game_utilities.PlayerVariables playerVariable in this.PlayerVariables)
				{
					num++;
					uint serializedSize = playerVariable.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasHost)
			{
				num++;
				uint serializedSize1 = this.Host.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			return num;
		}

		public static GetPlayerVariablesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetPlayerVariablesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetPlayerVariablesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetPlayerVariablesRequest instance)
		{
			if (instance.PlayerVariables.Count > 0)
			{
				foreach (bnet.protocol.game_utilities.PlayerVariables playerVariable in instance.PlayerVariables)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, playerVariable.GetSerializedSize());
					bnet.protocol.game_utilities.PlayerVariables.Serialize(stream, playerVariable);
				}
			}
			if (instance.HasHost)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.Host.GetSerializedSize());
				ProcessId.Serialize(stream, instance.Host);
			}
		}

		public void SetHost(ProcessId val)
		{
			this.Host = val;
		}

		public void SetPlayerVariables(List<bnet.protocol.game_utilities.PlayerVariables> val)
		{
			this.PlayerVariables = val;
		}
	}
}