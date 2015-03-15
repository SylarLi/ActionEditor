using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Action
{
    public class ActorAction : ActionView<ActorActionInfo>
    {
        private const float EndProgress = 1f;

        private static Dictionary<GameObject, ActorActionInfo> actorActions = new Dictionary<GameObject, ActorActionInfo>();

        private int playedTimes;

        private Animator animator;

        private AnimatorListener listener;

        private bool animatorEnabled;

        private float animatorSpeed;

        public ActorAction(ActorActionInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            // 初始化
            GameObject charactor = actionInfo.actor;
            animator = charactor.GetComponent<Animator>();
            listener = charactor.AddComponent<AnimatorListener>();
            listener.Listen(actionInfo.actorActionName);
            listener.AddEventListener(AnimatorListener.NormalizedTime, NormalizedTimeHandler);
            listener.AddEventListener(AnimatorListener.StateError, StateErrorHandler);
            listener.enabled = false;
            // 记录原始值
            animatorEnabled = animator.enabled;
            animatorSpeed = animator.speed;
            // 标记初始化完成
            actionInfo.ready = true;
        }

        protected override void Play()
        {
            listener.enabled = true;
            animator.enabled = true;
            if (playedTimes == 0)
            {
                playedTimes = 1;
                if (animator.GetBool(actionInfo.actorActionName))
                {
                    animator.CrossFade(actionInfo.actorActionName, animator.IsInTransition(0) ? 0 : actionInfo.inEase, 0, 0);
                }
                else
                {
                    playedTimes = Mathf.CeilToInt(actionInfo.duration);
                    listener.normalizedTime = actionInfo.duration + 1 - playedTimes;
                }
                ActorActionInfo expired = null;
                actorActions.TryGetValue(animator.gameObject, out expired);
                actorActions[animator.gameObject] = actionInfo;
                if (expired != null)
                {
                    expired.state = ActionState.Stop;
                }
            }
        }

        protected override void Pause()
        {
            listener.enabled = false;
            animator.enabled = false;
        }

        protected override void Stop()
        {
            listener.RemoveEventListener(AnimatorListener.NormalizedTime, NormalizedTimeHandler);
            listener.RemoveEventListener(AnimatorListener.StateError, StateErrorHandler);
            Component.Destroy(listener);
            animator.enabled = animatorEnabled;
            animator.speed = animatorSpeed;
            if (actorActions.ContainsKey(animator.gameObject) && actorActions[animator.gameObject] == actionInfo)
            {
                animator.CrossFade(setting.defaultActorAction, actionInfo.outEase, 0, 0);
                actorActions.Remove(animator.gameObject);
            }
            animator = null;
        }

        protected override void Speed()
        {
            float speed = actionInfo.speed;
            animator.speed = animatorSpeed * speed;
        }

        private void NormalizedTimeHandler(IEvent e)
        {
            float normalizedTime = listener.normalizedTime + playedTimes - 1;
            if (normalizedTime > playedTimes && normalizedTime < actionInfo.duration)
            {
                playedTimes += 1;
                animator.Play(actionInfo.actorActionName, 0, 0);
            }
            actionInfo.progress = normalizedTime / actionInfo.duration;
            actionInfo.InjectCondition(ConditionType.Progress, actionInfo.progress);
            if (actionInfo.progress >= EndProgress)
            {
                actionInfo.state = ActionState.Stop;
            }
        }

        private void StateErrorHandler(IEvent e)
        {
            actionInfo.state = ActionState.Stop;
        }
    }
}
