using System.Drawing;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using Image = DIKUArcade.Graphics.Image;

namespace SpaceTaxi.States {
    public class GamePaused : IGameState {
        private static GamePaused instance;
        private int activeMenuButton;
        private Entity backGroundImage;
        private Text continueGame;
        private GameEventBus<object> eventBus = TaxiBus.GetBus();
        private Text mainMenu;
        private int maxMenuButtons = 2;
        private Text[] menuButtons = new Text[3];
        private Text paused;
        private Text quit;

        // Works the exact same way as the constructor for the MainMenu except with a
        // different background image and a new button that makes it possible to access the
        // Main Menu from the pause screen
        public GamePaused() {
            backGroundImage = new Entity(new StationaryShape(new Vec2F(0.0f, 0.0f),
                    new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));
            continueGame = new Text("Continue Game", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.35f, 0.25f));
            mainMenu = new Text("Main Menu", new Vec2F(0.3f, 0.4f),
                new Vec2F(0.35f, 0.25f));
            quit = new Text("Quit", new Vec2F(0.3f, 0.3f),
                new Vec2F(0.35f, 0.25f));
            paused = new Text("GAME PAUSED", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.5f, 0.35f));
            menuButtons[0] = continueGame;
            menuButtons[1] = mainMenu;
            menuButtons[2] = quit;
        }

        // Empty methods because of the IGameState interface 
        public void GameLoop() { }

        public void InitializeGameState() { }

        public void UpdateGameLogic() { }

        // Shows the background image and the buttons in the window
        public void RenderState() {
            // Making the different buttons different colours
            continueGame.SetColor(Color.White);
            mainMenu.SetColor(Color.White);
            quit.SetColor(Color.White);
            paused.SetColor(Color.Red);
            // Making the activeMenuButton Golden
            menuButtons[activeMenuButton].SetColor(Color.Gold);

            // The buttons and background image are rendered
            backGroundImage.RenderEntity();
            menuButtons[0].RenderText();
            menuButtons[1].RenderText();
            menuButtons[2].RenderText();
            paused.RenderText();
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
                switch (keyValue) {
                case "KEY_UP":
                    if (activeMenuButton != 0) {
                        activeMenuButton -= 1;
                    }

                    break;
                case "KEY_DOWN":
                    if (activeMenuButton != maxMenuButtons) {
                        activeMenuButton += 1;
                    }

                    break;
                case "KEY_ENTER":
                    if (activeMenuButton == 0) {
                        eventBus.RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent, this,
                                "CHANGE_STATE",
                                "GAME_RUNNING", GameRunning.GetInstance().CurrentLevel));
                        break;
                    } else if (activeMenuButton == 1) {
                        eventBus.RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent, this,
                                "CHANGE_STATE",
                                "MAIN_MENU", ""));
                        break;
                    } else {
                        eventBus.RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.WindowEvent, this,
                                "CLOSE_WINDOW", "", ""));
                        break;
                    }
                }
            }
        }


        /// <summary>
        ///     Returns an instance of GamePaused (and instantiates one if it has not already been)
        /// </summary>
        /// <returns> An instance of GamePaused</returns>
        public static GamePaused GetInstance() {
            return GamePaused.instance ?? (GamePaused.instance = new GamePaused());
        }
    }
}