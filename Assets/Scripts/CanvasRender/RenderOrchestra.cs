using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderOrchestra : MonoBehaviour {

    public TrackScriptable track;

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
                InstantiateChorus(c, new Vector3(xInterval * i, 183, 0));
                i++;
            }
        }
	}

	void Update ()
    {
		
	}

    void InstantiateChorus(ChorusScriptable c, Vector3 position)
    {
        GameObject go = new GameObject(c.instrumentName);
        go.AddComponent<Image>();
        go.GetComponent<Image>().sprite = c.instrumentImage;
        go.transform.position = position;
        go.transform.SetParent(this.transform);
    }
}
