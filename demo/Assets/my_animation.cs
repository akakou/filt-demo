using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class my_animation : MonoBehaviour {
	private static int unit = 60;
	private static int remain = 5 * unit;

    private Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (remain <= 0 && animator.GetInteger("wait") == 1) {
            animator.SetInteger("wait", 0);
			remain = 10 * unit;
        }
        if (remain <= 0 && animator.GetInteger("wait") == 0) {
			animator.SetInteger("wait", 1);
			remain = 5 * unit;
        }

		remain -= 1;
	}
}
