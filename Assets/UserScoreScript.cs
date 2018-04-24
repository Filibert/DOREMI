using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserScoreScript : MonoBehaviour {
	public Orchestra OrchestraPrefab;
	public GameObject ClefPrefab;
    public int MeaningFullTime;


    private float _intermediateScore = 100.0f;
	private float _realScore;

	private Text _userScore;
	private uint _totalTime = 0;

	private Gradient g = new Gradient();
	private GradientColorKey[] gck = new GradientColorKey[3];
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];

	float toCS(float ms)
	{
		return ms / 100;
	}
	
	uint toCS(uint ms)
	{
		return ms / 100;
	}
	
	// Use this for initialization
	void Start () {
		_realScore = _intermediateScore;
		_userScore = GetComponent<Text>();

		gck[0].color = Color.red;
		gck[0].time = 0.0f;
		gck[1].color = Color.red;
		gck[1].time = 0.75f;
		gck[2].color = Color.black;
		gck[2].time = 1.0f;
		
		gak[0].alpha = 1.0f;
		gak[0].time = 0.0f;
		gak[1].alpha = 1.0f;
		gak[1].time = 0.75f;
		gak[2].alpha = 1.0f;
		gak[2].time = 1.0f;
		g.SetKeys(gck, gak);

		UpdateText(100);
	}
	
	void LateUpdate () {
	    if (!OrchestraPrefab.Running) return;
	    float mean  = 0.0f;
	    float deviation = 0.0f;
	    List<float> allPositions = new List<float>();
			
	    foreach (var s in OrchestraPrefab.Sources)
	    {
	        uint position;
	        s.Channel.getPosition(out position, FMOD.TIMEUNIT.MS);
				
	        if (s.Volume == 0) continue;
	        allPositions.Add(position);
	    }

	    if (allPositions.Count != 0)
	    {
	        foreach (var p in allPositions) mean += toCS(p);
	        mean /= allPositions.Count;

	        foreach (var p in allPositions) deviation += Mathf.Abs(toCS(p) - mean);

	        _totalTime = (uint) toCS(MeaningFullTime * 1000);

	        if (_totalTime != 0)
	            deviation /= (float) _totalTime;
	    }

	    deviation *= 100.0f / allPositions.Count;
	    deviation = Mathf.Clamp(deviation, 0.0f, 100.0f);

	    _realScore = 100.0f - deviation;
	    _intermediateScore = Mathf.Lerp(_intermediateScore, _realScore, Time.deltaTime);

	    int score = Mathf.RoundToInt(_intermediateScore);
			
	    UpdateText(score);
	    UpdateNote(score);
	}


    void UpdateText(int score)
	{
		_userScore.text = score.ToString();
		_userScore.color = g.Evaluate(_intermediateScore / 100.0f);
	}

	void UpdateNote(int score)
	{
		Animator animator = ClefPrefab.GetComponent<Animator>();

		if (score > 95.0f)      animator.Play("Happy");
		else if (score > 90.0f) animator.Play("Jiggle");
		else if (score > 75)    animator.Play("Sad");
		else if (score > 50) 
		{
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

			if (info.IsName("Disappear"))
			{
				animator.Play("Appear");
			}
			else if ((!info.IsName("Appear")) ||
					 (info.normalizedTime > 1) && (!animator.IsInTransition(0)))
			{
				animator.Play("Sad");
			}
		}
		else animator.Play("Disappear");
	}
}
