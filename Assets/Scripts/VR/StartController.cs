using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StartController : MonoBehaviour {
	public Orchestra OrchestraPrefab;
	private SteamVR_TrackedController trackedController;

	private void OnEnable()
	{
		trackedController = GetComponent<SteamVR_TrackedController>();
		if (trackedController == null)
		{
			trackedController = GetComponentInParent<SteamVR_TrackedController>();
		}

		trackedController.PadClicked += (object sender, ClickedEventArgs e) => Toto(e);
	}

	void Toto(ClickedEventArgs args)
	{
		OrchestraPrefab.StartPlaying ();
	}

	void Update()
	{
		
	}
}
