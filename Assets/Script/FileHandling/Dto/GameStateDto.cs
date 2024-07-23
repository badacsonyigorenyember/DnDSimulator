using System;
using System.Collections.Generic;
using Unity.Netcode;
using Newtonsoft.Json;

[Serializable]
public class GameStateDto : INetworkSerializable
{
    private byte[] mapPicture;
    
    private Dictionary<string, byte[]> creaturePictures;

    private string sceneData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref mapPicture);
        serializer.SerializeValue(ref sceneData, true);
        
        string creaturePictureJson = "";
        if (serializer.IsWriter) {
            creaturePictureJson = JsonConvert.SerializeObject(creaturePictures);
        }
        serializer.SerializeValue(ref creaturePictureJson, true);
        if (serializer.IsReader) {
            creaturePictures = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(creaturePictureJson);
        }
    }

    public int GetSize() {
        int resultSize = 0; // Magic number
    
        // Size of mapPicture (byte array length + content)
        resultSize += sizeof(int); // Length prefix for the byte array
        resultSize += mapPicture.Length;

        // Size of sceneData (string length + content)
        resultSize += sizeof(int); // Length prefix for the string
        resultSize += sceneData.Length;

        // Size of creaturePictures (JSON string length + content)
        string creaturePictureJson = JsonConvert.SerializeObject(creaturePictures);
        resultSize += sizeof(int); // Length prefix for the JSON string
        resultSize += creaturePictureJson.Length;

        return resultSize;
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
