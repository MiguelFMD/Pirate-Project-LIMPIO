using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class InputController : MonoBehaviour
    {
        public float Vertical; //Positivo con W o Up y negativo con S o Down
        public float Horizontal; //Positivo con D o Right y negativo con A o Left 
        public Vector2 MouseInput; //Movimiento del ratón en X y en Y
        public bool ChangeMoveMode; //True cuando se pulsa C
        public bool CenterRudder; //True cuando se pulsa C
        public bool LockTarget; //True cuando se pulsa la rueda del ratón
        public bool Running; //True cuando se mantiene shift (izquierdo o derecho)
        public bool Shooting; //True cuando se pulsa el click izquierdo del ratón
        public bool Attack; //True cuando se pulsa el click izquierdo del ratón
        public bool Block; //True cuando se mantiene el click derecho del ratón
        public bool Grab; //True cuando se mantiene el click izquierdo del ratón

        public bool IncreaseNumber; //True cuando se pulsa W
        public bool DecreaseNumber; //True cuando se pulsa S
        public bool ChangeSelectedNumberRight; //True cuando se pulsa D
        public bool ChangeSelectedNumberLeft; //True cuando se pulsa A

        public bool EnterIntoPuzle; //True cuando se pulsa Z
        public bool CheckNumbers; //True cuando se pulsa Z
        public bool ExitFromPuzle; //True cuando se pulsa X
        public bool ExitFromPause; //True cuando se pulsa X

        public bool ToMap; //True cuando se pulsa M

        public bool Pause; //True cuando se pulsa P

        void Update()
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");
            
            IncreaseNumber = Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0f;
            DecreaseNumber = Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0f;
            
            ChangeSelectedNumberRight = Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0f;
            ChangeSelectedNumberLeft = Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0f;

            MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            Shooting = Input.GetButtonDown("MouseLeftClick");
            Attack = Input.GetButtonDown("MouseLeftClick");
            LockTarget = Input.GetButtonDown("MouseMiddleClick");

            Grab = Input.GetButton("MouseLeftClick");
            Block = Input.GetButton("MouseRightClick");

            CheckNumbers = Input.GetButtonDown("Interact");
            EnterIntoPuzle = Input.GetButtonDown("Interact");

            ExitFromPuzle = Input.GetButtonDown("Cancel");
            ExitFromPause = Input.GetButtonDown("Cancel");

            ChangeMoveMode = Input.GetButtonDown("Change");
            CenterRudder = Input.GetButtonDown("Change");

            Running = Input.GetButton("Running");

            ToMap = Input.GetButtonDown("Map");

            Pause = Input.GetButtonDown("Pause");
        } 
    }
}