using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class LoadSceneAfterTime : MonoBehaviour
{
    public SceneLoader _loader = null;

    public string _targetScene = string.Empty;

    public float _waitTime = 5;

    private void Awake()
    {
        if (!_loader)
            _loader = GetComponent<SceneLoader>();

        LoadScene();
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(_waitTime);

        _loader.LoadScene("Lobby");
    }
}
