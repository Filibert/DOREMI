using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class RenderOrchestra : MonoBehaviour {

    public TrackScriptable track;
    public GameObject audioSourcePrefab;
    public Orchestra orchestraPrefab;
    public VolumeDisplay VolumeDislayer;
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

        //int i = 1;
        // TODO: change this part
        int i = 0;
        float rot =-30f;
        Vector3[] positionChorus = new Vector3[3];
        positionChorus[0] = new Vector3(-3000,450,2650);
        positionChorus[1] = new Vector3(-550,450,2400);
        positionChorus[2] = new Vector3(2000,450,2650);
		if (track != null)
        {
			float angleBetweenInstruments = Mathf.PI / chorusSize;
			float currentAngle = angleBetweenInstruments / 2;

			Debug.Log(canvas.transform.position);

            foreach (ChorusScriptable c in track.chorusList)
            {
				        //Vector3 pos = new Vector3(xInterval * (i - (chorusSize / 2)), canvas.GetComponent<RectTransform>().rect.height / 2, 0);
				        //Vector3 pos = new Vector3(distanceFromCamera * Mathf.Cos(currentAngle), 10, distanceFromCamera * Mathf.Sin(currentAngle));
                Vector3 pos = positionChorus[i];
                InstantiateChorus(c, pos /*- canvas.transform.position * 7.0f*/,rot);
                i++;
                rot +=30;
				currentAngle += angleBetweenInstruments;
            }
        }
	}

	void Update ()
    {

	}

    GameObject InstantiateChorus(ChorusScriptable c, Vector3 position, float rot)
    {
        GameObject go = (GameObject) Instantiate(audioSourcePrefab);
        go.name = c.instrumentName;

		CustomAudioSource source = go.GetComponent<CustomAudioSource>();
        source.SetSound(AudioMixer.Instance.Load(c.path));

        go.transform.SetParent(this.transform);
        go.AddComponent<Image>();
        go.GetComponent<Image>().sprite = c.instrumentImage;
        go.transform.localScale = new Vector3(5,5,5);//Vector3.one;
        go.transform.localPosition =  position; // *10;
        go.transform.eulerAngles = new Vector3(0,rot,0);
        orchestraPrefab.AddSource(source);

        VolumeDisplay volume = Instantiate(VolumeDislayer,go.transform);
        volume.transform.localPosition = new Vector3(100,-50,0);
        volume.Source = go.GetComponent<CustomAudioSource>();

        return go;
    }
}
