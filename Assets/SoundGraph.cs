using UnityEngine;

public class SoundGraph : MonoBehaviour {
	public static Color[] Colors = {Color.red, Color.green, Color.blue, Color.yellow};
	
	public CustomAudioSource Source;

	public uint FrameBufferSize = 5;
	public Color Color = Color.red;
	public float Width = Screen.width / 4;
	public float Height = Screen.height / 4;

	private uint _frameBufferCount = 0;
	private float _oldValue = 0;
	private float _targetValue = 0;

	private Rect _rect;

	// Just to use SmoothDamp.
	private float _velocity = 0.0f;

	void Awake() {
		Width = Screen.width / 8;
		Height = Screen.height / 3;

		_rect = new Rect(transform.position.x, transform.position.y, Width, Height);
	}
	
	unsafe void Update() {
		if ((Source == null) || (Source.WaveData == null) || (Source.Channel == null)) return;

        bool isPlaying;
        Source.Channel.isPlaying(out isPlaying);

        if (!isPlaying) {
            _targetValue = 0;
        }
        // Read data every 'FrameBufferSize' frames
        else if (_frameBufferCount == FrameBufferSize) {
            float currentMaxAbsValue = 0;

            // Get the maximum amplitude (either positive or negative).
            // NOTE: It's visually better than the average or just the max / min.
            for (int j = 0; j < AudioMixer.DSP_BUFFER_SIZE; ++j)
            {
                float absValue = Mathf.Abs(Source.WaveData[j]);

                if (absValue > currentMaxAbsValue) {
                    currentMaxAbsValue = absValue;
                }
            }

            _targetValue = currentMaxAbsValue;

            _frameBufferCount = 0;
        }
        else {
            ++_frameBufferCount;
        }

		// Interpolate between old and new value to get a smoother
		// curve.
		_oldValue = Mathf.SmoothDamp(_oldValue, _targetValue, ref _velocity, Time.deltaTime * FrameBufferSize);

		// FIXME: This does not work with rotations.
		_rect.x = transform.position.x / transform.lossyScale.x;
		_rect.y = transform.position.y / transform.lossyScale.y;
		_rect.width = Width;
		_rect.height = Height;
		GraphManager.Graph.Plot(name, _oldValue, Color, _rect);
	}

	void OnDestroy() {
		Source = null;
	}
}
