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

		if (remain > 0) {
			remain -= 1;
			return;
		}

		bool is_changed = false;
		while(!is_changed) {
			int randint = (int)Random.Range(0f, 10.0f);
			is_changed = ChangeState(randint);
		}

	}

	bool ChangeState(int state) {
		if (state > 5 && animator.GetInteger("wait") != 0) {
			animator.SetInteger("wait", 0);
			remain = 10 * unit;
			return true;
		}
		if (animator.GetInteger("wait") != 1) {
			animator.SetInteger("wait", 1);
			remain = 3 * unit;
			return true;
		}

		return false;
	}
}
