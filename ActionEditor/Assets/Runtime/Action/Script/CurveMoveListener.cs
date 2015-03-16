using UnityEngine;

namespace Action
{
    public class CurveMoveListener : MonoListener
    {
        public const string NormalizedTime = "NormalizedTime";

        public const float ProbeTime = 0.033f;

        // <----> 参数 <---->

        private AnimationCurve xCurve;

        private AnimationCurve yCurve;

        private AnimationCurve zCurve;

        private float speed;

        private Vector3 from;

        private Vector3 to;

        // <----> 计算 <---->

        private float duration = 0;

        private float time = 0;

        private Vector3 position = Vector3.zero;

        private Vector3 forward = Vector3.zero;

        private float mNormalizedTime = 0;

        public void SetCurves(AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve zCurve)
        {
            this.xCurve = xCurve;
            this.yCurve = yCurve;
            this.zCurve = zCurve;
            FormatCurve(xCurve);
            FormatCurve(yCurve);
            FormatCurve(zCurve);
        }

        private void FormatCurve(AnimationCurve curve)
        {
            if (curve != null)
            {
                Keyframe start = curve.keys[0];
                start.time = 0;
                curve.keys[0] = start;
                Keyframe end = curve.keys[curve.length - 1];
                end.time = 1;
                curve.keys[curve.length - 1] = end;
            }
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetPoint(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
            float newDuration = Vector3.Distance(from, to) / speed;
            time *= duration == 0 ? 0 : newDuration / duration;
            duration = newDuration;
            mNormalizedTime = time / duration;
        }

        private void CaculateFly()
        {
            bool edgeIn = !this.position.Equals(Vector3.zero);
            float progress = Mathf.Min(1 - ProbeTime, mNormalizedTime);
            Vector3 position = Vector3.Lerp(from, to, progress);
            Vector3 offset = GetOffset(progress);
            Vector3 nextPosition = Vector3.Lerp(from, to, progress + ProbeTime);
            Vector3 nextOffset = GetOffset(progress + ProbeTime);
            UpdatePosition(position + offset, edgeIn);
            UpdateDir((nextPosition + nextOffset) - (position + offset), edgeIn);
        }

        private Vector3 GetOffset(float progress)
        {
            return Quaternion.LookRotation(to - from, Vector3.up) * new Vector3
            (
                xCurve != null ? xCurve.Evaluate(progress) : 0,
                yCurve != null ? yCurve.Evaluate(progress) : 0,
                zCurve != null ? zCurve.Evaluate(progress) : 0
            );
        }

        private void UpdatePosition(Vector3 position, bool edgeIn)
        {
            this.position = position;
            if (!edgeIn)
            {
                transform.position = this.position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, this.position, 0.5f);
            }
        }

        private void UpdateDir(Vector3 forward, bool edgeIn)
        {
            this.forward = forward;
            if (!edgeIn)
            {
                transform.forward = this.forward;
            }
            else
            {
                transform.forward = Vector3.Lerp(transform.forward, this.forward, 0.5f);
            }
        }

        void Update()
        {
            CaculateFly();
            time = Mathf.Min(time + Time.deltaTime, duration);
            normalizedTime = time / duration;
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
