using System;
using System.Threading.Tasks;
using DefaultNamespace;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using Mirror;
using TMPro;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        [SerializeField] private Transform _cameraAttach;
        private SolarSystemNetworkManager _netManager;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;

        private Vector3 currentPositionSmoothVelocity;
        public Action<NetworkConnection> onPlayerCollided;
        public Action<NetworkConnection> onCrysCollided;

        public int NumOfCrys { get; set; }
        
        protected override float speed => _shipSpeed;

        
        #region ChangePlayerName
        [TargetRpc]
        public void TargetChangePlayerName(NetworkConnection conn, string newValue)
        {
            var beh = conn.identity.NetworkBehaviours;
            foreach (var item in beh)
            {
                if (!item.gameObject.GetComponentInChildren<PlayerLabel>()) continue;
                item.gameObject.GetComponentInChildren<PlayerLabel>().SetName(newValue);
                break;
            }
            //conn.identity.gameObject.GetComponentInChildren<PlayerLabel>().SetName(newValue);
        }
        
        [Command] 
        public void CmdChangePlayerName(string newValue)
        {
            TargetChangePlayerName(gameObject.GetComponent<NetworkIdentity>().connectionToClient, newValue);
        }
        #endregion
        
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), NumOfCrys.ToString());
            if (_cameraOrbit == null)            
                return;
            _cameraOrbit.ShowPlayerLabels(playerLabel);
            
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)            
                return;
            
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            playerLabel = GetComponentInChildren<PlayerLabel>();

            _netManager = FindObjectOfType<SolarSystemNetworkManager>();
            _netManager._onNameSetted+= CmdChangePlayerName;

            base.OnStartAuthority();
        }
       

       protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)            
                return;            

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            if (isServer)
            {
                SendToClients();
            }
            else
            {
                CmdSendTransform(transform.position, transform.rotation.eulerAngles);
            }
        }

        protected override void FromOwnerUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentPositionSmoothVelocity, speed);
            transform.rotation = Quaternion.Euler(serverEulers);                       
        }

        protected override void SendToClients()
        {
            serverPosition = transform.position;
            serverEulers = transform.eulerAngles;
        }

        [Command]
        private void CmdSendTransform(Vector3 position, Vector3 eulers)
        {
            serverPosition = position;
            serverEulers = eulers;
        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CrysMarker>(out var component))
            {
                onCrysCollided?.Invoke(NetworkClient.connection);
                Destroy(other.gameObject);
            }
            else onPlayerCollided?.Invoke(NetworkClient.connection);
        }
        
        
    }
}
