using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Action
{
    public class UterusAction : ActionView<UterusActionInfo>
    {
        private const float EndProgress = 1;

        private List<IActionInfo> embryos;

        public UterusAction(UterusActionInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            embryos = new List<IActionInfo>();
            GameObject[] children = actionInfo.children;
            foreach (GameObject child in children)
            {
                IActionInfo embryo = actionInfo.embryo.Clone();
                UterusDependInfo dependInfo = new UterusDependInfo();
                dependInfo.SetActor(dependInfo.motherIndex, actionInfo.mother);
                dependInfo.SetActor(dependInfo.childIndex, child);
                RootActionInfo rootInfo = new RootActionInfo(embryo, dependInfo);
                RootAction rootAction = new RootAction(rootInfo);
                rootAction.setting = setting;
                rootAction.Load();
                rootInfo.paused = actionInfo.paused;
                rootInfo.AddEventListener(ActionInfoEvent.ActionStateChange, EmbryoStateChangeHandler);
                rootInfo.AddEventListener(ActionInfoEvent.ActionReadyChange, EmbryoReadyChangeHandler);
                embryos.Add(rootInfo);
            }
            CheckStop();
            CheckReady();
        }

        private void EmbryoStateChangeHandler(IEvent e)
        {
            CheckStop();
        }

        private void EmbryoReadyChangeHandler(IEvent e)
        {
            CheckReady();
        }

        private void CheckStop()
        {
            if (embryos.Count == 0 || embryos.TrueForAll((IActionInfo each) => each.state == ActionState.Stop))
            {
                actionInfo.progress = EndProgress;
                actionInfo.InjectCondition(ConditionType.Progress, actionInfo.progress);
                actionInfo.state = ActionState.Stop;
            }
        }

        private void CheckReady()
        {
            if (embryos.Count == 0 || embryos.TrueForAll((IActionInfo each) => each.ready))
            {
                actionInfo.ready = true;
            }
        }

        protected override void Play()
        {
            foreach (IActionInfo embryo in embryos)
            {
                if (embryo.state == ActionState.None)
                {
                    embryo.state = ActionState.Start;
                }
                embryo.paused = false;
            }
        }

        protected override void Pause()
        {
            foreach (IActionInfo embryo in embryos)
            {
                embryo.paused = true;
            }
        }

        protected override void Stop()
        {
            foreach (IActionInfo embryo in embryos)
            {
                embryo.RemoveEventListener(ActionInfoEvent.ActionStateChange, EmbryoStateChangeHandler);
                embryo.RemoveEventListener(ActionInfoEvent.ActionReadyChange, EmbryoReadyChangeHandler);
                embryo.state = ActionState.Stop;
            }
            embryos = null;
        }

        protected override void Speed()
        {
            foreach (IActionInfo embryo in embryos)
            {
                embryo.speed = actionInfo.speed;
            }
        }
    }
}
