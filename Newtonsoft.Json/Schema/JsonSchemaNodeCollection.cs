using System;
using System.Collections.ObjectModel;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaNodeCollection : KeyedCollection<string, JsonSchemaNode>
	{
		public JsonSchemaNodeCollection()
		{
		}

		protected override string GetKeyForItem(JsonSchemaNode item)
		{
			return item.Id;
		}
	}
}