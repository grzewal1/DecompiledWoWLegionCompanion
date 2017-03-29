using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaModel
	{
		public JsonSchemaModel AdditionalProperties
		{
			get;
			set;
		}

		public bool AllowAdditionalProperties
		{
			get;
			set;
		}

		public JsonSchemaType Disallow
		{
			get;
			set;
		}

		public double? DivisibleBy
		{
			get;
			set;
		}

		public IList<JToken> Enum
		{
			get;
			set;
		}

		public bool ExclusiveMaximum
		{
			get;
			set;
		}

		public bool ExclusiveMinimum
		{
			get;
			set;
		}

		public IList<JsonSchemaModel> Items
		{
			get;
			set;
		}

		public double? Maximum
		{
			get;
			set;
		}

		public int? MaximumItems
		{
			get;
			set;
		}

		public int? MaximumLength
		{
			get;
			set;
		}

		public double? Minimum
		{
			get;
			set;
		}

		public int? MinimumItems
		{
			get;
			set;
		}

		public int? MinimumLength
		{
			get;
			set;
		}

		public IDictionary<string, JsonSchemaModel> PatternProperties
		{
			get;
			set;
		}

		public IList<string> Patterns
		{
			get;
			set;
		}

		public IDictionary<string, JsonSchemaModel> Properties
		{
			get;
			set;
		}

		public bool Required
		{
			get;
			set;
		}

		public JsonSchemaType Type
		{
			get;
			set;
		}

		public JsonSchemaModel()
		{
			this.Type = JsonSchemaType.Any;
			this.AllowAdditionalProperties = true;
			this.Required = false;
		}

		private static void Combine(JsonSchemaModel model, JsonSchema schema)
		{
			bool flag;
			bool flag1;
			bool flag2;
			JsonSchemaModel jsonSchemaModel = model;
			if (model.Required)
			{
				flag = true;
			}
			else
			{
				bool? required = schema.Required;
				flag = (!required.HasValue ? false : required.Value);
			}
			jsonSchemaModel.Required = flag;
			JsonSchemaModel jsonSchemaModel1 = model;
			JsonSchemaType type = model.Type;
			JsonSchemaType? nullable = schema.Type;
			jsonSchemaModel1.Type = type & (!nullable.HasValue ? JsonSchemaType.Any : nullable.Value);
			model.MinimumLength = MathUtils.Max(model.MinimumLength, schema.MinimumLength);
			model.MaximumLength = MathUtils.Min(model.MaximumLength, schema.MaximumLength);
			model.DivisibleBy = MathUtils.Max(model.DivisibleBy, schema.DivisibleBy);
			model.Minimum = MathUtils.Max(model.Minimum, schema.Minimum);
			model.Maximum = MathUtils.Max(model.Maximum, schema.Maximum);
			JsonSchemaModel jsonSchemaModel2 = model;
			if (model.ExclusiveMinimum)
			{
				flag1 = true;
			}
			else
			{
				bool? exclusiveMinimum = schema.ExclusiveMinimum;
				flag1 = (!exclusiveMinimum.HasValue ? false : exclusiveMinimum.Value);
			}
			jsonSchemaModel2.ExclusiveMinimum = flag1;
			JsonSchemaModel jsonSchemaModel3 = model;
			if (model.ExclusiveMaximum)
			{
				flag2 = true;
			}
			else
			{
				bool? exclusiveMaximum = schema.ExclusiveMaximum;
				flag2 = (!exclusiveMaximum.HasValue ? false : exclusiveMaximum.Value);
			}
			jsonSchemaModel3.ExclusiveMaximum = flag2;
			model.MinimumItems = MathUtils.Max(model.MinimumItems, schema.MinimumItems);
			model.MaximumItems = MathUtils.Min(model.MaximumItems, schema.MaximumItems);
			model.AllowAdditionalProperties = (!model.AllowAdditionalProperties ? false : schema.AllowAdditionalProperties);
			if (schema.Enum != null)
			{
				if (model.Enum == null)
				{
					model.Enum = new List<JToken>();
				}
				model.Enum.AddRangeDistinct<JToken>(schema.Enum, new JTokenEqualityComparer());
			}
			JsonSchemaModel jsonSchemaModel4 = model;
			JsonSchemaType disallow = model.Disallow;
			JsonSchemaType? disallow1 = schema.Disallow;
			jsonSchemaModel4.Disallow = disallow | (!disallow1.HasValue ? JsonSchemaType.None : disallow1.Value);
			if (schema.Pattern != null)
			{
				if (model.Patterns == null)
				{
					model.Patterns = new List<string>();
				}
				model.Patterns.AddDistinct<string>(schema.Pattern);
			}
		}

		public static JsonSchemaModel Create(IList<JsonSchema> schemata)
		{
			JsonSchemaModel jsonSchemaModel = new JsonSchemaModel();
			IEnumerator<JsonSchema> enumerator = schemata.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					JsonSchemaModel.Combine(jsonSchemaModel, enumerator.Current);
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return jsonSchemaModel;
		}
	}
}