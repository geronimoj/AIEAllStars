using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager s_instance = null;
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
    /// <summary>
    /// Which player won
    /// </summary>
    public static byte s_winningPlayer = 0;
    /// <summary>
    /// Does the game contain AI
    /// </summary>
    public static bool s_useAI = false;
    /// <summary>
    /// The scores the players have
    /// </summary>
    public static byte[] s_scores = new byte[0];
    /// <summary>
    /// Reference to a scene loader for moving to other scenes after game end
    /// </summary>
    private SceneLoader _loader = null;
    /// <summary>
    /// The players in the game
    /// </summary>
    [HideInInspector]
    public Player[] _players = null;
    /// <summary>
    /// The default map
    /// </summary>
    public GameObject _defaultMap = null;
    /// <summary>
    /// Default player character if none is assigned
    /// </summary>
    public GameObject _defaultCharacter = null;
    /// <summary>
    /// Input object for player 1
    /// </summary>
    public PlayerInput _p1Input = null;
    /// <summary>
    /// Input object for player 2
    /// </summary>
    public PlayerInput _p2Input = null;
    /// <summary>
    /// The maximum game time
    /// </summary>
    public float _maxGameTime = 99;
    /// <summary>
    /// The count down timer for the game start
    /// </summary>
    public float _startCountDown = 3;
    /// <summary>
    /// The timer for when the game finishes
    /// </summary>
    public float _endGameTime = 3;
    /// <summary>
    /// The number of points required to win
    /// </summary>
    public byte _winAmount = 3;
    /// <summary>
    /// Called when the setup is complete
    /// </summary>
    public UnityEvent OnSetupComplete;
    /// <summary>
    /// Called when the game starts
    /// </summary>
    public UnityEvent OnGameStart;
    /// <summary>
    /// Called when the game ends
    /// </summary>
    public UnityEvent OnGameEnd;

    public CinemachineTargetGroup group;

    [HideInInspector]
    public float m_startTime = 0;

    private void Awake()
    {
        s_instance = this;
    }

    private void Start()
    {
        _loader = GetComponent<SceneLoader>();
        SpawnLevel();
    }
    /// <summary>
    /// Spawns the level and gets the spawn points. This then tells players to spawn
    /// </summary>
    private void SpawnLevel()
    {
        if (!s_map)
            s_map = _defaultMap;
        //Spawn map
        GameObject map = GameObject.Instantiate(s_map, Vector3.zero, s_map.transform.rotation);
        //Get spawn points
        SpawnPoint[] points = map.GetComponentsInChildren<SpawnPoint>();

        if (points.Length < 2)
            Debug.LogError("Not enough spawn points");
        else//Spawn players
            SpawnPlayers(points);
    }
    /// <summary>
    /// Spawns the 2 players in
    /// </summary>
    /// <param name="points"></param>
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

        GameObject obj;
        CinemachineTargetGroup.Target p1 = new CinemachineTargetGroup.Target() { weight = 1, radius = 2 };
        CinemachineTargetGroup.Target p2 = new CinemachineTargetGroup.Target() { weight = 1, radius = 2 };
        _players = new Player[2];
        s_scores = new byte[2];
        //Spawn player 1
        if (points[0])
        {
            obj = Instantiate(s_p1Char, points[0].transform.position, s_p1Char.transform.rotation);
            _players[0] = obj.GetComponent<Player>();

            _players[0].Controls = _p1Input;
            p1.target = obj.transform;
        }
        //Spawn player 2
        if (points[1])
        {
            obj = Instantiate(s_p2Char, points[1].transform.position, s_p1Char.transform.rotation);
            _players[1] = obj.GetComponent<Player>();

            _players[1].Controls = _p2Input;
            p2.target = obj.transform;
        }

        group.m_Targets = new CinemachineTargetGroup.Target[2] { p1, p2 };

        StartCoroutine(GameStart());
    }
    /// <summary>
    /// Called after both players have been spawned
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameStart()
    {
        OnSetupComplete.Invoke();

        for (byte i = 0; i < _players.Length; i++)
            _players[i].enabled = false;
        Debug.LogError("Player Freezing / Unfreezing not implemented");
        yield return new WaitForSeconds(_startCountDown);

        for (byte i = 0; i < _players.Length; i++)
            _players[i].enabled = true;
        //Start GameTimer
        StartCoroutine(GameTimer());
    }
    /// <summary>
    /// The timer for the game. When it reaches 0, game ends reguardless
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameTimer()
    {
        OnGameStart.Invoke();
        m_startTime = Time.time;
        yield return new WaitForSeconds(_maxGameTime);
        //End game if its not already over
        GameEnd();
    }
    /// <summary>
    /// Timer for the game end
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameEndTimer()
    {   //Disable on game end
        for (byte i = 0; i < _players.Length; i++)
            _players[i].enabled = false;

        yield return new WaitForSeconds(_endGameTime);
        Debug.LogError("Game End Timer not implemented");

        if (s_scores[s_winningPlayer] >= _winAmount)
            //Load the win scene
            _loader.LoadScene("GameWin");
        else
            _loader.LoadScene("Game");
    }
    /// <summary>
    /// Call to end the game
    /// </summary>
    private void GameEnd()
    {
        Debug.LogError("Game End not implemented");

        float winnerHealth = _players[0].CurrentHealth;
        s_winningPlayer = 0;
        for (byte i = 1; i < _players.Length; i++)
        {   //If this player has max health
            if (_players[1].CurrentHealth > winnerHealth)
            {   //They are the winner
                s_winningPlayer = i;
                winnerHealth = _players[i].CurrentHealth;
            }
        }
        //Give the winning player points
        s_scores[s_winningPlayer]++;

        OnGameEnd.Invoke();
        //Start Game end timer
        StartCoroutine(GameEndTimer());
    }
    /// <summary>
    /// Checks if either players have died
    /// </summary>
    private void Update()
    {
        for (byte i = 0; i < _players.Length; i++)
            if (_players[i].CurrentHealth <= 0)
            {   //Game has ended
                GameEnd();
                return;
            }
    }
}
