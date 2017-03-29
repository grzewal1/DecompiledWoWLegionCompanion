using System;
using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]
public class SetRenderQueue : MonoBehaviour
{
	[SerializeField]
	protected int[] m_queues = new int[] { 3000 };

	public SetRenderQueue()
	{
	}

	protected void Awake()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Material[] mQueues = componentsInChildren[i].materials;
			for (int j = 0; j < (int)mQueues.Length && j < (int)this.m_queues.Length; j++)
			{
				mQueues[j].renderQueue = this.m_queues[j];
			}
		}
	}
}