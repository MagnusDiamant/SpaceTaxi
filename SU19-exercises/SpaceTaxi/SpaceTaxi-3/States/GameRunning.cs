using DIKUArcade.EventBus;
using DIKUArcade.State;
using SpaceTaxi_3.Entities;
using SpaceTaxi_3.LevelParser;
using SpaceTaxi_3.Taxi;

namespace SpaceTaxi_3.States {
    public class GameRunning : IGameState {
        private static GameRunning instance;
        public string CurrentLevel;
        public CustomerGroup CustomerGroup;
        public DrawLevel LevelDrawer;
        public GetLevel LevelGetter;
        public ProcessLevel LevelProcessor;

        // Creating an instance field player
        public Player Player;

        // Creating a score
        public Score Score;

        public GameRunning() {
            // Instantiating player as a new Player
            Player = new Player();
            Player.SetPosition(0.45f, 0.6f);
            Player.SetExtent(0.05f, 0.05f);
            TaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, Player);
            LevelGetter = new GetLevel("short-n-sweet.txt");
            LevelProcessor = new ProcessLevel(LevelGetter);
            LevelDrawer = new DrawLevel(LevelProcessor);
            CustomerGroup = new CustomerGroup(LevelDrawer.LevelEntities);
            CustomerGroup.CreateCustomers(LevelProcessor.CustomerInfoList);
            Score = new Score();
        }

        public void GameLoop() { }

        public void InitializeGameState() { }

        /// <summary>
        ///     Updates game logic. The method is called in Game.gameLoop().
        /// </summary>
        public void UpdateGameLogic() {
            // Making the player move
            Player.Move();
            // Detecting collision with obstacles 
            LevelDrawer.LevelEntities.Iterate(Player.LevelCollision);
            // Detecting collision with customers
            CustomerGroup.Customers.Iterate(Player.CustomerCollision);
            // Shows Game Over Screen if player has died
            if (Player.Entity.IsDeleted()) {
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE",
                        "GAME_OVER", ""));
            }

            // Changes the level if the player flies through the portal 
            if (Player.Entity.Shape.Position.Y > 1.0f) {
                Player.ChangeLevel(CurrentLevel);
            }
        }

        /// <summary>
        /// Renders the different aspects of the game. For instance the player or the explosion when
        /// the player collides with an obstacle. Also renders the levels.
        /// </summary>
        public void RenderState() {
            // Renders the explosions
            Player.ExplosionContainer.ExplosionContainer.RenderAnimations();

            // Renders the player
            if (Player.Entity.IsDeleted()) {
                Player.SetPosition(20f, 20f);
            } else {
                Player.RenderPlayer();
            }

            // Render level images
            LevelDrawer.LevelEntities.RenderEntities();

            // Render customers 
            CustomerGroup.RenderCustomers();

            // Render score
            Score.RenderScore();
        }

        /// <summary>
        ///     If StateMachine.ProcessEvent has received an inputEvent HandleKeyEvent is
        ///     called. HandleKeyEvent lets you choose a button with the up- and down-key and press
        ///     it with the enter key
        /// </summary>
        /// <param name="keyValue"> The string representing which button was pressed</param>
        /// <param name="keyAction">
        ///     The string determining if the button was pushed or
        ///     released
        /// </param>
        public void HandleKeyEvent(string keyValue, string keyAction) {
            if (keyAction == "KEY_PRESS") {
                KeyPress(keyValue);
            } else {
                KeyRelease(keyValue);
            }
        }

        /// <summary>
        ///     Returns an instance of GameRunning (and instantiates one if it has not already been)
        /// </summary>
        /// <returns> An instance of GameRunning </returns>
        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }

        /// <summary>
        /// Making sure that when a key is pressed the event is registered to the TaxiBus - is to
        /// be used in HandleKeyEvent
        /// </summary>
        /// <param name="key"> A string with the name of the key that was pressed by the user</param>
        public void KeyPress(string key) {
            switch (key) {
            // If the escape key is pressed an event is created for all processors to change 
            // the activeState to GamePaused. 
            case "KEY_ESCAPE":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this,
                        "CHANGE_STATE", "GAME_PAUSED", ""));
                break;
            case "KEY_UP":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_UPWARDS",
                        "", ""));
                break;
            case "KEY_LEFT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_LEFT",
                        "", ""));
                break;
            case "KEY_RIGHT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_RIGHT",
                        "", ""));
                break;
            }
        }

        /// <summary>
        ///     Making sure that when a key is released the event is registered to the TaxiBus - is to
        ///     be used in HandleKeyEvent
        /// </summary>
        /// <param name="key">
        ///     A string with the name of the key that was released by the user
        /// </param>
        public void KeyRelease(string key) {
            switch (key) {
            case "KEY_LEFT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this,
                        "STOP_ACCELERATE_LEFT", "", ""));
                break;
            case "KEY_RIGHT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this,
                        "STOP_ACCELERATE_RIGHT", "", ""));
                break;
            case "KEY_UP":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this,
                        "STOP_ACCELERATE_UP", "", ""));
                break;
            }
        }
    }
}