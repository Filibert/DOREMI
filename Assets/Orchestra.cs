using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Orchestra : MonoBehaviour
{
	[Range(0, 1.5f)]
	public float Volume = 1.0f;

	[Range(0.85f, 1.2f)]
	public float Speed = 1.0f;

	[Range(0.0f, 0.3f)]
	public float SpeedVariationOnDesync = 0.1f;

	public float StartupTime = 3.0f;

	[HideInInspector]
	public bool Running {get; private set;}
    public bool MaestroIsYellingHisOrder { get; private set; }

    public List<CustomAudioSource> Sources {get; private set;}

	public BeatThingy UserTempoFeedback;
	public BeatThingy BeatButtonPrefab;
	public CustomAudioSource MutedSourceJustForDefaultSpeed;

	private float _oldVolume;
	private float _oldSpeed;
	private bool _isStarting = false;

	private NormDistrib _tempoDegradationDistrib = new NormDistrib(15, 3);
	private float _timeUntilTempoDegradation;
	
	List<BeatThingy> _buttons = new List<BeatThingy>();

    void Awake() {
		_oldVolume = Volume;
		_oldSpeed = Speed;


		Sources = new List<CustomAudioSource>();

		MutedSourceJustForDefaultSpeed.Mute = true;
		MutedSourceJustForDefaultSpeed.Speed = 1.0f;
	}

	public void StartPlaying()
	{
		if (_isStarting)
			return;
			
		_isStarting = true;
		Running = true;

		foreach (var s in Sources)
		{
			if (s.Channel == null)
				continue;
			s.Channel.setPaused(true);
			s.Channel.setPosition (1, FMOD.TIMEUNIT.MS);
		}

		if (MutedSourceJustForDefaultSpeed.Channel != null) {
			MutedSourceJustForDefaultSpeed.Channel.setPaused (true);
			MutedSourceJustForDefaultSpeed.Channel.setPosition (1, FMOD.TIMEUNIT.MS);
		}

		foreach (var b in _buttons)
		{
			b.MyBeat.Reset();
			b.MyBeat.Pause();
		}

		StartCoroutine (ReallyStart());
	}

	private IEnumerator ReallyStart()
	{
		yield return new WaitForSeconds(StartupTime);

		foreach (var s in Sources)
		{
			Debug.Log(s.name);
			s.Speed = MutedSourceJustForDefaultSpeed.Speed;
			s.Play();
			MusicSelector.Source = s;
		}
		MutedSourceJustForDefaultSpeed.Play();

		foreach (var b in _buttons)
		{
			b.MyBeat.Reset();
			b.MyBeat.Run();
		}
			
		ResetTempoDegrationTime();

		_isStarting = false;
	}

	void ResetTempoDegrationTime()
	{
		_timeUntilTempoDegradation = _tempoDegradationDistrib.Value();
	}

	void FixedUpdate()
	{
		if (!_isStarting && Running && !MaestroIsYellingHisOrder) 
		{
			_timeUntilTempoDegradation -= Time.fixedDeltaTime;

			if (_timeUntilTempoDegradation <= 0)
			{
				int randomSourceToDesynchronize = Random.Range(0, Sources.Count);
				float variation = Random.Range(-SpeedVariationOnDesync, SpeedVariationOnDesync);
				Sources[randomSourceToDesynchronize].Speed += variation;
				
				ResetTempoDegrationTime();
			}
		}
	}

	void Update() {
        if (Input.GetKeyDown(KeyCode.A))

        {
			StartPlaying ();
        }
	    if (Input.GetKey(KeyCode.G))
	    {
	        EveryoneIsJoinningNow();
	        MaestroIsYellingHisOrder = true;
	    }
	    if (Input.GetKeyUp(KeyCode.G))
	    {
	        ResetTempoDegrationTime();
	    }
        else
	    {
	        MaestroIsYellingHisOrder = false;
        }
        if (_oldVolume != Volume) {
			foreach (var s in Sources) {
				s.Volume = Volume;
			}
		}

		if (_oldSpeed != Speed) {
			foreach (var s in Sources) {
				s.Speed = Speed;
			}
		}

		_oldVolume = Volume;
		_oldSpeed = Speed;
	}

	public void AddSource(CustomAudioSource source) {
		Sources.Add(source);

		BeatThingy button = Instantiate(BeatButtonPrefab);
		button.Reference = source;

		Vector3 buttonOffset = new Vector3(0, -100, 0);

		button.transform.SetParent(button.Reference.transform);
		button.transform.localPosition = buttonOffset;
		
		_buttons.Add(button);
	}

	public void Reset() {
		Sources.Clear();
		
		Volume = 1.0f;
		Speed = 1.0f;
	}

    public void EveryoneIsJoinningNow()
    {
        foreach (var source in Sources)
        {
            source.JoinReference(MutedSourceJustForDefaultSpeed);
        }
    }
	void OnDestroy() {
		Sources.Clear();
	}
}
