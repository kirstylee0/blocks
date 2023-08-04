using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField] List<Image> _highlightTileUIs = new List<Image>();

    private void Start()
    {
        TileHighlight(0);
    }
    public void TileHighlight(int rotation)
    {
        foreach(Image highlight in _highlightTileUIs)
        {
            highlight.enabled = false; 
        }

        _highlightTileUIs[rotation].enabled = true;

    }

}
