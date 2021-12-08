using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Player 1s Character
    /// </summary>
    public static GameObject s_p1Char = null;
    /// <summary>
    /// Player 2s character
    /// </summary>
    public static GameObject s_p2Char = null;
    /// <summary>
    /// Map to spawn
    /// </summary>
    public static GameObject s_map = null;

    public GameObject _defaultCharacter = null;

    public float _maxGameTime = 99;

    public float _startCountDown = 3;

    public float _endGameTime = 3;

    private void Start()
    {
        SpawnLevel();
    }

    private void SpawnLevel()
    {   //Spawn map
        GameObject map = GameObject.Instantiate(s_map, Vector3.zero, s_map.transform.rotation);
        //Get spawn points
        SpawnPoint[] points = map.GetComponentsInChildren<SpawnPoint>();

        if (points.Length < 2)
            Debug.LogError("Not enough spawn points");
        else//Spawn players
            SpawnPlayers(points);
    }

    private void SpawnPlayers(SpawnPoint[] points)
    {   //Do nothing
        if (points.Length < 2)
            return;

        if (_defaultCharacter)
        {
            //Null catch players
            if (!s_p1Char)
                s_p1Char = _defaultCharacter;

            if (!s_p2Char)
                s_p2Char = _defaultCharacter;
        }

        //Spawn player 1
        if (points[0])
            Instantiate(s_p1Char, points[0].transform.position, s_p1Char.transform.rotation);
        //Spawn player 2
        if (points[1])
            Instantiate(s_p1Char, points[1].transform.position, s_p1Char.transform.rotation);

        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        Debug.LogError("Player Freezing / Unfreezing not implemented");
        yield return new WaitForSeconds(_startCountDown);
        //Start GameTimer
        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(_maxGameTime);
        //End game if its not already over
        GameEnd();
    }

    private IEnumerator GameEndTimer()
    {
        yield return new WaitForSeconds(_endGameTime);
        Debug.LogError("Game End Timer not implemented");
    }

    private void GameEnd()
    {
        Debug.LogError("Game End not implemented");
        //Start Game end timer
        StartCoroutine(GameEndTimer());
    }
}
