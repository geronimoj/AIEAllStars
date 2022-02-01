using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads the given scene
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {   //If in a networked lobby, use photon stuff
        if (NetworkManager.InRoom)
            PhotonNetwork.LoadLevel(sceneName);
        else
            //Load the target scene
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    /// <summary>
    /// Closes the app
    /// </summary>
    public void CloseApp() => Application.Quit();
}
