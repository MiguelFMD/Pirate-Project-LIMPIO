using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DefinitiveScript
{
    public class PlayerBehaviour : CharacterBehaviour
    {
        [System.Serializable]
        public class MouseInput
        {
            public Vector2 Damping; //Valor que permitirá hacer una gradación en el input recibido por el movimiento del ratón
            public Vector2 Sensitivity; //Valor que indica el nivel de sensibilidad del movimiento del ratón, es decir, como de grande será el movimiento de cámara en función del movimiento de ratón realizado
        }

        [SerializeField] private MouseInput m_MouseControl;
        public MouseInput MouseControl
        {
            get {
                return m_MouseControl;
            }
        }

        private InputController m_InputController; //Instancia del InputController
        public InputController InputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        private PlayerAnimatorController m_PlayerAnimatorController;
        public PlayerAnimatorController PlayerAnimatorController {
            get {
                if(m_PlayerAnimatorController == null) m_PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
                return m_PlayerAnimatorController;
            }
        }

        private PlayerMoveController m_MoveController;
        public PlayerMoveController MoveController {
            get { 
                if(m_MoveController == null) m_MoveController = GetComponent<PlayerMoveController>();
                return m_MoveController;
            }
        }

        private HealthController m_HealthController;
        public HealthController HealthController {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<HealthController>();
                return m_HealthController;
            }
        }

        private PlayerLockTargetController m_PlayerLockTargetController;
        public PlayerLockTargetController PlayerLockTargetController
        {
            get {
                if(m_PlayerLockTargetController == null) m_PlayerLockTargetController = GetComponent<PlayerLockTargetController>();
                return m_PlayerLockTargetController;
            }
        }

        private PlayerCameraController m_PlayerCameraController;
        public PlayerCameraController PlayerCameraController
        {
            get {
                if(m_PlayerCameraController == null) m_PlayerCameraController = GetComponentInChildren<PlayerCameraController>();
                return m_PlayerCameraController;
            }
        }

        private PlayerSoundController m_PlayerSoundController;
        public PlayerSoundController PlayerSoundController
        {
            get {
                if(m_PlayerSoundController == null) m_PlayerSoundController = GetComponent<PlayerSoundController>();
                return m_PlayerSoundController;
            }
        }

        private Vector2 m_MouseInput; //Atributo donde se guardará los valores graduales del input del ratón hasta alcanzar el valor final
        public Vector2 mouseInput {
            get {
                return m_MouseInput;
            }
            set {
                m_MouseInput = value;
            }
        }

        private Vector2 m_MovementInput;
        public Vector2 movementInput {
            get {
                return m_MovementInput;
            }
            set {
                m_MovementInput = value;
            }
        }

        private bool m_RunningInput;
        public bool runningInput {
            get {
                return m_RunningInput;
            }
            set {
                m_RunningInput = value;
            }
        }

        private bool m_AttackInput;
        public bool attackInput {
            get {
                return m_AttackInput;
            }
            set {
                m_AttackInput = value;
            }
        }

        private bool m_ShootInput;
        public bool shootInput {
            get {
                return m_ShootInput;
            }
            set {
                m_ShootInput = value;
            }
        }

        private bool m_BlockInput;
        public bool blockInput {
            get {
                return m_BlockInput;
            }
            set {
                m_BlockInput = value;
            }
        }

        private bool m_SableMode; //Sable mode = true, Gun mode = false
        public bool sableMode {
            get {
                return m_SableMode;
            }
            set {
                m_SableMode = value;
            }
        }

        private bool m_LockedTarget;
        public bool lockedTarget {
            get {
                return m_LockedTarget;
            }
            set {
                m_LockedTarget = value;
            }
        }

        private bool m_StopMovement; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopMovement {
            get {
                return m_StopMovement;
            }
            set {
                m_StopMovement = value;
            }
        }

        private bool m_StopInput; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopInput {
            get {
                return m_StopInput;
            }
            set {
                m_StopInput = value;
                PlayerCameraController.InputChanged(value);
            }
        }

        private bool m_PlayerOff; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool playerOff {
            get {
                return m_PlayerOff;
            }
            set {
                m_PlayerOff = value;    
                PlayerCameraController.InputChanged(value);
                PlayerAnimatorController.SetPlayerOff(playerOff);
            }
        }

        public GameObject model;
        public GameObject gunObject;
        public GameObject sableObject;

        void Start()
        {
            sableMode = true; //Se inicializa el modo de movimiento en modo sable
            PlayerAnimatorController.SetSableMode(sableMode);
            PlayerLockTargetController.SetSableMode(sableMode);

            stopMovement = false;
            stopInput = false;
            lockedTarget = false;
            playerOff = false;

            ChangeWeapon();
        }     

        void Update()
        {
            if(alive)
            {
                if(!stopInput && !playerOff)
                {
                    if(!lockedTarget && InputController.ChangeMoveMode)
                    {
                        ChangeMode();
                    }

                    if(sableMode && InputController.LockTarget)
                    {
                        LockUnlockTarget();
                    }

                    if(!stopMovement)
                    {
                        runningInput = !HealthController.GetRunOutOfStamina() && InputController.Running;

                        movementInput = new Vector2(InputController.Horizontal, InputController.Vertical);

                        float mouseInputX = Mathf.Lerp(mouseInput.x, InputController.MouseInput.x, 1f / MouseControl.Damping.x); //Calcula el valor gradual del movimiento de ratón en x para hacer un giro más natural
                        float mouseInputY = Mathf.Lerp(mouseInput.y, InputController.MouseInput.y, 1f / MouseControl.Damping.y);

                        mouseInput = new Vector2(mouseInputX, mouseInputY);
                    }
                    else
                    {
                        movementInput = Vector2.zero;
                        mouseInput = Vector2.zero;
                        runningInput = false;
                    }

                    attackInput = InputController.Attack;
                    shootInput = InputController.Shooting;
                    blockInput = InputController.Block && !HealthController.GetRunOutOfStamina();
                }
                else
                {   
                    movementInput = Vector2.zero;
                    mouseInput = Vector2.zero;
                    runningInput = attackInput = shootInput = blockInput = false;
                }
            }
            else
            {
                movementInput = Vector2.zero;
                mouseInput = Vector2.zero;
                runningInput = attackInput = shootInput = blockInput = false;
            }

            if(stopInput || playerOff) PlayerAnimatorController.ResetMovement();
        }

        void ChangeWeapon()
        {
            if(sableMode) PlayerSoundController.PlaySableDrawn();
            else PlayerSoundController.PlayGunDrawn();

            sableObject.SetActive(sableMode);
            gunObject.SetActive(!sableMode);
        }

        public void MakeVisible(bool param)
        {
            model.GetComponent<SkinnedMeshRenderer>().enabled = param;
            gunObject.GetComponentInChildren<MeshRenderer>().enabled = param;
            sableObject.GetComponentInChildren<MeshRenderer>().enabled = param;
        }

        public void ChangeMode()
        {
            stopInput = true;
            sableMode = !sableMode;
            PlayerLockTargetController.SetSableMode(sableMode);

            PlayerAnimatorController.ExitMode();
            
            MoveController.ResetXRotation();
            ChangeWeapon();
        }

        public void LockUnlockTarget()
        {
            if((!lockedTarget && PlayerLockTargetController.LockUnlockTarget()) || (lockedTarget && !PlayerLockTargetController.LockUnlockTarget()))
            {  
                stopInput = true;
                lockedTarget = !lockedTarget;

                PlayerAnimatorController.LockUnlockTarget();
            }
        }
    }
}