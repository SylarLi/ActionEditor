namespace Action
{
    public class RootActionInfo : ActionInfo
    {
        public IActionInfo actionInfo;

        public IDependInfo dependInfo;

        public RootActionInfo() : base()
        {

        }

        public RootActionInfo(IActionInfo actionInfo, IDependInfo dependInfo) : base()
        {
            this.actionInfo = actionInfo;
            this.dependInfo = dependInfo;
        }

        public override ActionType type
        {
            get
            {
                return ActionType.Root;
            }
        }

        public override ActionState state
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    UpdateState();
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionStateChange));
                }
            }
        }

        public override bool paused
        {
            get
            {
                return _paused;
            }
            set
            {
                if (_paused != value)
                {
                    _paused = value;
                    UpdateState();
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionPauseChange));
                }
            }
        }

        public override float speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (System.Math.Abs(_speed - value) >= 0.01f)
                {
                    _speed = value;
                    UpdateSpeed();
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionSpeedChange));
                }
            }
        }

        public override bool ready
        {
            get
            {
                return _ready;
            }
            set
            {
                if (_ready != value)
                {
                    _ready = value;
                    UpdateState();
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionReadyChange));
                }
            }
        }

        private void UpdateState()
        {
            if (ready)
            {
                switch (state)
                {
                    case ActionState.Start:
                        {
                            UpdatePauseRecursion(actionInfo, paused);
                            if (actionInfo.state == ActionState.None)
                            {
                                actionInfo.state = ActionState.Start;
                            }
                            break;
                        }
                    case ActionState.Stop:
                        {
                            UpdateStateRecursion(actionInfo, ActionState.Stop);
                            break;
                        }
                }
            }
        }

        private void UpdateSpeed()
        {
            UpdateSpeedRecursion(actionInfo, speed);
        }

        public override IActionInfo Clone()
        {
            throw new System.NotSupportedException();
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeRecursion(actionInfo);
        }
    }
}
