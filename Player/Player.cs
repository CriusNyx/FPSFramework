using GameEngine.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Player : Actor
{
    public EventPlayerNumberChannel playerNumber = EventPlayerNumberChannel.player1;

    private void Start()
    {
        InitializeActor();
        var playerCharacter = gameObject.AddComponent<PlayerCharacter>();
        playerCharacter.playerNumber = playerNumber;
        var weapon = gameObject.AddComponent<TestWeapon>();
    }
}
