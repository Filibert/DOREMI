using UnityEngine;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class SpeedController : MonoBehaviour
{
    public bool IsModifyingSpeed{ get; private set; }
    public Orchestra OrchestraPrefab;

    private SteamVR_TrackedController trackedController;
    private Vector3 _positionWhenTriggerPressStarted;
    private float _speedWhenTriggerPressStarted;

    private void OnEnable()
    {
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
    }

    private void Update()
    {
        bool triggerPressed = trackedController.triggerPressed;

        if (triggerPressed)
        {
            if (IsModifyingSpeed)
            {
                Vector3 deltaPos = trackedController.transform.position - _positionWhenTriggerPressStarted;
                OrchestraPrefab.Speed = Mathf.Clamp(_speedWhenTriggerPressStarted + deltaPos.x, 0.85f, 1.2f);
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
            IsModifyingSpeed = false;
        }
    }
}