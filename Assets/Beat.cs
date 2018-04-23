using System;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
	public static readonly float TEMPO_DURATION_IN_SECONDS = 0.75f;

	[HideInInspector]
	public Pulseable ThingToNotify;
	
	[HideInInspector]
	public float Speed = 1.0f;
	
	private float _timeSinceLastPulse = TEMPO_DURATION_IN_SECONDS;
	private bool _running = false;
	
	public void Run()
	{
		_running = true;
	}

	public void Pause()
	{
		_running = false;
	}

	public void Reset()
	{
		_timeSinceLastPulse = TEMPO_DURATION_IN_SECONDS;
		ThingToNotify.OnReset();
	}
	
	void FixedUpdate()
	{
		if (_running) {
			_timeSinceLastPulse -= Time.fixedDeltaTime * Speed;
		
			if (_timeSinceLastPulse <= 0)
			{
				Pulse();
			
				_timeSinceLastPulse += TEMPO_DURATION_IN_SECONDS;
			}
		}
	}
	
	void Pulse()
	{
		ThingToNotify.OnPulse();
	}
}
