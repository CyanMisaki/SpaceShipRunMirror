using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlanetSpawner : NetworkBehaviour
{
    [SerializeField] PlanetOrbit _planetPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        var planetInstance = Instantiate(_planetPrefab.gameObject);
        NetworkServer.Spawn(planetInstance, NetworkServer.localConnection);

        Destroy(gameObject);
    }
}
