using FPSFramework.Movement;
using FPSFramework.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;

public class DebugServer : NetBehaviour
{
    Vector3 pos;
    public int number;

    private void Start()
    {
        state[Temp.none] = 0;
        pos = transform.position;
    }

    private enum Temp
    {
        none
    }

    public override void AcceptEvent(CEvent e)
    {
        throw new NotImplementedException();
    }

    protected override void RunByClient()
    {
        Run();
    }

    protected override void RunByOwner()
    {
        Run();
    }

    private void Run()
    {
        try
        {
            isOwner = NetworkServer.isServer;
            this.number = (int)state[Temp.none];
            transform.position = pos + Vector3.up * number;
        }
        catch
        {

        }

        //if(Input.GetKeyDown(KeyCode.U))
        //{
        //    this.isOwner = true;
        //    state[Temp.none] = 0;

        //    GameObject network = new GameObject("Network");
        //    network.AddComponent<NetworkServer>();
        //    NetworkClient.Create(network, "127.0.0.1");

        //    var player = FindObjectOfType<Player>();
        //    player.gameObject.AddNetComponent<NetworkMovementController>();
        //}

        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    this.isOwner = false;

        //    //string ip = gameObject.transform.Find("ipbox").Find("Text").GetComponent<UnityEngine.UI.Text>().text;

        //    GameObject network = new GameObject("Network");
        //    NetworkClient.Create(network, "192.168.1.84");

        //    var player = FindObjectOfType<Player>();
        //    player.gameObject.AddNetComponent<NetworkMovementController>();
        //}

        if(Input.GetKeyDown(KeyCode.O))
        {
            if(isOwner)
            {
                state[Temp.none] = (int)state[Temp.none] + 1;
            }
        }
    }

    public override NetBehaviourSpawn GetSpawn()
    {
        return null;
    }
}