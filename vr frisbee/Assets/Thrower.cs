using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour {
    public GameObject thisFrisbee;
    public GameObject thisHand;
    public GameObject elbow;

    //Bools to check which state Thrower is in
    private bool holdingFrisbee, triggerPressed, triggerReleased;
    float thrust;
    int strokeLength, strokeCutoff;
    bool debugging;

    //LinkedList to help with throwing
    private LinkedList<Vector3> positions;
    private LinkedListNode<Vector3> middle;

    // Use this for initialization
    void Start() {
        positions = new LinkedList<Vector3>();
        strokeLength = 20;
        strokeCutoff = 10;
        thrust = 2500f;
        debugging = false;
    }

    // Update is called once per frame
    void Update() {
        updateController();
        updateBools(); //check all booleans
        updatePositionsList();
        if (triggerPressed)
        {
            takeFrisbee();
        } else if (triggerReleased)
        {
            ThrowFrisbee();
        }

    }
    void updatePositionsList()
    {
        positions.AddFirst(thisHand.transform.position);
        if (positions.Count > strokeLength)
        {
            positions.RemoveLast();
        } else
        {
            middle = positions.First;
        }

        if (middle.Equals(positions.First) && positions.Count >= strokeLength)
        {
            middle = getMiddle();
        }
        else
        {
            middle = middle.Previous;
        }
    }

    LinkedListNode<Vector3> getMiddle()
    {
        LinkedListNode<Vector3> current = positions.First;
        int count = 0;
        while( count++ < strokeCutoff)
        {
            current = current.Next;
        }
        return current;
    }

    void updateController()
    {
        //Quaternion current = elbow.transform.localRotation;
        //elbow.transform.localRotation = 
        //    Quaternion.Slerp(current,
        //    OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote)
        //    , Time.deltaTime * 10);
        if (!debugging)
        {
            elbow.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        }
        else
        {
            if (Input.GetKey("left"))
            {
                elbow.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 60f);
            }
            else if (Input.GetKey("right"))
            {
                elbow.transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * 60f);
            }else if (Input.GetKey("up"))
            {
                transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 60f);
            }
            else if (Input.GetKey("down"))
            {
                transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * 60f);
            }
        }
    }

    //if frisbee is not at hand, then transform frisbee to hand
    //if frisbee is at hand, then let frisbee follow hand's position
    void takeFrisbee()
    {
        Rigidbody rb = thisFrisbee.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        if (holdingFrisbee)
        {
            //transform frisbee to be at hand
            rb.position = thisHand.transform.position;
            rb.rotation = thisHand.transform.rotation;
        }
        else
        {
            //lerping position
            rb.position += (thisHand.transform.position - rb.position) * Time.deltaTime * 10f;
            //slerping rotation
            Quaternion current = rb.rotation;
            Quaternion target = thisHand.transform.rotation; 
            rb.rotation = Quaternion.Slerp(current, target, Time.deltaTime*10);
        }
    }

    void ThrowFrisbee()
    {
        //for debugging
        if (debugging)
        {
            thisFrisbee.GetComponent<Frisbee>().Fly((transform.forward + new Vector3(0,1) * 0.3f) * thrust * 0.2f);
        }
        else
        {
            //getting averages of positions to smooth out throw stroke 
            Vector3 direction = (positions.Last.Value + positions.Last.Previous.Value + positions.Last.Previous.Previous.Value);
            direction -= middle.Value + middle.Next.Value + middle.Previous.Value;
            direction /= 3;
            thisFrisbee.GetComponent<Frisbee>().Fly(direction * thrust);
        }
    }

    void updateBools()
    {
        triggerReleased =
            OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) ||
            Input.GetButtonUp("Jump");
        triggerPressed = triggerReleased ? false :
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ||
            Input.GetButton("Jump"));
        holdingFrisbee = holdingFrisbee ?
            //if player is holding frisbee and trigger is released
            !triggerReleased : 
            //if player not holding frisbee but frisbee is close to hand
            ((Vector3.Distance(thisHand.transform.position, thisFrisbee.transform.position) < 0.1) ?
            true : false);
    }
}
