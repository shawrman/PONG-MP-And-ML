using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class onlineBall : NetworkBehaviour
{
    private NetworkVariable<Vector2> serverPostion = new NetworkVariable<Vector2>();
    private const float maxHight = 5;
    private const float minHight = -5;
    private const float maxWidth = 7.8f;
    private const float minWidth = -7.8f;


    private const float playerMax = 1;
    private const float playerMin = -1.25f;
    private const float playerWidth = 1f;



    static Rigidbody2D rb;
    [SerializeField] static private float speed = 5.0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TestLobby.ball = this.gameObject;


    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            serverPostion.Value = transform.position;

        }
        else
        {
            transform.position = serverPostion.Value;

        }
    }
    public static void startPoint()
    {
        rb.position = new Vector2(0, 0);
        rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;

    }
    private void Update()
    {
        if (IsServer)
        {
            serverPostion.Value = transform.position;

        }
        else
        {
            transform.position = serverPostion.Value;

        }
        if (!IsServer) return;
        // Limit the speed of the ball
        if (rb.velocity.magnitude > 20)
        {
            rb.velocity = rb.velocity.normalized * 20;
        }
        if(transform.position.y >= maxHight)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1);
            }
        }
        if (transform.position.y <= minHight)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1);
            }
        }

        if (transform.position.x >= maxWidth)
        {


            if (transform.position.x >= maxWidth + 1.2)
            {

      

         
                addScorePlayerOneClientRpc();


            }

            if (transform.position.x <= maxWidth + playerWidth)
            {

                if (TestLobby.playerTwo.transform.position.y > transform.position.y + playerMin && TestLobby.playerTwo.transform.position.y < transform.position.y + playerMax)
                {
                    if (rb.velocity.x > 0)
                    {
                        rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);

                    }

                }
            }

          
        }      
        if (transform.position.x <= minWidth)
        {
            if (transform.position.x <= minWidth - 1.2)
            {


             

                addScorePlayerTwoClientRpc();


            }
            if (transform.position.x >= minWidth - playerWidth)
            {

                if (TestLobby.playerOne.transform.position.y > transform.position.y + playerMin && TestLobby.playerOne.transform.position.y < transform.position.y + playerMax)
                {
                    if (rb.velocity.x < 0)
                    {
                        rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);

                    }

                }
            }
        }
     

    }
    void addScorePlayerOne()
    {
        TestLobby.playerOneScore.text = (int.Parse(TestLobby.playerOneScore.text) + 1).ToString();

        startPoint();
        if (int.Parse(TestLobby.playerOneScore.text) == 5)
        {
            TestLobby.playerOneScore.text = "$";
            TestLobby.playerTwoScore.text = ":/";
  
        }

    }
    void addScorePlayerTwo()
    {
        TestLobby.playerTwoScore.text = (int.Parse(TestLobby.playerTwoScore.text) + 1).ToString();

        startPoint();
        if (int.Parse(TestLobby.playerTwoScore.text) == 5)
        {
            TestLobby.playerOneScore.text =":/";
            TestLobby.playerTwoScore.text = "$";
        }

    }
    [ClientRpc]
    void addScorePlayerOneClientRpc()
    {
        addScorePlayerOne();
    }
    [ClientRpc]
    void addScorePlayerTwoClientRpc()
    {
        addScorePlayerTwo();
    }

}
