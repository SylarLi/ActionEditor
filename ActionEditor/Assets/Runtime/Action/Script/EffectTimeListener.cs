using UnityEngine;
using Core;

namespace Action
{
    public class EffectTimeListener : MonoListener
    {
        public const string NormalizedTime = "NormalizedTime";

        private EffectPlayInfo effectPlayInfo;

        private float duration;

        private float mNormalizedTime;

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        void Start()
        {
            effectPlayInfo = GetComponent<EffectPlayInfo>();
        }

        void Update()
        {
            // 这儿对循环特效的时间计算有问题
            if (duration > 0)
            {
                normalizedTime = effectPlayInfo.progress * effectPlayInfo.duration / duration;
            }
            else
            {
                normalizedTime = effectPlayInfo.progress;
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
