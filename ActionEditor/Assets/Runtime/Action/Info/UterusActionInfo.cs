using ProtoBuf;
using UnityEngine;

namespace Action
{
    [ProtoContract]
    public class UterusActionInfo : ActionInfo
    {
        // <----> 依赖 <---->

        public GameObject mother;

        public GameObject[] children;

        // <----> 参数 <---->

        [ProtoMember(3)]
        public IActionInfo embryo = new ActionInfo();           // 子动作

        public override ActionType type
        {
            get
            {
                return ActionType.Uterus;
            }
        }

        public override DependType[] dependTypes
        {
            get
            {
                return new DependType[] { DependType.Actor, DependType.Actors };
            }
        }

        public override string[] dependDescripts
        {
            get
            {
                return new string[] { "母体", "目标对象群" };
            }
        }

        public override void InjectDepends(object[] depends)
        {
            mother = depends[0] as GameObject;
            children = depends[1] as GameObject[];
        }

        public override bool IsDependValid()
        {
            return children != null && children.Length > 0;
        }

        public override IActionInfo Clone()
        {
            UterusActionInfo actionInfo = base.Clone() as UterusActionInfo;
            actionInfo.embryo = embryo.Clone();
            return embryo;
        }

        public override void Dispose()
        {
            base.Dispose();
            mother = null;
            children = null;
        }
    }
}
