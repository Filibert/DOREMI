using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderOrchestra : MonoBehaviour {

    public TrackScriptable track;
    public GameObject audioSourcePrefab;
    public Orchestra orchestraPrefab;
	public float distanceFromCamera = 800.0f;

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
			float angleBetweenInstruments = Mathf.PI / chorusSize;
			float currentAngle = angleBetweenInstruments / 2;

			Debug.Log(canvas.transform.position);
			
            foreach (ChorusScriptable c in track.chorusList)
            {
				//Vector3 pos = new Vector3(xInterval * (i - (chorusSize / 2)), canvas.GetComponent<RectTransform>().rect.height / 2, 0);
				Vector3 pos = new Vector3(distanceFromCamera * Mathf.Cos(currentAngle), 10, distanceFromCamera * Mathf.Sin(currentAngle));
                InstantiateChorus(c, pos - canvas.transform.position * 7.0f);
                i++;

				currentAngle += angleBetweenInstruments;
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
		
		CustomAudioSource source = go.GetComponent<CustomAudioSource>();
        source.SetSound(AudioMixer.Instance.Load(c.path));
		
        go.transform.SetParent(this.transform);
        go.AddComponent<Image>();
        go.GetComponent<Image>().sprite = c.instrumentImage;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = 10 * position;

        orchestraPrefab.AddSource(source);

        return go;
    }
}
