using UnityEngine;

class CustomAudioSource : MonoBehaviour {
	[Range(0, 1)]
	public float Volume = 1.0f;
	// TODO: Add tempo, that's what we are all waiting for!
	
	[HideInInspector]
	public FMOD.Channel Channel;
	
	void Update() {
		FMOD.VECTOR pos         = FMODUtils.Vector3ToFMOD(transform.position);
		FMOD.VECTOR vel         = FMODUtils.Vector3ToFMOD(Vector3.zero); // Only needed if we use doppler effect.
		FMOD.VECTOR alt_pan_pos = FMODUtils.Vector3ToFMOD(Vector3.zero); // FIXME: I do not know what this is.

		if (Channel != null) {
			Channel.set3DAttributes(ref pos, ref vel, ref alt_pan_pos);
			Channel.setVolume(Volume);
		}
	}
}
