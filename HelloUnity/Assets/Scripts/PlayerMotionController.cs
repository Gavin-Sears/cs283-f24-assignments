using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotionController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Toggles Walking bool
    public void ToggleWalk()
    {
        animator.SetBool("Walking", !animator.GetBool("Walking"));
    }

    public void walk()
    {
        animator.SetBool("Walking", true);
    }

    public void idle()
    {
        animator.SetBool("Walking", false);
    }
}
