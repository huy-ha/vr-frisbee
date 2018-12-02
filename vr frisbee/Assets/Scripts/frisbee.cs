using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frisbee : MonoBehaviour {
    Vector3 velocity;
    Rigidbody rb;
    bool flying;
    public float airResistance;
    public float lift;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        velocity = rb.velocity;
        flying = false;
    }
	
	// Update is called once per frame
	void Update () {
        velocity = rb.velocity;
        if (flying)
        {
            if ((airResistance * velocity.sqrMagnitude * Time.deltaTime * velocity).magnitude > 0.95 * velocity.magnitude)
            {
                rb.velocity *= 0.95f;
            }
            else
            {
                rb.velocity -= airResistance * velocity.sqrMagnitude * Time.deltaTime * velocity; // air resistance
            }
            rb.AddForce(Vector3.up * Time.deltaTime * velocity.sqrMagnitude * lift);  //lift from glide

            handleTilt();
        }
	}

    void handleTilt()
    {
        //TODO
        //Debug.Log("rotation " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z + " ");
    }

    public void setFly(bool val)
    {
        flying = val;
    }

    void OnCollisionEnter(Collision collision)
    {
        //if collide with ground then stop flying
        flying = false;
    }

    public void Fly(Vector3 releaseVelocity)
    {
        //Debug.Log("releaseVelocity " + releaseVelocity.x + " " + releaseVelocity.y + " " + releaseVelocity.z);
        flying = true;
        GetComponent<Rigidbody>().velocity = releaseVelocity;
    }
}
