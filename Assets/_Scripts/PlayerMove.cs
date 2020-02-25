using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerMove : MonoBehaviour
{
    public CharacterController cc;
    public PlayerInput inputComponent;
    public float speed;
    public float sprintSpeed;

    public InputActionReference moveAction;
    
    private void Start()
    {
        foreach (InputAction action in inputComponent.currentActionMap.actions)
        {
            print($"----{action.name}----");
            //print(action.GetBindingDisplayString());
            //for (int i = 0; i < action.bindings.Count; i++)
            //    print(action.GetBindingDisplayString(i));
            
            int index = action.GetBindingIndex(InputBinding.MaskByGroups("Keyboard", "Mouse"));
            if(index >= 0)
                print(action.GetBindingDisplayString(index));
            
            //foreach (InputBinding bind in action.bindings)
            //{
            //    if(bind.effectivePath.Contains("<Keyboard>") || bind.effectivePath.Contains("<Mouse>"))
            //        print($"{action.name} bound to {bind.path}");
            //}
        }
        //TODO: okay cool this actually works wow.
        //yield return new WaitForSeconds(1f);
        //var controller = Gamepad.current;
        //if (controller != null)
        //{
        //    controller.SetMotorSpeeds(3f, 3f); //left and right,
        //    yield return new WaitForSeconds(0.5f);
        //    controller.SetMotorSpeeds(0.0f, 0.0f);
        //}
    }

    void Update()
    {
        //print(inputComponent.currentControlScheme); //Keyboard&Mouse || Gamepad

        Vector2 input = moveAction.action.ReadValue<Vector2>();
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
