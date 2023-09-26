using System.Drawing;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using Image = DIKUArcade.Graphics.Image;

namespace SpaceTaxi.States {
    public class MainMenu : IGameState {
        private static MainMenu instance;
        private int activeMenuButton;
        private Entity backGroundImage;
        private GameEventBus<object> eventBus = TaxiBus.GetBus();
        private Text mainMenu;
        private int maxMenuButtons = 1;
        private Text[] menuButtons = new Text[2];
        private Text newGame;
        private Text quit;


        public MainMenu() {
            // The background image is instantiated with a position, size and image called
            // SpaceBackground.png
            backGroundImage = new Entity(new StationaryShape(new Vec2F(0.0f, 0.0f),
                    new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));
            // The buttons newGame and quit are instantiated with a string displayed on the 
            // screen and a position and size 
            newGame = new Text("New Game", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.35f, 0.25f));
            quit = new Text("Quit", new Vec2F(0.3f, 0.4f),
                new Vec2F(0.35f, 0.25f));
            mainMenu = new Text("MAIN MENU", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.5f, 0.35f));
            // The buttons are inserted in the array menuButtons
            menuButtons[0] = newGame;
            menuButtons[1] = quit;
        }

        // In order for the MainMenu class to be of the IGameState interface, there are 
        // a couple of methods we have to implement even though we don't use them. GameLoop, 
        // InitializeGameState and UpdateGameLogic are those methods. 
        public void GameLoop() { }

        public void InitializeGameState() { }

        public void UpdateGameLogic() { }

        /// <summary>
        ///     Shows the background image and the buttons in the window
        /// </summary>
        public void RenderState() {
            // The colors of the buttons are changed to make the interaction design more 
            // user friendly 
            newGame.SetColor(Color.White);
            quit.SetColor(Color.White);
            mainMenu.SetColor(Color.Red);
            // The activeMenuButton (a.k.a. the button that you are "hovering" above) is 
            // colored gold so the user can see which button is about to be pressed 
            menuButtons[activeMenuButton].SetColor(Color.Gold);

            // The buttons and background image are rendered
            backGroundImage.RenderEntity();
            menuButtons[0].RenderText();
            menuButtons[1].RenderText();
            mainMenu.RenderText();
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
                // If Key-Up is pressed 1 is subtracted from activeButton, unless its already 0
                case "KEY_UP":
                    if (activeMenuButton != 0) {
                        activeMenuButton -= 1;
                    }

                    break;
                // If Key-Down is pressed 1 is added to activeButton, unless activeButton is 
                // bigger than the number of buttons in menuButtons. 
                case "KEY_DOWN":
                    if (activeMenuButton != maxMenuButtons) {
                        activeMenuButton += 1;
                    }

                    break;
                case "KEY_ENTER":
                    // If the enter key is pressed and the activeMenuButton = 0, a GameEvent
                    // is created for all EventProcessors with the message to switch the
                    // state to GameRunning 
                    if (activeMenuButton == 0) {
                        eventBus.RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent, this,
                                "CHANGE_STATE",
                                "LEVEL_STATE", ""));

                        break;
                    }
                    // If the activeButton is 1, a GameEvent is created to close the window. 
                    else {
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
        ///     Using the singleton pattern, this method creates an instance of MainMenu or returns
        ///     the instance if it already exists
        /// </summary>
        /// <returns> An instance of MainMenu</returns>
        public static MainMenu GetInstance() {
            return MainMenu.instance ?? (MainMenu.instance = new MainMenu());
        }
    }
}