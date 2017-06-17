using System;

namespace SQLite.Net
{
    public delegate bool CanSerializeAction(Type type);

    public delegate object DeserializeAction(byte[] data, Type type);

    public delegate byte[] SerializeAction(object obj);

    public class BlobSerializer : IBlobSerializer
    {
        private readonly CanSerializeAction _canDeserializeAction;
        private readonly DeserializeAction _deserializeAction;
        private readonly SerializeAction _serializeAction;

        public BlobSerializer(SerializeAction serializeAction,
            DeserializeAction deserializeAction,
            CanSerializeAction canDeserializeAction)
        {
            _serializeAction = serializeAction;
            _deserializeAction = deserializeAction;
            _canDeserializeAction = canDeserializeAction;
        }

        #region IBlobSerializer implementation

        
        public byte[] Serialize<T>(T obj)
        {
            return _serializeAction(obj);
        }

        
        public object Deserialize(byte[] data, Type type)
        {
            return _deserializeAction(data, type);
        }

        
        public bool CanDeserialize(Type type)
        {
            return _canDeserializeAction(type);
        }

        #endregion
    }
}