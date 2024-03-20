using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    private Animator anim;

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

        // Bewegung "up" + Animationen
        if (direction_up != 0) 
        {
            anim.SetFloat("xMove", 0f);
            anim.SetFloat("yMove", direction_up);
            Vector2 movement_up = Vector2.up * speed * direction_up * Time.deltaTime;
            transform.Translate(movement_up);
        }

        // Bewegung "down" + Animationen
        if (direction_down != 0) 
        {
            anim.SetFloat("xMove", 0f);
            anim.SetFloat("yMove", direction_down);
            Vector2 movement_down = Vector2.down * speed * direction_down * Time.deltaTime;
            transform.Translate(movement_down);
        }
        
        // Bewegung "left" + Animationen
        if (direction_left != 0) 
        {
            anim.SetFloat("xMove", direction_left);
            anim.SetFloat("yMove", 0f);
            Vector2 movement_left = Vector2.left * speed * direction_left * Time.deltaTime;
            transform.Translate(movement_left);
        }

        // Bewegung "right" + Animationen
        if (direction_right != 0) 
        {
            anim.SetFloat("xMove", direction_right);
            anim.SetFloat("yMove", 0f);
            Vector2 movement_right = Vector2.right * speed * direction_right * Time.deltaTime;
            transform.Translate(movement_right);
        }
    }
}
