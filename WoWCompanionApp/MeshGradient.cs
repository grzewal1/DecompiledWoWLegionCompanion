using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	[AddComponentMenu("UI/Effects/MeshGradient")]
	public class MeshGradient : BaseMeshEffect
	{
		[SerializeField]
		private UnityEngine.Gradient m_gradient;

		public bool m_horizontal;

		public UnityEngine.Gradient Gradient
		{
			get
			{
				return this.m_gradient;
			}
			set
			{
				this.m_gradient = value;
				base.GetComponent<Graphic>().SetVerticesDirty();
			}
		}

		public MeshGradient()
		{
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (!this.IsActive())
			{
				return;
			}
			List<UIVertex> uIVertices = new List<UIVertex>();
			vh.GetUIVertexStream(uIVertices);
			this.ModifyVertices(uIVertices);
			vh.Clear();
			vh.AddUIVertexTriangleStream(uIVertices);
		}

		private void ModifyVertices(List<UIVertex> vertexList)
		{
			int count = vertexList.Count;
			if (count == 0)
			{
				return;
			}
			float single = (!this.m_horizontal ? vertexList[0].position.y : vertexList[0].position.x);
			float single1 = single;
			float single2 = single;
			for (int i = 1; i < count; i++)
			{
				float single3 = (!this.m_horizontal ? vertexList[i].position.y : vertexList[i].position.x);
				if (single3 > single2)
				{
					single2 = single3;
				}
				else if (single3 < single1)
				{
					single1 = single3;
				}
			}
			float single4 = single2 - single1;
			for (int j = 0; j < count; j++)
			{
				UIVertex item = vertexList[j];
				float single5 = ((!this.m_horizontal ? vertexList[j].position.y : vertexList[j].position.x) - single1) / single4;
				item.color = this.m_gradient.Evaluate((!this.m_horizontal ? 1f - single5 : single5));
				vertexList[j] = item;
			}
		}
	}
}