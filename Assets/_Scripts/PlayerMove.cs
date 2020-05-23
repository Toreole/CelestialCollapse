using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

namespace Celestial
{
    public class PlayerMove : MonoBehaviour
    {
        public CharacterController cc;
        public PlayerInput inputComponent;
        public float speed;
        public float sprintSpeed;

        public InputActionReference moveAction;
        public InputActionReference interAction;
        public InputActionReference attackAction;

        public Animator anim;
        public int attackCycles = 2;

        private int currentAttack;

        private void Start()
        {
            if(attackAction)
                attackAction.action.performed += OnAttack;
        }

        void Update()
        {
            //print(inputComponent.currentControlScheme); //Keyboard&Mouse || Gamepad

            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);
            move *= speed;
            cc.SimpleMove(move);
        }
        
        private void OnTriggerStay(Collider other)
        {
            var x = other.GetComponent<IInteractable>();
            if(x != null)
            {
                //print("found interactable.");
                if(interAction.action.triggered)
                    x.Interact();
            }
        }

        void OnAttack(InputAction.CallbackContext context)
        {
            anim.SetInteger("Cycle", currentAttack);
            currentAttack = (currentAttack + 1) % attackCycles;
            anim.SetTrigger("Attack");
        }
    }
}