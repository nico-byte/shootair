using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float speed = 3f;
//    private Animator anim;
//    private bool state_lock = false;

    // Defining PlayerStates for each Condition 
//    public enum PlayerState 
//    {
//        Idle,
//        Moving,
//        Aiming,
//        MovingAiming
//    }
//    private PlayerState current_state = PlayerState.Idle;

//    private void SetState(PlayerState newState)
//    {
//        if (!state_lock) 
//        {
//            current_state = newState;
//            switch(current_state) 
//            {
//                case PlayerState.Idle:
//                    anim.Play("Idle");
//                    break;
//                case PlayerState.Moving:
//                    anim.Play("Moving");
//                    break;
//                case PlayerState.Aiming:
//                    anim.Play("Aiming");
//                    break;
//                case PlayerState.MovingAiming:
//                    anim.Play("MovingAiming");
//                    break;        
//            }
//        }
//    }

//    void Start()
//    {
//        anim = GetComponent<Animator>();
//    }

//    void Update()
//{
    // direction movement and binding it to the Keys: W, A, S, D
//    float direction_up = Input.GetKey(KeyCode.W) ? 1f : 0f;
//    float direction_down = Input.GetKey(KeyCode.S) ? -1f : 0f;
//    float direction_left = Input.GetKey(KeyCode.A) ? -1f : 0f;
//    float direction_right = Input.GetKey(KeyCode.D) ? 1f : 0f;
    
    // aiming keybinds for each Arrow Keys 
//    bool aiming_up = Input.GetKey(KeyCode.UpArrow);
//    bool aiming_down = Input.GetKey(KeyCode.DownArrow);
//    bool aiming_left = Input.GetKey(KeyCode.LeftArrow);
//    bool aiming_right = Input.GetKey(KeyCode.RightArrow);

    // Set the PlayerState between "Moving" and "MovingAiming"
    // MovingAiming: This combines the Animation from Moving + Aiming 
//    if ((direction_up != 0 || direction_down != 0 || direction_left != 0 || direction_right != 0) &&
//        (aiming_up || aiming_down || aiming_left || aiming_right))
//    {
//        SetState(PlayerState.MovingAiming);
//        anim.SetFloat("xMove", direction_right + direction_left);
//        anim.SetFloat("yMove", direction_up + direction_down);
//        anim.SetBool("IsMoving", true);
//        Vector2 movement = new Vector2(direction_right + direction_left, direction_up + direction_down) * speed * Time.deltaTime;
//        transform.Translate(movement);
//    }

    // Moving: This is for the Moving animation 
//    else if (direction_up != 0 || direction_down != 0 || direction_left != 0 || direction_right != 0)
//    {
//        SetState(PlayerState.Moving);
//        anim.SetFloat("xMove", direction_right + direction_left);
//        anim.SetFloat("yMove", direction_up + direction_down);
//        anim.SetBool("IsMoving", true);
//        Vector2 movement = new Vector2(direction_right + direction_left, direction_up + direction_down) * speed * Time.deltaTime;
//        transform.Translate(movement);
//    }

    // Aiming: This is for the Aiming animation
//    else if (aiming_up || aiming_down || aiming_left || aiming_right)
//    {
//        SetState(PlayerState.Aiming);
//        anim.SetBool("IsMoving", false);
//    }

    // If every State is not active -> Idle State
//    else
//    {
//        SetState(PlayerState.Idle);
//        anim.SetBool("IsMoving", false);
//    }
//    }
//}
