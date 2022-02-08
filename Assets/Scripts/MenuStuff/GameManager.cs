using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using Photon.Pun;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviourPun
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
    public static bool s_useP2AI = false;

    public static bool s_useP1AI = false;
    /// <summary>
    /// The scores the players have
    /// </summary>
    public static byte[] s_scores = null;
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

    private bool _gameOver = false;
    /// <summary>
    /// Used to determine if a player can spawn themself in a networked lobby
    /// </summary>
    private bool _doSpawnPlayer = false;
    private static bool _otherPlayerReady = false;
    private GameObject map;

    private void Awake()
    {
        s_instance = this;
    }

    private void Start()
    {
        _gameOver = false;
        _players = new Player[2];
        if (s_scores == null)
            s_scores = new byte[2];
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
        map = GameObject.Instantiate(s_map, Vector3.zero, s_map.transform.rotation);
        //Get spawn points
        SpawnPoint[] points = map.GetComponentsInChildren<SpawnPoint>();
        _doSpawnPlayer = true;

        if (points.Length < 2)
            Debug.LogError("Not enough spawn points");
        //If we are in a networked room, we want to wait until everyone has loaded the scene to spawn ourself
        //to avoid the spawned self being spawned in the old scene on other clients
        else if (NetworkManager.InRoom)
        {
            StartCoroutine(SpawnSelfNetworked());
            //Tell the other player we are ready for them to spawn
            photonView.RPC("OtherPlayerReady", RpcTarget.Others);
        }
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

        //Local or networked game check
        if (!NetworkManager.InRoom)
        {
            //Spawn player 1
            if (points[0])
            {
                obj = Instantiate(s_p1Char, points[0].transform.position, s_p1Char.transform.rotation);
                _players[0] = obj.GetComponent<Player>();
                _players[0].IsAI = s_useP1AI;
                _players[0].Controls = _p1Input;
                p1.target = obj.transform;
            }
            //Spawn player 2
            if (points[1])
            {
                obj = Instantiate(s_p2Char, points[1].transform.position, s_p1Char.transform.rotation);
                _players[1] = obj.GetComponent<Player>();
                _players[1].IsAI = s_useP2AI;
                _players[1].Controls = _p2Input;
                p2.target = obj.transform;
            }
        }
        else
        {   //Host is always P1
            if (NetworkManager.AmHost)
            {
                if (!points[0])
                {
                    Debug.LogError("The hosts spawn point doesn't exist. Cannot spawn them");
                    return;
                }
                //Spawn the player their character
                obj = PhotonNetwork.Instantiate("Characters/" + s_p1Char.name, points[0].transform.position, s_p1Char.transform.rotation);
                _players[0] = obj.GetComponent<Player>();
                _players[0].IsAI = false;
                _players[0].Controls = _p1Input;
                p1.target = obj.transform;
            }
            //Client is always P2
            else
            {
                if (!points[1])
                {
                    Debug.LogError("The client spawn point doesn't exist. Cannot spawn them");
                    return;
                }
                //Spawn the player their character
                obj = PhotonNetwork.Instantiate("Characters/" + s_p2Char.name, points[1].transform.position, s_p2Char.transform.rotation);
                _players[1] = obj.GetComponent<Player>();
                _players[1].IsAI = false;
                //P2 can still use p1 input for consistency
                _players[1].Controls = _p1Input;
                p2.target = obj.transform;
            }
            //Tell the other player about us
            PhotonView v = obj.GetComponent<PhotonView>();
            photonView.RPC("SendOtherPlayer", RpcTarget.Others, v.ViewID, !NetworkManager.AmHost);
            //Start a coroutine to make sure the camreas targets get assigned propperly.
            StartCoroutine(SetCameraTargets());
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
            if (_players[i])
                _players[i].enabled = false;

        yield return new WaitForSeconds(_startCountDown);

        for (byte i = 0; i < _players.Length; i++)
            if (_players[i])
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

        if (s_scores[s_winningPlayer] >= _winAmount)
        {
            //Load the win scene
            _loader.LoadScene("GameWin");
            //Clear the scores for later
            s_scores = null;
        }
        else
            _loader.LoadScene("Game");
    }
    /// <summary>
    /// Call to end the game
    /// </summary>
    private void GameEnd()
    {   //Avoid duplicate calls
        if (_gameOver)
            return;

        _gameOver = true;
        //Let the host perform the winner math
        if (NetworkManager.AmHost)
        {
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
            //If its a networked game, update the other players score
            if (NetworkManager.InRoom)
                photonView.RPC("UpdateScores", RpcTarget.Others, s_scores, s_winningPlayer);
        }
        OnGameEnd.Invoke();
        //Start Game end timer
        StartCoroutine(GameEndTimer());
    }
    /// <summary>
    /// Checks if either players have died
    /// </summary>
    private void Update()
    {   //If its a networked game, don't allow this functionality
        if (Input.GetKeyDown(KeyCode.Escape) && !PhotonNetwork.IsConnected)
            GetComponent<SceneLoader>().LoadScene("MainMenu");

        //If we are alone in a networked lobby, return to main menu
        if (!_gameOver && NetworkManager.InRoom && NetworkManager.AmAlone)
        {   //Disconnect from photon and return to main menu if the other player leaves
            NetworkManager.DisconnectFromServers();
            GetComponent<SceneLoader>().LoadScene("MainMenu");
            _gameOver = true;
            return;
        }

        //Only let host check for game over
        if (!_gameOver && NetworkManager.AmHost)
            for (byte i = 0; i < _players.Length; i++)
                if (_players[i] && _players[i].CurrentHealth <= 0)
                {   //Game has ended
                    GameEnd();
                    return;
                }
    }

    [PunRPC]
    private void UpdateScores(byte[] newScores, byte winningPlayer)
    {   //Store the new scores
        s_scores = newScores;
        s_winningPlayer = winningPlayer;
        //Make sure the game has ended for the client
        if (!_gameOver)
            GameEnd();
    }

    [PunRPC]
    private void SendOtherPlayer(int viewIndex, bool isHost)
    {
        Debug.Log("Sending myself to the other player");
        //Get the photonView
        PhotonView view = PhotonNetwork.GetPhotonView(viewIndex);
        //Index for storing stuff
        int index = isHost ? 1 : 0;
        //Store the player
        _players[index] = view.GetComponent<Player>();
        //Set the second target for the camera
        group.m_Targets[index].target = view.transform;
    }
    /// <summary>
    /// For telling the other player you are ready to begin
    /// </summary>
    [PunRPC]
    private void OtherPlayerReady()
    {
        _otherPlayerReady = true;
    }

    private IEnumerator SpawnSelfNetworked()
    {   //Wait until we are able to spawn our player
        yield return new WaitUntil(() => _otherPlayerReady && _doSpawnPlayer);
        //Reset the booleans to false to avoid spawning again somehow
        _doSpawnPlayer = false;
        _otherPlayerReady = false;
        //Spawn ourself
        SpawnPoint[] points = map.GetComponentsInChildren<SpawnPoint>();
        SpawnPlayers(points);
    }

    private IEnumerator SetCameraTargets()
    {   //Wait for camera to be intiialized
        yield return new WaitUntil(() => group && group.m_Targets.Length == 2);

        byte passes = 0;
        //Loop until no targets are null
        while (passes < 2)
        {
            passes = 0;
            //Loop over targets and check for any null targets
            for (byte i = 0; i < group.m_Targets.Length; i++)
                if (!group.m_Targets[i].target)
                {   //I hate doing an if stack like this but it saves having to check if player is null afterwards in an if elseif
                    if (_players[i])
                        //If a target is null, attempt to assign
                        group.m_Targets[i].target = _players[i].transform;
                }
                else//Otherwise increase the passes
                    passes++;
            //Wait a cycle
            yield return null;
        }
    }
}
