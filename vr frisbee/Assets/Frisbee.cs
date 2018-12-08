using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frisbee : MonoBehaviour {
    bool flying,debugging;
    float airResistance, lift;

    private void Start()
    {
        airResistance = 0.003f;
        lift = 0.7f;
        debugging = false;
    }
    // Update is called once per frame
    void Update () {
        if (flying)
        {
            handleFlight();
        }
	}

    public void Fly(Vector3 direction)
    {
        flying = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(direction);
        rb.useGravity = true;
    }

    void handleFlight()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        //calculating air resistance
        Vector3 airResistanceForce = airResistance * rb.velocity.sqrMagnitude * Time.deltaTime * rb.velocity;
        if(airResistanceForce.magnitude > 0.03 * rb.velocity.magnitude)
        {
            rb.velocity *= 0.97f;
        }
        else
        {
            rb.velocity -= airResistanceForce;
        }

        //calculating lift, which only depends on forward velocity
        Vector3 forwardVelocity = rb.velocity - new Vector3(0, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * Time.deltaTime * forwardVelocity.sqrMagnitude * lift);

        if (rb.velocity.y < 0)
        {
            handleTilt();
        }

    }

    void handleTilt()
    {
        //calculating force due to tilt
        Vector3 v = GetComponent<Rigidbody>().velocity;
        Vector3 tilt = Vector3.Project(transform.up,Vector3.Cross(Vector3.up, v)); //difference between up direction and its current direction
        GetComponent<Rigidbody>().AddForce(tilt * Time.deltaTime * 300f * -(v.y));

        //reducing tilt
        Quaternion target = Quaternion.LookRotation(v,Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if collide with ground then stop flying
        flying = false;
    }
}
