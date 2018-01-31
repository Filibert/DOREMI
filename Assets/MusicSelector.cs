using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MusicSelector : MonoBehaviour
{

    public Dropdown DropdownMusic;
    public Canvas Canvas;
    public Button MuteButton;
    public AudioSource SourcePrefab;
    public Slider VolumeSlider;
    private Dictionary<string,AudioSource> _sources = new Dictionary<string, AudioSource>();
    private Dictionary<string, string> _dictionnaryMusic = new Dictionary<string, string>();

	// Use this for initialization
	void Start ()
	{
	    foreach (var item in Directory.GetDirectories(Directory.GetCurrentDirectory() + "/Assets/Resources/").ToList())
	    {
	        if (!_dictionnaryMusic.ContainsKey(Path.GetFileName(item)))
	            _dictionnaryMusic.Add(Path.GetFileName(item),item);
	    }
	    DropdownMusic.AddOptions(_dictionnaryMusic.Keys.ToList());
	    DisplayAndPlayMusicInstrument();
	}

    public void DisplayAndPlayMusicInstrument()
    {
        DestroyEverything();
        string musicFolderPath = _dictionnaryMusic[DropdownMusic.options[DropdownMusic.value].text];
        int i = 1;
        foreach (var track in Directory.GetFiles(musicFolderPath).Where(n => Path.GetExtension(n) == ".wav"))
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
                    if (Math.Abs(_sources[goButton.GetComponentInChildren<Text>().text].volume) > 0.2)
                    {
                        _sources[goButton.GetComponentInChildren<Text>().text].volume = 0;
                        slider.value = 0;
                    }
                    else
                    {
                        _sources[goButton.GetComponentInChildren<Text>().text].volume = 1;
                        slider.value = 1;

                    }
                }

            });

          
            AudioSource source = Instantiate(SourcePrefab);
            source.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(DropdownMusic.options[DropdownMusic.value].text + "/" + Path.GetFileNameWithoutExtension(track));

            
            _sources.Add(Path.GetFileName(track),source);
            i += 30;
        }
        foreach (var source in _sources)
        {
            source.Value.Play();
        }
    }

    public void DestroyEverything()
    {
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
    }
    public void ChangeVolume(Slider s, string trackName)
    {
        _sources[trackName].volume = s.value;
        Debug.Log(_sources[trackName].volume);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
