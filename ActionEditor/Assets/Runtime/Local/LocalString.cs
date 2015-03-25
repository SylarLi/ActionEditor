using System;
using ProtoBuf;

namespace Local
{
    [Serializable, ProtoContract(Name = @"LocalString")]
    public class LocalString : LocalData
    {
        protected string _value;

        public LocalString() : base()
        {

        }

        [ProtoMember(1, IsRequired = true, Name = @"key", DataFormat = DataFormat.Default)]
        public new string key
        {
            get { return _key; }
            set { _key = value; }
        }

        [ProtoMember(2, IsRequired = true, Name = @"timeStamp", DataFormat = DataFormat.Default)]
        public new DateTime timeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        [ProtoMember(3, IsRequired = true, Name = @"value", DataFormat = DataFormat.Default)]
        public string value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override byte type
        {
            get { return LocalDataType.STRING; }
        }
    }
}
