using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class SpeedController : MonoBehaviour
{
    public bool IsModifyingSpeed{ get; private set; }
    public Orchestra OrchestraPrefab;
	public bool IsInJoinningPhase = false;

    private SteamVR_TrackedController trackedController;
	private SteamVR_Controller.Device _device;
    private Vector3 _positionWhenTriggerPressStarted;
    private float _speedWhenTriggerPressStarted;
	private float _speed;
	private bool _lastVelocityWasPositive = false;

	private float _delayBetweenTwoMoves = 1.0f;
	private WaitForSeconds waitDelay;
	private List<float> _timestamps;
	private int _count = 0;

	public float treshold = 0.1f;
	void Awake()
	{
		_device = SteamVR_Controller.Input ((int)GetComponent<SteamVR_TrackedObject> ().index);
		waitDelay = new WaitForSeconds(_delayBetweenTwoMoves);
		_timestamps = new List<float> ();
	
	}

	void Start()
	{
		InvokeRepeating ("Join", 0.0f, 0.33f);
	}


    private void OnEnable()
    {
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }


    }

	public void Join()
	{
		if (IsInJoinningPhase) {
			OrchestraPrefab.MutedSourceJustForDefaultSpeed.Speed = Mathf.Lerp (OrchestraPrefab.MutedSourceJustForDefaultSpeed.Speed, _speed, 0.33f);
			OrchestraPrefab.EveryoneIsJoinningNow ();
		}
	}	

    private void Update()
    {
		
        bool triggerPressed = trackedController.triggerPressed;


        if (triggerPressed)
        {
			if (!OrchestraPrefab.Running)
				return;
			
			if (Mathf.Abs (_device.velocity.x) > treshold || Mathf.Abs (_device.velocity.y) > treshold) {

				//Debug.Log ("x " + _device.velocity.x);
				//Debug.Log ("y " + _device.velocity.y);

				if (!IsModifyingSpeed)
					StartCoroutine (WaitFor ());

				if (_device != null) {
					bool newVelocityIsPositive = _device.velocity.x > 0;

					if (_lastVelocityWasPositive != newVelocityIsPositive) {
						_lastVelocityWasPositive = newVelocityIsPositive;
						_timestamps.Add (Time.timeSinceLevelLoad);
						++_count;

						//Debug.Log (_count);
					}
				}
			}

            if (IsModifyingSpeed)
            {
				if (_device != null)
					_lastVelocityWasPositive = _device.velocity.x > 0;

            }
            else
            {
                IsModifyingSpeed = true;
                _positionWhenTriggerPressStarted = trackedController.transform.position;
                _speedWhenTriggerPressStarted = OrchestraPrefab.Speed;
            }
        }
        else
        {
			_count = 0;
            IsModifyingSpeed = false;
        }

    }

	private void FixedUpdate()
	{
		_device = SteamVR_Controller.Input ((int)GetComponent<SteamVR_TrackedObject> ().index);
	}

	private IEnumerator WaitFor()
	{
		do
		{
			yield return waitDelay;

			_timestamps.RemoveAll (s => s < Time.timeSinceLevelLoad - 3.75f);

			if (_timestamps.Count > 0)
			{
				// TODO: Save old timestamp count, if delta timestamp count is not much, do nothing.
				// step = 0.75s,  5 steps = 3.75s
				float speed = _timestamps.Count / 5.0f;
				float demiTemps = _timestamps.Count / 2.0f;
				if(demiTemps < 3 )
					speed = Mathf.Lerp(0.8f, 1.0f, (demiTemps - 1) / 3.0f);
				else if (demiTemps > 6 )
					speed = Mathf.Lerp(1.0f, 1.2f, (demiTemps - 6) / 3.0f);
				else
					speed = 1;
				
				_speed = Mathf.Clamp(speed, 0.8f, 1.2f);
				int speedInt = Mathf.RoundToInt(speed * 10);
				_speed = speedInt / 10.0f;
				IsInJoinningPhase = true;
			}
			else 		
				IsInJoinningPhase = false;

		} while(IsModifyingSpeed);

		IsInJoinningPhase = false;

	}
}