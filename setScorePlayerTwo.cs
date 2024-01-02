
using UnityEngine;
using TMPro;


public class setScorePlayerTwo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerTwoScore;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(2);

        TestLobby.playerTwoScore = playerTwoScore;
    }

    
}
