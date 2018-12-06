using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dog : MonoBehaviour {
    public GameObject frisbee;
    public float maxSpeed;

    //for catching
    bool caught,flying,wait;

	// Use this for initialization
	void Start () {
        caught = false;
        flying = frisbee.GetComponent<frisbee>().getFly();
        wait = true;

    }
	
	// Update is called once per frame
	void Update () {
        flying = frisbee.GetComponent<frisbee>().getFly();

        if (caught)
        {
            //carrying back to player
            frisbee.GetComponent<frisbee>().setFly(false);
            frisbee.GetComponent<Rigidbody>().useGravity = false;
            carryFrisbee();
        }else if(caught && !flying){
            //waiting for player to throw
            //if (/*not at waiting spot yet then go to waiting spot*/)
            //{

            //}
            //else
            //{
                //sit and wait for player to throw
            //}
        } else if (flying)
        {
            //chase frisbee
            caught = false;
            if (Vector3.Distance(transform.position,frisbee.transform.position) < 1 && frisbee.transform.position.y < 3)
            {
                //dog have reached frisbee
                caught = true;
            }
            else
            {
                //move closer to frisbee
            }
        }
	}

    void carryFrisbee()
    {

    }
}
