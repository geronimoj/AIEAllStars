using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class RoundWinUI : MonoBehaviour
{
    public RoundUI _roundPrefab = null;

    public Transform _uiParent = null;

    public byte _player = 0;

    private List<RoundUI> _roundUIs = new List<RoundUI>();

    private void Start()
    {
        for (byte i = 0; i < GameManager.s_instance._winAmount; i++)
            _roundUIs.Add(Instantiate(_roundPrefab, _uiParent));
        //Update the UI
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (byte i = 0; i < _roundUIs.Count; i++)
            _roundUIs[i].IsWon = GameManager.s_scores[_player] <= i;
    }
}
