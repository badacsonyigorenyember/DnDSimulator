using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneHandler 
{
    public static void LoadSceen(string scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single); 
    }
}
