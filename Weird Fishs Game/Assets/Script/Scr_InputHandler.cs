using UnityEngine;

/// Fait par Arthur Deleye (arthurdeleye@gmail.com / a.deleye@rubika-edu.com)
/// 
/// [SerializeField] private Scr_InputHandler _input;        
/// _input = GameObject.FindGameObjectWithTag("GameController").GetComponent<Scr_InputHandler>();

namespace Management
{
    public class Scr_InputHandler : MonoBehaviour
    {

        [Header("Control")]
        public bool _canInput = true;
        public bool _KeyboardControl = true;

        [Header("Input")]
        public Vector2 _mouvAxisDirection = Vector2.zero;
        public float _mouvAxisMagnitude;
        [Space(10)]
        public bool _interaction;
        public bool _jump;

        [Header("Information")]
        public Vector2 _CharacterOrientation = Vector2.right;

        private void Update()
        {
            //changement de controller
            if (Input.GetButtonDown("SwitchController"))
            {
                _KeyboardControl = !_KeyboardControl;
            }

            if (_canInput)
            {
                if (_KeyboardControl)
                {
                    //Je prends les valeurs du stick
                    _mouvAxisDirection = new Vector2(Input.GetAxis("Keyboard-AxisX"), Input.GetAxis("Keyboard-AxisY"));
                    _mouvAxisMagnitude = _mouvAxisDirection.magnitude;

                    //Je prends les buttons
                    _interaction = Input.GetButton("Keyboard-InteractionButton");
                    _jump = Input.GetButton("Keyboard-JumpButton");


                    //update de l'orientation du joueur
                    if (_mouvAxisDirection.x > 0.5f) 
                    { _CharacterOrientation = Vector2.right; }
                    else
                    if (_mouvAxisDirection.x < -0.5f) 
                    { _CharacterOrientation = Vector2.left; }
                }
                else
                {
                    //Je prends les valeurs du stick
                    _mouvAxisDirection = new Vector2(Input.GetAxis("LStickAxisX"), Input.GetAxis("LStickAxisY"));
                    _mouvAxisMagnitude = _mouvAxisDirection.magnitude;

                    //Je prends les buttons
                    _interaction = Input.GetButton("B/Circle");
                    _jump = Input.GetButton("A/Cross");

                    //update de l'orientation du joueur
                    if (_mouvAxisDirection.x > 0.5f)
                    { 
                        _CharacterOrientation = Vector2.right; 
                    }
                    else if (_mouvAxisDirection.x < -0.5f)
                    { 
                        _CharacterOrientation = Vector2.left; 
                    }
                }
            }
        }

        public void DesactivateControl()
        {
            _canInput = false;

            _mouvAxisMagnitude = 0;

            _interaction = false;
            _jump = false;
        }
        public void ReActivateControl()
        {
            _canInput = true;
        }
    }
}
