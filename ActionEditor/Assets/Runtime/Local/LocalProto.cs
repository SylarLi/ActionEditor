using System;
using ProtoBuf;

namespace Local
{
    [Serializable, ProtoContract(Name = @"LocalProto")]
    public class LocalProto<T> : LocalData where T : IExtensible
    {
        protected T _value;

        public LocalProto() : base()
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

        [ProtoMember(3, IsRequired = true, Name = @"value", DataFormat = DataFormat.Default, DynamicType = true)]
        public T value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string protoName
        {
            get { return typeof(T).FullName; }
        }

        public override byte type
        {
            get { return LocalDataType.PROTO; }
        }
    }
}
