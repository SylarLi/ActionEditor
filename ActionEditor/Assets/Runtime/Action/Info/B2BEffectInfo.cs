using ProtoBuf;
using UnityEngine;

namespace Action
{
    public class B2BEffectInfo : ActionInfo
    {
        // <-----> 依赖 <----->

        public GameObject fromActor;

        public GameObject toActor;

        // <-----> 参数 <----->

        [ProtoMember(3)]
        public int effectId = 0;                                // 特效id

        public Vector3 scale = Vector3.one;                     // 特效缩放

        public Vector3 fromOffset = Vector3.zero;               // 起点偏移(相对于起点-->终点向量，如果为向量为zero，则相对于起点forward)

        public Vector3 toOffset = Vector3.zero;                 // 终点偏移(相对于起点-->终点向量，如果为向量为zero，则相对于起点forward)

        public AnimationCurve xCurve;                           // x偏移曲线(time:0-->1, value真实值)

        public AnimationCurve yCurve;                           // y偏移曲线(time:0-->1, value真实值)

        public AnimationCurve zCurve;                           // z偏移曲线(time:0-->1, value真实值)

        public float flySpeed = 4f;                             // 飞行速度(m/s)

        public override ActionType type
        {
            get
            {
                return ActionType.B2BEffect;
            }
        }

        public override DependType[] dependTypes
        {
            get
            {
                return new DependType[] { DependType.Actor, DependType.Actor };
            }
        }

        public override string[] dependDescripts
        {
            get
            {
                return new string[] { "起始对象", "目标对象" };
            }
        }

        public override void InjectDepends(object[] depends)
        {
            fromActor = depends[0] as GameObject;
            toActor = depends[1] as GameObject;
        }

        public override bool IsDependValid()
        {
            return fromActor != null && toActor != null;
        }

        public override IActionInfo Clone()
        {
            B2BEffectInfo actionInfo = base.Clone() as B2BEffectInfo;
            actionInfo.effectId = effectId;
            actionInfo.scale = scale;
            actionInfo.fromOffset = fromOffset;
            actionInfo.toOffset = toOffset;
            actionInfo.xCurve = xCurve != null ? new AnimationCurve((Keyframe[])xCurve.keys.Clone()) : null;
            actionInfo.yCurve = yCurve != null ? new AnimationCurve((Keyframe[])yCurve.keys.Clone()) : null;
            actionInfo.zCurve = zCurve != null ? new AnimationCurve((Keyframe[])zCurve.keys.Clone()) : null;
            actionInfo.flySpeed = flySpeed;
            return actionInfo;
        }

        public override void Dispose()
        {
            base.Dispose();
            fromActor = null;
            toActor = null;
        }
    }
}
