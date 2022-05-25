using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SpaceShipSettings", menuName = "Geekbrains/Settings/Space Ship Settings")]
    public class SpaceShipSettings : ScriptableObject
    {
        public float Acceleration => acceleration;
        public float ShipSpeed => shipSpeed;
        public float Faster => faster;
        public float NormalFov => normalFov;
        public float FasterFov => fasterFov;
        public float ChangeFovSpeed => changeFovSpeed;

        [SerializeField, Range(.01f, 0.1f)] private float acceleration;
        [SerializeField, Range(1f, 2000f)] private float shipSpeed;
        [SerializeField, Range(1f, 5f)] private int faster;
        [SerializeField, Range(.01f, 179)] private float normalFov = 60;
        [SerializeField, Range(.01f, 179)] private float fasterFov = 30;
        [SerializeField, Range(.1f, 5f)] private float changeFovSpeed = .5f;
    }
}
