using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ScoreManager : MonoBehaviour
{

    private Vector3 _initPos;
	// Use this for initialization
	void Start ()
	{
	    _initPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	    if (MusicSelector.Track != null)
	    {
	        uint length, position;
	        MusicSelector.Track.Channel.getPosition(out position, TIMEUNIT.MS);
	        MusicSelector.Track.Sound.getLength(out length, TIMEUNIT.MS);
	        float o = length / 1000f;
	        float translation = (Time.deltaTime * ((GetComponent<RectTransform>().rect.width) / o)) * transform.parent.localScale.x;
	        float pos = (position /(float)length) * GetComponent<RectTransform>().rect.width * transform.parent.localScale.x;

            transform.position = new Vector3(_initPos.x - pos, 0, transform.position.z);
	    }
    }
}
