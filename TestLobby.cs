using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;


using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;
using QFSW.QC;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;


public class TestLobby : MonoBehaviour
{

    //text score
    public static TextMeshProUGUI playerOneScore;
    public static TextMeshProUGUI playerTwoScore;
    public  TextMeshProUGUI relayCode;
    public GameObject HUDJoinButton;

    [SerializeField] private GameObject scoreHUD;
    // Start is called before the first frame update
    [SerializeField] private GameObject LobbyTemplate;
    [SerializeField] private GameObject LobbiesMenu;
    [SerializeField] private GameObject MembersMenu;

    [SerializeField] private Transform LobbiesContent;
    [SerializeField] private Transform MembersContent;

    [SerializeField] private GameObject HUD;

    static public GameObject playerOne;
    static public GameObject playerTwo;
    static public GameObject ball;



    const string RELAY_JOIN_CODE = "RELAY_JOIN_CODE";

    private static Lobby hostLobby;
    private float timer;

    private void Update()
    {
        HandLobbyTimer();
    }
    private async void HandLobbyTimer()
    {
        if (hostLobby != null && hostLobby.HostId == AuthenticationService.Instance.PlayerId)
        {

            timer -= Time.deltaTime;
            if (timer < 0f)
            {

                timer = 15f;// 4 times a minute
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);


            }
        }
    }

    private async void Start()
    {
        

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in " + AuthenticationService.Instance.PlayerId);
        };
        try
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
            scoreHUD.SetActive(false);
        }
        catch (LobbyServiceException e)
        {

            Debug.Log(e);
        }

    }
    [Command]
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, DataObject>
            {
                {RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member,"0") },
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("created Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
            hostLobby = lobby;
            ListMembers();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);


        }
    }
    [Command]
    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log(" Lobbies found: " + queryResponse.Results.Count);

            DestroyChilds(LobbiesContent);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " : " + lobby.Id + " : " + lobby.LobbyCode);
                var t = Instantiate(LobbyTemplate, LobbiesContent);
                t.GetComponent<setText>().SetText(lobby.Name, lobby.Id);
                t.GetComponent<Button>().onClick.AddListener(delegate { JoinLobby(lobby.Id); });


            }

        }
        catch (LobbyServiceException e)
        {

            Debug.Log(e);
        }
    }
    [Command]
    public async void JoinLobby(string id)
    {
        try
        {
            hostLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            ListMembers();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void ListMembers()
    {
        LobbiesMenu.SetActive(false);
        MembersMenu.SetActive(true);
        DestroyChilds(MembersContent);
        Debug.Log("Refrash");
        hostLobby = await Lobbies.Instance.GetLobbyAsync(hostLobby.Id);
        var members = hostLobby.Players;
        foreach (Player item in members)
        {

            string Name = item.Id;
            bool IsHost = (item.Id == hostLobby.HostId);
            var t = Instantiate(LobbyTemplate, MembersContent);
            t.GetComponent<setText>().SetText(Name, IsHost.ToString());
            t.GetComponent<Button>().onClick.AddListener(delegate { KickPlayer(Name); });

        }
    }
    private async void KickPlayer(string Id)
    {
        if (AuthenticationService.Instance.PlayerId == hostLobby.HostId || AuthenticationService.Instance.PlayerId == Id)
        {
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, Id);
        }
    }
    private void DestroyChilds(Transform parent)
    {
        while (parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }

  
    [Command]
    public async void StartRelay()
    {
        relayCode.text = "relay code:";
        try
        {
            string joinCode = await TestRelay.CreateRelay(4);


            await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {

                Data = new Dictionary<string, DataObject>
            {
                {RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
            }
            );
            HUD.SetActive(false);
            scoreHUD.SetActive(true);
            relayCode.text = relayCode.text + " " + joinCode;
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e);
        }

    }
    public void JoinRelay(TextMeshProUGUI code)
    {
        try
        {
            //string code = hostLobby.Data[RELAY_JOIN_CODE].Value;
            TestRelay.JoinRelay(code.text.Substring(0, 6));

            HUDJoinButton.SetActive(false);
            scoreHUD.SetActive(true);
        }
        catch (RelayServiceException e)
        {

            Debug.LogError(e);
        }
    }

    [Command]
    public void JoinRelayButton()
    {
        HUD.SetActive(false);
        HUDJoinButton.SetActive(true);
        relayCode.text = hostLobby.Data[RELAY_JOIN_CODE].Value;
    }
}
