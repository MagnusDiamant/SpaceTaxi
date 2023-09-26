using DIKUArcade.EventBus;
using DIKUArcade.State;
using SpaceTaxi_3.Entities;
using SpaceTaxi_3.LevelParser;

namespace SpaceTaxi_3.States {
    public class StateMachine : IGameEventProcessor<object> {
        public StateMachine() {
            // Subscribes every instance of StateMachine to the TaxiBus, so it will receive
            // GameStateEvents and InputEvents 
            TaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            TaxiBus.GetBus().Subscribe(GameEventType.InputEvent, this);

            // The default state of the StateMachine is MainMenu 
            ActiveState = MainMenu.GetInstance();
        }

        public IGameState ActiveState { get; private set; }

        /// <summary>
        ///     Since the StateMachine is subscribed to the TaxiBus, it will receive its
        ///     messages/events. ProcessEvent takes either a GameStateEvent and then uses SwitchState
        ///     to change the ActiveState, or it takes an InputEvent and calls HandleKeyEvent with
        ///     the proper message and parameter1.
        /// </summary>
        /// <param name="eventType">
        ///     A GameEventType which is only relevant to this method if it is
        ///     a GameStateEvent or an InputEvent
        /// </param>
        /// <param name="gameEvent">
        ///     A GameEvent containing a message and a Parameter1 with a string
        ///     in both
        /// </param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.GameStateEvent) {
                SwitchState(
                    GameStateType.StateTransformer.TransformStringToState(gameEvent.Parameter1));
                if (gameEvent.Parameter1 == "GAME_RUNNING") {
                    var gameRunning = GameRunning.GetInstance();
                    gameRunning.CurrentLevel = gameEvent.Parameter2;
                    // GetLevel
                    gameRunning.LevelGetter = new GetLevel(gameRunning.CurrentLevel);

                    // ProcessLevel
                    gameRunning.LevelProcessor = new ProcessLevel(gameRunning.LevelGetter);

                    // DrawLevel
                    gameRunning.LevelDrawer = new DrawLevel(gameRunning.LevelProcessor);

                    gameRunning.CustomerGroup =
                        new CustomerGroup(gameRunning.LevelDrawer.LevelEntities);
                    gameRunning.CustomerGroup.CreateCustomers(gameRunning.LevelProcessor
                        .CustomerInfoList);
                    gameRunning.CustomerGroup.AddExistingCustomers(gameRunning.Player
                        .PickedUpCustomer);
                }
            } else if (eventType == GameEventType.InputEvent) {
                ActiveState.HandleKeyEvent(gameEvent.Message,
                    gameEvent.Parameter1);
            }
        }

        // Switches the ActiveState depending on the input. 
        /// <summary>
        ///     Switches the ActiveState to whichever state is passed to the method.
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
    }
}