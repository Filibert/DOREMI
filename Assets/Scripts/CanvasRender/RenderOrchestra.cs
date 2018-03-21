using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderOrchestra : MonoBehaviour {

    public TrackScriptable track;
    public GameObject audioSourcePrefab;
    public Orchestra orchestraPrefab;
    public GameObject soundGraphPrefab;

    Canvas canvas;

    void Start ()
    {
        canvas = GetComponent<Canvas>();
        var score = (GameObject)Resources.Load("SpiritedAway\\SpiritedAwayScore", typeof(GameObject));
        Debug.Log(score);

        if (score != null)
        {
            Debug.Log(score);
            score = Instantiate(score, transform);
        }
        int chorusSize = track.chorusList.Count;
        float width = canvas.GetComponent<RectTransform>().rect.width;
		float height = canvas.GetComponent<RectTransform>().rect.height;
        float xInterval = width / (chorusSize + 1);
		float yInterval = 0.75f * height / (chorusSize + 1);

        int i = 1;

		if (track != null)
        {
            foreach (ChorusScriptable c in track.chorusList)
            {
				Vector3 pos = new Vector3(xInterval * (i - (chorusSize / 2)), canvas.GetComponent<RectTransform>().rect.height / 2, 0);
                Vector3 posGraph = new Vector3((Screen.width - 200) / 2, (Screen.height - 100) / 2 - (chorusSize / 2 - i) * yInterval, 100);
                GameObject go = InstantiateChorus(c, pos);
                InstantiateGraph(go.GetComponent<CustomAudioSource>(), posGraph, 200, 100, SoundGraph.Colors[i % SoundGraph.Colors.Length]);
                i++;
            }
        }
        

	}

	void Update ()
    {
		
	}

    GameObject InstantiateChorus(ChorusScriptable c, Vector3 position)
    {
        GameObject go = (GameObject) Instantiate(audioSourcePrefab);
        go.name = c.instrumentName;
        go.GetComponent<CustomAudioSource>().SetSound(AudioMixer.Instance.Load(c.path));
        go.transform.SetParent(this.transform);
        go.AddComponent<Image>();
        go.GetComponent<Image>().sprite = c.instrumentImage;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = position;

        orchestraPrefab.AddSource(go.GetComponent<CustomAudioSource>());

        return go;
    }

    void InstantiateGraph(CustomAudioSource source, Vector3 position, float width, float height, Color color)
    {
        GameObject go = (GameObject) Instantiate(soundGraphPrefab);
        SoundGraph soundGraph = go.GetComponent<SoundGraph>();

        soundGraph.Source = source;
        soundGraph.name = source.name + "Graph";
        //soundGraph.transform.SetParent(this.transform);
        soundGraph.transform.localScale = Vector3.one;
        soundGraph.transform.localPosition = position;

        soundGraph.Width = width;
        soundGraph.Height = height;
        soundGraph.Color = color;

    }
}
