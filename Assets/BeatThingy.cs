using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatThingy : MonoBehaviour, Pulseable
{
	public Material Bright;
	public Material Dim;

	public float FadeFactor = 40.0f;

	public CustomAudioSource Reference;

	public Beat MyBeat {get; private set;}

	private Material _ownBright;
	
	private Color _defaultBrightColor;
	private int _vertexIndex = 0;

	void Start()
	{
		_defaultBrightColor = Bright.color;

		MyBeat = gameObject.AddComponent<Beat>() as Beat;
		MyBeat.ThingToNotify = this;

		_ownBright = new Material(Bright);

		int childCount = transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			GameObject vertex = transform.GetChild(i).gameObject;
			vertex.GetComponent<Renderer>().material = Dim;
		}
	}
	
	void Update()
	{
		_ownBright.color = Color.Lerp(_ownBright.color, Dim.color, 1 / FadeFactor);
	}

	void FixedUpdate()
	{
		if (MyBeat != null)
		{
			MyBeat.Speed = Reference.Speed;
		}
	}

	public void OnPulse()
	{
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			GameObject vertex = transform.GetChild(i).gameObject;
			vertex.GetComponent<Renderer>().material = (i == _vertexIndex) ? _ownBright : Dim;
		}
		
		_ownBright.color = _defaultBrightColor;
		
		++_vertexIndex;

		if (_vertexIndex >= childCount)
		{
			_vertexIndex = 0;
		}
	}
}
