using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thrower : MonoBehaviour {
    public GameObject thisFrisbee;
    public GameObject thisHand;
    public GameObject elbow;
    public GameObject dog;
    private OVRCameraRig head;
    private float shoulderWidth;

    //Bools to check which state Thrower is in
    private bool holdingFrisbee, triggerPressed, triggerReleased;
    float thrust;
    int strokeLength, strokeCutoff;

    //LinkedList to help with throwing
    private LinkedList<Vector3> positions;
    private LinkedListNode<Vector3> middle;

    //interface for dog to interact with
    Vector3 sittingSpot;

    //for debugging
    bool debugging;
    public Text logger;

    // Use this for initialization
    void Start() {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.5f;//set this to lower if frame rate is too slow
        QualitySettings.antiAliasing = 2;
        head = GetComponent<OVRCameraRig>();
        positions = new LinkedList<Vector3>();
        shoulderWidth = 0.32f;
        strokeLength = 30;
        strokeCutoff = 10;
        thrust = 2500f;
        debugging = false;
        sittingSpot = transform.position + Camera.main.transform.rotation * Vector3.forward;
    }

    // Update is called once per frame
    void Update() {
        logger.text = "";
        log("holding frisbee " + holdingFrisbee);
        log("triggerPressed " + triggerPressed);
        log("triggerReleased " + triggerReleased);

        updateController();
        triggerReleased = 
            OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) ||
            Input.GetButtonUp("Jump");
        triggerPressed = triggerReleased ? false :
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ||
            Input.GetButton("Jump"));

        updatePositionsList();
        if (triggerPressed && Vector3.Distance(thisFrisbee.transform.position, transform.position) < 2.5)
        {
            dog.GetComponent<Dog>().letGo();
            log("taking frisbee");
            takeFrisbee();
            
        } else if (triggerReleased && holdingFrisbee)
        {
            log("Throwing Frisbee!");
            ThrowFrisbee();
        }
        
        //Vector2 touch = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);
    }
         
    //Update Linked List of positions to calculate stroke
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

    public Vector3 getSittingSpot()
    {
        return sittingSpot;
    }

    //helper function for updatePositionList
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
        if (!debugging)
        {
            elbow.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
            //turn entire OVRrig, so hands also follow
            //TODO try making elbow a child of camera main
            elbow.transform.localPosition = new Vector3(
                shoulderWidth * Mathf.Cos(Mathf.PI * Camera.main.transform.rotation.eulerAngles.y/180),
                elbow.transform.localPosition.y,
                shoulderWidth * Mathf.Sin(Mathf.PI * Camera.main.transform.rotation.eulerAngles.y/180));
            //transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y,0));
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
            }
            else if (Input.GetKey("up"))
            {
                Camera.main.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 60f);
            }
            else if (Input.GetKey("down"))
            {
                Camera.main.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * 60f);
            }
        }
    }

    //if frisbee is not at hand, then transform frisbee to hand
    //if frisbee is at hand, then let frisbee follow hand's position
    void takeFrisbee()
    {
        Rigidbody f = thisFrisbee.GetComponent<Rigidbody>();
        f.useGravity = false;
        f.angularVelocity = Vector3.zero;
        f.velocity = Vector3.zero;
        if (holdingFrisbee)
        {
            //transform frisbee to be at hand
            f.position = thisHand.transform.position;
            f.rotation = thisHand.transform.rotation;
        }
        else
        {
            //lerping position
            f.position += (thisHand.transform.position - f.position) * Time.deltaTime * 10f;
            //slerping rotation
            Quaternion current = f.rotation;
            Quaternion target = thisHand.transform.rotation; 
            f.rotation = Quaternion.Slerp(current, target, Time.deltaTime*10);
            if(Vector3.Distance(thisHand.transform.position,thisFrisbee.transform.position) < 0.1)
            {
                holdingFrisbee = true;
            }
        }
    }

    void ThrowFrisbee()
    {
        holdingFrisbee = false;
        //for debugging
        if (debugging)
        {
            thisFrisbee.GetComponent<Frisbee>().Fly((transform.forward + new Vector3(0,1) * 0.6f) * thrust * Random.Range(0.2f, 0.4f));
        }
        else
        {
            //getting averages of positions to smooth out throw stroke 
            Vector3 direction = (positions.Last.Value + positions.Last.Previous.Value + positions.Last.Previous.Previous.Value);
            direction -= middle.Value + middle.Next.Value + middle.Previous.Value;
            direction /= 3;

            //if player's accidentally throws the frisbee down, then correct it
            if (direction.y < 0)
            {
                direction = direction - new Vector3(0,direction.y,0);
            }
            thisFrisbee.GetComponent<Frisbee>().Fly(direction * thrust);
        }
    }

    void log(string s)
    {
        logger.text += s + "\n";
    }

    public bool hasFrisbee()
    {
        return holdingFrisbee || triggerPressed;
    }
}
