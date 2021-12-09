using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class LoadSceneAfterTime : MonoBehaviour
{
    /// <summary>
    /// Reference to a sceneLoader
    /// </summary>
    private SceneLoader _loader = null;
    /// <summary>
    /// The scene to load after the timer
    /// </summary>
    public string m_targetScene = string.Empty;
    /// <summary>
    /// How long to wait
    /// </summary>
    public float m_waitTime = 5;

    private void Awake()
    {   //Get reference
        if (!_loader)
            _loader = GetComponent<SceneLoader>();
        //Load scene after timer
        StartCoroutine(LoadScene());
    }
    /// <summary>
    /// Loads the target scene after waitTime
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(m_waitTime);
        //Load scene
        _loader.LoadScene(m_targetScene);
    }
}
