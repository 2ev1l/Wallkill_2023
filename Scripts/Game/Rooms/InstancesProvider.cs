using EditorCustom.Attributes;
using Game.CameraView;
using Game.UI;
using Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.Rooms
{
    public class InstancesProvider : SingleSceneInstance<InstancesProvider>
    {
        #region fields & properties
        public Camera MainCamera => mainCamera;
        [Title("Camera")][SerializeField] private Camera mainCamera;
        public Camera OverlayCamera => overlayCamera;
        [SerializeField] private Camera overlayCamera;
        public CameraController CameraController => cameraController;
        [SerializeField] private CameraController cameraController;
        public CameraPosition CameraPosition => cameraPosition;
        [SerializeField] private CameraPosition cameraPosition;
        public CameraCollision CameraCollision => cameraCollision;
        [SerializeField] private CameraCollision cameraCollision;
        public CameraCrop CameraCrop => cameraCrop;
        [SerializeField] private CameraCrop cameraCrop;

        public Player.Input PlayerInput => playerInput;
        [Title("Player")][SerializeField] private Player.Input playerInput;
        public Player.Moving PlayerMoving => playerMoving;
        [SerializeField] private Player.Moving playerMoving;
        public Player.Jumping PlayerJumping => playerJumping;
        [SerializeField] private Player.Jumping playerJumping;
        public Player.Attack PlayerAttack => playerAttack;
        [SerializeField] private Player.Attack playerAttack;
        public Player.Animations PlayerAnimations => playerAnimations;
        [SerializeField] private Player.Animations playerAnimations;
        public Player.Rigging PlayerRigging => playerRigging;
        [SerializeField] private Player.Rigging playerRigging;
        public Player.Inventory PlayerInventory => playerInventory;
        [SerializeField] private Player.Inventory playerInventory;
        public DamageReceiver PlayerDamageReceiver => playerDamageReceiver;
        [SerializeField] private DamageReceiver playerDamageReceiver;
        public KeyCodeStateMachine OverlayStateMachine => overlayStateMachine;
        [Title("Overlay")][SerializeField] private KeyCodeStateMachine overlayStateMachine;
        public PortalOverlay PortalOverlay => portalOverlay;
        [SerializeField] private PortalOverlay portalOverlay;
        public TargetCounterStaticUI TargetsCounterUI => targetsCounterUI;
        [SerializeField] private TargetCounterStaticUI targetsCounterUI;
        public DevCameraFollow DevCameraFollow => devCameraFollow;
        [SerializeField] private DevCameraFollow devCameraFollow;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}