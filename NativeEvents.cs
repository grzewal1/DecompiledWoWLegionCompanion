using System;
using UnityEngine;

[ExecuteInEditMode]
public class NativeEvents : MonoBehaviour
{
	public NativeEvents()
	{
	}

	private void LateUpdate()
	{
		Events.SignalPendingUniqueEvents();
	}

	private void OnDestroy()
	{
		Events.Clear();
	}

	private static void RegisterForEvents()
	{
		Events.Clear();
		EventInterface.RegisterForAllEvents();
	}

	[ContextMenu("Re-register events now")]
	private void ResetContextMenu()
	{
		NativeEvents.RegisterForEvents();
	}

	private void Start()
	{
		NativeEvents.RegisterForEvents();
		NativeEvents.WatchForDllLoaded();
	}

	private static void WatchForDllLoaded()
	{
	}
}