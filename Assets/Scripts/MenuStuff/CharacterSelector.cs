using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
/// <summary>
/// Controls the selection of characters for players
/// </summary>
public class CharacterSelector : MonoBehaviourPun
{
    /// <summary>
    /// The map that has been selected
    /// </summary>
    public static SelectableCharacter s_selectedMap = null;

    public static SelectableCharacter s_p1Selected = null;
    public static SelectableCharacter s_p2Selected = null;
    /// <summary>
    /// Prefabs for the buttons
    /// </summary>
    [Tooltip("The UI button for selecting a character")]
    public Button _characterButtonPrefab = null;
    /// <summary>
    /// Parent for player 1s buttons
    /// </summary>
    [Tooltip("The parent for the buttons")]
    public Transform _p1ButtonParent = null;
    /// <summary>
    /// Parent for player 2s button
    /// </summary>
    [Tooltip("The parent for the buttons")]
    public Transform _p2ButtonParent = null;
    /// <summary>
    /// The map parent
    /// </summary>
    public Transform _mapParent = null;
    /// <summary>
    /// The charcaters to select from
    /// </summary>
    public SelectableCharacter[] _characters = new SelectableCharacter[0];
    /// <summary>
    /// The maps the players can play on
    /// </summary>
    public SelectableCharacter[] _maps = new SelectableCharacter[0];

    public SelectedUI p1, p2, map;

    public Transform photoBoothL, photoBoothR, mapBooth;

    private UnityEngine.Events.UnityEvent OnSelectCharacter = new UnityEngine.Events.UnityEvent();

    private UnityEngine.Events.UnityEvent OnSelectMap = new UnityEngine.Events.UnityEvent();

    private bool[] _playerReady = null;

    public Button nextButton = null;
    public Button startGameButton = null;

    public SelectableCharacter _randomCharacter;

    private void Awake()
    {   //When a character is selected, sync it with the client
        OnSelectCharacter.AddListener(SyncSelectedCharacter);
        OnSelectMap.AddListener(SyncSelectedMap);
    }
    /// <summary>
    /// Initialize charcaters
    /// </summary>
    private void Start()
    {   //Spawn uI
        SpawnPlayerUI(_p1ButtonParent, true);
        SpawnPlayerUI(_p2ButtonParent, false);
        SpawnMapUI(_mapParent);
        GameManager.s_useP2AI = false;
        GameManager.s_useP1AI = false;
        //Make sure the scores are cleared from any previous games
        GameManager.s_scores = null;

        s_selectedMap = _maps[0];
        s_p1Selected = _characters[0];
        s_p2Selected = _characters[0];
        p1.Target = s_p1Selected;
        p2.Target = s_p2Selected;
        map.Target = s_selectedMap;

        OnSelectCharacter.AddListener(UpdateBoothChar);
        UpdateBoothChar();

        OnSelectMap.AddListener(UpdateBoothMap);

        //Set defaults if null
        GameManager.s_p1Char = _characters[0].Prefab;
        GameManager.s_p2Char = _characters[0].Prefab;
        GameManager.s_map = _maps[0].Prefab;
        //Ready players
        _playerReady = new bool[2];
    }

    private void OnDestroy()
    {
        if (s_p1Selected == _randomCharacter)
            RandomPlayer1();
        if (s_p2Selected == _randomCharacter)
            RandomPlayer2();
    }
    /// <summary>
    /// Spawns a button for each selectable character
    /// </summary>
    /// <param name="parent">The parent for the button</param>
    /// <param name="isP1">Is this for player 1. Exists for the sake of assigning static values</param>
    private void SpawnPlayerUI(Transform parent, bool isP1)
    {   //Spawn UI
        for (int i = 0; i < _characters.Length; i++)
        {   //Store index for lambda
            int index = i;
            //Spawn button
            GameObject button = Instantiate(_characterButtonPrefab.gameObject, parent);

            Button b = button.GetComponent<Button>();
            SelectedUI ui = b.GetComponent<SelectedUI>();
            ui.Target = _characters[index];
            //Setup lambda
            if (isP1)
            {
                b.onClick.AddListener(() =>
                {   //If its a networked game, or we are not p1 (host), skip
                    if (NetworkManager.InRoom && !NetworkManager.AmHost)
                        return;

                    GameManager.s_p1Char = _characters[index].Prefab;
                    p1.Target = _characters[index];
                    s_p1Selected = _characters[index];
                    OnSelectCharacter.Invoke();
                });
            }
            else
            {
                b.onClick.AddListener(() =>
                {   //If its networked and we are the host, skip
                    if (NetworkManager.InRoom && NetworkManager.AmHost)
                        return;

                    GameManager.s_p2Char = _characters[index].Prefab;
                    p2.Target = _characters[index];
                    s_p2Selected = _characters[index];
                    OnSelectCharacter.Invoke();
                });
            }
        }
    }

    private void SpawnMapUI(Transform parent)
    {   //Spawn UI
        for (int i = 0; i < _maps.Length; i++)
        {   //Store index for lambda
            int index = i;
            //Spawn button
            GameObject button = Instantiate(_characterButtonPrefab.gameObject, parent);

            Button b = button.GetComponent<Button>();
            SelectedUI ui = b.GetComponent<SelectedUI>();
            ui.Target = _maps[index];
            //Setup lambda
            b.onClick.AddListener(() =>
            {   //If we are in a networked game but not the host, don't do anything
                if (NetworkManager.InRoom && !NetworkManager.AmHost)
                    return;

                GameManager.s_map = _maps[index].Prefab;
                map.Target = _maps[index];
                s_selectedMap = _maps[index];
                OnSelectMap.Invoke();
            });
        }
    }
    /// <summary>
    /// Apply to ToggleField to change if the game should use AI
    /// </summary>
    /// <param name="useAI"></param>
    public void ToggleAI(bool useAI) => GameManager.s_useP2AI = useAI;

