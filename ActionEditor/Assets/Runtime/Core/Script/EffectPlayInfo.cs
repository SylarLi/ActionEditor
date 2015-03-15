using UnityEngine;

namespace Core
{
    public class EffectPlayInfo : MonoBehaviour, IRadio
    {
        [HideInInspector]
        [SerializeField]
        public float duration = 0;

        private RadioState _state;

        private bool _pause;

        private float _speed;

        private float _progress;

        private int _frame;

        private ParticleSystem[] _particles;

        private Animator[] _animators;

        private TrailRenderer[] _trails;

        void Awake()
        {
            _state = RadioState.None;
            _pause = false;
            _speed = 1;
            _progress = 0;
            _frame = 0;
            _particles = GetComponentsInChildren<ParticleSystem>();
            _animators = GetComponentsInChildren<Animator>();
            _trails = GetComponentsInChildren<TrailRenderer>();

            state = RadioState.Stop;
        }

        void Update()
        {
            if (state == RadioState.Play)
            {
                if (duration > 0)
                {
                    progress += Time.deltaTime * speed / duration;
                }
                if (_frame == 1)
                {
                    foreach (TrailRenderer trail in _trails)
                    {
                        trail.gameObject.SetActive(true);
                    }
                    foreach (ParticleSystem particle in _particles)
                    {
                        particle.gameObject.SetActive(true);
                    }
                }
                _frame++;
            }
        }

        public RadioState state
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
                }
            }
        }

        public bool pause
        {
            get
            {
                return _pause;
            }
            set
            {
                if (_pause != value)
                {
                    _pause = value;
                    UpdateState();
                }
            }
        }

        public float speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    UpdateSpeed();
                }
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
                if (_progress != value)
                {
                    _progress = value;
                    UpdateProgress();
                }
            }
        }

        protected virtual void UpdateState()
        {
            switch (state)
            {
                case RadioState.Play:
                    {
                        if (pause)
                        {
                            Pause();
                        }
                        else
                        {
                            Play();
                        }
                        break;
                    }
                case RadioState.Stop:
                    {
                        Stop();
                        break;
                    }
            }
        }

        protected virtual void Play()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null)
                {
                    particle.Play();
                }
            }
            foreach (Animator animator in _animators)
            {
                if (animator != null)
                {
                    animator.enabled = true;
                }
            }
            foreach (TrailRenderer trail in _trails)
            {
                if (trail != null)
                {
                    trail.enabled = true;
                }
            }
        }

        protected virtual void Pause()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null)
                {
                    particle.Pause();
                }
            }
            foreach (Animator animator in _animators)
            {
                if (animator != null)
                {
                    animator.enabled = false;
                }
            }
            foreach (TrailRenderer trail in _trails)
            {
                if (trail != null)
                {
                    trail.enabled = false;
                }
            }
        }

        protected virtual void Stop()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null)
                {
                    particle.Clear();
                    particle.Stop();
                    particle.gameObject.SetActive(false);
                }
            }
            foreach (Animator animator in _animators)
            {
                if (animator != null)
                {
                    animator.gameObject.SetActive(false);
                    animator.gameObject.SetActive(true);
                    animator.enabled = false;
                }
            }
            foreach (TrailRenderer trail in _trails)
            {
                if (trail != null)
                {
                    trail.enabled = false;
                    trail.gameObject.SetActive(false);
                }
            }
            progress = 0;
            _frame = 0;
        }

        protected virtual void UpdateSpeed()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null)
                {
                    particle.playbackSpeed = speed;
                }
            }
            foreach (Animator animator in _animators)
            {
                if (animator != null)
                {
                    animator.speed = speed;
                }
            }
        }

        protected virtual void UpdateProgress()
        {
            if (progress >= 1)
            {
                state = RadioState.Stop;
            }
        }
    }
}
