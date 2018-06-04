using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private DirectionVariable currentPlayerDirection;
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                currentPlayerDirection.Value = Input.GetAxisRaw("Horizontal") > 0 ? Vector2.right: Vector2.left;
            }
            else if (Input.GetButtonDown("Vertical"))
            {
                currentPlayerDirection.Value = Input.GetAxisRaw("Vertical") > 0 ? Vector2.up : Vector2.down;
            }
        }
    }
}
