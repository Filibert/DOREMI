using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("CustomAudio/Custom Audio Source")]

unsafe public class CustomAudioSource : MonoBehaviour {
	[Range(0, 1.5f)]
	public float Volume = 1.0f;

	// NOTE: Below and above those values, the sound kind of starts to degrade.
	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;
	
	
	public FMOD.Channel Channel;

	// TODO: Change this to float[] (so this _whole_ class (and every
	// code that uses this value) does not need to be 'unsafe').
	[HideInInspector]
	public float *WaveData { get; private set; }
	
	private FMOD.System _system;
	private FMOD.DSP _pitchShift;
	private FMOD.DSP _getWaveData;
	
	public FMOD.Sound Sound { get; private set;}
	
	private float _defaultFrequency;
	private IntPtr _waveData;

	// NOTE: Technically, we should check which one it is with
	// getNumParameters and getParameterDescription, but... come on!
	private const int PITCH_INDEX = 0;

	void Awake() {
		_system = AudioMixer.Instance.FMODSystem;
		_waveData = Marshal.AllocHGlobal(AudioMixer.DSP_BUFFER_SIZE * sizeof(float));
		WaveData = (float *)_waveData.ToPointer();
	}

	void Update() {
		FMOD.VECTOR pos         = FMODUtils.Vector3ToFMOD(transform.position);
		FMOD.VECTOR vel         = FMODUtils.Vector3ToFMOD(Vector3.zero); // Only needed if we use doppler effect.
		FMOD.VECTOR alt_pan_pos = FMODUtils.Vector3ToFMOD(Vector3.zero); // FIXME: I do not know what this is.

		if (Channel != null) {
			Channel.set3DAttributes(ref pos, ref vel, ref alt_pan_pos);
			Channel.setVolume(Volume);

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

		FMOD.DSP_DESCRIPTION dspDesc = FMOD_GetWaveDataDSP.CreateDSPDesc(_waveData);
		FMODUtils.ERRCHECK(_system.createDSP(ref dspDesc, out _getWaveData));

		FMODUtils.ERRCHECK(Channel.addDSP(1, _getWaveData));
	}

	void OnDestroy() {
		if (Channel != null) {
			Channel.stop();
			Channel.removeDSP(_pitchShift);
			Channel.removeDSP(_getWaveData);
		}

		if (_pitchShift != null) {
			_pitchShift.release();
		}

		if (_getWaveData != null) {
			_getWaveData.release();
		}

		WaveData = null;
		Marshal.FreeHGlobal(_waveData);
		_waveData = IntPtr.Zero;
	}
}
