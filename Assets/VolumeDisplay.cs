using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class VolumeDisplay : MonoBehaviour
{
    [Range(0, 2f)]
    public float Volume;

    private Vector3 _initialPos;
    private Color _blue = new Color(0,0,150/255f);
    private Color _green = new Color(0, 150/255f, 0);
    private Color _red = new Color(150/255f, 0, 0);

    // Use this for initialization
    void Start ()
	{
	    _initialPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.localScale = new Vector3(1,Volume,1);
        transform.localPosition = new Vector3(0,50*Volume,0) + _initialPos;
	    if (Volume <= 1)
	        GetComponent<Image>().color = Color.Lerp(_blue, _green, Volume);
	    else
	        GetComponent<Image>().color = Color.Lerp(_green, _red, (Volume - 1));

    }
}
