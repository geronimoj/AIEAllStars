using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(1)]
public class SelectedUI : MonoBehaviour
{
    public TextMeshProUGUI nameText = null;

    public Image _mainImage = null;

    private SelectableCharacter _target = null;
    
    public SelectableCharacter Target
    {
        get => _target;
        set
        {
            if (_target == value)
                return;
            _target = value;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {   //Null catch
        if (!_target)
            return;

        if (nameText)
            nameText.text = _target.Name;

        if (_mainImage)
            _mainImage.sprite = _target.Image;
    }
}
