using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private float Acceleration = 0.5f;
    [SerializeField] float maxMoveSpeed = 25f;
    [SerializeField] float minMoveSpeed = 10f;
    [SerializeField] CinemachineCamera virtualCamera;
    [BoxGroup("Zoom")]
    [SerializeField][Range(1, 10)] float zoomSpeed;
    [BoxGroup("Zoom")]
    [SerializeField] int minFOV;
    [BoxGroup("Zoom")]
    [SerializeField] int maxFOV;
    private InputActionMap _inputActionMap;
    private InputAction cameraMovement;
    private InputAction cameraZoom;
    float moveSpeed;
    float t = 0.0f;
    private float targetFOV;

    void Awake(){
        _inputActionMap = controls.FindActionMap("Player");
        cameraMovement = _inputActionMap.FindAction("Camera");
        cameraZoom = _inputActionMap.FindAction("CameraZoom");

        cameraMovement.canceled += resetMovement;
         targetFOV = virtualCamera.Lens.FieldOfView;
    }
    void Update()
    {
        if(cameraMovement.IsPressed()){ //when the player pressed WASD
            Vector2 inputVector = cameraMovement.ReadValue<Vector2>();//reads how much the camera moved;
            moveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, t);
            t += Acceleration;
            transform.position += new Vector3(inputVector.x, 0, inputVector.y)  * moveSpeed * Time.deltaTime;
        }

        if (cameraZoom.IsPressed())//when the use uses scroll wheel
        {
            Vector2 scrollInput = cameraZoom.ReadValue<Vector2>();
            AdjustCameraZoom(scrollInput.y);
        }
        
    }
    void resetMovement(InputAction.CallbackContext context){
        
        t = 0.0f;
    }
    private void AdjustCameraZoom(float increment)
    {
        // Calculate new target FOV based on scroll input
        targetFOV -= increment * zoomSpeed;
        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
    }
    private void adjustCameraPositionZoom(){
        
    }

    private void LateUpdate()
    {
        // Smoothly interpolate to the target FOV using Lerp
        virtualCamera.Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );
    }
}
