using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Network
{
    public class NetworkCameraVisibleChecker : NetworkBehaviour
    {
        [SerializeField] private float updatePeriod = .1f;
        private float timer;
        private NetworkIdentity networkIdentity;
        private NetworkIdentity cameraIdentity;
        [SerializeField] private Camera cam;


        private void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            cameraIdentity = cam.GetComponent<NetworkIdentity>();
        }

        private void Update()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (Time.time - timer > updatePeriod)
            {
                cam ??= Camera.current;
                cameraIdentity ??= cam.GetComponent<NetworkIdentity>();
            }
        }
    }
}
