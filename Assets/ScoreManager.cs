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
    public CustomAudioSource MutedAudioSource;

    public int Adjust;
	// Use this for initialization
	void Start ()
	{
	    MutedAudioSource =  GameObject.Find("MutedSourceJustForDefaultSpeed").GetComponent<CustomAudioSource>();
        Debug.Log(GameObject.Find("MutedSourceJustForDefaultSpeed"));
	    _initPos = Score.transform.position;
	    _length = (Score.GetComponent<RectTransform>().rect.width) + (Cursor.transform.localPosition.x);
	    _cursoreInitPosition = Cursor.transform.position;
    }

    // Update is called once per frame
    void Update () {
	    if (MutedAudioSource.Channel != null)
	    {
	        uint length, position;
	        MutedAudioSource.Channel.getPosition(out position, TIMEUNIT.MS);
	        MutedAudioSource.Sound.getLength(out length, TIMEUNIT.MS);
	        float pos = (position /(float)length) * _length * transform.parent.localScale.x;
            Score.transform.position = new Vector3(_initPos.x - pos, _initPos.y, transform.position.z);
	    }
    }
}
