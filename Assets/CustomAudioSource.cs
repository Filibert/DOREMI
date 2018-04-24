using System;
using System.Runtime.InteropServices;
using FMOD;
using UnityEngine;
using UnityEngine.Collections;
using Debug = UnityEngine.Debug;

[AddComponentMenu("CustomAudio/Custom Audio Source")]

public class CustomAudioSource : MonoBehaviour {
	[Range(0, 1.5f)]
	public float Volume = 1.0f;

	public bool Mute = false;

	// NOTE: Below and above those values, the sound kind of starts to degrade.
	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;
	
	
	public FMOD.Channel Channel;

	private FMOD.System _system;
	private FMOD.DSP _pitchShift;
	private FMOD.DSP _getWaveData;
	
	public FMOD.Sound Sound { get; private set;}
	
	private float _defaultFrequency;

	// NOTE: Technically, we should check which one it is with
	// getNumParameters and getParameterDescription, but... come on!
	private const int PITCH_INDEX = 0;

	void Awake() {
		_system = AudioMixer.Instance.FMODSystem;
	}

	void Update() {
		FMOD.VECTOR pos         = FMODUtils.Vector3ToFMOD(transform.position);
		FMOD.VECTOR vel         = FMODUtils.Vector3ToFMOD(Vector3.zero); // Only needed if we use doppler effect.
		FMOD.VECTOR alt_pan_pos = FMODUtils.Vector3ToFMOD(Vector3.zero); // FIXME: I do not know what this is.

		if (Volume < 0) Volume = 0;
		if (Speed < 0)  Speed  = 0;

		if (Channel != null) {
			Channel.set3DAttributes(ref pos, ref vel, ref alt_pan_pos);
			Channel.setVolume(Volume);

			Channel.setMute(Mute);

			SetSpeed(Speed);
		}
	}

	public void SetSound(FMOD.Sound sound)
	{
		if (Channel != null) {
			Channel.stop();
		}
		
		Sound = sound;
	}

	public void SetSpeed(float speed) {
		if (Channel != null) {
			Channel.setFrequency(_defaultFrequency * speed);
			_pitchShift.setParameterFloat(PITCH_INDEX, 1.0f / speed);
		}
		
		Speed = speed;
	}

	public void Play(FMOD.Sound sound) {
		SetSound(sound);
		Play();
	}

	public void Play() {
		// TODO: Replace hard-coded values by parameters.
		_system.playSound(Sound, null, false, out Channel);

		Channel.getFrequency(out _defaultFrequency);
		
		_system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, out _pitchShift);
		FMODUtils.ERRCHECK(Channel.addDSP(0, _pitchShift));
	}

    public void JoinReference(CustomAudioSource reference)
    {
       uint currentPosition, currentReferencePosition;
        Channel.getPosition(out currentPosition, TIMEUNIT.PCM);
        reference.Channel.getPosition(out currentReferencePosition, TIMEUNIT.PCM);
        var gapRatio = Mathf.Round((currentPosition /(float)currentReferencePosition)*1000)/1000;

        Speed = reference.Speed - (gapRatio - 1)*10;
        Volume = reference.Volume - Mathf.Abs(gapRatio - 1) * 5;
        if (currentPosition - currentReferencePosition >= 3 || currentPosition - currentReferencePosition == 0) return;
        Channel.setPosition(currentReferencePosition, TIMEUNIT.PCM);
        Speed = reference.Speed;
    }

    void OnDestroy() {
		if (Channel != null) {
			Channel.stop();
			Channel.removeDSP(_pitchShift);
		}

		if (_pitchShift != null) {
			_pitchShift.release();
		}
	}
}
