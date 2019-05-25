using Assets.FPSFramework.Demo.NetworkDemo;
using FPSFramework.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.DataStructures;
using UnityUtilities.Networking;

public class NetworkDemoCanvas : MonoBehaviour, CEventListener
{
    enum Mode
    {
        none,
        server,
        client
    }

    Mode mode = Mode.none;

    int count = 0;

    UnityEngine.UI.Text statusLabel;

    void Awake()
    {
        statusLabel = gameObject.transform.Find("Status").GetComponentInChildren<UnityEngine.UI.Text>();

        CEventSystem.AddEventListener(ServerEventChannel.channel, ServerEventChannel.subchannel, this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            switch (mode)
            {
                case (Mode.none):
                    break;
                case (Mode.server):
                    {
                        Vector3 pos = Vector3.left * 3f + Vector3.forward * 2f * count;
                        CEventSystem.Broadcast(ServerEventChannel.channel, ServerEventChannel.subchannel, new SpawnBox(pos));
                        count++;
                        break;
                    }
                case (Mode.client):
                    {
                        Vector3 pos = Vector3.right * 3f + Vector3.forward * 2f * count;
                        CEventSystem.Broadcast(ServerEventChannel.channel, ServerEventChannel.subchannel, new SpawnBox(pos));
                        count++;
                        break;
                    }
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            local.name = "local";
            local.transform.position = Random.insideUnitSphere * 5f;
            local.AddNetComponent<TestGUIDObjectLocal>();
        }
    }

    public void BecomeServer()
    {
        SetStatus("Server");

        GameObject network = new GameObject("Network");
        network.AddComponent<NetworkServer>();
        NetworkClient.Create(network, "127.0.0.1");
        mode = Mode.server;

        HideCanvas();
    }

    public void BecomeClient()
    {
        SetStatus("Clinet");

        string ip = gameObject.transform.Find("ipbox").Find("Text").GetComponent<UnityEngine.UI.Text>().text;

        GameObject network = new GameObject("Network");
        NetworkClient.Create(network, ip);
        mode = Mode.client;

        HideCanvas();
    }

    Thunk<GameObject> playerPrefab = new Thunk<GameObject>(() => Resources.Load<GameObject>(ResourceFiles.Prefabs_Player));

    public void HideCanvas()
    {
        transform.Find("Host").gameObject.SetActive(false);
        transform.Find("Connect").gameObject.SetActive(false);
        transform.Find("ipbox").gameObject.SetActive(false);

        StartCoroutine(PlayerSpawnRoutine());
    }

    public IEnumerator PlayerSpawnRoutine()
    {
        for(int i = 0; i < 5; i++)
        {
            yield return null;
        }

        GameObject player = Instantiate<GameObject>(playerPrefab);
        player.AddNetComponent<NetworkMovementController>();

        for(int i = 0; i < 5; i++)
        {
            yield return null;
        }

        player.transform.position = Vector3.back * 10f + Vector3.up * 2f;
    }

    public void SetStatus(string status)
    {
        statusLabel.text = "Status: " + status;
    }

    public void AcceptEvent(CEvent e)
    {
        if(e is SpawnBox s)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = s.pos;
        }
    }

    [System.Serializable]
    public class SpawnBox : NetworkedCEvent
    {
        public readonly Vector3Wrapper pos;

        public override NetworkControlCode eventType
        {
            get
            {
                return NetworkControlCode.runOnClient;
            }
        }

        public SpawnBox(Vector3Wrapper pos)
        {
            this.pos = pos;
        }
    }
}