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


public class TestRelay : MonoBehaviour
{


    [Command]
    public async static Task<string> CreateRelay(int MaxPlayers)
    {
        try { 
        
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
           
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

         
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return null;
        }
    }
    [Command]
    public async static void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("join Relay with " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
           

        }
        catch (RelayServiceException e)
        {

            Debug.Log(e);

        }
    }
    
}
