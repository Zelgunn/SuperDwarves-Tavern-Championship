using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkSkills : NetworkBehaviour
{
    static private float s_horizontalSpeed = 10;
    static private float s_speedFactor = 50;
    static private float s_maximumForce = 10;

    [SerializeField] private GameObject m_pintPrefab;
    private float m_reloadTime = 0.5f;
    private float m_lastFireTime;

    private bool m_closeToTrapTrigger = false;
    private CharacterManager m_characterManager;

	public void Start()
	{
        m_lastFireTime = Time.time;
        m_characterManager = GetComponent<CharacterManager>();
	}

    public void Update()
    {
        if (!hasAuthority)
            return;

        // Lancer de bière
        if (Input.GetMouseButton(0))
        {
            ThrowBeer();
        }

        // Activation des trappes
        if(m_closeToTrapTrigger && Input.GetButtonDown("Interaction"))
        {
            TrapManager.TriggerTraps();
        }
    }

    public void ThrowBeer()
    {
        if ((Time.time - m_lastFireTime) >= m_reloadTime)
        {
            m_lastFireTime = Time.time;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            CmdThrowBeer(mousePos);
            if(m_characterManager == GameManager.LocalCharacter())
            {
                ProfileInfo.SelectedProfileInfo().IncreaseWastedBeer(1);
            }
        }
    }

    [Command]
    public void CmdThrowBeer(Vector3 mousePos)
    {
        RpcThrowBeer(mousePos);
    }

    [ClientRpc]
    public void RpcThrowBeer(Vector3 mousePos)
    {
        Vector3 pintOrigin = transform.position;
        float throwingDirection = 1;
        if (mousePos.x < pintOrigin.x)
            throwingDirection = -1;

        pintOrigin.y += 0.5f;
        pintOrigin.x += throwingDirection * 0.1f;
        pintOrigin.z -= 0.25f;

        GameObject pintObject = (GameObject)GameObject.Instantiate(m_pintPrefab, pintOrigin, m_pintPrefab.transform.rotation);

        // vx = 10                                  // Vitesse horizontal choisie.
        // vy = k - 9.81*t                          // k : Vitesse verticale initiale (variable à calculer).
        // y = k*t - 9.81*t*t/2                     // y = Intégrale de vy (position verticale).
        // x = vx*t = 10*t                          // x = Intégrale de vx (position horizontale).
        // tf = dx/vx = dx/10                       // tf : Temps nécessaire pour atteindre la cible.
        // y = k*(dx/10) - 9.81*(dx/10)*(dx/10)/2
        // k = y/(dx/10) + 9.81*(dx/10)/2
        // k = y/tf + 9.81*tf/2
        Vector2 distanceToTarget = new Vector2(mousePos.x - pintOrigin.x, mousePos.y - pintOrigin.y);
        float timeToReachTarget = Mathf.Abs(distanceToTarget.x / s_horizontalSpeed);
        float initialVerticalSpeed = distanceToTarget.y / timeToReachTarget - Physics.gravity.y * timeToReachTarget / 2;
        float horizontalSpeed = s_horizontalSpeed;
        if (initialVerticalSpeed > s_maximumForce)
        {
            horizontalSpeed /= (initialVerticalSpeed / s_maximumForce);
            initialVerticalSpeed = s_maximumForce;
        }

        if (initialVerticalSpeed < - s_maximumForce)
        {
            horizontalSpeed /= (initialVerticalSpeed / s_maximumForce);
            initialVerticalSpeed = - s_maximumForce;
        }

        Rigidbody testRigidbody = pintObject.GetComponent<Rigidbody>();
        testRigidbody.AddForce(throwingDirection * horizontalSpeed * s_speedFactor, initialVerticalSpeed * s_speedFactor, 0);
        testRigidbody.AddTorque(Vector3.forward * 1000);

        PintThrow pintThrow = pintObject.GetComponent<PintThrow>();
        pintThrow.SetTeam(GetComponent<PlayerNetworkData>().PlayerTeam());
    }

    public bool IsCloseToTriggerTrap()
    {
        return m_closeToTrapTrigger;
    }

    public void SetCloseToTriggerTrap(bool closeToTrapTrigger)
    {
        m_closeToTrapTrigger = closeToTrapTrigger;
    }

    //public void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {

    //        if (RayCastMouse(out rayCastHit))
    //        {
    //            Transform hitTransform = rayCastHit.transform;
    //        }
    //    }
    //}

    //public bool RayCastMouse(out RaycastHit hit)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    return Physics.Raycast(ray, out hit);
    //}
}
