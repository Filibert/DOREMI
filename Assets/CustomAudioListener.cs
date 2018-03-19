using UnityEngine;

[AddComponentMenu("CustomAudio/Custom Audio Listener")]
class CustomAudioListener : MonoBehaviour
{
	FMOD.System _system;
	int _id;
	
	void Start() {
		_id = AudioMixer.Instance.AddListener();
		_system = AudioMixer.Instance.FMODSystem;
	}
	
	void Update() {
		if (_id != -1) {
			Quaternion rotation = transform.rotation;
		
			FMOD.VECTOR pos     = FMODUtils.Vector3ToFMOD(transform.position);
			FMOD.VECTOR vel     = FMODUtils.Vector3ToFMOD(Vector3.zero); // Only needed if we use doppler effect.
			FMOD.VECTOR forward = FMODUtils.Vector3ToFMOD(rotation * Vector3.forward);
			FMOD.VECTOR up      = FMODUtils.Vector3ToFMOD(rotation * Vector3.up);

			_system.set3DListenerAttributes(_id, ref pos, ref vel, ref forward, ref up);
		}
	}
}
