using System;
using ProtoBuf;

namespace Local
{
    public abstract class LocalData : IExtensible
    {
        protected string _key;

        protected DateTime _timeStamp;

        protected string _serial;

        public virtual byte type
        {
            get { return 0; }
        }

        public string key
        {
            get { return _key; }
        }

        public string serial
        {
            get { return _serial; }
            set { _serial = value; }
        }

        public DateTime timeStamp
        {
            get { return _timeStamp; }
        }

        public void MarkTimeStamp()
        {
            _timeStamp = DateTime.Now;
        }

        private IExtension extensionObject;
        public IExtension GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
