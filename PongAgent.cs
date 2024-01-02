using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using Unity.MLAgents.Actuators;
using TMPro;

public class PongAgent : Agent
{
    private Rigidbody m_AgentRb;
    private float m_InvertMult;
    public bool invertX;
    public Transform BallTransform;
    public Rigidbody BallRigidbody;

    public TextMeshProUGUI score;
    
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_InvertMult = invertX ? -1f : 1f;
    }

    void SetResetParameters()
    {
    }

    public override void OnEpisodeBegin()
    {
        Vector3 pos = this.transform.position;
        pos.z = Random.Range(-6, 6);
        this.transform.position = pos;
        // m_AgentRb.velocity = Vector3.zero;

        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_InvertMult * (this.transform.position.x - BallTransform.localPosition.x));
        sensor.AddObservation(this.transform.localPosition.z - BallTransform.localPosition.z);
        sensor.AddObservation(BallTransform.localPosition.z);
        sensor.AddObservation(transform.localPosition.z);

        sensor.AddObservation(BallRigidbody.velocity.x);
        sensor.AddObservation(BallRigidbody.velocity.z);

        sensor.AddObservation(m_AgentRb.velocity.z);
    }


    public float forceMultiplier = 1f;

   
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = vectorAction.ContinuousActions[0];
        // m_AgentRb.AddForce(controlSignal * forceMultiplier);
        // m_AgentRb.velocity = controlSignal * forceMultiplier;
        m_AgentRb.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);
        // this.transform.position += controlSignal;

        if (m_InvertMult * (this.transform.position.x - BallTransform.position.x) < 0.5f)
        {
            AddReward(-5f);
            BallRigidbody.gameObject.GetComponent<BallController>().BallReset();
            score.text = (int.Parse(score.text) + 1).ToString();

            EndEpisode();
        }
        else
        {
            AddReward(0.05f);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
      
        continuousActionsOut[0] = Input.GetAxis("Vertical");
       
    }
}