    public void ToggleAIP1(bool useAI) => GameManager.s_useP1AI = useAI;

    public void RandomMap()
    {   //Disable random map if I am not the host
        if (!NetworkManager.AmHost)
            return;

        int rand = Random.Range(0, _maps.Length);

        GameManager.s_map = _maps[rand].Prefab;
        map.Target = _maps[rand];
        s_selectedMap = _maps[rand];
        OnSelectMap.Invoke();
    }

    public void RandomPlayer1()
    {   //Don't let the other player mess with each other
        if (NetworkManager.InRoom)
            if (!NetworkManager.AmHost)
                return;

        int rand = Random.Range(0, _characters.Length);
        GameManager.s_p1Char = _characters[rand].Prefab;
        p1.Target = _characters[rand];
        s_p1Selected = _characters[rand];
        OnSelectCharacter.Invoke();
    }

    public void RandomPlayer2()
    {   //Don't let the other player mess with each other
        if (NetworkManager.InRoom)
            if (NetworkManager.AmHost)
                return;

        int rand = Random.Range(0, _characters.Length);
        GameManager.s_p2Char = _characters[rand].Prefab;
        p2.Target = _characters[rand];
        s_p2Selected = _characters[rand];
        OnSelectCharacter.Invoke();
    }

    private void UpdateBoothChar()
    {
        GameObject obj;
        if (s_p1Selected)
        {
            for (byte i = 0; i < photoBoothL.childCount; i++)
                Destroy(photoBoothL.GetChild(0).gameObject);

            if (s_p1Selected.Prefab != null)
            {
                obj = Instantiate(s_p1Selected.Prefab, photoBoothL);
                obj.transform.localRotation = Quaternion.identity;
                obj.GetComponent<Player>().Gravity = 0;
            }
        }
        if (s_p2Selected)
        {
            for (byte i = 0; i < photoBoothR.childCount; i++)
                Destroy(photoBoothR.GetChild(0).gameObject);

            if (s_p2Selected.Prefab != null)
            {
                obj = Instantiate(s_p2Selected.Prefab, photoBoothR);
                obj.transform.localRotation = Quaternion.identity;
                obj.GetComponent<Player>().Gravity = 0;
            }
        }
    }

    public void UpdateBoothMap()
    {
        if (s_selectedMap)
        {
            DestroyCurrentMap();

            Instantiate(s_selectedMap.Prefab, mapBooth);
        }
    }

    public void DestroyCurrentMap()
    {
        for (byte i = 0; i < mapBooth.childCount; i++)
            Destroy(mapBooth.GetChild(0).gameObject);
    }

    private void SyncSelectedCharacter()
    {   //Only work if in a room
        if (!NetworkManager.InRoom)
            return;

        byte i;
        //Search for the index of our character
        for (i = 0; i < _characters.Length; i++)
            if (NetworkManager.AmHost && _characters[i] == s_p1Selected)
                break;
            //If not host, check p2
            else if (!NetworkManager.AmHost && _characters[i] == s_p2Selected)
                break;

        Debug.Log("Character Index: " + i);
        //Tell the other player our selected character
        photonView.RPC("RPCSelectCharacter", RpcTarget.Others, i);
    }

    [PunRPC]
    private void RPCSelectCharacter(byte selectedCharIndex)
    {
        Debug.Log("OtherPlayer Selected Index: " + selectedCharIndex);
        //Store the selected character to display
        if (NetworkManager.AmHost)
        {
            s_p2Selected = _characters[selectedCharIndex];
            p2.Target = s_p2Selected;
            GameManager.s_p2Char = s_p2Selected.Prefab;
        }
        else
        {
            s_p1Selected = _characters[selectedCharIndex];
            p1.Target = s_p1Selected;
            GameManager.s_p1Char = s_p1Selected.Prefab;
        }
        //Update the booth
        UpdateBoothChar();
    }

    private void SyncSelectedMap()
    {   //Make sure we are in a room and are the host.
        if (!NetworkManager.InRoom || !NetworkManager.AmHost)
            return;

        byte i;
        //Set i to be the selected maps index
        for (i = 0; i < _maps.Length; i++)
            if (s_selectedMap == _maps[i])
                break;

        photonView.RPC("RPCSelectMap", RpcTarget.OthersBuffered, i);
    }

    [PunRPC]
    private void RPCSelectMap(byte mapIndex)
    {   //Set the map
        s_selectedMap = _maps[mapIndex];
        map.Target = s_selectedMap;
        GameManager.s_map = s_selectedMap.Prefab;
        //Update the booth
        UpdateBoothMap();
    }

    public void ToggleReady(bool ready)
    {   //If not in a room, ignore
        if (!NetworkManager.InRoom)
            return;

        if (NetworkManager.AmHost)
            _playerReady[0] = ready;
        else
            _playerReady[1] = ready;

        photonView.RPC("SyncReady", RpcTarget.OthersBuffered, ready);
        //If both players are ready, head to map selection
        if (_playerReady[0] == (_playerReady[1] == true))
            //Move to the map selection
            nextButton.onClick.Invoke();
    }

    [PunRPC]
    private void SyncReady(bool readyState)
    {
        if (NetworkManager.AmHost)
            _playerReady[1] = readyState;
        else
            _playerReady[0] = readyState;

        if (_playerReady[0] == (_playerReady[1] == true))
            //Move to the map selection
            nextButton.onClick.Invoke();
    }
}
