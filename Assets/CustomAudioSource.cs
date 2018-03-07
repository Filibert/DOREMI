using UnityEngine;

class CustomAudioSource : MonoBehaviour {
	[Range(0, 1)]
	public float Volume = 1.0f;

	// NOTE: Below and above those values, the sound kind of starts to degrade.
	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;
	
	
	[HideInInspector]
	public FMOD.Channel Channel;

	private FMOD.System _system;
	private FMOD.DSP _pitchShift;
	
	private float _defaultFrequency;

	// NOTE: Technically, we should check which one it is with
	// getNumParameters and getParameterDescription, but... come on!
	private const int PITCH_INDEX = 0;

	void Start() {
		_system = AudioMixer.Instance.FMODSystem;
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

	public void SetSpeed(float speed) {
		if (Channel != null) {
			Channel.setFrequency(_defaultFrequency * speed);
			_pitchShift.setParameterFloat(PITCH_INDEX, 1.0f / speed);
		}
		
		Speed = speed;
	}

	public void Play(FMOD.Sound track) {
		// TODO: Replace hard-coded values by parameters.
		_system.playSound(track, null, false, out Channel);

		Channel.getFrequency(out _defaultFrequency);
		
		_system.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, out _pitchShift);
		FMODUtils.ERRCHECK(Channel.addDSP(0, _pitchShift));

		Debug.Log(_defaultFrequency);
	}

	void OnDestroy() {
		Channel.removeDSP(_pitchShift);
		_pitchShift.release();
	}
}
