using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management;

namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Scr_PlayerMouvement : MonoBehaviour
    {
        [Header("Component à attribué")]
        private Scr_InputHandler _input = null;
        [SerializeField] public Rigidbody2D _body = null;
        [SerializeField] private BoxCollider2D _collid = null;
        [SerializeField] private Scr_PlayerSpriteHandler _spritManager = null; //l'animator de votre personnage

        [Header("Variable")]
        public float _inputDeadZone = 0.5f;
        public float _maxRunSpeed = 15f;
        public float _maxAirSpeed = 8f;
        public float _runDeadZone = 1f;
        public float _runAccTimer = 0.4f;
        public float _runDecTimer = 0.4f;
        public float _airAccTimer = 0.4f;
        public float _airFriction = 8f;
        [Space(10)]
        public float jumpHeight = 45f;
        public float ascensionFactor = 2f;
        public float fallFactor = 3f;
        public float lowJumpFactor = 4f;
        [Space(10)]
        public LayerMask _TerrainLayerMask;

        [Header("State")]
        private Color _castColor = Color.red;
        public bool _isOnGround;
        public bool _canJump = true;
        public bool _isRuning;
        public bool _isAirControl;
        public bool _isJumping;

        [Header("Course")]
        private float _runAcc = 0f;
        private float _runDec = 0f;
        private float _runAccTime = 0f;
        private float _runDecTime = 0f;
        private float _airAcc = 0f;
        private float _airAccTime = 0f;

        void Start()
        {
            _input = GameObject.FindGameObjectWithTag("GameController").GetComponent<Scr_InputHandler>();
            _body = this.GetComponent<Rigidbody2D>();

            if (_input == null)
            {
                Debug.LogError("Le script Scr_InputHandler n'a pas été trouvé, vérifier qu'il est présent dans la scene et qu'il possède le tag GameController ");
            }
            if (_collid == null)
            {
                Debug.LogError("Le Collider du joueur n'est pas attribué", this.gameObject);
            }
        }

        void Update()
        {
            CheckGround();
        }

        private void FixedUpdate()
        {
            Run();
            Jump();
        }

        void Run()
        {
            //Au sol 
            if (_isOnGround)
            {
                if (Mathf.Abs(_input._mouvAxisDirection.x) > _inputDeadZone)
                {
                    //Avatar en train de courrir
                    _isRuning = true;
                    //reset décélération
                    _runDecTime = 0f;
                    //Lerp pour calculer son accélération
                    _runAcc = Mathf.Lerp(0, _maxRunSpeed, _runAccTime);
                    //Force appliqué sur l'avatar
                    _body.velocity = new Vector2(_runAcc * _input._CharacterOrientation.x, _body.velocity.y);
                    //Le temps s'incrémente d'où l'accélération
                    _runAccTime += Time.deltaTime * (1 / _runAccTimer);
                }
                else if (_isRuning)
                {
                    //reset accélération
                    _runAccTime = 0f;
                    //Lerp pour calculer sa décélération
                    _runDec = Mathf.Lerp(_runAcc, 0, _runDecTime);
                    //Force appliqué sur l'avatar                                                                  
                    _body.velocity = new Vector2(_runDec * _input._CharacterOrientation.x * _input._mouvAxisMagnitude, _body.velocity.y);
                    //Le temps s'incrémente d'où l'accélération
                    _runDecTime += Time.deltaTime * (1 / _runDecTimer);

                    //quand tu as suffisement deccéléré
                    if (Mathf.Abs(_body.velocity.x) < _runDeadZone)
                    {
                        _isRuning = false;
                    }
                }


            }
            //En l'air
            else
            {
                if (Mathf.Abs(_input._mouvAxisDirection.x) > _inputDeadZone)
                {
                    //Avatar en train de courrir
                    _isAirControl = true;
                    //Lerp pour calculer son accélération
                    _airAcc = Mathf.Lerp(_isRuning? _runAcc: _runDec, _maxAirSpeed, _airAccTime);
                    //Force appliqué sur l'avatar
                    _body.velocity = new Vector2(_airAcc * _input._CharacterOrientation.x, _body.velocity.y);
                    //Le temps s'incrémente d'où l'accélération
                    _airAccTime += Time.deltaTime * (1 / _airAccTimer);
                }
                else if (_isAirControl)
                {
                    //reset accélération
                    _airAccTime = 0f;
                    //Force appliqué sur l'avatar                                                                  
                    _body.velocity = new Vector2(_body.velocity.x - (_airFriction * _input._CharacterOrientation.x * Time.deltaTime), _body.velocity.y);

                    //quand tu as suffisement deccéléré
                    if (Mathf.Abs(_body.velocity.x) < _runDeadZone)
                    {
                        _isAirControl = false;
                    }
                }

            }
        }

        void Jump()
        {
            //il était tard donc c'est fait à l'arrache
            if (_input._jump && _isOnGround && !_isJumping)
            {
                _spritManager.TriggerJump();
                _isJumping = true;

                _body.velocity = new Vector2(_body.velocity.x, jumpHeight);
            }

            if (_isJumping)
            {
                if (_body.velocity.y < 0)
                {
                    //Si le joueur à fini de monter, il redescend plus vite afin d'avoir un meilleur game feel
                    _body.gravityScale = fallFactor;
                }
                else if (_body.velocity.y > 0 && !_input._jump)
                {
                    //Si le joueur est en train de monter MAIS qu'il lâche la touche de saut bam on augment la gravité pour stop son saut
                    _body.gravityScale = lowJumpFactor;
                }
                else
                {                    
                    //retour à la normal
                    _body.gravityScale = ascensionFactor;
                }

                if (_body.velocity.y < 1 && _isOnGround)
                {
                    _isJumping = false;
                }
            }
        }

        private void CheckGround()
        {
            //Cast Couleur
            _isOnGround = _collid.IsTouchingLayers(_TerrainLayerMask);
            //_isOnGround = Physics2D.BoxCast();
        }
    }
}
