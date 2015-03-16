using UnityEngine;
using ProtoBuf;

namespace Action
{
    [ProtoContract]
    public class BodyEffectInfo : ActionInfo
    {
        // <----> 依赖 <---->

        public GameObject actor;

        // <----> 参数 <---->

        [ProtoMember(3)]
        public int effectId = 0;                                // 特效id

        [ProtoMember(4)]
        public float duration = 0;                              // 时间长度(s)(0表示不做限制)

        [ProtoMember(5)]
        public string bonePath = "";                            // 骨骼路径

        [ProtoMember(6)]
        public Vector3 localPosition = Vector3.zero;            // 本地偏移

        [ProtoMember(7)]
        public Vector3 localRotation = Vector3.zero;            // 本地旋转

        [ProtoMember(8)]
        public Vector3 localScale = Vector3.one;                // 本地缩放

        public override ActionType type
        {
            get
            {
                return ActionType.BodyEffect;
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
                return new string[] { "附加对象" };
            }
        }

        public override void InjectDepends(object[] depends)
        {
            actor = depends[0] as GameObject;
        }

        public override bool IsDependValid()
        {
            return actor != null;
        }

        public override IActionInfo Clone()
        {
            BodyEffectInfo actionInfo = base.Clone() as BodyEffectInfo;
            actionInfo.effectId = effectId;
            actionInfo.duration = duration;
            actionInfo.bonePath = bonePath;
            actionInfo.localPosition = localPosition;
            actionInfo.localRotation = localRotation;
            actionInfo.localScale = localScale;
            return actionInfo;
        }

        public override void Dispose()
        {
            base.Dispose();
            actor = null;
        }
    }
}
