//using FPSFramework.Movement;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityUtilities;
//using UnityUtilities.ExecutionOrder.ExecutionOrderControl;
//using UnityUtilities.Networking;

////[AutoInit]
//public class GraphicSpawner : MonoBehaviour, CEventListener
//{
//    void Start()
//    {
//        CEventSystem.AddEventListener(GraphicSpawnerChannel.channel, GraphicSpawnerChannel.subchannel, this);
//    }

//    public void AcceptEvent(CEvent e)
//    {

//        if (e is NetworkMovementController.NetworkMovementControllerSpawn spawn)
//        {
//            if (spawn.networkClientNumber != NetworkClient.ClientNumber)
//            {
//                GameObject graphic = new GameObject("Graphic");
//                graphic.transform.position = spawn.pos;
//                NetBehaviour.Create<NetworkMovementControllerGraphic>(graphic, spawn.networkClientNumber, new NetGUID(spawn.guid));
//            }
//        }
//    }

//    public enum GraphicSpawnerChannel
//    {
//        channel,
//        subchannel
//    }
//}