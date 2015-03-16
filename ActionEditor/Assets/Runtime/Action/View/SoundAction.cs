using Core;
using UnityEngine;

namespace Action
{
    public class SoundAction : ActionView<SoundActionInfo>
    {
        private const float EndProgress = 1f;

        private GameObject sound;

        private AudioSource audio;

        private SoundListener listener;

        public SoundAction(SoundActionInfo actionInfo) : base(actionInfo)
        {

        }

        public override void Load()
        {
            setting.loader.GetSound(actionInfo.soundId, (GameObject gameObject) =>
            {
                // 初始化
                sound = gameObject;
                audio = sound.GetComponent<AudioSource>();
                listener = sound.AddComponent<SoundListener>();
                listener.AddEventListener(SoundListener.NormalizedTime, NormalizedTimeHandler);
                listener.enabled = false;
                // 标记初始化完成
                actionInfo.ready = true;
            });
        }

        protected override void Play()
        {
            sound.SetActive(true);
            audio.Play();
            listener.enabled = true;
        }

        protected override void Pause()
        {
            if (audio.isPlaying)
            {
                audio.Pause();
            }
            listener.enabled = false;
        }

        protected override void Stop()
        {
            listener.RemoveEventListener(SoundListener.NormalizedTime, NormalizedTimeHandler);
            Component.Destroy(listener);
            setting.loader.ReturnSound(actionInfo.soundId, sound);
            sound = null;
            audio = null;
        }

        protected override void Speed()
        {
            // 无效
        }

        private void NormalizedTimeHandler(IEvent e)
        {
            actionInfo.progress = listener.normalizedTime;
            actionInfo.InjectCondition(ConditionType.Progress, actionInfo.progress);
            if (actionInfo.progress >= EndProgress)
            {
                actionInfo.state = ActionState.Stop;
            }
        }
    }
}
