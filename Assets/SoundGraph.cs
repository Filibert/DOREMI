using UnityEngine;

public class SoundGraph : MonoBehaviour {
	public static Color[] Colors = {Color.red, Color.green, Color.blue, Color.yellow};
	
	public CustomAudioSource Source;
	
	public Color color = Color.red;
	public float width = Screen.width / 8;
	public float height = Screen.height / 4;

	private AudioMixer _mixer;

	unsafe void Update() {
		if ((Source != null) && (Source.WaveData != null)) {
			float avg = 0;
			float sign = 1;
			for (int j = 0; j < AudioMixer.DSP_BUFFER_SIZE; ++j)
			{
				float value = Source.WaveData[j];
				float absValue = Mathf.Abs(value);
				
				if (absValue > avg) {
					avg = absValue;
					// sign = (value < 0) ? -1 : 1;
				}
			}

			Rect GraphRect = new Rect(transform.position.x, transform.position.y, width, height);
			GraphManager.Graph.Plot(name, avg * sign// / AudioMixer.DSP_BUFFER_SIZE
									, color, GraphRect);
		}
	}

	void OnDestroy() {
		Source = null;
	}
}
