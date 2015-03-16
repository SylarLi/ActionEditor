using UnityEngine;

namespace Action
{
    public class SoundListener : MonoListener
    {
        public const string NormalizedTime = "NormalizedTime";

        private float mNormalizedTime;

        void Update()
        {
            if (audio.isPlaying)
            {
                normalizedTime += Time.deltaTime / audio.clip.length;
            }
        }

        public float normalizedTime
        {
            get
            {
                return mNormalizedTime;
            }
            set
            {
                if (Mathf.Abs(mNormalizedTime - value) > 0.001f)
                {
                    mNormalizedTime = value;
                    DispatchEvent(new Core.Event(NormalizedTime));
                }
            }
        }
    }
}
