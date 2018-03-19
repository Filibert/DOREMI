using UnityEngine;

class FMODUtils
{
	// Return true on error (and log the error).
	// Return false otherwhise.
	// NOTE: This could be inverted. I don't care.
	public static bool ERRCHECK(FMOD.RESULT r) {
		if (r != FMOD.RESULT.OK) {
			Debug.LogError("FMOD error: " + r);
			return true;
		}

		return false;
	}

	public static FMOD.VECTOR Vector3ToFMOD(Vector3 v) {
		FMOD.VECTOR w;
		w.x = v.x;
		w.y = v.y;
		w.z = v.z;
		
		return w;
	}
}
