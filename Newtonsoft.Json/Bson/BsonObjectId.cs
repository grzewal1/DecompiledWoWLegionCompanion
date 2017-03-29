using Newtonsoft.Json.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Bson
{
	public class BsonObjectId
	{
		public byte[] Value
		{
			get;
			private set;
		}

		public BsonObjectId(byte[] value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if ((int)value.Length != 12)
			{
				throw new Exception("An ObjectId must be 12 bytes");
			}
			this.Value = value;
		}
	}
}