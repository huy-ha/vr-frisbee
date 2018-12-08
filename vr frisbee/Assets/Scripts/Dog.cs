using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour {
    public GameObject frisbee;
    public GameObject player;

    NavMeshAgent agent;
    Vector3 head;
    private bool waiting;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        head = new Vector3(0,0,0);
        waiting = true;
    }
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(frisbee.transform.position);
        if (waiting)
        {
            TrotToPlayerAndSit();
        }
        //if (caught)
        //{
        //    //carrying back to player
        //    frisbee.GetComponent<Rigidbody>().useGravity = false;
        //    carryFrisbee();
        //}else if(caught && !flying){
        //    //waiting for player to throw
        //    //if (/*not at waiting spot yet then go to waiting spot*/)
        //    //{

        //    //}
        //    //else
        //    //{
        //        //sit and wait for player to throw
        //    //}
        //} else if (flying)
        //{
        //    //chase frisbee
        //    caught = false;
        //    if (Vector3.Distance(transform.position,frisbee.transform.position) < 1 && frisbee.transform.position.y < 3)
        //    {
        //        //dog have reached frisbee
        //        caught = true;
        //    }
        //    else
        //    {
        //        //move closer to frisbee
        //    }
        //}
	}

    void TrotToPlayerAndSit()
    {
        transform.position = Vector3.Lerp(transform.position, );
    }

    void carryFrisbee()
    {
        frisbee.transform.position = transform.position + transform.rotation*head;
    }
}
