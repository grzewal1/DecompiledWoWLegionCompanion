using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class iTween : MonoBehaviour
{
	public static List<Hashtable> tweens;

	private static GameObject cameraFade;

	public string id;

	public string type;

	public string method;

	public iTween.EaseType easeType;

	public float time;

	public float delay;

	public iTween.LoopType loopType;

	public bool isRunning;

	public bool isPaused;

	public string _name;

	private float runningTime;

	private float percentage;

	private float delayStarted;

	private bool kinematic;

	private bool isLocal;

	private bool loop;

	private bool reverse;

	private bool wasPaused;

	private bool physics;

	private Hashtable tweenArguments;

	private Space space;

	private iTween.EasingFunction ease;

	private iTween.ApplyTween apply;

	private AudioSource audioSource;

	private Vector3[] vector3s;

	private Vector2[] vector2s;

	private Color[,] colors;

	private float[] floats;

	private Rect[] rects;

	private iTween.CRSpline path;

	private Vector3 preUpdate;

	private Vector3 postUpdate;

	private iTween.NamedValueColor namedcolorvalue;

	private float lastRealTime;

	private bool useRealTime;

	private Transform thisTransform;

	static iTween()
	{
		iTween.tweens = new List<Hashtable>();
	}

	private iTween(Hashtable h)
	{
		this.tweenArguments = h;
	}

	private void ApplyAudioToTargets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.audioSource.volume = this.vector2s[2].x;
		this.audioSource.pitch = this.vector2s[2].y;
		if (this.percentage == 1f)
		{
			this.audioSource.volume = this.vector2s[1].x;
			this.audioSource.pitch = this.vector2s[1].y;
		}
	}

	private void ApplyColorTargets()
	{
		this.colors[0, 2].r = this.ease(this.colors[0, 0].r, this.colors[0, 1].r, this.percentage);
		this.colors[0, 2].g = this.ease(this.colors[0, 0].g, this.colors[0, 1].g, this.percentage);
		this.colors[0, 2].b = this.ease(this.colors[0, 0].b, this.colors[0, 1].b, this.percentage);
		this.colors[0, 2].a = this.ease(this.colors[0, 0].a, this.colors[0, 1].a, this.percentage);
		this.tweenArguments["onupdateparams"] = this.colors[0, 2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.colors[0, 1];
		}
	}

	private void ApplyColorToTargets()
	{
		for (int i = 0; i < this.colors.GetLength(0); i++)
		{
			this.colors[i, 2].r = this.ease(this.colors[i, 0].r, this.colors[i, 1].r, this.percentage);
			this.colors[i, 2].g = this.ease(this.colors[i, 0].g, this.colors[i, 1].g, this.percentage);
			this.colors[i, 2].b = this.ease(this.colors[i, 0].b, this.colors[i, 1].b, this.percentage);
			this.colors[i, 2].a = this.ease(this.colors[i, 0].a, this.colors[i, 1].a, this.percentage);
		}
		if (base.GetComponent<GUITexture>())
		{
			base.GetComponent<GUITexture>().color = this.colors[0, 2];
		}
		else if (base.GetComponent<GUIText>())
		{
			base.GetComponent<GUIText>().material.color = this.colors[0, 2];
		}
		else if (base.GetComponent<Renderer>())
		{
			for (int j = 0; j < this.colors.GetLength(0); j++)
			{
				base.GetComponent<Renderer>().materials[j].SetColor(this.namedcolorvalue.ToString(), this.colors[j, 2]);
			}
		}
		else if (base.GetComponent<Light>())
		{
			base.GetComponent<Light>().color = this.colors[0, 2];
		}
		if (this.percentage == 1f)
		{
			if (base.GetComponent<GUITexture>())
			{
				base.GetComponent<GUITexture>().color = this.colors[0, 1];
			}
			else if (base.GetComponent<GUIText>())
			{
				base.GetComponent<GUIText>().material.color = this.colors[0, 1];
			}
			else if (base.GetComponent<Renderer>())
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					base.GetComponent<Renderer>().materials[k].SetColor(this.namedcolorvalue.ToString(), this.colors[k, 1]);
				}
			}
			else if (base.GetComponent<Light>())
			{
				base.GetComponent<Light>().color = this.colors[0, 1];
			}
		}
	}

	private void ApplyFloatTargets()
	{
		this.floats[2] = this.ease(this.floats[0], this.floats[1], this.percentage);
		this.tweenArguments["onupdateparams"] = this.floats[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.floats[1];
		}
	}

	private void ApplyLookToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (!this.isLocal)
		{
			this.thisTransform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			this.thisTransform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
	}

	private void ApplyMoveByTargets()
	{
		this.preUpdate = this.thisTransform.position;
		Vector3 vector3 = new Vector3();
		if (this.tweenArguments.Contains("looktarget"))
		{
			vector3 = this.thisTransform.eulerAngles;
			this.thisTransform.eulerAngles = this.vector3s[4];
		}
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.thisTransform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.thisTransform.eulerAngles = vector3;
		}
		this.postUpdate = this.thisTransform.position;
		if (this.physics)
		{
			this.thisTransform.position = this.preUpdate;
			base.GetComponent<Rigidbody>().MovePosition(this.postUpdate);
		}
	}

	private void ApplyMoveToPathTargets()
	{
		float single;
		this.preUpdate = this.thisTransform.position;
		float single1 = this.ease(0f, 1f, this.percentage);
		if (!this.isLocal)
		{
			this.thisTransform.position = this.path.Interp(Mathf.Clamp(single1, 0f, 1f));
		}
		else
		{
			this.thisTransform.localPosition = this.path.Interp(Mathf.Clamp(single1, 0f, 1f));
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			single = (!this.tweenArguments.Contains("lookahead") ? iTween.Defaults.lookAhead : (float)this.tweenArguments["lookahead"]);
			float single2 = this.ease(0f, 1f, Mathf.Min(1f, this.percentage + single));
			this.tweenArguments["looktarget"] = this.path.Interp(Mathf.Clamp(single2, 0f, 1f));
		}
		this.postUpdate = this.thisTransform.position;
		if (this.physics)
		{
			this.thisTransform.position = this.preUpdate;
			base.GetComponent<Rigidbody>().MovePosition(this.postUpdate);
		}
	}

	private void ApplyMoveToTargets()
	{
		this.preUpdate = this.thisTransform.position;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (!this.isLocal)
		{
			this.thisTransform.position = this.vector3s[2];
		}
		else
		{
			this.thisTransform.localPosition = this.vector3s[2];
		}
		if (this.percentage == 1f)
		{
			if (!this.isLocal)
			{
				this.thisTransform.position = this.vector3s[1];
			}
			else
			{
				this.thisTransform.localPosition = this.vector3s[1];
			}
		}
		this.postUpdate = this.thisTransform.position;
		if (this.physics)
		{
			this.thisTransform.position = this.preUpdate;
			base.GetComponent<Rigidbody>().MovePosition(this.postUpdate);
		}
	}

	private void ApplyPunchPositionTargets()
	{
		this.preUpdate = this.thisTransform.position;
		Vector3 vector3 = new Vector3();
		if (this.tweenArguments.Contains("looktarget"))
		{
			vector3 = this.thisTransform.eulerAngles;
			this.thisTransform.eulerAngles = this.vector3s[4];
		}
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.thisTransform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.thisTransform.eulerAngles = vector3;
		}
		this.postUpdate = this.thisTransform.position;
		if (this.physics)
		{
			this.thisTransform.position = this.preUpdate;
			base.GetComponent<Rigidbody>().MovePosition(this.postUpdate);
		}
	}

	private void ApplyPunchRotationTargets()
	{
		this.preUpdate = this.thisTransform.eulerAngles;
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.thisTransform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = this.thisTransform.eulerAngles;
		if (this.physics)
		{
			this.thisTransform.eulerAngles = this.preUpdate;
			base.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	private void ApplyPunchScaleTargets()
	{
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.thisTransform.localScale = this.vector3s[0] + this.vector3s[2];
	}

	private void ApplyRectTargets()
	{
		this.rects[2].x = this.ease(this.rects[0].x, this.rects[1].x, this.percentage);
		this.rects[2].y = this.ease(this.rects[0].y, this.rects[1].y, this.percentage);
		this.rects[2].width = this.ease(this.rects[0].width, this.rects[1].width, this.percentage);
		this.rects[2].height = this.ease(this.rects[0].height, this.rects[1].height, this.percentage);
		this.tweenArguments["onupdateparams"] = this.rects[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.rects[1];
		}
	}

	private void ApplyRotateAddTargets()
	{
		this.preUpdate = this.thisTransform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.thisTransform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = this.thisTransform.eulerAngles;
		if (this.physics)
		{
			this.thisTransform.eulerAngles = this.preUpdate;
			base.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	private void ApplyRotateToTargets()
	{
		this.preUpdate = this.thisTransform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (!this.isLocal)
		{
			this.thisTransform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			this.thisTransform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
		if (this.percentage == 1f)
		{
			if (!this.isLocal)
			{
				this.thisTransform.rotation = Quaternion.Euler(this.vector3s[1]);
			}
			else
			{
				this.thisTransform.localRotation = Quaternion.Euler(this.vector3s[1]);
			}
		}
		this.postUpdate = this.thisTransform.eulerAngles;
		if (this.physics)
		{
			this.thisTransform.eulerAngles = this.preUpdate;
			base.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	private void ApplyScaleToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.thisTransform.localScale = this.vector3s[2];
		if (this.percentage == 1f)
		{
			this.thisTransform.localScale = this.vector3s[1];
		}
	}

	private void ApplyShakePositionTargets()
	{
		if (!this.isLocal)
		{
			this.preUpdate = this.thisTransform.position;
		}
		else
		{
			this.preUpdate = this.thisTransform.localPosition;
		}
		Vector3 vector3 = new Vector3();
		if (this.tweenArguments.Contains("looktarget"))
		{
			vector3 = this.thisTransform.eulerAngles;
			this.thisTransform.eulerAngles = this.vector3s[3];
		}
		if (this.percentage == 0f)
		{
			this.thisTransform.Translate(this.vector3s[1], this.space);
		}
		if (!this.isLocal)
		{
			this.thisTransform.position = this.vector3s[0];
		}
		else
		{
			this.thisTransform.localPosition = this.vector3s[0];
		}
		float single = 1f - this.percentage;
		this.vector3s[2].x = UnityEngine.Random.Range(-this.vector3s[1].x * single, this.vector3s[1].x * single);
		this.vector3s[2].y = UnityEngine.Random.Range(-this.vector3s[1].y * single, this.vector3s[1].y * single);
		this.vector3s[2].z = UnityEngine.Random.Range(-this.vector3s[1].z * single, this.vector3s[1].z * single);
		if (!this.isLocal)
		{
			Transform transforms = this.thisTransform;
			transforms.position = transforms.position + this.vector3s[2];
		}
		else
		{
			Transform transforms1 = this.thisTransform;
			transforms1.localPosition = transforms1.localPosition + this.vector3s[2];
		}
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.thisTransform.eulerAngles = vector3;
		}
		this.postUpdate = this.thisTransform.position;
		if (this.physics)
		{
			this.thisTransform.position = this.preUpdate;
			base.GetComponent<Rigidbody>().MovePosition(this.postUpdate);
		}
	}

	private void ApplyShakeRotationTargets()
	{
		this.preUpdate = this.thisTransform.eulerAngles;
		if (this.percentage == 0f)
		{
			this.thisTransform.Rotate(this.vector3s[1], this.space);
		}
		this.thisTransform.eulerAngles = this.vector3s[0];
		float single = 1f - this.percentage;
		this.vector3s[2].x = UnityEngine.Random.Range(-this.vector3s[1].x * single, this.vector3s[1].x * single);
		this.vector3s[2].y = UnityEngine.Random.Range(-this.vector3s[1].y * single, this.vector3s[1].y * single);
		this.vector3s[2].z = UnityEngine.Random.Range(-this.vector3s[1].z * single, this.vector3s[1].z * single);
		this.thisTransform.Rotate(this.vector3s[2], this.space);
		this.postUpdate = this.thisTransform.eulerAngles;
		if (this.physics)
		{
			this.thisTransform.eulerAngles = this.preUpdate;
			base.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	private void ApplyShakeScaleTargets()
	{
		if (this.percentage == 0f)
		{
			this.thisTransform.localScale = this.vector3s[1];
		}
		this.thisTransform.localScale = this.vector3s[0];
		float single = 1f - this.percentage;
		this.vector3s[2].x = UnityEngine.Random.Range(-this.vector3s[1].x * single, this.vector3s[1].x * single);
		this.vector3s[2].y = UnityEngine.Random.Range(-this.vector3s[1].y * single, this.vector3s[1].y * single);
		this.vector3s[2].z = UnityEngine.Random.Range(-this.vector3s[1].z * single, this.vector3s[1].z * single);
		Transform transforms = this.thisTransform;
		transforms.localScale = transforms.localScale + this.vector3s[2];
	}

	private void ApplyStabTargets()
	{
	}

	private void ApplyVector2Targets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector2s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector2s[1];
		}
	}

	private void ApplyVector3Targets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector3s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector3s[1];
		}
	}

	public static void AudioFrom(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioFrom(target, iTween.Hash(new object[] { "volume", volume, "pitch", pitch, "time", time }));
	}

	public static void AudioFrom(GameObject target, Hashtable args)
	{
		Vector2 vector2 = new Vector2();
		Vector2 item = new Vector2();
		AudioSource component;
		args = iTween.CleanArgs(args);
		if (!args.Contains("audiosource"))
		{
			if (!target.GetComponent<AudioSource>())
			{
				UnityEngine.Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
				return;
			}
			component = target.GetComponent<AudioSource>();
		}
		else
		{
			component = (AudioSource)args["audiosource"];
		}
		float single = component.volume;
		float single1 = single;
		item.x = single;
		vector2.x = single1;
		float single2 = component.pitch;
		single1 = single2;
		item.y = single2;
		vector2.y = single1;
		if (args.Contains("volume"))
		{
			item.x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			item.y = (float)args["pitch"];
		}
		component.volume = item.x;
		component.pitch = item.y;
		args["volume"] = vector2.x;
		args["pitch"] = vector2.y;
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void AudioTo(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioTo(target, iTween.Hash(new object[] { "volume", volume, "pitch", pitch, "time", time }));
	}

	public static void AudioTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void AudioUpdate(GameObject target, Hashtable args)
	{
		AudioSource component;
		float item;
		iTween.CleanArgs(args);
		Vector2[] vector2Array = new Vector2[4];
		if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		if (!args.Contains("audiosource"))
		{
			if (!target.GetComponent<AudioSource>())
			{
				UnityEngine.Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
				return;
			}
			component = target.GetComponent<AudioSource>();
		}
		else
		{
			component = (AudioSource)args["audiosource"];
		}
		Vector2 vector2 = new Vector2(component.volume, component.pitch);
		Vector2 vector21 = vector2;
		vector2Array[1] = vector2;
		vector2Array[0] = vector21;
		if (args.Contains("volume"))
		{
			vector2Array[1].x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			vector2Array[1].y = (float)args["pitch"];
		}
		vector2Array[3].x = Mathf.SmoothDampAngle(vector2Array[0].x, vector2Array[1].x, ref vector2Array[2].x, item);
		vector2Array[3].y = Mathf.SmoothDampAngle(vector2Array[0].y, vector2Array[1].y, ref vector2Array[2].y, item);
		component.volume = vector2Array[3].x;
		component.pitch = vector2Array[3].y;
	}

	public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioUpdate(target, iTween.Hash(new object[] { "volume", volume, "pitch", pitch, "time", time }));
	}

	private void Awake()
	{
		this.thisTransform = base.transform;
		this.RetrieveArgs();
		this.lastRealTime = Time.realtimeSinceStartup;
	}

	private void CallBack(string callbackType)
	{
		GameObject gameObject;
		if (this.tweenArguments.Contains(callbackType) && !this.tweenArguments.Contains("ischild"))
		{
			gameObject = (!this.tweenArguments.Contains(string.Concat(callbackType, "target")) ? base.gameObject : (GameObject)this.tweenArguments[string.Concat(callbackType, "target")]);
			if (this.tweenArguments[callbackType].GetType() != typeof(string))
			{
				UnityEngine.Debug.LogError("iTween Error: Callback method references must be passed as a String!");
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				gameObject.SendMessage((string)this.tweenArguments[callbackType], this.tweenArguments[string.Concat(callbackType, "params")], SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static GameObject CameraFadeAdd(Texture2D texture, int depth)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)depth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	public static GameObject CameraFadeAdd(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	public static GameObject CameraFadeAdd()
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = iTween.CameraTexture(Color.black);
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	public static void CameraFadeDepth(int depth)
	{
		if (iTween.cameraFade)
		{
			Transform vector3 = iTween.cameraFade.transform;
			float single = iTween.cameraFade.transform.position.x;
			Vector3 vector31 = iTween.cameraFade.transform.position;
			vector3.position = new Vector3(single, vector31.y, (float)depth);
		}
	}

	public static void CameraFadeDestroy()
	{
		if (iTween.cameraFade)
		{
			UnityEngine.Object.Destroy(iTween.cameraFade);
		}
	}

	public static void CameraFadeFrom(float amount, float time)
	{
		if (!iTween.cameraFade)
		{
			UnityEngine.Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
		else
		{
			iTween.CameraFadeFrom(iTween.Hash(new object[] { "amount", amount, "time", time }));
		}
	}

	public static void CameraFadeFrom(Hashtable args)
	{
		if (!iTween.cameraFade)
		{
			UnityEngine.Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
		else
		{
			iTween.ColorFrom(iTween.cameraFade, args);
		}
	}

	public static void CameraFadeSwap(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		}
	}

	public static void CameraFadeTo(float amount, float time)
	{
		if (!iTween.cameraFade)
		{
			UnityEngine.Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
		else
		{
			iTween.CameraFadeTo(iTween.Hash(new object[] { "amount", amount, "time", time }));
		}
	}

	public static void CameraFadeTo(Hashtable args)
	{
		if (!iTween.cameraFade)
		{
			UnityEngine.Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
		else
		{
			iTween.ColorTo(iTween.cameraFade, args);
		}
	}

	public static Texture2D CameraTexture(Color color)
	{
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
		Color[] colorArray = new Color[Screen.width * Screen.height];
		for (int i = 0; i < (int)colorArray.Length; i++)
		{
			colorArray[i] = color;
		}
		texture2D.SetPixels(colorArray);
		texture2D.Apply();
		return texture2D;
	}

	private static Hashtable CleanArgs(Hashtable args)
	{
		Hashtable hashtables = new Hashtable(args.Count);
		Hashtable hashtables1 = new Hashtable(args.Count);
		IDictionaryEnumerator enumerator = args.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry current = (DictionaryEntry)enumerator.Current;
				hashtables.Add(current.Key, current.Value);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
		IDictionaryEnumerator dictionaryEnumerator = hashtables.GetEnumerator();
		try
		{
			while (dictionaryEnumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)dictionaryEnumerator.Current;
				if (dictionaryEntry.Value.GetType() == typeof(int))
				{
					float value = (float)((int)dictionaryEntry.Value);
					args[dictionaryEntry.Key] = value;
				}
				if (dictionaryEntry.Value.GetType() != typeof(double))
				{
					continue;
				}
				float single = (float)((double)dictionaryEntry.Value);
				args[dictionaryEntry.Key] = single;
			}
		}
		finally
		{
			IDisposable disposable1 = dictionaryEnumerator as IDisposable;
			if (disposable1 == null)
			{
			}
			disposable1.Dispose();
		}
		IDictionaryEnumerator enumerator1 = args.GetEnumerator();
		try
		{
			while (enumerator1.MoveNext())
			{
				DictionaryEntry current1 = (DictionaryEntry)enumerator1.Current;
				hashtables1.Add(current1.Key.ToString().ToLower(), current1.Value);
			}
		}
		finally
		{
			IDisposable disposable2 = enumerator1 as IDisposable;
			if (disposable2 == null)
			{
			}
			disposable2.Dispose();
		}
		args = hashtables1;
		return args;
	}

	private float clerp(float start, float end, float value)
	{
		float single = 0f;
		float single1 = 360f;
		float single2 = Mathf.Abs((single1 - single) * 0.5f);
		float single3 = 0f;
		float single4 = 0f;
		if (end - start < -single2)
		{
			single4 = (single1 - start + end) * value;
			single3 = start + single4;
		}
		else if (end - start <= single2)
		{
			single3 = start + (end - start) * value;
		}
		else
		{
			single4 = -(single1 - end + start) * value;
			single3 = start + single4;
		}
		return single3;
	}

	public static void ColorFrom(GameObject target, Color color, float time)
	{
		iTween.ColorFrom(target, iTween.Hash(new object[] { "color", color, "time", time }));
	}

	public static void ColorFrom(GameObject target, Hashtable args)
	{
		Color item = new Color();
		Color color = new Color();
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = (Transform)enumerator.Current;
					Hashtable hashtables = (Hashtable)args.Clone();
					hashtables["ischild"] = true;
					iTween.ColorFrom(current.gameObject, hashtables);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		if (target.GetComponent<GUITexture>())
		{
			Color component = target.GetComponent<GUITexture>().color;
			item = component;
			color = component;
		}
		else if (target.GetComponent<GUIText>())
		{
			Color component1 = target.GetComponent<GUIText>().material.color;
			item = component1;
			color = component1;
		}
		else if (target.GetComponent<Renderer>())
		{
			Color color1 = target.GetComponent<Renderer>().material.color;
			item = color1;
			color = color1;
		}
		else if (target.GetComponent<Light>())
		{
			Color component2 = target.GetComponent<Light>().color;
			item = component2;
			color = component2;
		}
		if (!args.Contains("color"))
		{
			if (args.Contains("r"))
			{
				item.r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				item.g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				item.b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				item.a = (float)args["a"];
			}
		}
		else
		{
			item = (Color)args["color"];
		}
		if (args.Contains("amount"))
		{
			item.a = (float)args["amount"];
			args.Remove("amount");
		}
		else if (args.Contains("alpha"))
		{
			item.a = (float)args["alpha"];
			args.Remove("alpha");
		}
		if (target.GetComponent<GUITexture>())
		{
			target.GetComponent<GUITexture>().color = item;
		}
		else if (target.GetComponent<GUIText>())
		{
			target.GetComponent<GUIText>().material.color = item;
		}
		else if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = item;
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = item;
		}
		args["color"] = color;
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void ColorTo(GameObject target, Color color, float time)
	{
		iTween.ColorTo(target, iTween.Hash(new object[] { "color", color, "time", time }));
	}

	public static void ColorTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = (Transform)enumerator.Current;
					Hashtable hashtables = (Hashtable)args.Clone();
					hashtables["ischild"] = true;
					iTween.ColorTo(current.gameObject, hashtables);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void ColorUpdate(GameObject target, Hashtable args)
	{
		float item;
		Color color;
		iTween.CleanArgs(args);
		Color[] colorArray = new Color[4];
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.ColorUpdate(((Transform)enumerator.Current).gameObject, args);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
		if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		if (target.GetComponent<GUITexture>())
		{
			Color component = target.GetComponent<GUITexture>().color;
			color = component;
			colorArray[1] = component;
			colorArray[0] = color;
		}
		else if (target.GetComponent<GUIText>())
		{
			Color component1 = target.GetComponent<GUIText>().material.color;
			color = component1;
			colorArray[1] = component1;
			colorArray[0] = color;
		}
		else if (target.GetComponent<Renderer>())
		{
			Color color1 = target.GetComponent<Renderer>().material.color;
			color = color1;
			colorArray[1] = color1;
			colorArray[0] = color;
		}
		else if (target.GetComponent<Light>())
		{
			Color component2 = target.GetComponent<Light>().color;
			color = component2;
			colorArray[1] = component2;
			colorArray[0] = color;
		}
		if (!args.Contains("color"))
		{
			if (args.Contains("r"))
			{
				colorArray[1].r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				colorArray[1].g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				colorArray[1].b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				colorArray[1].a = (float)args["a"];
			}
		}
		else
		{
			colorArray[1] = (Color)args["color"];
		}
		colorArray[3].r = Mathf.SmoothDamp(colorArray[0].r, colorArray[1].r, ref colorArray[2].r, item);
		colorArray[3].g = Mathf.SmoothDamp(colorArray[0].g, colorArray[1].g, ref colorArray[2].g, item);
		colorArray[3].b = Mathf.SmoothDamp(colorArray[0].b, colorArray[1].b, ref colorArray[2].b, item);
		colorArray[3].a = Mathf.SmoothDamp(colorArray[0].a, colorArray[1].a, ref colorArray[2].a, item);
		if (target.GetComponent<GUITexture>())
		{
			target.GetComponent<GUITexture>().color = colorArray[3];
		}
		else if (target.GetComponent<GUIText>())
		{
			target.GetComponent<GUIText>().material.color = colorArray[3];
		}
		else if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = colorArray[3];
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = colorArray[3];
		}
	}

	public static void ColorUpdate(GameObject target, Color color, float time)
	{
		iTween.ColorUpdate(target, iTween.Hash(new object[] { "color", color, "time", time }));
	}

	private void ConflictCheck()
	{
		Component[] components = base.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (_iTween.type == "value")
			{
				return;
			}
			if (_iTween.isRunning && _iTween.type == this.type)
			{
				if (_iTween.method != this.method)
				{
					return;
				}
				if (_iTween.tweenArguments.Count != this.tweenArguments.Count)
				{
					_iTween.Dispose();
					return;
				}
				IDictionaryEnumerator enumerator = this.tweenArguments.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry current = (DictionaryEntry)enumerator.Current;
						if (_iTween.tweenArguments.Contains(current.Key))
						{
							if (_iTween.tweenArguments[current.Key].Equals(this.tweenArguments[current.Key]) || !((string)current.Key != "id"))
							{
								continue;
							}
							_iTween.Dispose();
							return;
						}
						else
						{
							_iTween.Dispose();
							return;
						}
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable == null)
					{
					}
					disposable.Dispose();
				}
				this.Dispose();
			}
		}
	}

	public static int Count()
	{
		return iTween.tweens.Count;
	}

	public static int Count(string type)
	{
		int num = 0;
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable item = iTween.tweens[i];
			if (string.Concat((string)item["type"], (string)item["method"]).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	public static int Count(GameObject target)
	{
		return (int)target.GetComponents<iTween>().Length;
	}

	public static int Count(GameObject target, string type)
	{
		int num = 0;
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	private void DisableKinematic()
	{
	}

	private void Dispose()
	{
		int num = 0;
		while (num < iTween.tweens.Count)
		{
			if ((string)iTween.tweens[num]["id"] != this.id)
			{
				num++;
			}
			else
			{
				iTween.tweens.RemoveAt(num);
				break;
			}
		}
		UnityEngine.Object.Destroy(this);
	}

	public static void DrawLine(Vector3[] line)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawLine(Vector3[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	public static void DrawLine(Transform[] line)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawLine(Transform[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Vector3[] line)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Vector3[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Transform[] line)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawLineGizmos(Transform[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, color, "gizmos");
		}
	}

	public static void DrawLineHandles(Vector3[] line)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "handles");
		}
	}

	public static void DrawLineHandles(Vector3[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "handles");
		}
	}

	public static void DrawLineHandles(Transform[] line)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, iTween.Defaults.color, "handles");
		}
	}

	public static void DrawLineHandles(Transform[] line, Color color)
	{
		if ((int)line.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)line.Length];
			for (int i = 0; i < (int)line.Length; i++)
			{
				vector3Array[i] = line[i].position;
			}
			iTween.DrawLineHelper(vector3Array, color, "handles");
		}
	}

	private static void DrawLineHelper(Vector3[] line, Color color, string method)
	{
		Gizmos.color = color;
		for (int i = 0; i < (int)line.Length - 1; i++)
		{
			if (method == "gizmos")
			{
				Gizmos.DrawLine(line[i], line[i + 1]);
			}
			else if (method == "handles")
			{
				UnityEngine.Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
		}
	}

	public static void DrawPath(Vector3[] path)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawPath(Vector3[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	public static void DrawPath(Transform[] path)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawPath(Transform[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Vector3[] path)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Vector3[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Transform[] path)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, iTween.Defaults.color, "gizmos");
		}
	}

	public static void DrawPathGizmos(Transform[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, color, "gizmos");
		}
	}

	public static void DrawPathHandles(Vector3[] path)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "handles");
		}
	}

	public static void DrawPathHandles(Vector3[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "handles");
		}
	}

	public static void DrawPathHandles(Transform[] path)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, iTween.Defaults.color, "handles");
		}
	}

	public static void DrawPathHandles(Transform[] path, Color color)
	{
		if ((int)path.Length > 0)
		{
			Vector3[] vector3Array = new Vector3[(int)path.Length];
			for (int i = 0; i < (int)path.Length; i++)
			{
				vector3Array[i] = path[i].position;
			}
			iTween.DrawPathHelper(vector3Array, color, "handles");
		}
	}

	private static void DrawPathHelper(Vector3[] path, Color color, string method)
	{
		Vector3[] vector3Array = iTween.PathControlPointGenerator(path);
		Vector3 vector3 = iTween.Interp(vector3Array, 0f);
		Gizmos.color = color;
		int length = (int)path.Length * 20;
		for (int i = 1; i <= length; i++)
		{
			float single = (float)i / (float)length;
			Vector3 vector31 = iTween.Interp(vector3Array, single);
			if (method == "gizmos")
			{
				Gizmos.DrawLine(vector31, vector3);
			}
			else if (method == "handles")
			{
				UnityEngine.Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
			vector3 = vector31;
		}
	}

	private float easeInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1f;
		float single = 1.70158f;
		return end * value * value * ((single + 1f) * value - single) + start;
	}

	private float easeInBounce(float start, float end, float value)
	{
		end -= start;
		float single = 1f;
		return end - this.easeOutBounce(0f, end, single - value) + start;
	}

	private float easeInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
	}

	private float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	private float easeInElastic(float start, float end, float value)
	{
		end -= start;
		float single = 1f;
		float single1 = single * 0.3f;
		float single2 = 0f;
		float single3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		float single4 = value / single;
		value = single4;
		if (single4 == 1f)
		{
			return start + end;
		}
		if (single3 == 0f || single3 < Mathf.Abs(end))
		{
			single3 = end;
			single2 = single1 / 4f;
		}
		else
		{
			single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
		}
		float single5 = value - 1f;
		value = single5;
		return -(single3 * Mathf.Pow(2f, 10f * single5) * Mathf.Sin((value * single - single2) * 6.28318548f / single1)) + start;
	}

	private float easeInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (value - 1f)) + start;
	}

	private float easeInOutBack(float start, float end, float value)
	{
		float single = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1f)
		{
			single *= 1.525f;
			return end * 0.5f * (value * value * ((single + 1f) * value - single)) + start;
		}
		value -= 2f;
		single *= 1.525f;
		return end * 0.5f * (value * value * ((single + 1f) * value + single) + 2f) + start;
	}

	private float easeInOutBounce(float start, float end, float value)
	{
		end -= start;
		float single = 1f;
		if (value < single * 0.5f)
		{
			return this.easeInBounce(0f, end, value * 2f) * 0.5f + start;
		}
		return this.easeOutBounce(0f, end, value * 2f - single) * 0.5f + end * 0.5f + start;
	}

	private float easeInOutCirc(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return -end * 0.5f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}
		value -= 2f;
		return end * 0.5f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
	}

	private float easeInOutCubic(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end * 0.5f * value * value * value + start;
		}
		value -= 2f;
		return end * 0.5f * (value * value * value + 2f) + start;
	}

	private float easeInOutElastic(float start, float end, float value)
	{
		end -= start;
		float single = 1f;
		float single1 = single * 0.3f;
		float single2 = 0f;
		float single3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		float single4 = value / (single * 0.5f);
		value = single4;
		if (single4 == 2f)
		{
			return start + end;
		}
		if (single3 == 0f || single3 < Mathf.Abs(end))
		{
			single3 = end;
			single2 = single1 / 4f;
		}
		else
		{
			single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
		}
		if (value < 1f)
		{
			float single5 = value - 1f;
			value = single5;
			return -0.5f * (single3 * Mathf.Pow(2f, 10f * single5) * Mathf.Sin((value * single - single2) * 6.28318548f / single1)) + start;
		}
		float single6 = value - 1f;
		value = single6;
		return single3 * Mathf.Pow(2f, -10f * single6) * Mathf.Sin((value * single - single2) * 6.28318548f / single1) * 0.5f + end + start;
	}

	private float easeInOutExpo(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end * 0.5f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}
		value -= 1f;
		return end * 0.5f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
	}

	private float easeInOutQuad(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end * 0.5f * value * value + start;
		}
		value -= 1f;
		return -end * 0.5f * (value * (value - 2f) - 1f) + start;
	}

	private float easeInOutQuart(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end * 0.5f * value * value * value * value + start;
		}
		value -= 2f;
		return -end * 0.5f * (value * value * value * value - 2f) + start;
	}

	private float easeInOutQuint(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end * 0.5f * value * value * value * value * value + start;
		}
		value -= 2f;
		return end * 0.5f * (value * value * value * value * value + 2f) + start;
	}

	private float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end * 0.5f * (Mathf.Cos(3.14159274f * value) - 1f) + start;
	}

	private float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	private float easeInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	private float easeInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	private float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value * 1.57079637f) + end + start;
	}

	private float easeOutBack(float start, float end, float value)
	{
		float single = 1.70158f;
		end -= start;
		value -= 1f;
		return end * (value * value * ((single + 1f) * value + single) + 1f) + start;
	}

	private float easeOutBounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;
		if (value < 0.363636374f)
		{
			return end * (7.5625f * value * value) + start;
		}
		if (value < 0.727272749f)
		{
			value -= 0.545454562f;
			return end * (7.5625f * value * value + 0.75f) + start;
		}
		if ((double)value < 0.909090909090909)
		{
			value -= 0.8181818f;
			return end * (7.5625f * value * value + 0.9375f) + start;
		}
		value -= 0.954545438f;
		return end * (7.5625f * value * value + 0.984375f) + start;
	}

	private float easeOutCirc(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - value * value) + start;
	}

	private float easeOutCubic(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value + 1f) + start;
	}

	private float easeOutElastic(float start, float end, float value)
	{
		end -= start;
		float single = 1f;
		float single1 = single * 0.3f;
		float single2 = 0f;
		float single3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		float single4 = value / single;
		value = single4;
		if (single4 == 1f)
		{
			return start + end;
		}
		if (single3 == 0f || single3 < Mathf.Abs(end))
		{
			single3 = end;
			single2 = single1 * 0.25f;
		}
		else
		{
			single2 = single1 / 6.28318548f * Mathf.Asin(end / single3);
		}
		return single3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * single - single2) * 6.28318548f / single1) + end + start;
	}

	private float easeOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2f, -10f * value) + 1f) + start;
	}

	private float easeOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2f) + start;
	}

	private float easeOutQuart(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return -end * (value * value * value * value - 1f) + start;
	}

	private float easeOutQuint(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value * value * value + 1f) + start;
	}

	private float easeOutSine(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Sin(value * 1.57079637f) + start;
	}

	private void EnableKinematic()
	{
	}

	public static void FadeFrom(GameObject target, float alpha, float time)
	{
		iTween.FadeFrom(target, iTween.Hash(new object[] { "alpha", alpha, "time", time }));
	}

	public static void FadeFrom(GameObject target, Hashtable args)
	{
		iTween.ColorFrom(target, args);
	}

	public static void FadeTo(GameObject target, float alpha, float time)
	{
		iTween.FadeTo(target, iTween.Hash(new object[] { "alpha", alpha, "time", time }));
	}

	public static void FadeTo(GameObject target, Hashtable args)
	{
		iTween.ColorTo(target, args);
	}

	public static void FadeUpdate(GameObject target, Hashtable args)
	{
		args["a"] = args["alpha"];
		iTween.ColorUpdate(target, args);
	}

	public static void FadeUpdate(GameObject target, float alpha, float time)
	{
		iTween.FadeUpdate(target, iTween.Hash(new object[] { "alpha", alpha, "time", time }));
	}

	private void FixedUpdate()
	{
		if (this.isRunning && this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage >= 1f)
				{
					this.TweenComplete();
				}
				else
				{
					this.TweenUpdate();
				}
			}
			else if (this.percentage <= 0f)
			{
				this.TweenComplete();
			}
			else
			{
				this.TweenUpdate();
			}
		}
	}

	public static float FloatUpdate(float currentValue, float targetValue, float speed)
	{
		float single = targetValue - currentValue;
		currentValue = currentValue + single * speed * Time.deltaTime;
		return currentValue;
	}

	private void GenerateAudioToTargets()
	{
		this.vector2s = new Vector2[3];
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (!base.GetComponent<AudioSource>())
		{
			UnityEngine.Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
			this.Dispose();
		}
		else
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		Vector2 vector2 = new Vector2(this.audioSource.volume, this.audioSource.pitch);
		Vector2 vector21 = vector2;
		this.vector2s[1] = vector2;
		this.vector2s[0] = vector21;
		if (this.tweenArguments.Contains("volume"))
		{
			this.vector2s[1].x = (float)this.tweenArguments["volume"];
		}
		if (this.tweenArguments.Contains("pitch"))
		{
			this.vector2s[1].y = (float)this.tweenArguments["pitch"];
		}
	}

	private void GenerateColorTargets()
	{
		this.colors = new Color[1, 3];
		this.colors[0, 0] = (Color)this.tweenArguments["from"];
		this.colors[0, 1] = (Color)this.tweenArguments["to"];
	}

	private void GenerateColorToTargets()
	{
		Color color;
		if (base.GetComponent<GUITexture>())
		{
			this.colors = new Color[1, 3];
			Color[,] colorArray = this.colors;
			Color[,] colorArray1 = this.colors;
			Color component = base.GetComponent<GUITexture>().color;
			color = component;
			colorArray1[0, 1] = component;
			colorArray[0, 0] = color;
		}
		else if (base.GetComponent<GUIText>())
		{
			this.colors = new Color[1, 3];
			Color[,] colorArray2 = this.colors;
			Color[,] colorArray3 = this.colors;
			Color component1 = base.GetComponent<GUIText>().material.color;
			color = component1;
			colorArray3[0, 1] = component1;
			colorArray2[0, 0] = color;
		}
		else if (base.GetComponent<Renderer>())
		{
			this.colors = new Color[(int)base.GetComponent<Renderer>().materials.Length, 3];
			for (int i = 0; i < (int)base.GetComponent<Renderer>().materials.Length; i++)
			{
				this.colors[i, 0] = base.GetComponent<Renderer>().materials[i].GetColor(this.namedcolorvalue.ToString());
				this.colors[i, 1] = base.GetComponent<Renderer>().materials[i].GetColor(this.namedcolorvalue.ToString());
			}
		}
		else if (!base.GetComponent<Light>())
		{
			this.colors = new Color[1, 3];
		}
		else
		{
			this.colors = new Color[1, 3];
			Color[,] colorArray4 = this.colors;
			Color[,] colorArray5 = this.colors;
			Color color1 = base.GetComponent<Light>().color;
			color = color1;
			colorArray5[0, 1] = color1;
			colorArray4[0, 0] = color;
		}
		if (!this.tweenArguments.Contains("color"))
		{
			if (this.tweenArguments.Contains("r"))
			{
				for (int j = 0; j < this.colors.GetLength(0); j++)
				{
					this.colors[j, 1].r = (float)this.tweenArguments["r"];
				}
			}
			if (this.tweenArguments.Contains("g"))
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					this.colors[k, 1].g = (float)this.tweenArguments["g"];
				}
			}
			if (this.tweenArguments.Contains("b"))
			{
				for (int l = 0; l < this.colors.GetLength(0); l++)
				{
					this.colors[l, 1].b = (float)this.tweenArguments["b"];
				}
			}
			if (this.tweenArguments.Contains("a"))
			{
				for (int m = 0; m < this.colors.GetLength(0); m++)
				{
					this.colors[m, 1].a = (float)this.tweenArguments["a"];
				}
			}
		}
		else
		{
			for (int n = 0; n < this.colors.GetLength(0); n++)
			{
				this.colors[n, 1] = (Color)this.tweenArguments["color"];
			}
		}
		if (this.tweenArguments.Contains("amount"))
		{
			for (int o = 0; o < this.colors.GetLength(0); o++)
			{
				this.colors[o, 1].a = (float)this.tweenArguments["amount"];
			}
		}
		else if (this.tweenArguments.Contains("alpha"))
		{
			for (int p = 0; p < this.colors.GetLength(0); p++)
			{
				this.colors[p, 1].a = (float)this.tweenArguments["alpha"];
			}
		}
	}

	private void GenerateFloatTargets()
	{
		this.floats = new float[] { (float)this.tweenArguments["from"], (float)this.tweenArguments["to"], default(float) };
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(this.floats[0] - this.floats[1]);
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private static string GenerateID()
	{
		return Guid.NewGuid().ToString();
	}

	private void GenerateLookToTargets()
	{
		int num;
		this.vector3s = new Vector3[] { this.thisTransform.eulerAngles, default(Vector3), default(Vector3) };
		if (!this.tweenArguments.Contains("looktarget"))
		{
			UnityEngine.Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
			this.Dispose();
		}
		else if (this.tweenArguments["looktarget"].GetType() == typeof(Transform))
		{
			Transform transforms = this.thisTransform;
			Transform item = (Transform)this.tweenArguments["looktarget"];
			Vector3? nullable = (Vector3?)this.tweenArguments["up"];
			transforms.LookAt(item, (!nullable.HasValue ? iTween.Defaults.up : nullable.Value));
		}
		else if (this.tweenArguments["looktarget"].GetType() == typeof(Vector3))
		{
			Transform transforms1 = this.thisTransform;
			Vector3 vector3 = (Vector3)this.tweenArguments["looktarget"];
			Vector3? item1 = (Vector3?)this.tweenArguments["up"];
			transforms1.LookAt(vector3, (!item1.HasValue ? iTween.Defaults.up : item1.Value));
		}
		this.vector3s[1] = this.thisTransform.eulerAngles;
		this.thisTransform.eulerAngles = this.vector3s[0];
		if (this.tweenArguments.Contains("axis"))
		{
			string str = (string)this.tweenArguments["axis"];
			if (str != null)
			{
				if (iTween.<>f__switch$map1D == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(3)
					{
						{ "x", 0 },
						{ "y", 1 },
						{ "z", 2 }
					};
					iTween.<>f__switch$map1D = strs;
				}
				if (iTween.<>f__switch$map1D.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							this.vector3s[1].y = this.vector3s[0].y;
							this.vector3s[1].z = this.vector3s[0].z;
							break;
						}
						case 1:
						{
							this.vector3s[1].x = this.vector3s[0].x;
							this.vector3s[1].z = this.vector3s[0].z;
							break;
						}
						case 2:
						{
							this.vector3s[1].x = this.vector3s[0].x;
							this.vector3s[1].y = this.vector3s[0].y;
							break;
						}
					}
				}
			}
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateMoveByTargets()
	{
		this.vector3s = new Vector3[] { default(Vector3), default(Vector3), default(Vector3), default(Vector3), this.thisTransform.eulerAngles, default(Vector3) };
		Vector3 vector3 = this.thisTransform.position;
		Vector3 vector31 = vector3;
		this.vector3s[3] = vector3;
		Vector3 vector32 = vector31;
		vector31 = vector32;
		this.vector3s[1] = vector32;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = this.vector3s[0].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = this.vector3s[0].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = this.vector3s[0].z + (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = this.vector3s[0] + (Vector3)this.tweenArguments["amount"];
		}
		this.thisTransform.Translate(this.vector3s[1], this.space);
		this.vector3s[5] = this.thisTransform.position;
		this.thisTransform.position = this.vector3s[0];
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateMoveToPathTargets()
	{
		Vector3[] vector3Array;
		bool flag;
		int num;
		if (this.tweenArguments["path"].GetType() != typeof(Vector3[]))
		{
			Transform[] item = (Transform[])this.tweenArguments["path"];
			if ((int)item.Length == 1)
			{
				UnityEngine.Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			vector3Array = new Vector3[(int)item.Length];
			for (int i = 0; i < (int)item.Length; i++)
			{
				vector3Array[i] = item[i].position;
			}
		}
		else
		{
			Vector3[] item1 = (Vector3[])this.tweenArguments["path"];
			if ((int)item1.Length == 1)
			{
				UnityEngine.Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			vector3Array = new Vector3[(int)item1.Length];
			Array.Copy(item1, vector3Array, (int)item1.Length);
		}
		if (this.thisTransform.position == vector3Array[0])
		{
			flag = false;
			num = 2;
		}
		else if (!this.tweenArguments.Contains("movetopath") || (bool)this.tweenArguments["movetopath"])
		{
			flag = true;
			num = 3;
		}
		else
		{
			flag = false;
			num = 2;
		}
		this.vector3s = new Vector3[(int)vector3Array.Length + num];
		if (!flag)
		{
			num = 1;
		}
		else
		{
			this.vector3s[1] = this.thisTransform.position;
			num = 2;
		}
		Array.Copy(vector3Array, 0, this.vector3s, num, (int)vector3Array.Length);
		this.vector3s[0] = this.vector3s[1] + (this.vector3s[1] - this.vector3s[2]);
		this.vector3s[(int)this.vector3s.Length - 1] = this.vector3s[(int)this.vector3s.Length - 2] + (this.vector3s[(int)this.vector3s.Length - 2] - this.vector3s[(int)this.vector3s.Length - 3]);
		if (this.vector3s[1] == this.vector3s[(int)this.vector3s.Length - 2])
		{
			Vector3[] vector3Array1 = new Vector3[(int)this.vector3s.Length];
			Array.Copy(this.vector3s, vector3Array1, (int)this.vector3s.Length);
			vector3Array1[0] = vector3Array1[(int)vector3Array1.Length - 3];
			vector3Array1[(int)vector3Array1.Length - 1] = vector3Array1[2];
			this.vector3s = new Vector3[(int)vector3Array1.Length];
			Array.Copy(vector3Array1, this.vector3s, (int)vector3Array1.Length);
		}
		this.path = new iTween.CRSpline(this.vector3s);
		if (this.tweenArguments.Contains("speed"))
		{
			float single = iTween.PathLength(this.vector3s);
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateMoveToTargets()
	{
		Vector3 vector3;
		this.vector3s = new Vector3[3];
		if (!this.isLocal)
		{
			Vector3 vector31 = this.thisTransform.position;
			vector3 = vector31;
			this.vector3s[1] = vector31;
			this.vector3s[0] = vector3;
		}
		else
		{
			Vector3 vector32 = this.thisTransform.localPosition;
			vector3 = vector32;
			this.vector3s[1] = vector32;
			this.vector3s[0] = vector3;
		}
		if (!this.tweenArguments.Contains("position"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else if (this.tweenArguments["position"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)this.tweenArguments["position"];
			this.vector3s[1] = item.position;
		}
		else if (this.tweenArguments["position"].GetType() == typeof(Vector3))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["position"];
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GeneratePunchPositionTargets()
	{
		this.vector3s = new Vector3[] { default(Vector3), default(Vector3), default(Vector3), default(Vector3), this.thisTransform.eulerAngles };
		this.vector3s[0] = this.thisTransform.position;
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = vector3;
		this.vector3s[3] = vector3;
		this.vector3s[1] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GeneratePunchRotationTargets()
	{
		this.vector3s = new Vector3[] { this.thisTransform.eulerAngles, default(Vector3), default(Vector3), default(Vector3) };
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = vector3;
		this.vector3s[3] = vector3;
		this.vector3s[1] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GeneratePunchScaleTargets()
	{
		this.vector3s = new Vector3[] { this.thisTransform.localScale, Vector3.zero, default(Vector3) };
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GenerateRectTargets()
	{
		this.rects = new Rect[] { (Rect)this.tweenArguments["from"], (Rect)this.tweenArguments["to"], default(Rect) };
	}

	private void GenerateRotateAddTargets()
	{
		this.vector3s = new Vector3[5];
		Vector3 vector3 = this.thisTransform.eulerAngles;
		Vector3 vector31 = vector3;
		this.vector3s[3] = vector3;
		Vector3 vector32 = vector31;
		vector31 = vector32;
		this.vector3s[1] = vector32;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x += (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y += (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z += (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateRotateByTargets()
	{
		this.vector3s = new Vector3[4];
		Vector3 vector3 = this.thisTransform.eulerAngles;
		Vector3 vector31 = vector3;
		this.vector3s[3] = vector3;
		Vector3 vector32 = vector31;
		vector31 = vector32;
		this.vector3s[1] = vector32;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				ref Vector3 item = ref this.vector3s[1];
				item.x = item.x + 360f * (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				ref Vector3 vector3Pointer = ref this.vector3s[1];
				vector3Pointer.y = vector3Pointer.y + 360f * (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				ref Vector3 item1 = ref this.vector3s[1];
				item1.z = item1.z + 360f * (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] += Vector3.Scale((Vector3)this.tweenArguments["amount"], new Vector3(360f, 360f, 360f));
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateRotateToTargets()
	{
		Vector3 vector3;
		this.vector3s = new Vector3[3];
		if (!this.isLocal)
		{
			Vector3 vector31 = this.thisTransform.eulerAngles;
			vector3 = vector31;
			this.vector3s[1] = vector31;
			this.vector3s[0] = vector3;
		}
		else
		{
			Vector3 vector32 = this.thisTransform.localEulerAngles;
			vector3 = vector32;
			this.vector3s[1] = vector32;
			this.vector3s[0] = vector3;
		}
		if (!this.tweenArguments.Contains("rotation"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else if (this.tweenArguments["rotation"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)this.tweenArguments["rotation"];
			this.vector3s[1] = item.eulerAngles;
		}
		else if (this.tweenArguments["rotation"].GetType() == typeof(Vector3))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["rotation"];
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateScaleAddTargets()
	{
		this.vector3s = new Vector3[3];
		Vector3 vector3 = this.thisTransform.localScale;
		Vector3 vector31 = vector3;
		this.vector3s[1] = vector3;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x += (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y += (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z += (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateScaleByTargets()
	{
		this.vector3s = new Vector3[3];
		Vector3 vector3 = this.thisTransform.localScale;
		Vector3 vector31 = vector3;
		this.vector3s[1] = vector3;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x *= (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y *= (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z *= (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = Vector3.Scale(this.vector3s[1], (Vector3)this.tweenArguments["amount"]);
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateScaleToTargets()
	{
		this.vector3s = new Vector3[3];
		Vector3 vector3 = this.thisTransform.localScale;
		Vector3 vector31 = vector3;
		this.vector3s[1] = vector3;
		this.vector3s[0] = vector31;
		if (!this.tweenArguments.Contains("scale"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else if (this.tweenArguments["scale"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)this.tweenArguments["scale"];
			this.vector3s[1] = item.localScale;
		}
		else if (this.tweenArguments["scale"].GetType() == typeof(Vector3))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["scale"];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateShakePositionTargets()
	{
		this.vector3s = new Vector3[] { default(Vector3), default(Vector3), default(Vector3), this.thisTransform.eulerAngles };
		this.vector3s[0] = this.thisTransform.position;
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GenerateShakeRotationTargets()
	{
		this.vector3s = new Vector3[] { this.thisTransform.eulerAngles, default(Vector3), default(Vector3) };
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GenerateShakeScaleTargets()
	{
		this.vector3s = new Vector3[] { this.thisTransform.localScale, default(Vector3), default(Vector3) };
		if (!this.tweenArguments.Contains("amount"))
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		else
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
	}

	private void GenerateStabTargets()
	{
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (!base.GetComponent<AudioSource>())
		{
			base.gameObject.AddComponent<AudioSource>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.audioSource.playOnAwake = false;
		}
		else
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		this.audioSource.clip = (AudioClip)this.tweenArguments["audioclip"];
		if (this.tweenArguments.Contains("pitch"))
		{
			this.audioSource.pitch = (float)this.tweenArguments["pitch"];
		}
		if (this.tweenArguments.Contains("volume"))
		{
			this.audioSource.volume = (float)this.tweenArguments["volume"];
		}
		this.time = this.audioSource.clip.length / this.audioSource.pitch;
	}

	private void GenerateTargets()
	{
		Dictionary<string, int> strs;
		int num;
		string str;
		int num1;
		string str1 = this.type;
		if (str1 != null)
		{
			if (iTween.<>f__switch$map1C == null)
			{
				strs = new Dictionary<string, int>(10)
				{
					{ "value", 0 },
					{ "color", 1 },
					{ "audio", 2 },
					{ "move", 3 },
					{ "scale", 4 },
					{ "rotate", 5 },
					{ "shake", 6 },
					{ "punch", 7 },
					{ "look", 8 },
					{ "stab", 9 }
				};
				iTween.<>f__switch$map1C = strs;
			}
			if (iTween.<>f__switch$map1C.TryGetValue(str1, out num))
			{
				switch (num)
				{
					case 0:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map13 == null)
							{
								strs = new Dictionary<string, int>(5)
								{
									{ "float", 0 },
									{ "vector2", 1 },
									{ "vector3", 2 },
									{ "color", 3 },
									{ "rect", 4 }
								};
								iTween.<>f__switch$map13 = strs;
							}
							if (iTween.<>f__switch$map13.TryGetValue(str, out num1))
							{
								switch (num1)
								{
									case 0:
									{
										this.GenerateFloatTargets();
										this.apply = new iTween.ApplyTween(this.ApplyFloatTargets);
										break;
									}
									case 1:
									{
										this.GenerateVector2Targets();
										this.apply = new iTween.ApplyTween(this.ApplyVector2Targets);
										break;
									}
									case 2:
									{
										this.GenerateVector3Targets();
										this.apply = new iTween.ApplyTween(this.ApplyVector3Targets);
										break;
									}
									case 3:
									{
										this.GenerateColorTargets();
										this.apply = new iTween.ApplyTween(this.ApplyColorTargets);
										break;
									}
									case 4:
									{
										this.GenerateRectTargets();
										this.apply = new iTween.ApplyTween(this.ApplyRectTargets);
										break;
									}
								}
							}
						}
						break;
					}
					case 1:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map14 == null)
							{
								strs = new Dictionary<string, int>(1)
								{
									{ "to", 0 }
								};
								iTween.<>f__switch$map14 = strs;
							}
							if (iTween.<>f__switch$map14.TryGetValue(str, out num1))
							{
								if (num1 == 0)
								{
									this.GenerateColorToTargets();
									this.apply = new iTween.ApplyTween(this.ApplyColorToTargets);
								}
							}
						}
						break;
					}
					case 2:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map15 == null)
							{
								strs = new Dictionary<string, int>(1)
								{
									{ "to", 0 }
								};
								iTween.<>f__switch$map15 = strs;
							}
							if (iTween.<>f__switch$map15.TryGetValue(str, out num1))
							{
								if (num1 == 0)
								{
									this.GenerateAudioToTargets();
									this.apply = new iTween.ApplyTween(this.ApplyAudioToTargets);
								}
							}
						}
						break;
					}
					case 3:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map16 == null)
							{
								strs = new Dictionary<string, int>(3)
								{
									{ "to", 0 },
									{ "by", 1 },
									{ "add", 1 }
								};
								iTween.<>f__switch$map16 = strs;
							}
							if (iTween.<>f__switch$map16.TryGetValue(str, out num1))
							{
								if (num1 != 0)
								{
									if (num1 == 1)
									{
										this.GenerateMoveByTargets();
										this.apply = new iTween.ApplyTween(this.ApplyMoveByTargets);
									}
								}
								else if (!this.tweenArguments.Contains("path"))
								{
									this.GenerateMoveToTargets();
									this.apply = new iTween.ApplyTween(this.ApplyMoveToTargets);
								}
								else
								{
									this.GenerateMoveToPathTargets();
									this.apply = new iTween.ApplyTween(this.ApplyMoveToPathTargets);
								}
							}
						}
						break;
					}
					case 4:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map17 == null)
							{
								strs = new Dictionary<string, int>(3)
								{
									{ "to", 0 },
									{ "by", 1 },
									{ "add", 2 }
								};
								iTween.<>f__switch$map17 = strs;
							}
							if (iTween.<>f__switch$map17.TryGetValue(str, out num1))
							{
								switch (num1)
								{
									case 0:
									{
										this.GenerateScaleToTargets();
										this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
										break;
									}
									case 1:
									{
										this.GenerateScaleByTargets();
										this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
										break;
									}
									case 2:
									{
										this.GenerateScaleAddTargets();
										this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
										break;
									}
								}
							}
						}
						break;
					}
					case 5:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map18 == null)
							{
								strs = new Dictionary<string, int>(3)
								{
									{ "to", 0 },
									{ "add", 1 },
									{ "by", 2 }
								};
								iTween.<>f__switch$map18 = strs;
							}
							if (iTween.<>f__switch$map18.TryGetValue(str, out num1))
							{
								switch (num1)
								{
									case 0:
									{
										this.GenerateRotateToTargets();
										this.apply = new iTween.ApplyTween(this.ApplyRotateToTargets);
										break;
									}
									case 1:
									{
										this.GenerateRotateAddTargets();
										this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
										break;
									}
									case 2:
									{
										this.GenerateRotateByTargets();
										this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
										break;
									}
								}
							}
						}
						break;
					}
					case 6:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map19 == null)
							{
								strs = new Dictionary<string, int>(3)
								{
									{ "position", 0 },
									{ "scale", 1 },
									{ "rotation", 2 }
								};
								iTween.<>f__switch$map19 = strs;
							}
							if (iTween.<>f__switch$map19.TryGetValue(str, out num1))
							{
								switch (num1)
								{
									case 0:
									{
										this.GenerateShakePositionTargets();
										this.apply = new iTween.ApplyTween(this.ApplyShakePositionTargets);
										break;
									}
									case 1:
									{
										this.GenerateShakeScaleTargets();
										this.apply = new iTween.ApplyTween(this.ApplyShakeScaleTargets);
										break;
									}
									case 2:
									{
										this.GenerateShakeRotationTargets();
										this.apply = new iTween.ApplyTween(this.ApplyShakeRotationTargets);
										break;
									}
								}
							}
						}
						break;
					}
					case 7:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map1A == null)
							{
								strs = new Dictionary<string, int>(3)
								{
									{ "position", 0 },
									{ "rotation", 1 },
									{ "scale", 2 }
								};
								iTween.<>f__switch$map1A = strs;
							}
							if (iTween.<>f__switch$map1A.TryGetValue(str, out num1))
							{
								switch (num1)
								{
									case 0:
									{
										this.GeneratePunchPositionTargets();
										this.apply = new iTween.ApplyTween(this.ApplyPunchPositionTargets);
										break;
									}
									case 1:
									{
										this.GeneratePunchRotationTargets();
										this.apply = new iTween.ApplyTween(this.ApplyPunchRotationTargets);
										break;
									}
									case 2:
									{
										this.GeneratePunchScaleTargets();
										this.apply = new iTween.ApplyTween(this.ApplyPunchScaleTargets);
										break;
									}
								}
							}
						}
						break;
					}
					case 8:
					{
						str = this.method;
						if (str != null)
						{
							if (iTween.<>f__switch$map1B == null)
							{
								strs = new Dictionary<string, int>(1)
								{
									{ "to", 0 }
								};
								iTween.<>f__switch$map1B = strs;
							}
							if (iTween.<>f__switch$map1B.TryGetValue(str, out num1))
							{
								if (num1 == 0)
								{
									this.GenerateLookToTargets();
									this.apply = new iTween.ApplyTween(this.ApplyLookToTargets);
								}
							}
						}
						break;
					}
					case 9:
					{
						this.GenerateStabTargets();
						this.apply = new iTween.ApplyTween(this.ApplyStabTargets);
						break;
					}
				}
			}
		}
	}

	private void GenerateVector2Targets()
	{
		this.vector2s = new Vector2[] { (Vector2)this.tweenArguments["from"], (Vector2)this.tweenArguments["to"], default(Vector2) };
		if (this.tweenArguments.Contains("speed"))
		{
			Vector3 vector3 = new Vector3(this.vector2s[0].x, this.vector2s[0].y, 0f);
			Vector3 vector31 = new Vector3(this.vector2s[1].x, this.vector2s[1].y, 0f);
			float single = Math.Abs(Vector3.Distance(vector3, vector31));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GenerateVector3Targets()
	{
		this.vector3s = new Vector3[] { (Vector3)this.tweenArguments["from"], (Vector3)this.tweenArguments["to"], default(Vector3) };
		if (this.tweenArguments.Contains("speed"))
		{
			float single = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = single / (float)this.tweenArguments["speed"];
		}
	}

	private void GetEasingFunction()
	{
		switch (this.easeType)
		{
			case iTween.EaseType.easeInQuad:
			{
				this.ease = new iTween.EasingFunction(this.easeInQuad);
				break;
			}
			case iTween.EaseType.easeOutQuad:
			{
				this.ease = new iTween.EasingFunction(this.easeOutQuad);
				break;
			}
			case iTween.EaseType.easeInOutQuad:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutQuad);
				break;
			}
			case iTween.EaseType.easeInCubic:
			{
				this.ease = new iTween.EasingFunction(this.easeInCubic);
				break;
			}
			case iTween.EaseType.easeOutCubic:
			{
				this.ease = new iTween.EasingFunction(this.easeOutCubic);
				break;
			}
			case iTween.EaseType.easeInOutCubic:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutCubic);
				break;
			}
			case iTween.EaseType.easeInQuart:
			{
				this.ease = new iTween.EasingFunction(this.easeInQuart);
				break;
			}
			case iTween.EaseType.easeOutQuart:
			{
				this.ease = new iTween.EasingFunction(this.easeOutQuart);
				break;
			}
			case iTween.EaseType.easeInOutQuart:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutQuart);
				break;
			}
			case iTween.EaseType.easeInQuint:
			{
				this.ease = new iTween.EasingFunction(this.easeInQuint);
				break;
			}
			case iTween.EaseType.easeOutQuint:
			{
				this.ease = new iTween.EasingFunction(this.easeOutQuint);
				break;
			}
			case iTween.EaseType.easeInOutQuint:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutQuint);
				break;
			}
			case iTween.EaseType.easeInSine:
			{
				this.ease = new iTween.EasingFunction(this.easeInSine);
				break;
			}
			case iTween.EaseType.easeOutSine:
			{
				this.ease = new iTween.EasingFunction(this.easeOutSine);
				break;
			}
			case iTween.EaseType.easeInOutSine:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutSine);
				break;
			}
			case iTween.EaseType.easeInExpo:
			{
				this.ease = new iTween.EasingFunction(this.easeInExpo);
				break;
			}
			case iTween.EaseType.easeOutExpo:
			{
				this.ease = new iTween.EasingFunction(this.easeOutExpo);
				break;
			}
			case iTween.EaseType.easeInOutExpo:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutExpo);
				break;
			}
			case iTween.EaseType.easeInCirc:
			{
				this.ease = new iTween.EasingFunction(this.easeInCirc);
				break;
			}
			case iTween.EaseType.easeOutCirc:
			{
				this.ease = new iTween.EasingFunction(this.easeOutCirc);
				break;
			}
			case iTween.EaseType.easeInOutCirc:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutCirc);
				break;
			}
			case iTween.EaseType.linear:
			{
				this.ease = new iTween.EasingFunction(this.linear);
				break;
			}
			case iTween.EaseType.spring:
			{
				this.ease = new iTween.EasingFunction(this.spring);
				break;
			}
			case iTween.EaseType.easeInBounce:
			{
				this.ease = new iTween.EasingFunction(this.easeInBounce);
				break;
			}
			case iTween.EaseType.easeOutBounce:
			{
				this.ease = new iTween.EasingFunction(this.easeOutBounce);
				break;
			}
			case iTween.EaseType.easeInOutBounce:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutBounce);
				break;
			}
			case iTween.EaseType.easeInBack:
			{
				this.ease = new iTween.EasingFunction(this.easeInBack);
				break;
			}
			case iTween.EaseType.easeOutBack:
			{
				this.ease = new iTween.EasingFunction(this.easeOutBack);
				break;
			}
			case iTween.EaseType.easeInOutBack:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutBack);
				break;
			}
			case iTween.EaseType.easeInElastic:
			{
				this.ease = new iTween.EasingFunction(this.easeInElastic);
				break;
			}
			case iTween.EaseType.easeOutElastic:
			{
				this.ease = new iTween.EasingFunction(this.easeOutElastic);
				break;
			}
			case iTween.EaseType.easeInOutElastic:
			{
				this.ease = new iTween.EasingFunction(this.easeInOutElastic);
				break;
			}
		}
	}

	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashtables = new Hashtable((int)args.Length / 2);
		if ((int)args.Length % 2 != 0)
		{
			UnityEngine.Debug.LogError("Tween Error: Hash requires an even number of arguments!");
			return null;
		}
		for (int i = 0; i < (int)args.Length - 1; i += 2)
		{
			hashtables.Add(args[i], args[i + 1]);
		}
		return hashtables;
	}

	public static void Init(GameObject target)
	{
		iTween.MoveBy(target, Vector3.zero, 0f);
	}

	private static Vector3 Interp(Vector3[] pts, float t)
	{
		int length = (int)pts.Length - 3;
		int num = Mathf.Min(Mathf.FloorToInt(t * (float)length), length - 1);
		float single = t * (float)length - (float)num;
		Vector3 vector3 = pts[num];
		Vector3 vector31 = pts[num + 1];
		Vector3 vector32 = pts[num + 2];
		Vector3 vector33 = pts[num + 3];
		return 0.5f * (((((((-vector3 + (3f * vector31)) - (3f * vector32)) + vector33) * (single * single * single)) + (((((2f * vector3) - (5f * vector31)) + (4f * vector32)) - vector33) * (single * single))) + ((-vector3 + vector32) * single)) + (2f * vector31));
	}

	private void LateUpdate()
	{
		if (this.tweenArguments.Contains("looktarget") && this.isRunning && (this.type == "move" || this.type == "shake" || this.type == "punch"))
		{
			iTween.LookUpdate(base.gameObject, this.tweenArguments);
		}
	}

	private static void Launch(GameObject target, Hashtable args)
	{
		if (!args.Contains("id"))
		{
			args["id"] = iTween.GenerateID();
		}
		if (!args.Contains("target"))
		{
			args["target"] = target;
		}
		iTween.tweens.Insert(0, args);
		target.AddComponent<iTween>();
	}

	private float linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}

	public static void LookFrom(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookFrom(target, iTween.Hash(new object[] { "looktarget", looktarget, "time", time }));
	}

	public static void LookFrom(GameObject target, Hashtable args)
	{
		int num;
		args = iTween.CleanArgs(args);
		Vector3 vector3 = target.transform.eulerAngles;
		if (args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transforms = target.transform;
			Transform item = (Transform)args["looktarget"];
			Vector3? nullable = (Vector3?)args["up"];
			transforms.LookAt(item, (!nullable.HasValue ? iTween.Defaults.up : nullable.Value));
		}
		else if (args["looktarget"].GetType() == typeof(Vector3))
		{
			Transform transforms1 = target.transform;
			Vector3 item1 = (Vector3)args["looktarget"];
			Vector3? nullable1 = (Vector3?)args["up"];
			transforms1.LookAt(item1, (!nullable1.HasValue ? iTween.Defaults.up : nullable1.Value));
		}
		if (args.Contains("axis"))
		{
			Vector3 vector31 = target.transform.eulerAngles;
			string str = (string)args["axis"];
			if (str != null)
			{
				if (iTween.<>f__switch$map12 == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(3)
					{
						{ "x", 0 },
						{ "y", 1 },
						{ "z", 2 }
					};
					iTween.<>f__switch$map12 = strs;
				}
				if (iTween.<>f__switch$map12.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							vector31.y = vector3.y;
							vector31.z = vector3.z;
							break;
						}
						case 1:
						{
							vector31.x = vector3.x;
							vector31.z = vector3.z;
							break;
						}
						case 2:
						{
							vector31.x = vector3.x;
							vector31.y = vector3.y;
							break;
						}
					}
				}
			}
			target.transform.eulerAngles = vector31;
		}
		args["rotation"] = vector3;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void LookTo(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookTo(target, iTween.Hash(new object[] { "looktarget", looktarget, "time", time }));
	}

	public static void LookTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("looktarget") && args["looktarget"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)args["looktarget"];
			float single = item.position.x;
			float single1 = item.position.y;
			Vector3 vector3 = item.position;
			args["position"] = new Vector3(single, single1, vector3.z);
			float single2 = item.eulerAngles.x;
			float single3 = item.eulerAngles.y;
			Vector3 vector31 = item.eulerAngles;
			args["rotation"] = new Vector3(single2, single3, vector31.z);
		}
		args["type"] = "look";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void LookUpdate(GameObject target, Hashtable args)
	{
		float item;
		int num;
		iTween.CleanArgs(args);
		Vector3[] vector3Array = new Vector3[5];
		if (args.Contains("looktime"))
		{
			item = (float)args["looktime"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		else if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"] * 0.15f;
			item *= iTween.Defaults.updateTimePercentage;
		}
		vector3Array[0] = target.transform.eulerAngles;
		if (!args.Contains("looktarget"))
		{
			UnityEngine.Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
			return;
		}
		if (args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transforms = target.transform;
			Transform item1 = (Transform)args["looktarget"];
			Vector3? nullable = (Vector3?)args["up"];
			transforms.LookAt(item1, (!nullable.HasValue ? iTween.Defaults.up : nullable.Value));
		}
		else if (args["looktarget"].GetType() == typeof(Vector3))
		{
			Transform transforms1 = target.transform;
			Vector3 vector3 = (Vector3)args["looktarget"];
			Vector3? nullable1 = (Vector3?)args["up"];
			transforms1.LookAt(vector3, (!nullable1.HasValue ? iTween.Defaults.up : nullable1.Value));
		}
		vector3Array[1] = target.transform.eulerAngles;
		target.transform.eulerAngles = vector3Array[0];
		vector3Array[3].x = Mathf.SmoothDampAngle(vector3Array[0].x, vector3Array[1].x, ref vector3Array[2].x, item);
		vector3Array[3].y = Mathf.SmoothDampAngle(vector3Array[0].y, vector3Array[1].y, ref vector3Array[2].y, item);
		vector3Array[3].z = Mathf.SmoothDampAngle(vector3Array[0].z, vector3Array[1].z, ref vector3Array[2].z, item);
		target.transform.eulerAngles = vector3Array[3];
		if (args.Contains("axis"))
		{
			vector3Array[4] = target.transform.eulerAngles;
			string str = (string)args["axis"];
			if (str != null)
			{
				if (iTween.<>f__switch$map1E == null)
				{
					Dictionary<string, int> strs = new Dictionary<string, int>(3)
					{
						{ "x", 0 },
						{ "y", 1 },
						{ "z", 2 }
					};
					iTween.<>f__switch$map1E = strs;
				}
				if (iTween.<>f__switch$map1E.TryGetValue(str, out num))
				{
					switch (num)
					{
						case 0:
						{
							vector3Array[4].y = vector3Array[0].y;
							vector3Array[4].z = vector3Array[0].z;
							break;
						}
						case 1:
						{
							vector3Array[4].x = vector3Array[0].x;
							vector3Array[4].z = vector3Array[0].z;
							break;
						}
						case 2:
						{
							vector3Array[4].x = vector3Array[0].x;
							vector3Array[4].y = vector3Array[0].y;
							break;
						}
					}
				}
			}
			target.transform.eulerAngles = vector3Array[4];
		}
	}

	public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookUpdate(target, iTween.Hash(new object[] { "looktarget", looktarget, "time", time }));
	}

	public static void MoveAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveAdd(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void MoveAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	public static void MoveBy(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveBy(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void MoveBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	public static void MoveFrom(GameObject target, Vector3 position, float time)
	{
		iTween.MoveFrom(target, iTween.Hash(new object[] { "position", position, "time", time }));
	}

	public static void MoveFrom(GameObject target, Hashtable args)
	{
		bool flag;
		Vector3[] vector3Array;
		Vector3 vector3;
		Vector3 item;
		args = iTween.CleanArgs(args);
		flag = (!args.Contains("islocal") ? iTween.Defaults.isLocal : (bool)args["islocal"]);
		if (!args.Contains("path"))
		{
			if (!flag)
			{
				Vector3 vector31 = target.transform.position;
				item = vector31;
				vector3 = vector31;
			}
			else
			{
				Vector3 vector32 = target.transform.localPosition;
				item = vector32;
				vector3 = vector32;
			}
			if (!args.Contains("position"))
			{
				if (args.Contains("x"))
				{
					item.x = (float)args["x"];
				}
				if (args.Contains("y"))
				{
					item.y = (float)args["y"];
				}
				if (args.Contains("z"))
				{
					item.z = (float)args["z"];
				}
			}
			else if (args["position"].GetType() == typeof(Transform))
			{
				item = ((Transform)args["position"]).position;
			}
			else if (args["position"].GetType() == typeof(Vector3))
			{
				item = (Vector3)args["position"];
			}
			if (!flag)
			{
				target.transform.position = item;
			}
			else
			{
				target.transform.localPosition = item;
			}
			args["position"] = vector3;
		}
		else
		{
			if (args["path"].GetType() != typeof(Vector3[]))
			{
				Transform[] transformArrays = (Transform[])args["path"];
				vector3Array = new Vector3[(int)transformArrays.Length];
				for (int i = 0; i < (int)transformArrays.Length; i++)
				{
					vector3Array[i] = transformArrays[i].position;
				}
			}
			else
			{
				Vector3[] item1 = (Vector3[])args["path"];
				vector3Array = new Vector3[(int)item1.Length];
				Array.Copy(item1, vector3Array, (int)item1.Length);
			}
			if (vector3Array[(int)vector3Array.Length - 1] == target.transform.position)
			{
				if (!flag)
				{
					target.transform.position = vector3Array[0];
				}
				else
				{
					target.transform.localPosition = vector3Array[0];
				}
				args["path"] = vector3Array;
			}
			else
			{
				Vector3[] vector3Array1 = new Vector3[(int)vector3Array.Length + 1];
				Array.Copy(vector3Array, vector3Array1, (int)vector3Array.Length);
				if (!flag)
				{
					vector3Array1[(int)vector3Array1.Length - 1] = target.transform.position;
					target.transform.position = vector3Array1[0];
				}
				else
				{
					vector3Array1[(int)vector3Array1.Length - 1] = target.transform.localPosition;
					target.transform.localPosition = vector3Array1[0];
				}
				args["path"] = vector3Array1;
			}
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void MoveTo(GameObject target, Vector3 position, float time)
	{
		iTween.MoveTo(target, iTween.Hash(new object[] { "position", position, "time", time }));
	}

	public static void MoveTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("position") && args["position"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)args["position"];
			float single = item.position.x;
			float single1 = item.position.y;
			Vector3 vector3 = item.position;
			args["position"] = new Vector3(single, single1, vector3.z);
			float single2 = item.eulerAngles.x;
			float single3 = item.eulerAngles.y;
			Vector3 vector31 = item.eulerAngles;
			args["rotation"] = new Vector3(single2, single3, vector31.z);
			float single4 = item.localScale.x;
			float single5 = item.localScale.y;
			Vector3 vector32 = item.localScale;
			args["scale"] = new Vector3(single4, single5, vector32.z);
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void MoveUpdate(GameObject target, Hashtable args)
	{
		float item;
		bool flag;
		Vector3 vector3;
		iTween.CleanArgs(args);
		Vector3[] vector3Array = new Vector3[4];
		Vector3 vector31 = target.transform.position;
		if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		flag = (!args.Contains("islocal") ? iTween.Defaults.isLocal : (bool)args["islocal"]);
		if (!flag)
		{
			Vector3 vector32 = target.transform.position;
			vector3 = vector32;
			vector3Array[1] = vector32;
			vector3Array[0] = vector3;
		}
		else
		{
			Vector3 vector33 = target.transform.localPosition;
			vector3 = vector33;
			vector3Array[1] = vector33;
			vector3Array[0] = vector3;
		}
		if (!args.Contains("position"))
		{
			if (args.Contains("x"))
			{
				vector3Array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				vector3Array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				vector3Array[1].z = (float)args["z"];
			}
		}
		else if (args["position"].GetType() == typeof(Transform))
		{
			Transform transforms = (Transform)args["position"];
			vector3Array[1] = transforms.position;
		}
		else if (args["position"].GetType() == typeof(Vector3))
		{
			vector3Array[1] = (Vector3)args["position"];
		}
		vector3Array[3].x = Mathf.SmoothDamp(vector3Array[0].x, vector3Array[1].x, ref vector3Array[2].x, item);
		vector3Array[3].y = Mathf.SmoothDamp(vector3Array[0].y, vector3Array[1].y, ref vector3Array[2].y, item);
		vector3Array[3].z = Mathf.SmoothDamp(vector3Array[0].z, vector3Array[1].z, ref vector3Array[2].z, item);
		if (args.Contains("orienttopath") && (bool)args["orienttopath"])
		{
			args["looktarget"] = vector3Array[3];
		}
		if (args.Contains("looktarget"))
		{
			iTween.LookUpdate(target, args);
		}
		if (!flag)
		{
			target.transform.position = vector3Array[3];
		}
		else
		{
			target.transform.localPosition = vector3Array[3];
		}
		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 vector34 = target.transform.position;
			target.transform.position = vector31;
			target.GetComponent<Rigidbody>().MovePosition(vector34);
		}
	}

	public static void MoveUpdate(GameObject target, Vector3 position, float time)
	{
		iTween.MoveUpdate(target, iTween.Hash(new object[] { "position", position, "time", time }));
	}

	private void OnDisable()
	{
		this.DisableKinematic();
	}

	private void OnEnable()
	{
		if (this.isRunning)
		{
			this.EnableKinematic();
		}
		if (this.isPaused)
		{
			this.isPaused = false;
			if (this.delay > 0f)
			{
				this.wasPaused = true;
				this.ResumeDelay();
			}
		}
	}

	private static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		Vector3[] vector3Array = path;
		Vector3[] vector3Array1 = new Vector3[(int)vector3Array.Length + 2];
		Array.Copy(vector3Array, 0, vector3Array1, 1, (int)vector3Array.Length);
		vector3Array1[0] = vector3Array1[1] + (vector3Array1[1] - vector3Array1[2]);
		vector3Array1[(int)vector3Array1.Length - 1] = vector3Array1[(int)vector3Array1.Length - 2] + (vector3Array1[(int)vector3Array1.Length - 2] - vector3Array1[(int)vector3Array1.Length - 3]);
		if (vector3Array1[1] == vector3Array1[(int)vector3Array1.Length - 2])
		{
			Vector3[] vector3Array2 = new Vector3[(int)vector3Array1.Length];
			Array.Copy(vector3Array1, vector3Array2, (int)vector3Array1.Length);
			vector3Array2[0] = vector3Array2[(int)vector3Array2.Length - 3];
			vector3Array2[(int)vector3Array2.Length - 1] = vector3Array2[2];
			vector3Array1 = new Vector3[(int)vector3Array2.Length];
			Array.Copy(vector3Array2, vector3Array1, (int)vector3Array2.Length);
		}
		return vector3Array1;
	}

	public static float PathLength(Transform[] path)
	{
		Vector3[] vector3Array = new Vector3[(int)path.Length];
		float single = 0f;
		for (int i = 0; i < (int)path.Length; i++)
		{
			vector3Array[i] = path[i].position;
		}
		Vector3[] vector3Array1 = iTween.PathControlPointGenerator(vector3Array);
		Vector3 vector3 = iTween.Interp(vector3Array1, 0f);
		int length = (int)path.Length * 20;
		for (int j = 1; j <= length; j++)
		{
			float single1 = (float)j / (float)length;
			Vector3 vector31 = iTween.Interp(vector3Array1, single1);
			single += Vector3.Distance(vector3, vector31);
			vector3 = vector31;
		}
		return single;
	}

	public static float PathLength(Vector3[] path)
	{
		float single = 0f;
		Vector3[] vector3Array = iTween.PathControlPointGenerator(path);
		Vector3 vector3 = iTween.Interp(vector3Array, 0f);
		int length = (int)path.Length * 20;
		for (int i = 1; i <= length; i++)
		{
			float single1 = (float)i / (float)length;
			Vector3 vector31 = iTween.Interp(vector3Array, single1);
			single += Vector3.Distance(vector3, vector31);
			vector3 = vector31;
		}
		return single;
	}

	public static void Pause(GameObject target)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (_iTween.delay > 0f)
			{
				iTween _iTween1 = _iTween;
				_iTween1.delay = _iTween1.delay - (Time.time - _iTween.delayStarted);
				_iTween.StopCoroutine("TweenDelay");
			}
			_iTween.isPaused = true;
			_iTween.enabled = false;
		}
	}

	public static void Pause(GameObject target, bool includechildren)
	{
		iTween.Pause(target);
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Pause(((Transform)enumerator.Current).gameObject, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void Pause(GameObject target, string type)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				if (_iTween.delay > 0f)
				{
					iTween _iTween1 = _iTween;
					_iTween1.delay = _iTween1.delay - (Time.time - _iTween.delayStarted);
					_iTween.StopCoroutine("TweenDelay");
				}
				_iTween.isPaused = true;
				_iTween.enabled = false;
			}
		}
	}

	public static void Pause(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				if (_iTween.delay > 0f)
				{
					iTween _iTween1 = _iTween;
					_iTween1.delay = _iTween1.delay - (Time.time - _iTween.delayStarted);
					_iTween.StopCoroutine("TweenDelay");
				}
				_iTween.isPaused = true;
				_iTween.enabled = false;
			}
		}
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Pause(((Transform)enumerator.Current).gameObject, type, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void Pause()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable item = iTween.tweens[i];
			iTween.Pause((GameObject)item["target"]);
		}
	}

	public static void Pause(string type)
	{
		ArrayList arrayLists = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			GameObject item = (GameObject)iTween.tweens[i]["target"];
			arrayLists.Insert(arrayLists.Count, item);
		}
		for (int j = 0; j < arrayLists.Count; j++)
		{
			iTween.Pause((GameObject)arrayLists[j], type);
		}
	}

	public static Vector3 PointOnPath(Transform[] path, float percent)
	{
		Vector3[] vector3Array = new Vector3[(int)path.Length];
		for (int i = 0; i < (int)path.Length; i++)
		{
			vector3Array[i] = path[i].position;
		}
		return iTween.Interp(iTween.PathControlPointGenerator(vector3Array), percent);
	}

	public static Vector3 PointOnPath(Vector3[] path, float percent)
	{
		return iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	private float punch(float amplitude, float value)
	{
		float single = 9f;
		if (value == 0f)
		{
			return 0f;
		}
		if (value == 1f)
		{
			return 0f;
		}
		float single1 = 0.3f;
		single = single1 / 6.28318548f * Mathf.Asin(0f);
		return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - single) * 6.28318548f / single1);
	}

	public static void PunchPosition(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchPosition(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void PunchPosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "position";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	public static void PunchRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchRotation(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void PunchRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "rotation";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	public static void PunchScale(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchScale(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void PunchScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "scale";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	public static void PutOnPath(GameObject target, Vector3[] path, float percent)
	{
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	public static void PutOnPath(Transform target, Vector3[] path, float percent)
	{
		target.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	public static void PutOnPath(GameObject target, Transform[] path, float percent)
	{
		Vector3[] vector3Array = new Vector3[(int)path.Length];
		for (int i = 0; i < (int)path.Length; i++)
		{
			vector3Array[i] = path[i].position;
		}
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(vector3Array), percent);
	}

	public static void PutOnPath(Transform target, Transform[] path, float percent)
	{
		Vector3[] vector3Array = new Vector3[(int)path.Length];
		for (int i = 0; i < (int)path.Length; i++)
		{
			vector3Array[i] = path[i].position;
		}
		target.position = iTween.Interp(iTween.PathControlPointGenerator(vector3Array), percent);
	}

	public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed)
	{
		Rect rect = new Rect(iTween.FloatUpdate(currentValue.x, targetValue.x, speed), iTween.FloatUpdate(currentValue.y, targetValue.y, speed), iTween.FloatUpdate(currentValue.width, targetValue.width, speed), iTween.FloatUpdate(currentValue.height, targetValue.height, speed));
		return rect;
	}

	public static void Resume(GameObject target)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			((iTween)components[i]).enabled = true;
		}
	}

	public static void Resume(GameObject target, bool includechildren)
	{
		iTween.Resume(target);
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Resume(((Transform)enumerator.Current).gameObject, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void Resume(GameObject target, string type)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				_iTween.enabled = true;
			}
		}
	}

	public static void Resume(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				_iTween.enabled = true;
			}
		}
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Resume(((Transform)enumerator.Current).gameObject, type, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void Resume()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable item = iTween.tweens[i];
			iTween.Resume((GameObject)item["target"]);
		}
	}

	public static void Resume(string type)
	{
		ArrayList arrayLists = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			GameObject item = (GameObject)iTween.tweens[i]["target"];
			arrayLists.Insert(arrayLists.Count, item);
		}
		for (int j = 0; j < arrayLists.Count; j++)
		{
			iTween.Resume((GameObject)arrayLists[j], type);
		}
	}

	private void ResumeDelay()
	{
		base.StartCoroutine("TweenDelay");
	}

	private void RetrieveArgs()
	{
		foreach (Hashtable tween in iTween.tweens)
		{
			if ((GameObject)tween["target"] != base.gameObject)
			{
				continue;
			}
			this.tweenArguments = tween;
			break;
		}
		this.id = (string)this.tweenArguments["id"];
		this.type = (string)this.tweenArguments["type"];
		this._name = (string)this.tweenArguments["name"];
		this.method = (string)this.tweenArguments["method"];
		if (!this.tweenArguments.Contains("time"))
		{
			this.time = iTween.Defaults.time;
		}
		else
		{
			this.time = (float)this.tweenArguments["time"];
		}
		if (base.GetComponent<Rigidbody>() != null)
		{
			this.physics = true;
		}
		if (!this.tweenArguments.Contains("delay"))
		{
			this.delay = iTween.Defaults.delay;
		}
		else
		{
			this.delay = (float)this.tweenArguments["delay"];
		}
		if (!this.tweenArguments.Contains("namedcolorvalue"))
		{
			this.namedcolorvalue = iTween.Defaults.namedColorValue;
		}
		else if (this.tweenArguments["namedcolorvalue"].GetType() != typeof(iTween.NamedValueColor))
		{
			try
			{
				this.namedcolorvalue = (iTween.NamedValueColor)((int)Enum.Parse(typeof(iTween.NamedValueColor), (string)this.tweenArguments["namedcolorvalue"], true));
			}
			catch
			{
				UnityEngine.Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
				this.namedcolorvalue = iTween.NamedValueColor._Color;
			}
		}
		else
		{
			this.namedcolorvalue = (iTween.NamedValueColor)((int)this.tweenArguments["namedcolorvalue"]);
		}
		if (!this.tweenArguments.Contains("looptype"))
		{
			this.loopType = iTween.LoopType.none;
		}
		else if (this.tweenArguments["looptype"].GetType() != typeof(iTween.LoopType))
		{
			try
			{
				this.loopType = (iTween.LoopType)((int)Enum.Parse(typeof(iTween.LoopType), (string)this.tweenArguments["looptype"], true));
			}
			catch
			{
				UnityEngine.Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
				this.loopType = iTween.LoopType.none;
			}
		}
		else
		{
			this.loopType = (iTween.LoopType)((int)this.tweenArguments["looptype"]);
		}
		if (!this.tweenArguments.Contains("easetype"))
		{
			this.easeType = iTween.Defaults.easeType;
		}
		else if (this.tweenArguments["easetype"].GetType() != typeof(iTween.EaseType))
		{
			try
			{
				this.easeType = (iTween.EaseType)((int)Enum.Parse(typeof(iTween.EaseType), (string)this.tweenArguments["easetype"], true));
			}
			catch
			{
				UnityEngine.Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
				this.easeType = iTween.Defaults.easeType;
			}
		}
		else
		{
			this.easeType = (iTween.EaseType)((int)this.tweenArguments["easetype"]);
		}
		if (!this.tweenArguments.Contains("space"))
		{
			this.space = iTween.Defaults.space;
		}
		else if (this.tweenArguments["space"].GetType() != typeof(Space))
		{
			try
			{
				this.space = (Space)((int)Enum.Parse(typeof(Space), (string)this.tweenArguments["space"], true));
			}
			catch
			{
				UnityEngine.Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
				this.space = iTween.Defaults.space;
			}
		}
		else
		{
			this.space = (Space)((int)this.tweenArguments["space"]);
		}
		if (!this.tweenArguments.Contains("islocal"))
		{
			this.isLocal = iTween.Defaults.isLocal;
		}
		else
		{
			this.isLocal = (bool)this.tweenArguments["islocal"];
		}
		if (!this.tweenArguments.Contains("ignoretimescale"))
		{
			this.useRealTime = iTween.Defaults.useRealTime;
		}
		else
		{
			this.useRealTime = (bool)this.tweenArguments["ignoretimescale"];
		}
		this.GetEasingFunction();
	}

	public static void RotateAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateAdd(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void RotateAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	public static void RotateBy(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateBy(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void RotateBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	public static void RotateFrom(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateFrom(target, iTween.Hash(new object[] { "rotation", rotation, "time", time }));
	}

	public static void RotateFrom(GameObject target, Hashtable args)
	{
		Vector3 vector3;
		Vector3 item;
		bool flag;
		args = iTween.CleanArgs(args);
		flag = (!args.Contains("islocal") ? iTween.Defaults.isLocal : (bool)args["islocal"]);
		if (!flag)
		{
			Vector3 vector31 = target.transform.eulerAngles;
			item = vector31;
			vector3 = vector31;
		}
		else
		{
			Vector3 vector32 = target.transform.localEulerAngles;
			item = vector32;
			vector3 = vector32;
		}
		if (!args.Contains("rotation"))
		{
			if (args.Contains("x"))
			{
				item.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				item.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				item.z = (float)args["z"];
			}
		}
		else if (args["rotation"].GetType() == typeof(Transform))
		{
			item = ((Transform)args["rotation"]).eulerAngles;
		}
		else if (args["rotation"].GetType() == typeof(Vector3))
		{
			item = (Vector3)args["rotation"];
		}
		if (!flag)
		{
			target.transform.eulerAngles = item;
		}
		else
		{
			target.transform.localEulerAngles = item;
		}
		args["rotation"] = vector3;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void RotateTo(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateTo(target, iTween.Hash(new object[] { "rotation", rotation, "time", time }));
	}

	public static void RotateTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("rotation") && args["rotation"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)args["rotation"];
			float single = item.position.x;
			float single1 = item.position.y;
			Vector3 vector3 = item.position;
			args["position"] = new Vector3(single, single1, vector3.z);
			float single2 = item.eulerAngles.x;
			float single3 = item.eulerAngles.y;
			Vector3 vector31 = item.eulerAngles;
			args["rotation"] = new Vector3(single2, single3, vector31.z);
			float single4 = item.localScale.x;
			float single5 = item.localScale.y;
			Vector3 vector32 = item.localScale;
			args["scale"] = new Vector3(single4, single5, vector32.z);
		}
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void RotateUpdate(GameObject target, Hashtable args)
	{
		bool flag;
		float item;
		iTween.CleanArgs(args);
		Vector3[] vector3Array = new Vector3[4];
		Vector3 vector3 = target.transform.eulerAngles;
		if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		flag = (!args.Contains("islocal") ? iTween.Defaults.isLocal : (bool)args["islocal"]);
		if (!flag)
		{
			vector3Array[0] = target.transform.eulerAngles;
		}
		else
		{
			vector3Array[0] = target.transform.localEulerAngles;
		}
		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transforms = (Transform)args["rotation"];
				vector3Array[1] = transforms.eulerAngles;
			}
			else if (args["rotation"].GetType() == typeof(Vector3))
			{
				vector3Array[1] = (Vector3)args["rotation"];
			}
		}
		vector3Array[3].x = Mathf.SmoothDampAngle(vector3Array[0].x, vector3Array[1].x, ref vector3Array[2].x, item);
		vector3Array[3].y = Mathf.SmoothDampAngle(vector3Array[0].y, vector3Array[1].y, ref vector3Array[2].y, item);
		vector3Array[3].z = Mathf.SmoothDampAngle(vector3Array[0].z, vector3Array[1].z, ref vector3Array[2].z, item);
		if (!flag)
		{
			target.transform.eulerAngles = vector3Array[3];
		}
		else
		{
			target.transform.localEulerAngles = vector3Array[3];
		}
		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 vector31 = target.transform.eulerAngles;
			target.transform.eulerAngles = vector3;
			target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(vector31));
		}
	}

	public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateUpdate(target, iTween.Hash(new object[] { "rotation", rotation, "time", time }));
	}

	public static void ScaleAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleAdd(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void ScaleAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	public static void ScaleBy(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleBy(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void ScaleBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	public static void ScaleFrom(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleFrom(target, iTween.Hash(new object[] { "scale", scale, "time", time }));
	}

	public static void ScaleFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		Vector3 vector3 = target.transform.localScale;
		Vector3 item = vector3;
		Vector3 vector31 = vector3;
		if (!args.Contains("scale"))
		{
			if (args.Contains("x"))
			{
				item.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				item.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				item.z = (float)args["z"];
			}
		}
		else if (args["scale"].GetType() == typeof(Transform))
		{
			item = ((Transform)args["scale"]).localScale;
		}
		else if (args["scale"].GetType() == typeof(Vector3))
		{
			item = (Vector3)args["scale"];
		}
		target.transform.localScale = item;
		args["scale"] = vector31;
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void ScaleTo(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleTo(target, iTween.Hash(new object[] { "scale", scale, "time", time }));
	}

	public static void ScaleTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("scale") && args["scale"].GetType() == typeof(Transform))
		{
			Transform item = (Transform)args["scale"];
			float single = item.position.x;
			float single1 = item.position.y;
			Vector3 vector3 = item.position;
			args["position"] = new Vector3(single, single1, vector3.z);
			float single2 = item.eulerAngles.x;
			float single3 = item.eulerAngles.y;
			Vector3 vector31 = item.eulerAngles;
			args["rotation"] = new Vector3(single2, single3, vector31.z);
			float single4 = item.localScale.x;
			float single5 = item.localScale.y;
			Vector3 vector32 = item.localScale;
			args["scale"] = new Vector3(single4, single5, vector32.z);
		}
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	public static void ScaleUpdate(GameObject target, Hashtable args)
	{
		float item;
		iTween.CleanArgs(args);
		Vector3[] vector3Array = new Vector3[4];
		if (!args.Contains("time"))
		{
			item = iTween.Defaults.updateTime;
		}
		else
		{
			item = (float)args["time"];
			item *= iTween.Defaults.updateTimePercentage;
		}
		Vector3 vector3 = target.transform.localScale;
		Vector3 vector31 = vector3;
		vector3Array[1] = vector3;
		vector3Array[0] = vector31;
		if (!args.Contains("scale"))
		{
			if (args.Contains("x"))
			{
				vector3Array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				vector3Array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				vector3Array[1].z = (float)args["z"];
			}
		}
		else if (args["scale"].GetType() == typeof(Transform))
		{
			Transform transforms = (Transform)args["scale"];
			vector3Array[1] = transforms.localScale;
		}
		else if (args["scale"].GetType() == typeof(Vector3))
		{
			vector3Array[1] = (Vector3)args["scale"];
		}
		vector3Array[3].x = Mathf.SmoothDamp(vector3Array[0].x, vector3Array[1].x, ref vector3Array[2].x, item);
		vector3Array[3].y = Mathf.SmoothDamp(vector3Array[0].y, vector3Array[1].y, ref vector3Array[2].y, item);
		vector3Array[3].z = Mathf.SmoothDamp(vector3Array[0].z, vector3Array[1].z, ref vector3Array[2].z, item);
		target.transform.localScale = vector3Array[3];
	}

	public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleUpdate(target, iTween.Hash(new object[] { "scale", scale, "time", time }));
	}

	public static void ShakePosition(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakePosition(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void ShakePosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "position";
		iTween.Launch(target, args);
	}

	public static void ShakeRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeRotation(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void ShakeRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "rotation";
		iTween.Launch(target, args);
	}

	public static void ShakeScale(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeScale(target, iTween.Hash(new object[] { "amount", amount, "time", time }));
	}

	public static void ShakeScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "scale";
		iTween.Launch(target, args);
	}

	private float spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * 3.14159274f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
		return start + (end - start) * value;
	}

	public static void Stab(GameObject target, AudioClip audioclip, float delay)
	{
		iTween.Stab(target, iTween.Hash(new object[] { "audioclip", audioclip, "delay", delay }));
	}

	public static void Stab(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "stab";
		iTween.Launch(target, args);
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		iTween.<Start>c__Iterator13 variable = null;
		return variable;
	}

	public static void Stop()
	{
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			Hashtable item = iTween.tweens[i];
			iTween.Stop((GameObject)item["target"]);
		}
		iTween.tweens.Clear();
	}

	public static void Stop(string type)
	{
		ArrayList arrayLists = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			GameObject item = (GameObject)iTween.tweens[i]["target"];
			arrayLists.Insert(arrayLists.Count, item);
		}
		for (int j = 0; j < arrayLists.Count; j++)
		{
			iTween.Stop((GameObject)arrayLists[j], type);
		}
	}

	public static void Stop(GameObject target)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			((iTween)components[i]).Dispose();
		}
	}

	public static void Stop(GameObject target, bool includechildren)
	{
		iTween.Stop(target);
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Stop(((Transform)enumerator.Current).gameObject, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void Stop(GameObject target, string type)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				_iTween.Dispose();
			}
		}
	}

	public static void Stop(GameObject target, string type, bool includechildren)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (string.Concat(_iTween.type, _iTween.method).Substring(0, type.Length).ToLower() == type.ToLower())
			{
				_iTween.Dispose();
			}
		}
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.Stop(((Transform)enumerator.Current).gameObject, type, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	public static void StopByName(string name)
	{
		ArrayList arrayLists = new ArrayList();
		for (int i = 0; i < iTween.tweens.Count; i++)
		{
			GameObject item = (GameObject)iTween.tweens[i]["target"];
			arrayLists.Insert(arrayLists.Count, item);
		}
		for (int j = 0; j < arrayLists.Count; j++)
		{
			iTween.StopByName((GameObject)arrayLists[j], name);
		}
	}

	public static void StopByName(GameObject target, string name)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (_iTween._name == name)
			{
				_iTween.Dispose();
			}
		}
	}

	public static void StopByName(GameObject target, string name, bool includechildren)
	{
		Component[] components = target.GetComponents<iTween>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			iTween _iTween = (iTween)components[i];
			if (_iTween._name == name)
			{
				_iTween.Dispose();
			}
		}
		if (includechildren)
		{
			IEnumerator enumerator = target.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					iTween.StopByName(((Transform)enumerator.Current).gameObject, name, true);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}
	}

	private void TweenComplete()
	{
		this.isRunning = false;
		if (this.percentage <= 0.5f)
		{
			this.percentage = 0f;
		}
		else
		{
			this.percentage = 1f;
		}
		this.apply();
		if (this.type == "value")
		{
			this.CallBack("onupdate");
		}
		if (this.loopType != iTween.LoopType.none)
		{
			this.TweenLoop();
		}
		else
		{
			this.Dispose();
		}
		this.CallBack("oncomplete");
	}

	[DebuggerHidden]
	private IEnumerator TweenDelay()
	{
		iTween.<TweenDelay>c__Iterator11 variable = null;
		return variable;
	}

	private void TweenLoop()
	{
		this.DisableKinematic();
		iTween.LoopType loopType = this.loopType;
		if (loopType == iTween.LoopType.loop)
		{
			this.percentage = 0f;
			this.runningTime = 0f;
			this.apply();
			base.StartCoroutine("TweenRestart");
		}
		else if (loopType == iTween.LoopType.pingPong)
		{
			this.reverse = !this.reverse;
			this.runningTime = 0f;
			base.StartCoroutine("TweenRestart");
		}
	}

	[DebuggerHidden]
	private IEnumerator TweenRestart()
	{
		iTween.<TweenRestart>c__Iterator12 variable = null;
		return variable;
	}

	private void TweenStart()
	{
		this.CallBack("onstart");
		if (!this.loop)
		{
			this.ConflictCheck();
			this.GenerateTargets();
		}
		if (this.type == "stab")
		{
			this.audioSource.PlayOneShot(this.audioSource.clip);
		}
		if (this.type == "move" || this.type == "scale" || this.type == "rotate" || this.type == "punch" || this.type == "shake" || this.type == "curve" || this.type == "look")
		{
			this.EnableKinematic();
		}
		this.isRunning = true;
	}

	private void TweenUpdate()
	{
		this.apply();
		this.CallBack("onupdate");
		this.UpdatePercentage();
	}

	private void Update()
	{
		if (this.isRunning && !this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage >= 1f)
				{
					this.TweenComplete();
				}
				else
				{
					this.TweenUpdate();
				}
			}
			else if (this.percentage <= 0f)
			{
				this.TweenComplete();
			}
			else
			{
				this.TweenUpdate();
			}
		}
	}

	private void UpdatePercentage()
	{
		if (!this.useRealTime)
		{
			this.runningTime += Time.deltaTime;
		}
		else
		{
			iTween _iTween = this;
			_iTween.runningTime = _iTween.runningTime + (Time.realtimeSinceStartup - this.lastRealTime);
		}
		if (!this.reverse)
		{
			this.percentage = this.runningTime / this.time;
		}
		else
		{
			this.percentage = 1f - this.runningTime / this.time;
		}
		this.lastRealTime = Time.realtimeSinceStartup;
	}

	public static void ValueTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
		{
			UnityEngine.Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return;
		}
		args["type"] = "value";
		if (args["from"].GetType() == typeof(Vector2))
		{
			args["method"] = "vector2";
		}
		else if (args["from"].GetType() == typeof(Vector3))
		{
			args["method"] = "vector3";
		}
		else if (args["from"].GetType() == typeof(Rect))
		{
			args["method"] = "rect";
		}
		else if (args["from"].GetType() != typeof(float))
		{
			if (args["from"].GetType() != typeof(Color))
			{
				UnityEngine.Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
				return;
			}
			args["method"] = "color";
		}
		else
		{
			args["method"] = "float";
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		iTween.Launch(target, args);
	}

	public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
	{
		Vector2 vector2 = targetValue - currentValue;
		currentValue = currentValue + ((vector2 * speed) * Time.deltaTime);
		return currentValue;
	}

	public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
	{
		Vector3 vector3 = targetValue - currentValue;
		currentValue = currentValue + ((vector3 * speed) * Time.deltaTime);
		return currentValue;
	}

	private delegate void ApplyTween();

	private class CRSpline
	{
		public Vector3[] pts;

		public CRSpline(params Vector3[] pts)
		{
			this.pts = new Vector3[(int)pts.Length];
			Array.Copy(pts, this.pts, (int)pts.Length);
		}

		public Vector3 Interp(float t)
		{
			int length = (int)this.pts.Length - 3;
			int num = Mathf.Min(Mathf.FloorToInt(t * (float)length), length - 1);
			float single = t * (float)length - (float)num;
			Vector3 vector3 = this.pts[num];
			Vector3 vector31 = this.pts[num + 1];
			Vector3 vector32 = this.pts[num + 2];
			Vector3 vector33 = this.pts[num + 3];
			return 0.5f * (((((((-vector3 + (3f * vector31)) - (3f * vector32)) + vector33) * (single * single * single)) + (((((2f * vector3) - (5f * vector31)) + (4f * vector32)) - vector33) * (single * single))) + ((-vector3 + vector32) * single)) + (2f * vector31));
		}
	}

	public static class Defaults
	{
		public static float time;

		public static float delay;

		public static iTween.NamedValueColor namedColorValue;

		public static iTween.LoopType loopType;

		public static iTween.EaseType easeType;

		public static float lookSpeed;

		public static bool isLocal;

		public static Space space;

		public static bool orientToPath;

		public static Color color;

		public static float updateTimePercentage;

		public static float updateTime;

		public static int cameraFadeDepth;

		public static float lookAhead;

		public static bool useRealTime;

		public static Vector3 up;

		static Defaults()
		{
			iTween.Defaults.time = 1f;
			iTween.Defaults.delay = 0f;
			iTween.Defaults.namedColorValue = iTween.NamedValueColor._Color;
			iTween.Defaults.loopType = iTween.LoopType.none;
			iTween.Defaults.easeType = iTween.EaseType.easeOutExpo;
			iTween.Defaults.lookSpeed = 3f;
			iTween.Defaults.isLocal = false;
			iTween.Defaults.space = Space.Self;
			iTween.Defaults.orientToPath = false;
			iTween.Defaults.color = Color.white;
			iTween.Defaults.updateTimePercentage = 0.05f;
			iTween.Defaults.updateTime = 1f * iTween.Defaults.updateTimePercentage;
			iTween.Defaults.cameraFadeDepth = 999999;
			iTween.Defaults.lookAhead = 0.05f;
			iTween.Defaults.useRealTime = false;
			iTween.Defaults.up = Vector3.up;
		}
	}

	public enum EaseType
	{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		linear,
		spring,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
		easeInBack,
		easeOutBack,
		easeInOutBack,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
		punch
	}

	private delegate float EasingFunction(float start, float end, float Value);

	public enum LoopType
	{
		none,
		loop,
		pingPong
	}

	public enum NamedValueColor
	{
		_Color,
		_SpecColor,
		_Emission,
		_ReflectColor
	}
}