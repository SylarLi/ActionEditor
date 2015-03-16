using UnityEngine;
using Core;
using System;

namespace Action
{
    public class B2BEffect : ActionView<B2BEffectInfo>
    {
        private const float EndProgress = 1f;

        private GameObject from;

        private GameObject to;

        private GameObject effect;

        private EffectPlayInfo effectPlayInfo;

        private CurveMoveListener listener;

        public B2BEffect(B2BEffectInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            from = actionInfo.fromActor;
            to = actionInfo.toActor;
            setting.loader.GetEffect(actionInfo.effectId, (GameObject gameObject) =>
            { 
                // 初始化
                effect = gameObject;
                effectPlayInfo = effect.GetComponent<EffectPlayInfo>();
                effectPlayInfo.scale = actionInfo.scale;
                listener = effect.AddComponent<CurveMoveListener>();
                listener.SetCurves(actionInfo.xCurve, actionInfo.yCurve, actionInfo.zCurve);
                listener.SetSpeed(actionInfo.flySpeed);
                listener.AddEventListener(CurveMoveListener.NormalizedTime, NormalizedTimeHandler);
                listener.enabled = false;
                UpdatePoint();
                // 标记初始化完成
                actionInfo.ready = true;
            });
        }

        protected override void Play()
        {
            effect.SetActive(true);
            effectPlayInfo.state = RadioState.Play;
            effectPlayInfo.pause = false;
            listener.enabled = true;
        }

        protected override void Pause()
        {
            effectPlayInfo.pause = true;
            listener.enabled = false;
        }

        protected override void Stop()
        {
            listener.RemoveEventListener(CurveMoveListener.NormalizedTime, NormalizedTimeHandler);
            Component.Destroy(listener);
            effect.transform.position = Vector3.zero;
            effect.transform.rotation = Quaternion.identity;
            effectPlayInfo.scale = Vector3.one;
            effectPlayInfo.state = RadioState.Stop;
            setting.loader.ReturnEffect(actionInfo.effectId, effect);
            from = null;
            to = null;
            effect = null;
            effectPlayInfo = null;
        }

        protected override void Speed()
        {
            listener.SetSpeed(actionInfo.flySpeed * actionInfo.speed);
            effectPlayInfo.speed = actionInfo.speed;
        }

        private void NormalizedTimeHandler(IEvent e)
        {
            UpdatePoint();
            actionInfo.progress = listener.normalizedTime;
            actionInfo.InjectCondition(ConditionType.Progress, actionInfo.progress);
            if (actionInfo.progress >= EndProgress)
            {
                actionInfo.state = ActionState.Stop;
            }
        }

        private void UpdatePoint()
        {
            Vector3 fromTo = to.transform.position - from.transform.position;
            Quaternion fromToRotation = fromTo.Equals(Vector3.zero) ? from.transform.rotation : Quaternion.LookRotation(fromTo, from.transform.position);
            listener.SetPoint
            (
                from.transform.position + fromToRotation * actionInfo.fromOffset,
                to.transform.position + fromToRotation * actionInfo.toOffset
            );
        }
    }
}
