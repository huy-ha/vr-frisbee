using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour {
    public GameObject frisbee;
    public GameObject thisHand;
    public GameObject elbow;
    Quaternion remoteRotation,rot;
    
    bool triggerPressed,holding;

    float distance;

	// Use this for initialization
	void Start () {
        //frisbee.Transform = OVRInput.GetLocalControllerRotation();
        triggerPressed = false;
        holding = false;
    }

    // Update is called once per frame
    void Update() {
        remoteRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        if (triggerPressed && (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || Input.GetButtonUp("Jump")))
        {
            //Frisbee released
            frisbee.GetComponent<Rigidbody>().useGravity = true;
            triggerPressed = false;
            holding = false;
            //frisbee.GetComponent<Rigidbody>().AddForce((-(thisHand.transform.position) + (elbow.transform.position))*10f);
            frisbee.GetComponent<Rigidbody>().AddForce(thisHand.transform.forward * 800f);
        }
        else if(!triggerPressed && (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)|| Input.GetButtonDown("Jump")))
        {
            //Frisbee Picked up
            triggerPressed = true;
            frisbee.GetComponent<Rigidbody>().velocity = Vector3.zero;
            frisbee.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
        }

        distance = Vector3.Distance(thisHand.transform.position, frisbee.transform.position);

        //frisbee floating to hand
        if (triggerPressed && distance != 0)
        {
            frisbee.GetComponent<Rigidbody>().useGravity = false;
            if (distance < 0.1)
                holding = true;
            else
            {
                frisbee.transform.position += 0.2f * (thisHand.transform.position - frisbee.transform.position);
                rot = frisbee.transform.rotation;
                rot.eulerAngles += 0.2f * (thisHand.transform.rotation.eulerAngles - frisbee.transform.rotation.eulerAngles);
                frisbee.transform.rotation = rot;
            }
        }
        rot = elbow.transform.rotation;
        rot.eulerAngles += 0.7f * (remoteRotation.eulerAngles - rot.eulerAngles);
        elbow.transform.rotation = rot;

        if (holding)
        {
            frisbee.transform.position = thisHand.transform.position;
            frisbee.transform.rotation = thisHand.transform.rotation;
            //frisbee.transform.Rotate(new Vector3(90,0,0));
        }

        
    }
}
