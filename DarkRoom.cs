using System;
using UnityEngine;
using UnityEngine.UI;

public class DarkRoom
{
	public DarkRoom()
	{
	}

	public static void MakeSnapshot(FreezeFrame freezeFrame)
	{
		GameObject gameObject = GameObject.Find("MainCanvas");
		if (gameObject == null)
		{
			Debug.LogError("Could not find MainCanvas, did you rename it?");
			return;
		}
		GameObject gameObject1 = new GameObject(string.Concat(freezeFrame.name, "_SnaphotCamera"));
		gameObject1.transform.SetParent(null, false);
		gameObject1.transform.Translate(0f, 0f, -1f);
		Camera color = gameObject1.AddComponent<Camera>();
		RenderTexture renderTexture = new RenderTexture(320, 320, 24);
		Material material = new Material(Shader.Find("UI/Unlit/Transparent"));
		material.SetTexture("_MainTex", renderTexture);
		color.orthographic = true;
		color.orthographicSize = 100f;
		color.targetTexture = renderTexture;
		color.depth = -1f;
		color.clearFlags = CameraClearFlags.Color;
		color.backgroundColor = new Color(0f, 0f, 0f, 1f);
		GameObject gameObject2 = new GameObject(string.Concat(freezeFrame.name, "_DarkRoomCanvas"));
		Canvas canva = gameObject2.AddComponent<Canvas>();
		canva.planeDistance = 500f;
		canva.renderMode = RenderMode.ScreenSpaceCamera;
		canva.worldCamera = color;
		CanvasScaler vector2 = gameObject2.AddComponent<CanvasScaler>();
		vector2.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		vector2.referenceResolution = new Vector2(1200f, 900f);
		vector2.matchWidthOrHeight = 1f;
		GameObject vector21 = new GameObject(string.Concat(freezeFrame.name, "_Snapshot"));
		vector21.transform.SetParent(gameObject.transform, false);
		Image image = vector21.AddComponent<Image>();
		vector21.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 320f);
		image.material = material;
		freezeFrame.transform.SetParent(gameObject2.transform, false);
		color.Render();
	}
}