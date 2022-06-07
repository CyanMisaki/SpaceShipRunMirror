using System;
using System.Collections.Generic;
using Characters;
using Messages;
using UnityEngine;
using Mirror;
using TMPro;
using Random = UnityEngine.Random;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
        [SerializeField] private GameObject _crysPrefab;
        [SerializeField] private int _spawnRadius = 100;
        [SerializeField] private int _crysNeededInLevel = 3;
        [SerializeField] private GameObject _panel;
        
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            for (var i = 0; i < _crysNeededInLevel; i++)
            {
                var crysInstance = Instantiate(_crysPrefab.gameObject);
                crysInstance.transform.position = Random.insideUnitSphere * _spawnRadius;
                NetworkServer.Spawn(crysInstance, NetworkServer.localConnection);
            }
            
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            var spawnTransform = GetStartPosition();
            
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var shControl = player.GetComponent<ShipController>();
            _players.Add(conn.connectionId, shControl);
            
            _players[conn.connectionId].onPlayerCollided += OnPlayerCollided;
            _players[conn.connectionId].onCrysCollided += OnCrysCollided;
            _players[conn.connectionId].NumOfCrys = 0;
            
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        [Server]
        private void OnCrysCollided(NetworkConnection conn)
        {
            _players[conn.connectionId].NumOfCrys++;
            if (_crysNeededInLevel <= 0)
            {
                
                    
                return;
            }

            _crysNeededInLevel--;
        }

        [Server]
        private void OnPlayerCollided(NetworkConnection conn)
        {
            conn.Disconnect();
            _players.Remove(conn.connectionId);
        }
    }
}
