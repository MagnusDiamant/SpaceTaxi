using DIKUArcade.EventBus;
using DIKUArcade.State;

namespace SpaceTaxi_2.States {
    public class StateMachine : IGameEventProcessor<object> {
        public IGameState ActiveState { get; private set; }

        public StateMachine() {
            // Subscribes every instance of StateMachine to the TaxiBus, so it will receive
            // GameStateEvents and InputEvents 
            TaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            TaxiBus.GetBus().Subscribe(GameEventType.InputEvent, this);

            // The default state of the StateMachine is MainMenu 
            ActiveState = MainMenu.GetInstance();
        }

        // Switches the ActiveState depending on the input. 
        /// <summary>
        /// Switches the ActiveState to whichever state is passed to the method. 
        /// </summary>
        /// <param name="stateType"> A GameStateType contained in the enum GameStateTypes</param>
        private void SwitchState(GameStateType.GameStateTypes stateType) {
            switch (stateType) {
            case GameStateType.GameStateTypes.GameRunning:
                // If the stateType is GameRunning, the ActiveState is set to an instance of 
                // GameRunning. 
                ActiveState = GameRunning.GetInstance();
                break;
            case GameStateType.GameStateTypes.GamePaused:
                ActiveState = GamePaused.GetInstance();
                break;
            case GameStateType.GameStateTypes.GameOver:
                ActiveState = GameOver.GetInstance();
                break;
            case GameStateType.GameStateTypes.LevelState:
                ActiveState = LevelState.GetInstance();
                break;
            default:
                ActiveState = MainMenu.GetInstance();
                break;
            }
        }
        
        /// <summary>
        /// Since the StateMachine is subscribed to the TaxiBus, it will receive its
        /// messages/events. ProcessEvent takes either a GameStateEvent and then uses SwitchState
        /// to change the ActiveState, or it takes an InputEvent and calls HandleKeyEvent with
        /// the proper message and parameter1.
        /// </summary>
        /// <param name="eventType"> A GameEventType which is only relevant to this method if it is
        /// a GameStateEvent or an InputEvent</param>
        /// <param name="gameEvent"> A GameEvent containing a message and a Parameter1 with a string
        /// in both </param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.GameStateEvent) {
                SwitchState(
                    GameStateType.StateTransformer.TransformStringToState(gameEvent.Parameter1));
            } else if (eventType == GameEventType.InputEvent) {
                  ActiveState.HandleKeyEvent(gameEvent.Message,
                        gameEvent.Parameter1);
                }
            }
        }
    }
