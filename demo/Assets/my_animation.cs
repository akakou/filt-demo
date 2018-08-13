using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class my_animation : MonoBehaviour {
	private static int unit = 60 * 5;
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

		int randint = (int)Random.Range(0f, 10.0f);

		animator.SetInteger("wait", randint);
	}
}
