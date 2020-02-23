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

    private void Start()
    {
        
    }

    void Update()
    {
        //inputComponent.onActionTriggered += HandleInput;
        var pad = Gamepad.current;
        if(pad != null)
        {
            Vector2 input = pad.leftStick.ReadValue();
            Vector3 move = new Vector3(input.x, 0, input.y);
            move *= pad.rightShoulder.isPressed? sprintSpeed : speed;
            cc.SimpleMove(move);
        }
    }

    void HandleInput(InputAction.CallbackContext context)
    {
        //thonk
    }
}
