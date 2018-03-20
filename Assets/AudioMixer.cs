using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// NOTE/IMPORTANT: Because of how things currently work (see Awake), no audio
// code should ever be used in an Awake method.
// Beware!
class AudioMixer : MonoBehaviour
{
	public const int DSP_BUFFER_SIZE = 256;
	
	
	public FMOD.System FMODSystem;
	public static AudioMixer Instance;

	// FIXME: When a listener is destroyed, we need to change the id
	// of every other listener with an id above this one.
	public int AddListener() {
		if (_listenerCount < FMOD.CONSTANTS.MAX_LISTENERS) {
			_listenerCount += 1;
			FMODSystem.set3DNumListeners(_listenerCount);
			
			return _listenerCount - 1;
		}

		return -1;
	}

	public FMOD.Sound Load(string filename) {
		FMOD.Sound result;
		
		// We only use spatialization (hence 3D mode).
		if (FMODUtils.ERRCHECK(FMODSystem.createStream(filename, FMOD.MODE._3D, out result))) {
			return null;
		}

		return result;
	}
	

	private int _listenerCount; // I do not even know why I did
								// that. We only have the camera
								// anyway...

	[DllImport("kernel32.dll")]
	private static extern IntPtr LoadLibrary(string dll);
	
	void Awake() {
		// FIXME: I do not know what's the best way to do this.
		// I need a way to make sure this is called before any use of
		// AudioMixer.
		// (So FMOD is loaded and initialised)
		// And I also need to be able to call FMODSystem.update at
		// each update (so spatialization works), and
		// FMODSystem.release (because FMOD is not fully reset when
		// editing) when the game closes.
		// This is my current solution. Feel free to improve it.
		if (Instance == null) {
			Instance = this;

			// So we do not need to reload FMOD between scenes.
			DontDestroyOnLoad(gameObject);

#if WIN64
			string dll = "fmod64";
#else
			string dll = "fmod";
#endif
			
			var path = System.IO.Path.GetFullPath("Assets\\" + dll);
			LoadLibrary(path);

			if (FMODUtils.ERRCHECK(FMOD.Factory.System_Create(out FMODSystem))) {
				Debug.LogError("Failed to create FMOD system.");
				return;
			}

			// TODO: I do not know what any of these values are! (except
			// FMOD.INITFLAGS.NORMAL)
			FMODSystem.setDSPBufferSize(DSP_BUFFER_SIZE, 10);
			FMODSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)0);
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}
	}

	void Update() {
		FMODSystem.update();
	}

	void OnDestroy() {
		FMODSystem.release();
	}
}
