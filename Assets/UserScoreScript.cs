using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserScoreScript : MonoBehaviour {
	public Orchestra OrchestraPrefab;
	public GameObject ClefPrefab;
	public CustomAudioSource MuteSourceJustForTotalTime;

	private float _intermediateScore = 100.0f;
	private float _realScore;

	private Text _userScore;
	private uint _totalTime = 0;

	private Gradient g = new Gradient();
	private GradientColorKey[] gck = new GradientColorKey[3];
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];
	
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
	
	// Update is called once per frame
	void Update () {
		if (OrchestraPrefab.Running)
		{
			if ((_totalTime == 0) && (MuteSourceJustForTotalTime.Sound != null))
			{
				MuteSourceJustForTotalTime.Sound.getLength(out _totalTime, FMOD.TIMEUNIT.MS);
			}
			
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
				foreach (var p in allPositions) mean += (p / 100);
				mean /= allPositions.Count;

				foreach (var p in allPositions) deviation += Mathf.Abs((p / 100) - mean);

				if (_totalTime == 0) _totalTime = (uint) Mathf.Max(allPositions.ToArray());
				deviation /= (float) _totalTime;
			}

			deviation *= 100.0f / allPositions.Count;
			deviation = Mathf.Clamp(deviation, 0.0f, 100.0f);
			
			_realScore = 100.0f - deviation;
			_intermediateScore = Mathf.Lerp(_intermediateScore, _realScore, Time.deltaTime);

			int score = Mathf.CeilToInt(_intermediateScore);
			
			UpdateText(score);
			UpdateNote(score);
		}
	}

	void UpdateText(int score)
	{
		_userScore.text = score.ToString();
		_userScore.color = g.Evaluate(_intermediateScore / 100.0f);
	}

	void UpdateNote(int score)
	{
		Animator animator = ClefPrefab.GetComponent<Animator>();

		if (score == 100.0f)
		{
			animator.Play("Happy");
		}
		else if (score > 75) {
			animator.Play("Jiggle");
		}
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
		else
		{
			animator.Play("Disappear");
		}
	}
}
