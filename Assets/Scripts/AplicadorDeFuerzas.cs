using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AplicadorDeFuerzas : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rbC = collision.gameObject.GetComponent<Rigidbody>();
        if (rbC != null)
        {
            rbC.AddForce(rb.velocity, ForceMode.Impulse);
        }
    }
}
