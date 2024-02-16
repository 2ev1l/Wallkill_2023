using Data.Stored;
using TMPro;
using UnityEngine;
using Universal.UI;
using EditorCustom.Attributes;
using Game.Labyrinth;
using Game.Rooms;
using Universal.UI.Triggers;

namespace Game.UI
{
    public class WorldPortal : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshProUGUI txt;
        [SerializeField] private DefaultStateMachine stateMachine;
        [SerializeField] private CustomButton customButton;
        [SerializeField] private WorldLoader worldLoader;
        [SerializeField] private EventTrigger portalAnimationsEventTrigger;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            stateMachine.Context.OnStateChanged += UpdateUIOnState;
        }
        private void OnDisable()
        {
            stateMachine.Context.OnStateChanged -= UpdateUIOnState;
        }
        private void UpdateUIOnState(StateChange state)
        {
            WorldChooseState worldState = (WorldChooseState)state;
            spriteRenderer.sprite = worldState.WorldInfo.Texture;
            txt.text = worldState.WorldInfo.Language.Text;
            customButton.enabled = worldState.WorldType != GameData.Data.WorldsData.CurrentWorld;
        }

        [SerializedMethod]
        public void LoadChoosedWorld()
        {
            WorldChooseState worldState = (WorldChooseState)stateMachine.Context.CurrentState;
            WorldType world = worldState.WorldType;
            InstancesProvider.Instance.OverlayStateMachine.ApplyDefaultState();
            portalAnimationsEventTrigger.SimulateExit();
            worldLoader.LoadWorld(world);
        }
        #endregion methods
    }
}