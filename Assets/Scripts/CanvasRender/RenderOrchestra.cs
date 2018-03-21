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

        int chorusSize = track.chorusList.Count;
        float width = canvas.GetComponent<RectTransform>().rect.width;
        float xInterval = width / (chorusSize + 1);

        int i = 1;

		if (track != null)
        {
            foreach (ChorusScriptable c in track.chorusList)
            {
                GameObject go = InstantiateChorus(c, new Vector3(xInterval * i, canvas.GetComponent<RectTransform>().rect.height / 2, 0));
                InstantiateGraph(go.GetComponent<CustomAudioSource>(), new Vector3(xInterval * i, canvas.GetComponent<RectTransform>().rect.height * 2/3 , 0), 200, 100, Color.red);
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
        soundGraph.transform.SetParent(this.transform);
        soundGraph.transform.localScale = Vector3.one;
        soundGraph.transform.localPosition = position;
        soundGraph.Width = width;
        soundGraph.Height = height;
        soundGraph.Color = color;

    }
}
