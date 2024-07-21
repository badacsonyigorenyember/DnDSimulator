using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class GameStateDto : INetworkSerializable
{
    private byte[] mapPicture;
    
    private Dictionary<string, byte[]> creaturePictures;

    private string sceneData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref mapPicture);
        serializer.SerializeValue(ref sceneData);
        serializer.SerializeValue<Dictionary<string, byte>>(ref creaturePictures, new FastBufferWriter.ForPrimitives());
        
        /*int count = creaturePictures?.Count ?? 0;
        serializer.SerializeValue(ref count);
        if (serializer.IsWriter && creaturePictures != null) {
            foreach (var creaturePicture in creaturePictures) {
                var asd = creaturePicture;
                serializer.SerializeValue(asd);
            }
            for (int i = 0; i < count; i++) {
                byte[] creaturePicture = creaturePictures[];
                serializer.SerializeValue(ref creaturePicture);
            }
        } else {
            creaturePictures = new List<byte[]>(count);
            for (int i = 0; i < count; i++) {
                byte[] creaturePicture = null;
                serializer.SerializeValue(ref creaturePicture);
                creaturePictures.Add(creaturePicture);
            }
        }*/
    }

    public GameStateDto() {
    }

    public GameStateDto(byte[] mapPicture, List<byte[]> creaturePictures, string sceneData) {
        this.mapPicture = mapPicture;
        this.creaturePictures = creaturePictures;
        this.sceneData = sceneData;
    }

    public byte[] GetMapPicture() {
        return mapPicture;
    }

    public List<byte[]> GetCreaturePictures() {
        return creaturePictures;
    }

    public string GetSceneData() {
        return sceneData;
    }
}
