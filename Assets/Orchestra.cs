using UnityEngine;
using System.Collections.Generic;

public class Orchestra : MonoBehaviour
{
	[Range(0, 1.5f)]
	public float Volume = 1.0f;

	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;

	private float _oldVolume;
	
    [SerializeField]
	List<CustomAudioSource> _sources = new List<CustomAudioSource>();

	void Awake() {
		_oldVolume = Volume;
        
	}

	void Update() {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var s in _sources)
            {
                Debug.Log(s.name);
                s.Play();
            }
        }

		if (_oldVolume != Volume) {
			foreach (var s in _sources) {
				s.Volume = Volume;
			}
		}
		
		foreach (var s in _sources) {
			s.Speed = Speed;
		}

		_oldVolume = Volume;
	}

	public void AddSource(CustomAudioSource source) {
            _sources.Add(source);
	}

	public void Reset() {
		_sources.Clear();
		
		Volume = 1.0f;
		Speed = 1.0f;
	}

	void OnDestroy() {
		_sources.Clear();
	}
}
