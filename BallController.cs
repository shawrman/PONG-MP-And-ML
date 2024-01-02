using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;


public class BallController : MonoBehaviour
{
    public PongAgent left;
    public PongAgent right;

    private Vector3 vvvvv;

    private Rigidbody m_BallRb;

    void Start()
    {
        m_BallRb = GetComponent<Rigidbody>();
        BallReset();
    }

    private float vel = 8f;

    private void Update()
    {
        m_BallRb.velocity = vvvvv;
    }

    public void BallReset()
    {
        Vector3 p = this.transform.position;
        p.x = 0f;
        p.z = 0f;
        transform.position = p;
        float vx = Random.Range(-10, 10);

        float vz = Random.Range(-10, 10);

        float xx = (vx > 0 ? 1 : -1) * vel;
        float zz = (vz > 0 ? 0.5f : -0.5f) * vel;

        vvvvv = new Vector3(xx, 0, zz);
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Vector3 v = vvvvv;
            v.z = -v.z;
            vvvvv = v;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 v = vvvvv;
            v.x = -v.x;
            vvvvv = v;
            other.gameObject.GetComponent<PongAgent>().AddReward(1f);
        }
    }
}