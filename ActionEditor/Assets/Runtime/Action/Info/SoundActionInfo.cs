using ProtoBuf;

namespace Action
{
    [ProtoContract]
    public class SoundActionInfo : ActionInfo
    {
        // <----> 参数 <---->

        [ProtoMember(3)]
        public int soundId = 0;

        public override ActionType type
        {
            get
            {
                return ActionType.Sound;
            }
        }

        public override IActionInfo Clone()
        {
            SoundActionInfo actionInfo = base.Clone() as SoundActionInfo;
            actionInfo.soundId = soundId;
            return actionInfo;
        }
    }
}
