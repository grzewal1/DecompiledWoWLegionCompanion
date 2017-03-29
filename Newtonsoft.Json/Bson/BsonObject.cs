using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Bson
{
	internal class BsonObject : BsonToken, IEnumerable, IEnumerable<BsonProperty>
	{
		private readonly List<BsonProperty> _children = new List<BsonProperty>();

		public override BsonType Type
		{
			get
			{
				return BsonType.Object;
			}
		}

		public BsonObject()
		{
		}

		public void Add(string name, BsonToken token)
		{
			List<BsonProperty> bsonProperties = this._children;
			BsonProperty bsonProperty = new BsonProperty()
			{
				Name = new BsonString(name, false),
				Value = token
			};
			bsonProperties.Add(bsonProperty);
			token.Parent = this;
		}

		public IEnumerator<BsonProperty> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}