using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.Collections;
using Debug = UnityEngine.Debug;

public class ScoreManager : MonoBehaviour
{

    private Vector3 _initPos,_cursoreInitPosition;
    private float _length;
    public GameObject Cursor;
    public GameObject Score;

    public int Adjust;
	// Use this for initialization
	void Start ()
	{
	    _initPos = Score.transform.position;
	    _length = (Score.GetComponent<RectTransform>().rect.width) + (Cursor.transform.localPosition.x) + Adjust;
	    _cursoreInitPosition = Cursor.transform.position;
        Debug.Log(transform.parent.localScale.x);
	    Debug.Log(Score.transform.parent.localScale.x);

    }

    // Update is called once per frame
    void Update () {
	    if (MusicSelector.Track != null)
	    {
	        uint length, position;
	        MusicSelector.Track.Channel.getPosition(out position, TIMEUNIT.MS);
	        MusicSelector.Track.Sound.getLength(out length, TIMEUNIT.MS);
	        float pos = (position /(float)length) * _length * transform.parent.localScale.x;
            Score.transform.position = new Vector3(_initPos.x - pos, 0, transform.position.z);
	    }
    }
}
