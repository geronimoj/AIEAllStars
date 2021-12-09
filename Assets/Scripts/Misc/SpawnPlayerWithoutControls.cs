using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerWithoutControls : MonoBehaviour
{
    /// <summary>
    /// Spawn player 1 or 2
    /// </summary>
    public bool _spawnPlayer1 = false;

    private void Start()
    {
        GameObject prefab;

        if (_spawnPlayer1)
            prefab = GameManager.s_p1Char;
        else
            prefab = GameManager.s_p2Char;

        if (prefab == null)
            return;
        else
            Instantiate(prefab, transform.position, transform.rotation);
    }
}
