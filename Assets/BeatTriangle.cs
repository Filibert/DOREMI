using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatTriangle : MonoBehaviour, Pulseable
{
	public GameObject TrianglePrefab;
	public Material Bright;
	public Material Dim;

	public float FadeFactor = 40.0f;

	public Orchestra _orchestra; // TODO: Change this to the desired user's speed.

	public Beat MyBeat {get; private set;}

	private GameObject _triangle;
	
	private Material _ownBright;
	
	private Color _defaultBrightColor;
	private int _vertexIndex = 0;

	void Start()
	{
		_defaultBrightColor = Bright.color;
		
		_triangle = Instantiate(TrianglePrefab);
		_triangle.transform.SetParent(transform);
		_triangle.transform.localPosition = Vector3.zero;

		MyBeat = gameObject.AddComponent<Beat>() as Beat;
		MyBeat.ThingToNotify = this;

		_ownBright = new Material(Bright);

		for (int i = 0; i < 3; ++i)
		{
			GameObject vertex = _triangle.transform.GetChild(i).gameObject;
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
			MyBeat.Speed = _orchestra.Speed;
		}
	}

	public void OnPulse()
	{
		for (int i = 0; i < 3; ++i)
		{
			GameObject vertex = _triangle.transform.GetChild(i).gameObject;
			vertex.GetComponent<Renderer>().material = (i == _vertexIndex) ? _ownBright : Dim;
		}
		
		_ownBright.color = _defaultBrightColor;
		
		++_vertexIndex;

		if (_vertexIndex > 2)
		{
			_vertexIndex = 0;
		}
	}
}
