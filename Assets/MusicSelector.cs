using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMOD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class MusicSelector : MonoBehaviour
{

    public Dropdown DropdownMusic;
    public Canvas Canvas;
    public Button MuteButton;
    public Slider VolumeSlider;
	public GameObject SourcePrefab;

	private AudioMixer _audioMixer;
    private Dictionary<string, CustomAudioSource> _sources = new Dictionary<string, CustomAudioSource>();
    private Dictionary<string, string> _dictionnaryMusic = new Dictionary<string, string>();
    public static CustomAudioSource Track;

	// Use this for initialization
	void Start ()
	{
		_audioMixer = AudioMixer.Instance;
		
	    foreach (var item in Directory.GetDirectories(Directory.GetCurrentDirectory() + "/Assets/Resources/").ToList())
	    {
	        if (!_dictionnaryMusic.ContainsKey(Path.GetFileName(item))) {
	            _dictionnaryMusic.Add(Path.GetFileName(item),item);
			}
	    }
	    DropdownMusic.AddOptions(_dictionnaryMusic.Keys.ToList());
	    DisplayAndPlayMusicInstrument();
	}

    public void DisplayAndPlayMusicInstrument()
    {


        DestroyEverything();
        string folderName = DropdownMusic.options[DropdownMusic.value].text;
        string musicFolderPath = _dictionnaryMusic[folderName];
        var score = (GameObject)Resources.Load(folderName + "/" + folderName + "Score" , typeof(GameObject));
        if (score != null)
        {
            Debug.Log(score);
            score = Instantiate(score,transform);
        }
        int i = 1;
        foreach (var track in Directory.GetFiles(musicFolderPath).Where(n => Path.GetExtension(n) == ".mp3"))
        {
            Slider slider = Instantiate(VolumeSlider);

            slider.transform.SetParent(Canvas.transform, false);
            slider.transform.Translate(new Vector3(-20, -i, 0));
            slider.onValueChanged.AddListener(delegate { ChangeVolume(slider, Path.GetFileName(track)); });


            Button goButton = Instantiate(MuteButton);
            goButton.transform.SetParent(Canvas.transform, false);
            goButton.GetComponentInChildren<Text>().text = Path.GetFileName(track);
            goButton.transform.Translate(new Vector3(i, 0, 0));
            goButton.transform.localScale = new Vector3(0.85f, 1, 1);

            goButton.onClick.AddListener(() =>
            {
                if (_sources.ContainsKey(goButton.GetComponentInChildren<Text>().text))
                {
                    if (Math.Abs(_sources[goButton.GetComponentInChildren<Text>().text].Volume) > 0.2)
                    {
                        _sources[goButton.GetComponentInChildren<Text>().text].Volume = 0;
                        slider.value = 0;
                    }
                    else
                    {
                        _sources[goButton.GetComponentInChildren<Text>().text].Volume = 1;
                        slider.value = 1;

                    }
                }

            });

			FMOD.Sound sound = _audioMixer.Load(track);

			CustomAudioSource source = Instantiate(SourcePrefab).GetComponent<CustomAudioSource>();
			source.SetSound(sound);
            
            _sources.Add(Path.GetFileName(track), source);
            i += 30;
            Track = source;
        }
        foreach (var source in _sources)
        {
            source.Value.Play();
        }
    }

    public void DestroyEverything()
    {
		foreach (var source in _sources)
		{
			Destroy(source.Value.gameObject);
		}
		_sources.Clear();

        foreach (var button in GameObject.FindGameObjectsWithTag("trackButton"))
        {
            Destroy(button);
        }
        foreach (var slider in GameObject.FindGameObjectsWithTag("trackVolume"))
        {
            Destroy(slider);
        }
        foreach (var audio in GameObject.FindGameObjectsWithTag("trackAudio"))
        {
            Destroy(audio);
        }
        foreach (var score in GameObject.FindGameObjectsWithTag("Score"))
        {
            Destroy(score);
        }
    }
    public void ChangeVolume(Slider s, string trackName)
    {
        _sources[trackName].Volume = s.value;

    }
    // Update is called once per frame
    void Update () {
		
	}
}
