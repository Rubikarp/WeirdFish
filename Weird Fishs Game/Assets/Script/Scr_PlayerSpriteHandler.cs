using UnityEngine;
using Game;

namespace Management
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Scr_PlayerSpriteHandler : MonoBehaviour
    {
        [Header("Component auto-attribué")]
        private Scr_InputHandler _input = null;
        private SpriteRenderer _spritRend = null; //le sprite de votre personnage
        private Animator _animator = null; //l'animator de votre personnage

        [Header("Component à attribué")]
        [SerializeField] private Scr_PlayerMouvement _avatar = null;

        void Start()
        {
            _input = GameObject.FindGameObjectWithTag("GameController").GetComponent<Scr_InputHandler>();
            _spritRend = this.gameObject.GetComponent<SpriteRenderer>();
            _animator = this.gameObject.GetComponent<Animator>();

            if (_input == null)
            {
                Debug.LogError("Le script Scr_InputHandler n'a pas été trouvé, vérifier qu'il est présent dans la scene et qu'il possède le tag GameController ");
            }
            if (_avatar == null)
            {
                Debug.LogError("Le script Scr_PlayerMouvement n'a pas été attribué", this.gameObject);
            }
        }

        void Update()
        {
            FlipSprite();

            //Paramètre de type float de l'animator
            _animator.SetFloat("VelocityX", Mathf.Abs(_avatar._body.velocity.x));
            _animator.SetFloat("VelocityY", _avatar._body.velocity.y);

            //Paramètre de type bool de l'animator
            _animator.SetBool("IsOnGround", _avatar._isOnGround);
        }

        void FlipSprite()
        {
            if (_input._CharacterOrientation == Vector2.right)
            {
                _spritRend.flipX = false;
            }
            else
            if (_input._CharacterOrientation == Vector2.left)
            {
                _spritRend.flipX = true;
            }
        }

        public void TriggerJump()
        {
            _animator.SetTrigger("Jump");
        }
    }
}
