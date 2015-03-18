using Core;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Action
{
    [ProtoContract]
    public class ActionInfo : EventDispatcher, IActionInfo
    {
        [ProtoMember(1)]
        protected IDictionary<ICondition, IActionInfo> _children;

        [ProtoMember(2)]
        protected string[] _dependActorIndexes;

        protected IList<ICondition> _expired;

        protected ActionState _state;

        protected bool _paused;

        protected float _speed;

        protected float _progress;

        protected bool _ready;

        public ActionInfo()
        {
            _children = new Dictionary<ICondition, IActionInfo>();
            _dependActorIndexes = new string[0];
            _expired = new List<ICondition>();
            _state = ActionState.None;
            _paused = false;
            _speed = 1;
            _progress = 0;
            _ready = false;
        }

        public virtual ActionType type
        {
            get 
            {
                return ActionType.None;
            }
        }

        public virtual ActionState state
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
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionStateChange));
                }
            }
        }

        public virtual bool paused
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
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionPauseChange));
                }
            }
        }

        public virtual float speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (Math.Abs(_speed - value) > 0.01f)
                {
                    _speed = value;
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionSpeedChange));
                }
            }
        }

        public virtual float progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (Math.Abs(_progress - value) > 0.01f)
                {
                    _progress = value;
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionProgressChange));
                }
            }
        }

        public virtual bool ready
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
                    DispatchEvent(new ActionInfoEvent(ActionInfoEvent.ActionReadyChange));
                }
            }
        }

        public virtual DependType[] dependTypes
        {
            get
            {
                return new DependType[0];
            }
        }

        public virtual string[] dependDescripts
        {
            get
            {
                return new string[0];
            }
        }

        public string[] dependActorIndexes
        {
            get
            {
                return _dependActorIndexes;
            }
            set
            {
                _dependActorIndexes = value;
            }
        }

        public virtual void InjectDepends(object[] depends)
        {
            
        }

        public virtual bool IsDependValid()
        {
            return true;
        }

        public void InjectCondition(ConditionType type, float val = 0)
        {
            foreach (ICondition condition in Keys)
            {
                if (_expired.IndexOf(condition) == -1 && condition.type == type && condition.Meet(val))
                {
                    if (this[condition].state == ActionState.None)
                    {
                        this[condition].state = ActionState.Start;
                    }
                    _expired.Add(condition);
                }
            }
        }

        public virtual IActionInfo Clone()
        {
            IActionInfo actionInfo = Activator.CreateInstance(GetType()) as IActionInfo;
            actionInfo.dependActorIndexes = (string[])dependActorIndexes.Clone();
            foreach (KeyValuePair<ICondition, IActionInfo> pair in _children)
            {
                actionInfo.Add(pair.Key.Clone(), pair.Value.Clone());
            }
            return actionInfo;
        }

        public ICollection<ICondition> Keys
        {
            get
            {
                return _children.Keys;
            }
        }

        public ICollection<IActionInfo> Values
        {
            get
            {
                return _children.Values;
            }
        }

        public IActionInfo this[ICondition key]
        {
            get
            {
                return _children[key];
            }
            set
            {
                _children[key] = value;
            }
        }

        public void Add(ICondition key, IActionInfo value)
        {
            _children.Add(key, value);
        }

        public bool ContainsKey(ICondition key)
        {
            return _children.ContainsKey(key);
        }

        public bool Remove(ICondition key)
        {
            return _children.Remove(key);
        }

        public void Clear()
        {
            _children.Clear();
        }

        public new virtual void Dispose()
        {
            _expired.Clear();
            _state = ActionState.None;
            _paused = false;
            _speed = 1;
            _progress = 0;
            _ready = false;
        }

        public static void UpdateReadyRecursion(IActionInfo actionInfo, bool ready)
        {
            actionInfo.ready = ready;
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                UpdateReadyRecursion(childInfo, ready);
            }
        }

        public static void UpdateStateRecursion(IActionInfo actionInfo, ActionState state)
        {
            actionInfo.state = state;
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                UpdateStateRecursion(childInfo, state);
            }
        }

        public static void UpdatePauseRecursion(IActionInfo actionInfo, bool paused)
        {
            actionInfo.paused = paused;
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                UpdatePauseRecursion(childInfo, paused);
            }
        }

        public static void UpdateSpeedRecursion(IActionInfo actionInfo, float speed)
        {
            actionInfo.speed = speed;
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                UpdateSpeedRecursion(childInfo, speed);
            }
        }

        public static void DisposeRecursion(IActionInfo actionInfo)
        {
            actionInfo.Dispose();
            foreach (IActionInfo childInfo in actionInfo.Values)
            {
                DisposeRecursion(childInfo);
            }
        }
    }
}
