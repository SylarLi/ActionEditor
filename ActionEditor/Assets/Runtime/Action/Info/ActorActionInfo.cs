using ProtoBuf;
using UnityEngine;

namespace Action
{
    [ProtoContract]
    public class ActorActionInfo : ActionInfo
    {
        // <-----> 依赖 <----->
        
        public GameObject actor;

        // <-----> 参数 <----->

        [ProtoMember(3)]
        public string actorActionName = ActorActionType.Atsk + "100";            // 动作名称

        [ProtoMember(4)]
        public float duration = 1f;                                             // 正交化生命周期(time: 0--?)(例如2代表循环两次)

        [ProtoMember(5)]
        public float inEase = 0.2f;                                             // 正交化淡入时间

        [ProtoMember(6)]
        public float outEase = 0.2f;                                             // 正交化淡出时间

        public override ActionType type
        {
            get
            {
                return ActionType.ActorAction;
            }
        }

        public override DependType[] dependTypes
        {
            get
            {
                return new DependType[] { DependType.Actor };
            }
        }

        public override string[] dependDescripts
        {
            get
            {
                return new string[] { "动作对象" };
            }
        }

        public override bool IsDependValid()
        {
            return actor != null;
        }

        public override void InjectDepends(object[] depends)
        {
            actor = depends[0] as GameObject;
        }

        public override IActionInfo Clone()
        {
            ActorActionInfo actionInfo = base.Clone() as ActorActionInfo;
            actionInfo.actorActionName = actorActionName;
            actionInfo.duration = duration;
            actionInfo.inEase = inEase;
            actionInfo.outEase = outEase;
            return actionInfo;
        }

        public override void Dispose()
        {
            base.Dispose();
            actor = null;
        }
    }
}
