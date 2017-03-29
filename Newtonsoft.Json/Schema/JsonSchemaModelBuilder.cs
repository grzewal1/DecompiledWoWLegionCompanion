using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaModelBuilder
	{
		private JsonSchemaNodeCollection _nodes = new JsonSchemaNodeCollection();

		private Dictionary<JsonSchemaNode, JsonSchemaModel> _nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();

		private JsonSchemaNode _node;

		public JsonSchemaModelBuilder()
		{
		}

		public void AddAdditionalProperties(JsonSchemaNode parentNode, JsonSchema schema)
		{
			parentNode.AdditionalProperties = this.AddSchema(parentNode.AdditionalProperties, schema);
		}

		public void AddItem(JsonSchemaNode parentNode, int index, JsonSchema schema)
		{
			JsonSchemaNode item;
			if (parentNode.Items.Count <= index)
			{
				item = null;
			}
			else
			{
				item = parentNode.Items[index];
			}
			JsonSchemaNode jsonSchemaNode = this.AddSchema(item, schema);
			if (parentNode.Items.Count > index)
			{
				parentNode.Items[index] = jsonSchemaNode;
			}
			else
			{
				parentNode.Items.Add(jsonSchemaNode);
			}
		}

		public void AddProperties(IDictionary<string, JsonSchema> source, IDictionary<string, JsonSchemaNode> target)
		{
			if (source != null)
			{
				IEnumerator<KeyValuePair<string, JsonSchema>> enumerator = source.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, JsonSchema> current = enumerator.Current;
						this.AddProperty(target, current.Key, current.Value);
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					enumerator.Dispose();
				}
			}
		}

		public void AddProperty(IDictionary<string, JsonSchemaNode> target, string propertyName, JsonSchema schema)
		{
			JsonSchemaNode jsonSchemaNode;
			target.TryGetValue(propertyName, out jsonSchemaNode);
			target[propertyName] = this.AddSchema(jsonSchemaNode, schema);
		}

		public JsonSchemaNode AddSchema(JsonSchemaNode existingNode, JsonSchema schema)
		{
			string id;
			if (existingNode == null)
			{
				id = JsonSchemaNode.GetId((IEnumerable<JsonSchema>)(new JsonSchema[] { schema }));
			}
			else
			{
				if (existingNode.Schemas.Contains(schema))
				{
					return existingNode;
				}
				id = JsonSchemaNode.GetId(existingNode.Schemas.Union<JsonSchema>((IEnumerable<JsonSchema>)(new JsonSchema[] { schema })));
			}
			if (this._nodes.Contains(id))
			{
				return this._nodes[id];
			}
			JsonSchemaNode jsonSchemaNode = (existingNode == null ? new JsonSchemaNode(schema) : existingNode.Combine(schema));
			this._nodes.Add(jsonSchemaNode);
			this.AddProperties(schema.Properties, jsonSchemaNode.Properties);
			this.AddProperties(schema.PatternProperties, jsonSchemaNode.PatternProperties);
			if (schema.Items != null)
			{
				for (int i = 0; i < schema.Items.Count; i++)
				{
					this.AddItem(jsonSchemaNode, i, schema.Items[i]);
				}
			}
			if (schema.AdditionalProperties != null)
			{
				this.AddAdditionalProperties(jsonSchemaNode, schema.AdditionalProperties);
			}
			if (schema.Extends != null)
			{
				jsonSchemaNode = this.AddSchema(jsonSchemaNode, schema.Extends);
			}
			return jsonSchemaNode;
		}

		public JsonSchemaModel Build(JsonSchema schema)
		{
			this._nodes = new JsonSchemaNodeCollection();
			this._node = this.AddSchema(null, schema);
			this._nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();
			return this.BuildNodeModel(this._node);
		}

		private JsonSchemaModel BuildNodeModel(JsonSchemaNode node)
		{
			JsonSchemaModel strs;
			if (this._nodeModels.TryGetValue(node, out strs))
			{
				return strs;
			}
			strs = JsonSchemaModel.Create(node.Schemas);
			this._nodeModels[node] = strs;
			foreach (KeyValuePair<string, JsonSchemaNode> property in node.Properties)
			{
				if (strs.Properties == null)
				{
					strs.Properties = new Dictionary<string, JsonSchemaModel>();
				}
				strs.Properties[property.Key] = this.BuildNodeModel(property.Value);
			}
			foreach (KeyValuePair<string, JsonSchemaNode> patternProperty in node.PatternProperties)
			{
				if (strs.PatternProperties == null)
				{
					strs.PatternProperties = new Dictionary<string, JsonSchemaModel>();
				}
				strs.PatternProperties[patternProperty.Key] = this.BuildNodeModel(patternProperty.Value);
			}
			for (int i = 0; i < node.Items.Count; i++)
			{
				if (strs.Items == null)
				{
					strs.Items = new List<JsonSchemaModel>();
				}
				strs.Items.Add(this.BuildNodeModel(node.Items[i]));
			}
			if (node.AdditionalProperties != null)
			{
				strs.AdditionalProperties = this.BuildNodeModel(node.AdditionalProperties);
			}
			return strs;
		}
	}
}