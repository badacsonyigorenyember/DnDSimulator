using System;
using Unity.Netcode;
using UnityEngine;

namespace Script.Utils
{
    [Serializable]
    public class EntityDto : INetworkSerializable
    {
        public string entityName;
        public int currentHp;
        public int maxHp;
        public bool isCharacter;
        public Vector2 position;
        public int initiativeModifier;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref entityName);
            serializer.SerializeValue(ref currentHp);
            serializer.SerializeValue(ref maxHp);
            serializer.SerializeValue(ref isCharacter);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref initiativeModifier);
        }
    }
}