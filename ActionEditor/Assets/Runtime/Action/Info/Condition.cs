using ProtoBuf;

namespace Action
{
    [ProtoContract]
    public class Condition : ICondition
    {
        [ProtoMember(1)]
        private ConditionType _type;

        [ProtoMember(2)]
        private float _progress;

        public Condition()
        {
            _type = ConditionType.Start;
            _progress = 0;
        }

        public Condition(ConditionType type)
        {
            _type = type;
            _progress = 0;
        }

        public Condition(ConditionType type, float progress)
        {
            _type = type;
            _progress = progress;
        }

        public ConditionType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public float progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
            }
        }

        public bool Meet(float value)
        {
            bool meet = false;
            switch (type)
            {
                case ConditionType.Progress:
                    {
                        meet = value >= progress;
                        break;
                    }
                case ConditionType.Start:
                case ConditionType.End:
                    {
                        meet = true;
                        break;
                    }
            }
            return meet;
        }

        public ICondition Clone()
        {
            return new Condition(type, progress);
        }
    }
}
