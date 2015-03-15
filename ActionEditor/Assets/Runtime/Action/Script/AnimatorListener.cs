using UnityEngine;

namespace Action
{
    public class AnimatorListener : MonoListener
    {
        public const string NormalizedTime = "NormalizedTime";

        public const string StateError = "StateError";

        private const int MaxErrorFrame = 10;

        private Animator animator;

        private string stateName;

        private float mNormalizedTime;

        private int errorFrame;

        public void Listen(string stateName)
        {
            this.stateName = stateName;
        }

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (!string.IsNullOrEmpty(stateName))
            {
                bool isInTransition = animator.IsInTransition(0);
                AnimatorStateInfo stateInfo = isInTransition ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(stateName))
                {
                    normalizedTime = stateInfo.normalizedTime;
                }
                else if (!isInTransition && ++errorFrame == MaxErrorFrame)
                {
                    DispatchEvent(new Core.Event(AnimatorListener.StateError));
                }
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
                if (System.Math.Abs(mNormalizedTime - value) > 0.001f)
                {
                    mNormalizedTime = value;
                    DispatchEvent(new Core.Event(NormalizedTime));
                }
            }
        }
    }
}
