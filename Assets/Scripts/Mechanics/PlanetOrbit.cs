using Network;
using UnityEngine;
using Mirror;
using UI;

public class PlanetOrbit : NetworkMovableObject
{
    protected override float speed => smoothTime;

    [SerializeField] private Vector3 aroundPoint;
    [SerializeField] private float smoothTime = .3f;
    [SerializeField] private float circleInSecond = 1f;

    [SerializeField] private float offsetSin = 1;
    [SerializeField] private float offsetCos = 1;
    [SerializeField] private float rotationSpeed;

    [SerializeField] float radius;
    private float currentAng;
    private Vector3 currentPositionSmoothVelocity;
    private float currentRotationAngle;
    

    private const float circleRadians = Mathf.PI * 2;

    private void Start()
    {
        
        Initiate(UpdatePhase.FixedUpdate);
    }

    protected override void HasAuthorityMovement()
    {
        if (!isServer)
            return;

        Vector3 p = aroundPoint;
        p.x += Mathf.Sin(currentAng) * radius * offsetSin;
        p.z += Mathf.Cos(currentAng) * radius * offsetCos;
        transform.position = p;
        currentRotationAngle += Time.deltaTime * rotationSpeed;
        currentRotationAngle = Mathf.Clamp(currentRotationAngle, 0, 361);
        if (currentRotationAngle >= 360)
            currentRotationAngle = 0;

        transform.rotation = Quaternion.AngleAxis(currentRotationAngle, transform.up);
        currentAng += circleRadians * circleInSecond * Time.deltaTime;

        SendToClients();
    }

    protected override void SendToClients()
    {
        serverPosition = transform.position;
        serverEulers = transform.eulerAngles;
    }

    protected override void FromOwnerUpdate()
    {
        if (!isClient)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentPositionSmoothVelocity, speed);
        transform.rotation = Quaternion.Euler(serverEulers);
    }
}

