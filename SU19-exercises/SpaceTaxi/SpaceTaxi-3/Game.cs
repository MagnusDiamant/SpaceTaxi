using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi_3.States;

namespace SpaceTaxi_3 {
    public class Game : IGameEventProcessor<object> {
        public static GameTimer GameTimer;
        private Entity backGroundImage;
        private GameEventBus<object> eventBus;
        private StateMachine stateMachine;
        private Window win;

        public Game() {
            // window
            win = new Window("Space Taxi Game v0.1", 500, AspectRatio.R1X1);

            // event bus
            eventBus = TaxiBus.GetBus();
            eventBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window, e.g. CloseWindow()
                GameEventType.PlayerEvent, // commands issued to the player object, e.g. move,
                // destroy, receive health, etc.
                GameEventType.GameStateEvent
            });
            win.RegisterEventBus(eventBus);


            // game timer
            Game.GameTimer = new GameTimer(60); // 60 UPS, no FPS limit

            // game assets
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f), new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png"))
            );
            backGroundImage.RenderEntity();

            //game entities

            // event delegation
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);

            // Instantiating the stateMachine
            stateMachine = new StateMachine();
        }

        /// <summary>
        ///     A method that processes events received from the taxiBus.
        /// </summary>
        /// <param name="eventType">
        ///     A GameEventType, that is only relevant for this method if
        ///     it is a WindowEvent or an InputEvent
        /// </param>
        /// <param name="gameEvent">
        ///     A GameEvent containing a message and potentially a parameter
        /// </param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                }
            } else if (eventType == GameEventType.InputEvent) {
                KeyPress(gameEvent.Message);
            }
        }


        public void GameLoop() {
            while (win.IsRunning()) {
                Game.GameTimer.MeasureTime();

                while (Game.GameTimer.ShouldUpdate()) {
                    win.PollEvents();
                    eventBus.ProcessEvents();
                    stateMachine.ActiveState.UpdateGameLogic();
                }

                if (Game.GameTimer.ShouldRender()) {
                    win.Clear();
                    backGroundImage.RenderEntity();
                    stateMachine.ActiveState.RenderState();


                    win.SwapBuffers();
                }

                if (Game.GameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps from the timer
                    win.Title = "Space Taxi | UPS: " + Game.GameTimer.CapturedUpdates + ", FPS: " +
                                Game.GameTimer.CapturedFrames;
                }
            }
        }

        /// <summary>
        ///     A method that allows the user to save a screenshot of the game, provided the user
        ///     presses F12
        /// </summary>
        /// <param name="key"> A string with the name of the key that was pressed by the user</param>
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_F12":
                Console.WriteLine("Saving screenshot");
                win.SaveScreenShot();
                break;
            }
        }
    }
}