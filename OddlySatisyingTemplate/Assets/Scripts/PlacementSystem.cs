using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] GameObject _cellIndicator;
    [SerializeField] Transform _cellIndicatorVisual;

    [SerializeField] Grid _grid;

    [SerializeField] private GameObject _gridVisualization;

    [SerializeField] MovementTile[] _movementTilePrefabs;


    [SerializeField] int _movementTileChoiceIndex = 0;
    [SerializeField] int _currentMovementTileIndex = 0;
    int _numberOfOptions;
    GameManager gameManager;
    public bool isDestroyed = false; 


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _numberOfOptions = _movementTilePrefabs.Length;
    }

    void FixedUpdate()
    {
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        _cellIndicator.transform.position = _grid.CellToWorld(gridPosition); 
      

        Vector3 castPosition = new Vector3(gridPosition.x + 0.5f, 5, gridPosition.z + 0.5f);

        if (Input.GetMouseButton(0))
        {
            MovementTile selectedMovementTile = MovementTileInPosition(castPosition);
            if (selectedMovementTile == null)
            {
                if (gameManager.blockCount >= 1)
                {
                    PlaceMovementTile(gridPosition);
                }
            }
        }

        if(Input.GetMouseButton(1))
        {
            MovementTile selectedMovementTile = MovementTileInPosition(castPosition);
            if (selectedMovementTile != null)
            {
                gameManager.blockCount += 1;
                selectedMovementTile.PlayDestroyFeedback();
                selectedMovementTile = null;

            }
        }

     
    }


    private void Update()
    {
        ScrollWheel();
    }

    

    private void PlaceMovementTile(Vector3Int gridPosition)
    {
        MovementTile newMovementTile = Instantiate(_movementTilePrefabs[_currentMovementTileIndex]);
        //newMovementTile.RotateVisual(_currentMovementTileIndex * 90);
        //newMovementTile.GetComponent<AssignColors>().RecolorMesh(_currentMovementTileIndex);
        newMovementTile.transform.position = gridPosition;
        gameManager.blockCount -= 1; 
        
    }

    MovementTile MovementTileInPosition(Vector3 castPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(castPosition, Vector3.down, out hit, Mathf.Infinity, 1 << Layer.MovementTiles))
        {
            return hit.collider.GetComponentInParent<MovementTile>();
            
        }
        return null;
    }

    void ScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) 
        {
            _movementTileChoiceIndex --;
            _currentMovementTileIndex = Mathf.Abs(_movementTileChoiceIndex % _numberOfOptions);
            UIManager.Instance.TileHighlight(_currentMovementTileIndex);

            ChangeIndicator.Instance.CycleIndicator(_currentMovementTileIndex);
  
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0) 
        {
            _movementTileChoiceIndex ++;
            _currentMovementTileIndex = Mathf.Abs(_movementTileChoiceIndex % _numberOfOptions);
            UIManager.Instance.TileHighlight(_currentMovementTileIndex);
            
            ChangeIndicator.Instance.CycleIndicator(_currentMovementTileIndex);
        }
    }

   

   

}
