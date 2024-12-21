using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset _controls;
    [SerializeField] private float _acceleration = 0.5f;
    [SerializeField] float _maxMoveSpeed = 25f;
    [SerializeField] float _minMoveSpeed = 10f;
    [SerializeField] CinemachineCamera _virtualCamera;
    [BoxGroup("Zoom")]
    [SerializeField] bool _activateZoom = false;
    [BoxGroup("Zoom")]
    [SerializeField][Range(1, 10)] float _zoomSpeed;
    [BoxGroup("Zoom")]
    [SerializeField] int _minFOV;
    [BoxGroup("Zoom")]
    [SerializeField] int _maxFOV;
    private InputActionMap _inputActionMap;
    private InputAction _cameraMovement;
    private InputAction _cameraZoom;
    float _moveSpeed;
    float t = 0.0f;
    private float _targetFOV;

    void Awake(){
        _inputActionMap = _controls.FindActionMap("Player");
        _cameraMovement = _inputActionMap.FindAction("Camera");
        _cameraZoom = _inputActionMap.FindAction("CameraZoom");

        _cameraMovement.canceled += resetMovement;
        _targetFOV = _virtualCamera.Lens.FieldOfView;
    }
    void Update()
    {
        if(_cameraMovement.IsPressed()){ //when the player pressed WASD
            Vector2 inputVector = _cameraMovement.ReadValue<Vector2>();//reads how much the camera moved;
            _moveSpeed = Mathf.Lerp(_minMoveSpeed, _maxMoveSpeed, t);
            t += _acceleration;
            transform.position += new Vector3(inputVector.x, 0, inputVector.y)  * _moveSpeed * Time.deltaTime;
        }

        if (_cameraZoom.IsPressed() && _activateZoom)//when the use uses scroll wheel
        {
            Vector2 scrollInput = _cameraZoom.ReadValue<Vector2>();
            AdjustCameraZoom(scrollInput.y);
        }
        
    }
    void resetMovement(InputAction.CallbackContext context){
        
        t = 0.0f;
    }
    private void AdjustCameraZoom(float increment)
    {
        // Calculate new target FOV based on scroll input
        _targetFOV -= increment * _zoomSpeed;
        _targetFOV = Mathf.Clamp(_targetFOV, _minFOV, _maxFOV);
    }

    private void LateUpdate()
    {

        // Smoothly interpolate to the target FOV using Lerp
        _virtualCamera.Lens.FieldOfView = Mathf.Lerp(_virtualCamera.Lens.FieldOfView, _targetFOV, Time.deltaTime * _zoomSpeed);
    }
}
