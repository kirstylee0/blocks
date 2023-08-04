using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;
using UnityEngine.EventSystems;

public class InputManager : SingletonBehaviour<InputManager>
{
    [SerializeField] Camera _mainCamera;
    Vector3 _lastPosition;

    [SerializeField] LayerMask placementLayermask; 

    public event Action OnClicked, OnExit;
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool isPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _mainCamera.nearClipPlane;
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, 1 << Layer.TilePlacement))
        {
            _lastPosition = hit.point;
        }

        return _lastPosition;
    }
}
