using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Tooltip("The UI button for selecting a character")]
    public Button _characterButtonPrefab = null;

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

    private void Start()
    {   //Spawn uI
        SpawnPlayerUI(_p1ButtonParent, true);
        SpawnPlayerUI(_p2ButtonParent, false);
        //Set defaults
        GameManager.s_p1Char = _characters[0];
        GameManager.s_p2Char = _characters[0];
    }

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
                b.onClick.AddListener(() => GameManager.s_p1Char = _characters[index]);
            else
                b.onClick.AddListener(() => GameManager.s_p2Char = _characters[index]);
        }
    }
}
