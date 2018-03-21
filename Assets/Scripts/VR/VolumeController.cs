using UnityEngine;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VolumeController : MonoBehaviour
{
    public bool IsModifyingSound { get; private set; }
    public Orchestra OrchestraPrefab;

    private SteamVR_TrackedController trackedController;
    private Vector3 _positionWhenTriggerPressStarted;
    private float _volumeWhenTriggerPressStarted;

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
            if (IsModifyingSound)
            {
                Vector3 deltaPos = trackedController.transform.position - _positionWhenTriggerPressStarted;
                OrchestraPrefab.Volume = Mathf.Clamp(_volumeWhenTriggerPressStarted + deltaPos.y, 0, 1.5f);
            }
            else
            {
                IsModifyingSound = true;
                _positionWhenTriggerPressStarted = trackedController.transform.position;
                _volumeWhenTriggerPressStarted = OrchestraPrefab.Volume;
            }
        }
        else
        {
            IsModifyingSound = false;
        }
    }
}