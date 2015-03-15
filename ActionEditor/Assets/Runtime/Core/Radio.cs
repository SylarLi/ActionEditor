namespace Core
{
    public class Radio : IRadio
    {
        private RadioState _state;

        private bool _pause;

        private float _speed;

        private float _progress;

        public Radio()
        {
            _state = RadioState.None;
            _pause = false;
            _speed = 1;
            _progress = 0;
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

        }

        protected virtual void Pause()
        {

        }

        protected virtual void Stop()
        {

        }

        protected virtual void UpdateSpeed()
        {

        }

        protected virtual void UpdateProgress()
        {

        }
    }
}
