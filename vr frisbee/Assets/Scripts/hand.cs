using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour {
    public GameObject thisFrisbee;
    public GameObject thisHand;
    public GameObject elbow;
    Quaternion remoteRot,rot;
    
    bool triggerPressed,holding;

    float distance;

    //for determining throwing direction and velocity
    Vector3 prevRemotePos;
    public float thrust; //14

	// Use this for initialization
	void Start () {
        triggerPressed = false;
        holding = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerPressed && (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || Input.GetButtonUp("Jump")))
        {
            if (holding) Throw1();
            else thisFrisbee.GetComponent<Rigidbody>().useGravity = true;
        }
        else if (!triggerPressed && (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetButtonDown("Jump")))
        {
            //Frisbee Picked up
            thisFrisbee.GetComponent<frisbee>().setFly(false);
            triggerPressed = true;
            thisFrisbee.GetComponent<Rigidbody>().velocity = Vector3.zero;
            thisFrisbee.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        distance = Vector3.Distance(thisHand.transform.position, thisFrisbee.transform.position);

        //frisbee floating to hand
        if (triggerPressed && distance != 0)
        {
            thisFrisbee.GetComponent<Rigidbody>().useGravity = true;
            if (distance < 0.1)
            {
                holding = true;
            }
            else
            {
                thisFrisbee.transform.position += 0.2f * (thisHand.transform.position - thisFrisbee.transform.position);
                rot = thisFrisbee.transform.rotation;
                rot.eulerAngles += 0.2f * (thisHand.transform.rotation.eulerAngles + new Vector3(90, 180, 0) - thisFrisbee.transform.rotation.eulerAngles);
                thisFrisbee.transform.rotation = rot;
            }
        }

        //TODO SMOOTHEN out remote control
        //rot = elbow.transform.rotation;
        //rot.eulerAngles += 0.7f * (remoteRot.eulerAngles - rot.eulerAngles);
        //elbow.transform.rotation = rot;
        elbow.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);

        if (holding)
        {
            thisFrisbee.transform.position = thisHand.transform.position;
            thisFrisbee.transform.rotation = thisHand.transform.rotation;
            thisFrisbee.transform.Rotate(new Vector3(90, 0, 0));
        }
        prevRemotePos = thisHand.transform.position;
    }

    void Throw1()
    {
        triggerPressed = false;
        holding = false;
        thisFrisbee.GetComponent<Rigidbody>().useGravity = true;
        if (thisFrisbee.GetComponent<frisbee>().getFly()) Debug.Log("frisbee is using gravity");
        thisFrisbee.GetComponent<frisbee>().Fly(((thisHand.transform.position) - (elbow.transform.position)) * thrust);
    }

    //TODO (actual throw)
    void Throw2()
    {
        triggerPressed = false;
        holding = false;
        //Debug.Log("prevRemoteRot " + prevRemoteRot.x + " " + prevRemoteRot.y + " " + prevRemoteRot.z);
        //Debug.Log("elbow.transform.rotation..eulerAngles " + elbow.transform.rotation.eulerAngles.x + " " + elbow.transform.rotation.eulerAngles.y + " " + elbow.transform.rotation.eulerAngles.z);
        Vector3 direction = thisHand.transform.position - prevRemotePos;
        //Debug.Log("direction " + direction.x + " " + direction.y + " " + direction.z);
        //thisFrisbee.GetComponent<frisbee>().Fly((prevRemoteRot - remoteRot.eulerAngles)*thrust);
        thisFrisbee.GetComponent<frisbee>().Fly(direction*thrust);
        thisFrisbee.GetComponent<Rigidbody>().useGravity = true;
    }
}
