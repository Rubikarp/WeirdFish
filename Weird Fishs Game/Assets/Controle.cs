using UnityEngine;
using UnityEngine.Events;

public class Controle : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                              // La force qui sera attribuée au joueur quand il sautera
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;              // La vitesse de déplacement du personnage quand il est baissé (ne sera peut être pas utilisé)
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;      // Fluidification du mouvement
    [SerializeField] private bool m_AirControl = false;                             // Il faudrait pouvoir controller le personnage dans les airs
    [SerializeField] private LayerMask m_WhatIsGround;                              // Le mask qui permettra de savoir si le personnage est sur un sol ou non
    [SerializeField] private Transform m_GroundCheck;                               // La position marquant où se trouve le sol pour le joueur
    [SerializeField] private Transform m_CeillingCheck;                             // La position marquant où se trouve le plafond pour le joueur
    [SerializeField] private Collider2D m_CrouchDisableCollider;                    // La collision qui sera supprimée quand le joueur se baissera (ne sera peut être pas utilisé)

    const float k_CeillingRadius = .2f;                                             // Rayon du cercle pour déterminer si le joueur peut se lever
    const float k_GroudedRadius = .2f;                                              // Rayon du cercle pour déterminer si le joueur est sur le sol
    private bool m_Grounded;                                                        // Si je joueur est, ou non, sur le sol
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;                                              // Pour déterminer vers quelle direction le joueur est
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;                                                 // Ne sera peut être pas utilisé
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroudedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // Si le joueur se baisse, il faut regarder si le personnage à la place pour se lever
        if (!crouch)
        {
            // Si le personnage a le plafond au dessus de lui, il ne pourra pas se lever et restera baissé
            if (Physics2D.OverlapCircle(m_CeillingCheck.position, k_CeillingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Réduction de la vitesse de déplacement en étant baissé (ne sera peut être pas utilisé)
                move *= m_CrouchSpeed;

                // Annulation temporaire d'une des collisions quand le personnage est baissé (ne sera peut être pas utilisé)
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Activation des collisions quand le personnage ne se baisse pas
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // Si le joueur veut aller à droite et que le personnage est vers la gauche...
            if (move > 0 && !m_FacingRight)
            {
                // ... ça retourne le personnage dans la direction vers laquelle il avancera
                Flip();
            }
            // Et à l'inverse, si le joueur veut aller à gauche et que le personnage est vers la droite...
            else if (move < 0 && m_FacingRight)
            {
                // ... ça retourne une nouvelle fois le personnage dans le bon sens
                Flip();
            }
        }

        // Si le joueur veut sauter
        if (m_Grounded && jump)
        {
            // On ajoute une force verticale au personnage
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
