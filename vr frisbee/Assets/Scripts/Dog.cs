using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Dog : MonoBehaviour {
    public GameObject frisbee;
    public GameObject player;
    public Text logger;
    private Thrower p;
    private Frisbee f;

    //TODO organize this
    NavMeshAgent agent;
    Vector3 head;

    //three states that dog can be in
    private bool hasFrisbee;

	// Use this for initialization
	void Start () {
        p = player.GetComponent<Thrower>();
        f = frisbee.GetComponent<Frisbee>();
        agent = GetComponent<NavMeshAgent>();
        head = new Vector3(0,0.7f,0.6f);
        hasFrisbee = false;
    }
	
	// Update is called once per frame
	void Update () {
        //agent.SetDestination(frisbee.transform.position);
        //logger.text = "";
        //log("distance " + Vector3.Distance(transform.position, f.transform.position));
        //log("f.transform.position.y " + f.transform.position.y);
        //log("hasFrisbee " + hasFrisbee.ToString());
        if (p.hasFrisbee() || hasFrisbee)
            TrotToPlayerAndSit();
        else
            chaseFrisbee();
    }

    void chaseFrisbee()
    {
        agent.SetDestination(f.transform.position);
        agent.acceleration = 80f;
        agent.speed = 18f;
        agent.angularSpeed = 360f;
        
        if(Vector3.Distance(transform.position, f.transform.position) < 1 && f.transform.position.y < 1)
        {
            f.transform.rotation = Quaternion.Euler(0, 0, 0);
            f.Catch();
            hasFrisbee = true;
        }
    }

    public void letGo()
    {
        hasFrisbee = false;
    }

    void TrotToPlayerAndSit()
    {
        agent.SetDestination(p.getSittingSpot());
        agent.acceleration = 20f;
        agent.speed = 7f;
        agent.angularSpeed = 150f;
        Vector3 lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime * 10f);
        if (hasFrisbee)
        {
            carryFrisbee();
        }
    }

    void carryFrisbee()
    {
        frisbee.transform.position = transform.position + transform.rotation*head;
    }

    void log(string s)
    {
        logger.text += s + "\n";
    }
}
