using System;
using System.Collections.Generic;
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
        
        string creaturePictureJson = "";
        if (serializer.IsWriter) {
            creaturePictureJson = JsonUtility.ToJson(creaturePictures);
        }
        serializer.SerializeValue(ref creaturePictureJson);
        if (serializer.IsReader) {
            creaturePictures = JsonUtility.FromJson<Dictionary<string, byte[]>>(creaturePictureJson);
        }
    }

    public GameStateDto() {
    }

    public GameStateDto(byte[] mapPicture, Dictionary<string, byte[]> creaturePictures, string sceneData) {
        this.mapPicture = mapPicture;
        this.creaturePictures = creaturePictures;
        this.sceneData = sceneData;
    }

    public byte[] GetMapPicture() {
        return mapPicture;
    }

    public Dictionary<string, byte[]> GetCreaturePictures() {
        return creaturePictures;
    }

    public string GetSceneData() {
        return sceneData;
    }
}
