using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class VolumeDisplay : MonoBehaviour
    {
        public CustomAudioSource Source;

        private Vector3 _initialPos;
        private Color _blue = new Color(0,0,150/255f);
        private Color _green = new Color(0, 150/255f, 0);
        private Color _red = new Color(150/255f, 0, 0);

        private Transform _volumeLevel;
        // Use this for initialization
        void Start ()
        {
            _volumeLevel = transform.GetChild(1);
            _initialPos = _volumeLevel.localPosition;
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (Source != null)
            {
                float Volume = Source.Volume;
				if (float.IsNaN (Volume))
					Volume = 1;
                _volumeLevel.localScale = new Vector3(1, Volume, 1);
                _volumeLevel.localPosition = new Vector3(0, 50 * Volume, 0) + _initialPos;
                if (Volume <= 1)
                    _volumeLevel.GetComponent<Image>().color = Color.Lerp(_blue, _green, Volume);
                else
                    _volumeLevel.GetComponent<Image>().color = Color.Lerp(_green, _red, (Volume - 1));
            }    
        }
    }
}
