using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

class Test : MonoBehaviour
{
	private AudioMixer _audioMixer;
	private CustomAudioSource _source;
	private FMOD.Sound _track;

    void Start() {
		_audioMixer = AudioMixer.Instance;
		_source = GetComponent<CustomAudioSource>();
		
		// Get the first track in Assets/Resources.
		foreach (var item in Directory.GetDirectories(Directory.GetCurrentDirectory() + "/Assets/Resources/").ToList()) {
			foreach (var track in Directory.GetFiles(item).Where(n => Path.GetExtension(n) == ".wav")) {
				_track = _audioMixer.Load(track);
				break;
			}
			
			break;
	    }

		_source.Play(_track);
		_source.Channel.setMode(FMOD.MODE.LOOP_NORMAL);
		_source.Channel.setLoopCount(-1);
	}
}
