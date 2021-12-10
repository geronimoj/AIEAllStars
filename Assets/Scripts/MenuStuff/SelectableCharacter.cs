using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 1)]
public class SelectableCharacter : ScriptableObject
{
    public string Name = string.Empty;

    public Sprite Image = null;

    public Sprite Icon = null;

    public Color Colour = Color.white;

    public GameObject Prefab = null;
}
