using System.Drawing;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using Image = DIKUArcade.Graphics.Image;

namespace SpaceTaxi.States {
    public class GameOver : IGameState {
        private static GameOver instance;
        private int activeMenuButton;
        private Entity backGroundImage;
        private GameEventBus<object> eventBus = TaxiBus.GetBus();
        private Text gameOver;
        private Text mainMenu;
        private int maxMenuButtons = 1;
        private Text[] menuButtons = new Text[2];
        private Text quit;


        public GameOver() {
            // The background image is instantiated with a position, size and image called
            // TitleImage.png
            backGroundImage = new Entity(new StationaryShape(new Vec2F(0.0f, 0.0f),
                    new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));
            // The buttons mainMenu and quit are instantiated with a string displayed on the 
            // screen and a position and size 
            mainMenu = new Text("Main Menu", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.35f, 0.25f));
            quit = new Text("Quit", new Vec2F(0.3f, 0.4f),
                new Vec2F(0.35f, 0.25f));
            gameOver = new Text("GAME OVER", new Vec2F(0.3f, 0.5f),
                new Vec2F(0.5f, 0.35f));
            // The buttons are inserted in the array menuButtons
            menuButtons[0] = mainMenu;
            menuButtons[1] = quit;
        }

        // In order for the GameOver class to be of the IGameState interface, there are 
        // a couple of methods we have to implement even though we don't use them. GameLoop, 
        // InitializeGameState and UpdateGameLogic are those methods. 
        public void GameLoop() { }

        public void InitializeGameState() { }

        public void UpdateGameLogic() { }

        // Shows the background image and the buttons in the window 
        public void RenderState() {
            // The colors of the buttons are changed to make the interaction design more 
            // user friendly 
            mainMenu.SetColor(Color.White);
            quit.SetColor(Color.White);
            gameOver.SetColor(Color.Red);
            // The activeMenuButton (a.k.a. the button that you are "hovering" above) is 
            // colored gold so the user can see which button is about to be pressed 
            menuButtons[activeMenuButton].SetColor(Color.Gold);

            // The buttons and background image are rendered
            backGroundImage.RenderEntity();
            menuButtons[0].RenderText();
            menuButtons[1].RenderText();
            gameOver.RenderText();
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
                    // state to Main Menu 
                    if (activeMenuButton == 0) {
                        eventBus.RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent, this,
                                "CHANGE_STATE",
                                "MAIN_MENU", ""));
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
        ///     A singleton returning an instance of gameOver if it already has been instantiated
        ///     and otherwise creates a new instance which is then returned
        /// </summary>
        /// <returns> An instance of GameOver</returns>
        public static GameOver GetInstance() {
            return GameOver.instance ?? (GameOver.instance = new GameOver());
        }
    }
}