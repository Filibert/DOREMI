using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserProgressionScript : MonoBehaviour {
	private CustomAudioSource _mutedSourceJustForUserSpeed;
	
	private Text _userProgression;
	private TimeSpan _totalTimeTimeSpan;

	private uint _i = 0;

	void Start()
	{
		_mutedSourceJustForUserSpeed = GameObject.Find("MutedSourceJustForDefaultSpeed").GetComponent<CustomAudioSource>();

		FMOD.Sound sound = _mutedSourceJustForUserSpeed.Sound;
			
		if (sound != null)
		{
			uint totalTime;
			_mutedSourceJustForUserSpeed.Sound.getLength(out totalTime, FMOD.TIMEUNIT.MS);
			_totalTimeTimeSpan = TimeSpan.FromMilliseconds(totalTime);
		}

		_userProgression = GetComponent<Text>();
	}
	
	void LateUpdate()
	{
		FMOD.Channel channel = _mutedSourceJustForUserSpeed.Channel;
		TimeSpan timespan = TimeSpan.FromMilliseconds(0);
		
		if (channel != null)
		{
			uint position;
			channel.getPosition(out position, FMOD.TIMEUNIT.MS);

			timespan = TimeSpan.FromMilliseconds(position);
		}

		_userProgression.text = String.Format("{0:D2}:{1:D2} / {2:D2}:{3:D2}", 
											  timespan.Minutes,  timespan.Seconds,
											  _totalTimeTimeSpan.Minutes,  _totalTimeTimeSpan.Seconds);
	}
}
