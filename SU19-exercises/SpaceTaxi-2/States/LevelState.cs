using System.Drawing;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using Image = DIKUArcade.Graphics.Image;

namespace SpaceTaxi_2.States {
    public class LevelState : IGameState{
        private static LevelState instance = null;
            private Entity backGroundImage;
            private Text level1;
            private Text level2;
            private Text back;
            private Text[] menuButtons = new Text[3];
            private int activeMenuButton = 0;
            private int maxMenuButtons = 2;
            private GameEventBus<object> eventBus = TaxiBus.GetBus();


            public LevelState() {
                // The background image is instantiated with a position, size and image called
                // TitleImage.png
                backGroundImage = new Entity(new StationaryShape(new Vec2F(0.0f, 0.0f), 
                        new Vec2F(1.0f,1.0f)), 
                    new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));
                // The buttons newGame and quit are instantiated with a string displayed on the 
                // screen and a position and size 
                level1 = new Text("Level 1: Short-n-Sweet", (new Vec2F(0.3f, 0.5f)),
                    new Vec2F(0.35f,0.25f) );
                level2 = new Text("Level 2: The-Beach", (new Vec2F(0.3f,0.4f)), 
                    new Vec2F(0.35f,0.25f));
                back = new Text("Back to Main Menu", (new Vec2F(0.3f,0.3f)), 
                    new Vec2F(0.35f,0.25f));
                // The buttons are inserted in the array menuButtons
                menuButtons[0] = level1;
                menuButtons[1] = level2;
                menuButtons[2] = back;
                
            }
            
            /// <summary>
            /// Returns an instance of LevelState (and instantiates one if it has not already been)
            /// </summary>
            /// <returns> An instance of LevelState</returns>
            public static LevelState GetInstance() {
                return LevelState.instance ?? (LevelState.instance = new LevelState());
            }

            // In order for the MainMenu class to be of the IGameState interface, there are 
            // a couple of methods we have to implement even though we don't use them. GameLoop, 
            // InitializeGameState and UpdateGameLogic are those methods. 
            public void GameLoop() {
                
            }

            public void InitializeGameState() {
                
            }

            public void UpdateGameLogic() {
                
            }

            /// <summary>
            /// Shows the background image and the buttons in the window 
            /// </summary>
            public void RenderState() { 
                // The colors of the buttons are changed to make the interaction design more 
                // user friendly 
                level1.SetColor(Color.White);
                level2.SetColor(Color.White);
                back.SetColor(Color.White);
                // The activeMenuButton (a.k.a. the button that you are "hovering" above) is 
                // colored gold so the user can see which button is about to be pressed 
                menuButtons[activeMenuButton].SetColor(Color.Gold);
                
                // The buttons and background image are rendered
                backGroundImage.RenderEntity();
                menuButtons[0].RenderText();
                menuButtons[1].RenderText();
                menuButtons[2].RenderText();
                
            }

            /// <summary>
            /// If StateMachine.ProcessEvent has received an inputEvent HandleKeyEvent is
            /// called. HandleKeyEvent lets you choose a button with the up- and down-key and press
            /// it with the enter key
            /// </summary>
            /// <param name="keyValue"> The string representing which button was pressed</param>
            /// <param name="keyAction"> The string determining if the button was pushed or
            /// released</param>
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
                        if (activeMenuButton == 0) {
                            eventBus.RegisterEvent(
                                GameEventFactory<object>.CreateGameEventForAllProcessors(
                                    GameEventType.GameStateEvent, this,
                                    "CHANGE_STATE",
                                    "GAME_RUNNING", "short-n-sweet.txt"));
                            break;
                        } else if (activeMenuButton == 1) {
                            eventBus.RegisterEvent(
                                GameEventFactory<object>.CreateGameEventForAllProcessors(
                                    GameEventType.GameStateEvent, this,
                                    "CHANGE_STATE",
                                    "GAME_RUNNING", "the-beach.txt"));
                            break;
                        }
                        else {
                            eventBus.RegisterEvent(
                                GameEventFactory<object>.CreateGameEventForAllProcessors(
                                    GameEventType.GameStateEvent, this,
                                    "CHANGE_STATE", "MAIN_MENU", ""));
                            break;
                        }
                    }
                }
            }

           
    }
}