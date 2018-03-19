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

		if (track != null)
        {
            foreach (ChorusScriptable c in track.chorusList)
            {
                GameObject go = new GameObject(c.instrumentName);
                go.transform.SetParent(this.transform);

                go.AddComponent<Renderer>();
                go.GetComponent<Renderer>().material.mainTexture = c.instrumentImage;

            }
        }
	}

	void Update ()
    {
		
	}
}
