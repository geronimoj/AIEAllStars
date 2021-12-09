using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player-Input", menuName = "PlayerInput", order = 1)]
public class PlayerInput : ScriptableObject
{
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Jump = KeyCode.W;
    public KeyCode Dash = KeyCode.J;
    public KeyCode Attack = KeyCode.K;
    public KeyCode Skill = KeyCode.L;
}
