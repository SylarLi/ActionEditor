using UnityEngine;
using Core;

namespace Action
{
    public class BodyEffect : ActionView<BodyEffectInfo>
    {
        private const float EndProgress = 1f;

        private GameObject actor;

        private GameObject effect;

        private EffectPlayInfo effectPlayInfo;

        private EffectTimeListener listener;

        public BodyEffect(BodyEffectInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            actor = actionInfo.actor;
            setting.loader.GetEffect(actionInfo.effectId, (GameObject gameObject) =>
            {
                // 初始化
                effect = gameObject;
                effect.transform.parent = actor.transform.Find(actionInfo.bonePath);
                effect.transform.localPosition = actionInfo.localPosition;
                effect.transform.localRotation = Quaternion.Euler(actionInfo.localRotation);
                effectPlayInfo = effect.GetComponent<EffectPlayInfo>();
                effectPlayInfo.scale = actionInfo.localScale;
                listener = effect.AddComponent<EffectTimeListener>();
                listener.SetDuration(actionInfo.duration);
                listener.AddEventListener(EffectTimeListener.NormalizedTime, NormalizedTimeHandler);
                listener.enabled = false;
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
            listener.RemoveEventListener(EffectTimeListener.NormalizedTime, NormalizedTimeHandler);
            Component.Destroy(listener);
            effect.transform.parent = null;
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
            effectPlayInfo.scale = Vector3.one;
            effectPlayInfo.state = RadioState.Stop;
            setting.loader.ReturnEffect(actionInfo.effectId, effect);
            actor = null;
            effect = null;
            effectPlayInfo = null;
        }

        protected override void Speed()
        {
            effectPlayInfo.speed = actionInfo.speed;
        }

        private void NormalizedTimeHandler(IEvent e)
        {
            actionInfo.progress = listener.normalizedTime;
            actionInfo.InjectCondition(ConditionType.Progress, actionInfo.progress);
            if (actionInfo.progress >= EndProgress)
            {
                actionInfo.state = ActionState.Stop;
            }
        }
    }
}
