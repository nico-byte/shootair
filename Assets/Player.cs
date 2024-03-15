using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // public float speed = 1f;
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

        // animation for walking up
        if (Input.GetKey(KeyCode.W)) 
        {
            anim.SetBool("walkingUp", true);
            if (Input.GetKey(KeyCode.Space)) 
            {
                anim.SetBool("aiming", true);
            }
            else 
            {
                anim.SetBool("aiming", false);
            }
        }   
        else 
        {
            anim.SetBool("walkingUp", false);
        }
        // animation for walking down
        if (Input.GetKey(KeyCode.S)) 
        {
            anim.SetBool("walkingDown", true);
            if (Input.GetKey(KeyCode.Mouse0)) 
            {
                anim.SetBool("aiming", true);
            }
            else 
            {
                anim.SetBool("aiming", false);
            }
        }   
        else 
        {
            anim.SetBool("walkingDown", false);
        }
        // animation for walking left
        if (Input.GetKey(KeyCode.A)) 
        {
            anim.SetBool("walkingLeft", true);
            if (Input.GetKey(KeyCode.Space)) 
            {
                anim.SetBool("aiming", true);
            }
            else 
            {
                anim.SetBool("aiming", false);
            }
        }   
        else 
        {
            anim.SetBool("walkingLeft", false);
        }
        // animation for walking right
        if (Input.GetKey(KeyCode.D)) 
        {
            anim.SetBool("walkingRight", true);
            if (Input.GetKey(KeyCode.Space)) 
            {
                anim.SetBool("aiming", true);
            }
            else 
            {
                anim.SetBool("aiming", false);
            }
        }   
        else 
        {
            anim.SetBool("walkingRight", false);
        }
        
        // movement in all directions
        // Vector2 movement = new Vector2(direction_right - direction_left, direction_up - direction_down);
        // transform.Translate(movement * speed * Time.deltaTime);
    }
}
