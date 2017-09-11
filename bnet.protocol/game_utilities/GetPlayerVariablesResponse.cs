using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_utilities
{
	public class GetPlayerVariablesResponse : IProtoBuf
	{
		private List<bnet.protocol.game_utilities.PlayerVariables> _PlayerVariables = new List<bnet.protocol.game_utilities.PlayerVariables>();

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

		public GetPlayerVariablesResponse()
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
			GetPlayerVariablesResponse.Deserialize(stream, this);
		}

		public static GetPlayerVariablesResponse Deserialize(Stream stream, GetPlayerVariablesResponse instance)
		{
			return GetPlayerVariablesResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetPlayerVariablesResponse Deserialize(Stream stream, GetPlayerVariablesResponse instance, long limit)
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
						instance.PlayerVariables.Add(bnet.protocol.game_utilities.PlayerVariables.DeserializeLengthDelimited(stream));
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

		public static GetPlayerVariablesResponse DeserializeLengthDelimited(Stream stream)
		{
			GetPlayerVariablesResponse getPlayerVariablesResponse = new GetPlayerVariablesResponse();
			GetPlayerVariablesResponse.DeserializeLengthDelimited(stream, getPlayerVariablesResponse);
			return getPlayerVariablesResponse;
		}

		public static GetPlayerVariablesResponse DeserializeLengthDelimited(Stream stream, GetPlayerVariablesResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetPlayerVariablesResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetPlayerVariablesResponse getPlayerVariablesResponse = obj as GetPlayerVariablesResponse;
			if (getPlayerVariablesResponse == null)
			{
				return false;
			}
			if (this.PlayerVariables.Count != getPlayerVariablesResponse.PlayerVariables.Count)
			{
				return false;
			}
			for (int i = 0; i < this.PlayerVariables.Count; i++)
			{
				if (!this.PlayerVariables[i].Equals(getPlayerVariablesResponse.PlayerVariables[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.game_utilities.PlayerVariables playerVariable in this.PlayerVariables)
			{
				hashCode ^= playerVariable.GetHashCode();
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
			return num;
		}

		public static GetPlayerVariablesResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetPlayerVariablesResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetPlayerVariablesResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetPlayerVariablesResponse instance)
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
		}

		public void SetPlayerVariables(List<bnet.protocol.game_utilities.PlayerVariables> val)
		{
			this.PlayerVariables = val;
		}
	}
}