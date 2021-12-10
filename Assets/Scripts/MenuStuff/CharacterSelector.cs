using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the selection of characters for players
/// </summary>
public class CharacterSelector : MonoBehaviour
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
    /// <summary>
    /// Initialize charcaters
    /// </summary>
    private void Start()
    {   //Spawn uI
        SpawnPlayerUI(_p1ButtonParent, true);
        SpawnPlayerUI(_p2ButtonParent, false);
        SpawnMapUI(_mapParent);
        GameManager.s_useAI = false;

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
        if (!GameManager.s_p1Char)
            GameManager.s_p1Char = _characters[0].Prefab;
        if (!GameManager.s_p2Char)
            GameManager.s_p2Char = _characters[0].Prefab;
        if (!GameManager.s_map)
            GameManager.s_map = _maps[0].Prefab;
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
                b.onClick.AddListener(() => { GameManager.s_p1Char = _characters[index].Prefab; p1.Target = _characters[index]; s_p1Selected = _characters[index]; OnSelectCharacter.Invoke(); });
            }
            else
            {
                b.onClick.AddListener(() => { GameManager.s_p2Char = _characters[index].Prefab; p2.Target = _characters[index]; s_p2Selected = _characters[index]; OnSelectCharacter.Invoke(); });
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
            b.onClick.AddListener(() => {GameManager.s_map = _maps[index].Prefab; map.Target = _maps[index]; s_selectedMap = _maps[index]; OnSelectMap.Invoke(); });
        }
    }
    /// <summary>
    /// Apply to ToggleField to change if the game should use AI
    /// </summary>
    /// <param name="useAI"></param>
    public void ToggleAI(bool useAI) => GameManager.s_useAI = useAI;

    private void UpdateBoothChar()
    {
        GameObject obj;
        if (s_p1Selected)
        {
            for (byte i = 0; i < photoBoothL.childCount; i++)
                Destroy(photoBoothL.GetChild(0).gameObject);

            obj = Instantiate(s_p1Selected.Prefab, photoBoothL);
            obj.transform.localRotation = Quaternion.identity;
        }
        if (s_p2Selected)
        {
            for (byte i = 0; i < photoBoothR.childCount; i++)
                Destroy(photoBoothR.GetChild(0).gameObject);

            obj = Instantiate(s_p2Selected.Prefab, photoBoothR);
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void UpdateBoothMap()
    {
        if (s_selectedMap)
        {
            for (byte i = 0; i < mapBooth.childCount; i++)
                Destroy(mapBooth.GetChild(0).gameObject);

            Instantiate(s_selectedMap.Prefab, mapBooth);
        }
    }
}
