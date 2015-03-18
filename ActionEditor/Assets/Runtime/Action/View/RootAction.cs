using Core;
using System.Collections.Generic;

namespace Action
{
    public class RootAction : ActionView<RootActionInfo>
    {
        private const float EndProgress = 1;

        public RootAction(RootActionInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            MakeAction(actionInfo.actionInfo, actionInfo.dependInfo);
            CheckReady();
            CheckStop();
        }

        private void MakeAction(IActionInfo actionInfo, IDependInfo dependInfo)
        {
            int actorCount = 0;
            object[] depends = System.Array.ConvertAll<DependType, object>(actionInfo.dependTypes, (DependType dependType) => (dependType == DependType.Actor || dependType == DependType.Actors) ? dependInfo.FindActor(actionInfo.dependActorIndexes[actorCount++]) : dependInfo[dependType]);
            actionInfo.InjectDepends(depends);
            if (actionInfo.IsDependValid())
            {
                CreateAction(actionInfo);
                ListenAction(actionInfo);
                foreach (IActionInfo childInfo in actionInfo.Values)
                {
                    MakeAction(childInfo, dependInfo);
                }
            }
            else
            {
                ActionInfo.UpdateReadyRecursion(actionInfo, true);
                ActionInfo.UpdateStateRecursion(actionInfo, ActionState.Stop);
            }
        }

        private void CreateAction(IActionInfo actionInfo)
        {
            IActionView action = null;
            switch (actionInfo.type)
            {
                case ActionType.ActorAction:
                    {
                        action = new ActorAction(actionInfo as ActorActionInfo);
                        break;
                    }
                case ActionType.BodyEffect:
                    {
                        action = new BodyEffect(actionInfo as BodyEffectInfo);
                        break;
                    }
                case ActionType.B2BEffect:
                    {
                        action = new B2BEffect(actionInfo as B2BEffectInfo);
                        break;
                    }
                case ActionType.Sound:
                    {
                        action = new SoundAction(actionInfo as SoundActionInfo);
                        break;
                    }
                case ActionType.Uterus:
                    {
                        action = new UterusAction(actionInfo as UterusActionInfo);
                        break;
                    }
                case ActionType.Root:
                    {
                        throw new System.NotSupportedException("RootAction不能创建自身，如果使用编辑器，请勿在树状结构中插入Root类型");
                    }
                default:
                    {
                        throw new System.NotSupportedException("不支持" + actionInfo.type + "类型");
                    }
            }
            action.setting = setting;
            action.Load();
        }

        private void ListenAction(IActionInfo actionInfo)
        {
            if (!actionInfo.ready)
            {
                actionInfo.AddEventListener(ActionInfoEvent.ActionReadyChange, ActionReadyChangeHandler);
            }
            actionInfo.AddEventListener(ActionInfoEvent.ActionStateChange, ActionStateChangeHandler);
        }

        private void ActionReadyChangeHandler(IEvent e)
        {
            CheckReady();
        }

        private void CheckReady()
        {
            if (IsReady(actionInfo.actionInfo))
            {
                actionInfo.ready = true;
            }
        }

        private bool IsReady(IActionInfo actionInfo)
        {
            return actionInfo.ready && !new List<IActionInfo>(actionInfo.Values).Exists((IActionInfo childInfo) => !IsReady(childInfo));
        }

        private void ActionStateChangeHandler(IEvent e)
        {
            CheckStop();
        }

        private void CheckStop()
        {
            if (IsStop(actionInfo.actionInfo))
            {
                actionInfo.progress = EndProgress;
                actionInfo.state = ActionState.Stop;
            }
        }

        private bool IsStop(IActionInfo actionInfo)
        {
            return actionInfo.state == ActionState.Stop && !new List<IActionInfo>(actionInfo.Values).Exists((IActionInfo childInfo) => !IsStop(childInfo));
        }

        protected override void Clear()
        {
            RemoveListenerRecurstion(actionInfo.actionInfo);
            base.Clear();
        }

        private void RemoveListenerRecurstion(IActionInfo actionInfo)
        {
            if (actionInfo != null)
            {
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionReadyChange, ActionReadyChangeHandler);
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionStateChange, ActionStateChangeHandler);
            }
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                RemoveListenerRecurstion(childInfo);
            }
        }
    }
}
