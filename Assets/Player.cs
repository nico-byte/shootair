using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    private Animator anim;
    private bool state_lock = false;

    public enum PlayerState 
    {
        Idle,
        Moving,
        MovingAiming
    }
    private PlayerState current_state = PlayerState.Idle;

    private void SetState(PlayerState newState)
    {
        if (!state_lock) 
        {
            current_state = newState;
            switch(current_state) 
            {
                case PlayerState.Idle:
                    anim.Play("Idle");
                    break;
                case PlayerState.Moving:
                    anim.Play("Moving");
                    break;
                case PlayerState.MovingAiming:
                    anim.Play("MovingAiming");
                    break;        
            }
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float direction_up = Input.GetAxis("Vertical");
        float direction_down = -Input.GetAxis("Vertical");
        float direction_left = -Input.GetAxis("Horizontal");
        float direction_right = Input.GetAxis("Horizontal");

        // Movement "upside" + animations
        if (direction_up != 0) 
        {
            SetState(PlayerState.Moving);
            anim.SetFloat("xMove", 0f);
            anim.SetFloat("yMove", direction_up);
            Vector2 movement_up = Vector2.up * speed * direction_up * Time.deltaTime;
            transform.Translate(movement_up);
        }

        // Movement "downside" + animations
        if (direction_down != 0) 
        {
            SetState(PlayerState.Moving);
            anim.SetFloat("xMove", 0f);
            anim.SetFloat("yMove", direction_down);
            Vector2 movement_down = Vector2.down * speed * direction_down * Time.deltaTime;
            transform.Translate(movement_down);
        }
        
        // Movement "leftside" + animations
        if (direction_left != 0) 
        {
            SetState(PlayerState.Moving);
            anim.SetFloat("xMove", direction_left);
            anim.SetFloat("yMove", 0f);
            Vector2 movement_left = Vector2.left * speed * direction_left * Time.deltaTime;
            transform.Translate(movement_left);
        }

        // Movement "rightside" + animations
        if (direction_right != 0) 
        {
            SetState(PlayerState.Moving);
            anim.SetFloat("xMove", direction_right);
            anim.SetFloat("yMove", 0f);
            Vector2 movement_right = Vector2.right * speed * direction_right * Time.deltaTime;
            transform.Translate(movement_right);
        }
    }
}
