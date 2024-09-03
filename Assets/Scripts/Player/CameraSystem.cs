using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    private InputActionMap _inputActionMap;
    private InputAction cameraMovement;
    Vector2 inputVector;

    [SerializeField] float moveSpeed = 50f;

    void Awake(){
        _inputActionMap = controls.FindActionMap("Player");
        cameraMovement = _inputActionMap.FindAction("Camera");
    }
    void FixedUpdate()
    {
        Vector2 inputVector = cameraMovement.ReadValue<Vector2>();
        //Debug.Log(inputVector);
        transform.position += new Vector3(inputVector.x, 0, inputVector.y)  * moveSpeed * Time.deltaTime;
    }
}
