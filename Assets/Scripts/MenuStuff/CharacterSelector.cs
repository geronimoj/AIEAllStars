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
    /// The charcaters to select from
    /// </summary>
    public GameObject[] _characters = new GameObject[0];
    /// <summary>
    /// Initialize charcaters
    /// </summary>
    private void Start()
    {   //Spawn uI
        SpawnPlayerUI(_p1ButtonParent, true);
        SpawnPlayerUI(_p2ButtonParent, false);
        //Set defaults if null
        if (!GameManager.s_p1Char)
            GameManager.s_p1Char = _characters[0];
        if (!GameManager.s_p2Char)
            GameManager.s_p2Char = _characters[0];
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
            //Setup lambda
            if (isP1)
            {
                b.onClick.AddListener(() => GameManager.s_p1Char = _characters[index]);
            }
            else
            {
                b.onClick.AddListener(() => GameManager.s_p2Char = _characters[index]);
            }
        }
    }
    /// <summary>
    /// Apply to ToggleField to change if the game should use AI
    /// </summary>
    /// <param name="useAI"></param>
    public void ToggleAI(bool useAI) => GameManager.s_useAI = useAI;
}
