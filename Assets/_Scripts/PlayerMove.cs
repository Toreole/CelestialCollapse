using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public CharacterController cc;
    public PlayerInput inputComponent;
    public float speed;
    public float sprintSpeed;

    private InputAction moveAction;

    private void Start()
    {
        moveAction = inputComponent.currentActionMap.FindAction("Move");
    }

    void Update()
    {
        //print(inputComponent.currentControlScheme); //Keyboard&Mouse || Gamepad
        
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move *= speed;
        cc.SimpleMove(move);
    }

    void GamepadMoveDirect()
    {
        var pad = Gamepad.current;
        if (pad != null)
        {
            Vector2 input = pad.leftStick.ReadValue();
            Vector3 move = new Vector3(input.x, 0, input.y);
            move *= pad.rightShoulder.isPressed ? sprintSpeed : speed;
            cc.SimpleMove(move);
        }
    }
}
