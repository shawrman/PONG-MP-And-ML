
using UnityEngine;
using TMPro;
public class setScorePlayerOne : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerOneScore;

    // Start is called before the first frame update.
    private void Start()
    {
        Debug.Log(2);
        TestLobby.playerOneScore = playerOneScore;
    }


    // Update is called once per frame
   
}
