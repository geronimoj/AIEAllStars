using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
[RequireComponent(typeof(Slider))]
public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// The player to display health for
    /// </summary>
    public int targetPlayer = 0;
    /// <summary>
    /// The player to display health for
    /// </summary>
    private Player target = null;
    /// <summary>
    /// The display image for player health bars
    /// </summary>
    private Slider _healthDisplay = null;

    private void Start()
    {
        target = GameManager.s_instance._players[targetPlayer];
        _healthDisplay = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        if (!target)
            return;
        //Set the fill
        _healthDisplay.value = target.CurrentHealth / target.MaxHealth;
    }
}
