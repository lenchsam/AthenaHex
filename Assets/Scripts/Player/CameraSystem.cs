using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private float Acceleration = 0.5f;
    [SerializeField] float maxMoveSpeed = 25f;
    [SerializeField] float minMoveSpeed = 10f;
    private InputActionMap _inputActionMap;
    private InputAction cameraMovement;
    Vector2 inputVector;
    float moveSpeed;
    float t = 0.0f;

    void Awake(){
        _inputActionMap = controls.FindActionMap("Player");
        cameraMovement = _inputActionMap.FindAction("Camera");

        cameraMovement.canceled += resetMovement;
    }
    void FixedUpdate()
    {
        if(cameraMovement.IsPressed()){
            moveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, t);
            t += Acceleration;
        }
        
        Vector2 inputVector = cameraMovement.ReadValue<Vector2>();
        //Debug.Log(inputVector);
        transform.position += new Vector3(inputVector.x, 0, inputVector.y)  * moveSpeed * Time.deltaTime;
    }
    void resetMovement(InputAction.CallbackContext context){
        
        t = 0.0f;
    }
}
