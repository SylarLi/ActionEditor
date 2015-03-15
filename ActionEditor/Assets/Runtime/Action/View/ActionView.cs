using Core;

namespace Action
{
    public class ActionView<T> : IActionView where T : IActionInfo
    {
        private T _actionInfo;

        private IActionSetting _setting;

        public ActionView(T actionInfo)
        {
            _actionInfo = actionInfo;
            _actionInfo.AddEventListener(ActionInfoEvent.ActionReadyChange, ActionReadyChangeHandler);
            _actionInfo.AddEventListener(ActionInfoEvent.ActionStateChange, ActionStateChangeHandler);
            _actionInfo.AddEventListener(ActionInfoEvent.ActionPauseChange, ActionPauseChangeHandler);
            _actionInfo.AddEventListener(ActionInfoEvent.ActionSpeedChange, ActionSpeedChangeHandler);
        }

        public T actionInfo
        {
            get
            {
                return _actionInfo;
            }
        }

        public IActionSetting setting
        {
            get
            {
                return _setting;
            }
            set
            {
                _setting = value;
            }
        }

        private void ActionReadyChangeHandler(IEvent obj)
        {
            UpdateState();
        }

        private void ActionStateChangeHandler(IEvent obj)
        {
            UpdateState();
        }

        private void ActionPauseChangeHandler(IEvent obj)
        {
            UpdateState();
        }

        private void ActionSpeedChangeHandler(IEvent obj)
        {
            UpdateSpeed();
        }

        protected virtual void UpdateState()
        {
            if (actionInfo.ready)
            {
                switch (actionInfo.state)
                {
                    case ActionState.Start:
                        {
                            if (actionInfo.paused)
                            {
                                Pause();
                            }
                            else
                            {
                                UpdateSpeed();
                                Play();
                            }
                            break;
                        }
                    case ActionState.Stop:
                        {
                            Stop();
                            actionInfo.InjectCondition(ConditionType.End);
                            Clear();
                            break;
                        }
                }
            }
        }

        private void UpdateSpeed()
        {
            if (actionInfo.ready)
            {
                Speed();
            }
        }

        public virtual void Load()
        {
            
        }

        protected virtual void Play()
        {

        }

        protected virtual void Pause()
        {

        }

        protected virtual void Stop()
        {

        }

        protected virtual void Speed()
        {

        }

        protected virtual void Clear()
        {
            if (actionInfo != null)
            {
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionReadyChange, ActionReadyChangeHandler);
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionStateChange, ActionStateChangeHandler);
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionPauseChange, ActionPauseChangeHandler);
                actionInfo.RemoveEventListener(ActionInfoEvent.ActionSpeedChange, ActionSpeedChangeHandler);
            }
        }
    }
}
