using UnityEngine;
using System.Collections.Generic;

public class Orchestra : MonoBehaviour
{
	[Range(0, 1.5f)]
	public float Volume = 1.0f;

	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;

	[Range(0.0f, 0.3f)]
	public float SpeedVariationOnDesync = 0.1f;

	[HideInInspector]
	public bool Running {get; private set;}

	public BeatThingy UserTempoFeedback;
	public BeatThingy BeatButtonPrefab;
	public CustomAudioSource MutedSourceJustForDefaultSpeed;

	private float _oldVolume;
	private float _oldSpeed;

	private NormDistrib _tempoDegradationDistrib = new NormDistrib(15, 3);
	private float _timeUntilTempoDegradation;
	
    [SerializeField]
	List<CustomAudioSource> _sources = new List<CustomAudioSource>();
	List<BeatThingy> _buttons = new List<BeatThingy>();

    void Awake() {
		_oldVolume = Volume;
		_oldSpeed = Speed;

		MutedSourceJustForDefaultSpeed.Volume = 0;
		MutedSourceJustForDefaultSpeed.Speed = 1.0f;

		UserTempoFeedback.Reference = MutedSourceJustForDefaultSpeed;
	}

	void ResetTempoDegrationTime()
	{
		_timeUntilTempoDegradation = _tempoDegradationDistrib.Value();
	}

	void FixedUpdate()
	{
		if (Running) // TODO: And not player is dictating tempo
		{
			_timeUntilTempoDegradation -= Time.fixedDeltaTime;

			if (_timeUntilTempoDegradation <= 0)
			{
				int randomSourceToDesynchronize = Random.Range(0, _sources.Count);
				float variation = Random.Range(-SpeedVariationOnDesync, SpeedVariationOnDesync);
				_sources[randomSourceToDesynchronize].Speed += variation;
				
				ResetTempoDegrationTime();
			}
		}
	}

	void Update() {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var s in _sources)
            {
                Debug.Log(s.name);
                s.Play();
                MusicSelector.Source = s;
            }

			foreach (var b in _buttons)
            {
				b.MyBeat.Reset();
                b.MyBeat.Run();
            }

			UserTempoFeedback.MyBeat.Reset();
			UserTempoFeedback.MyBeat.Run();

			ResetTempoDegrationTime();

			Running = true;
        }

		if (_oldVolume != Volume) {
			foreach (var s in _sources) {
				s.Volume = Volume;
			}
		}

		if (_oldSpeed != Speed) {
			foreach (var s in _sources) {
				s.Speed = Speed;
			}
		}

		_oldVolume = Volume;
		_oldSpeed = Speed;
	}

	public void AddSource(CustomAudioSource source) {
		_sources.Add(source);

		BeatThingy button = Instantiate(BeatButtonPrefab);
		button.Reference = source;

		Vector3 buttonOffset = new Vector3(0, -100, 0);

		button.transform.SetParent(button.Reference.transform);
		button.transform.localPosition = buttonOffset;
		
		_buttons.Add(button);
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
