using System.Collections.Generic;

public class GameStateDto
{
    public byte[] mapPicture;
    
    public Dictionary<string, byte[]> creaturePictures;

    public string sceneData;
    
    public GameStateDto() {
    }

    public GameStateDto(byte[] mapPicture, Dictionary<string, byte[]> creaturePictures, string sceneData) {
        this.mapPicture = mapPicture;
        this.creaturePictures = creaturePictures;
        this.sceneData = sceneData;
    }
}
