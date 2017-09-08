using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkMove : NetworkBehaviour
{
    // Paramètres du controlleur
    private Rigidbody m_rigidbody;
    //private Collider m_collider;
    private float m_speed = 1;
    private float m_jumpForce = 1;
    private bool m_grounded = true;
    private int m_direction = 1;
    private int m_team = -1;

    protected Animator m_animator;
    protected int m_jumpHash = Animator.StringToHash("Jumping");
    protected int m_runHash = Animator.StringToHash("Speed");

    private float m_previousVerticalInput = 0;

    public void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();

        MoveToSpawn();
    }
	
	public void Update ()
    {
        if (!hasAuthority)
            return;

        float verticalInput = Input.GetAxis("Vertical");

        m_animator.SetBool(m_jumpHash, false);
        if (m_grounded)
        {
            if(Mathf.Abs(m_rigidbody.velocity.y) < 0.1)
            {
                if (((verticalInput > 0) && !(m_previousVerticalInput > 0)) || Input.GetButtonDown("Jump"))
                {
                    m_grounded = false;

                    Vector3 jump = new Vector3(0, m_jumpForce * 550, 0);
                    m_rigidbody.AddForce(jump);
                    if (!m_animator.GetBool(m_jumpHash))
                    {
                        m_animator.SetTrigger(m_jumpHash);
                    }
                    CmdJumpAnim();
                }
            }
        }

        m_previousVerticalInput = verticalInput;

        // Déplacement horizontal
        float horizontalAxisValue = Input.GetAxis("Horizontal") * Time.deltaTime * 100;
        float horizontalMovement = horizontalAxisValue * m_speed * 2.5f;

        if (((horizontalAxisValue < 0) && (m_direction == 1)) || ((horizontalAxisValue > 0) && (m_direction == -1)))
        {
            m_rigidbody.transform.Rotate(0, 180, 0);
            m_direction = -m_direction;
        }

        Vector3 fixedVelocity = m_rigidbody.velocity;
        fixedVelocity.x = horizontalMovement;
        m_rigidbody.velocity = fixedVelocity;
    }

    public void LateUpdate()
    {
        m_animator.SetFloat(m_runHash, Mathf.Abs(m_rigidbody.velocity.x));
    }

    [Command]
    private void CmdJumpAnim()
    {
        RpcJumpAnim();
        if (!m_animator.GetBool(m_jumpHash))
        {
            m_animator.SetTrigger(m_jumpHash);
        }
    }

    [ClientRpc]
    private void RpcJumpAnim()
    {
        if (!m_animator.GetBool(m_jumpHash))
        {
            m_animator.SetTrigger(m_jumpHash);
        }
    }

    public void OnCollisionStay(Collision other)
    {
        foreach(ContactPoint contact in other.contacts)
        {
            if(contact.normal.y > 0.5)
            {
                m_grounded = true;
            }
        }
    }

    public void MoveToSpawn()
    {
        if(m_team == 0)
        {
            m_rigidbody.transform.position = GameManager.RedTeamSpawn().position;
            m_rigidbody.transform.eulerAngles = GameManager.RedTeamSpawn().eulerAngles;
            m_direction = 1;
        }
        else
        {
            m_rigidbody.transform.position = GameManager.BlueTeamSpawn().position;
            m_rigidbody.transform.eulerAngles = GameManager.BlueTeamSpawn().eulerAngles;
            m_direction = -1;
        }

        m_rigidbody.velocity = new Vector3(0, 0, 0);
    }

    public void EnableRigidbody(bool enable)
    {
        if (!enable)
            m_rigidbody.velocity = new Vector3(0, 0, 0);

        m_rigidbody.isKinematic = !enable;
        m_rigidbody.useGravity = enable;
    }

    public void SetTeam(int team)
    {
        m_team = team;
        //MoveToSpawn();
    }

    public void SetSpeed(float speed)
    {
        m_speed = speed;
    }
}
