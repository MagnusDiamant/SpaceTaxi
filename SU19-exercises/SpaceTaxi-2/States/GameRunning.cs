using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.State;
using SpaceTaxi_2.LevelParser;

namespace SpaceTaxi_2.States {
    public class GameRunning : IGameState, IGameEventProcessor<object> {
        private static GameRunning instance = null;
        private GetLevel levelGetter;
        public ProcessLevel levelProcessor;
        private DrawLevel levelDrawer;
        public string currentLevel;
        
        // Creating an instance field player
        private readonly Player player;

        public GameRunning() {
            // Instantiating player as a new Player
            player = new Player();
            player.SetPosition(0.45f, 0.6f);
            player.SetExtent(0.05f, 0.05f);
            TaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, player);
            TaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            

        }

        /// <summary>
        /// Returns an instance of GameRunning (and instantiates one if it has not already been)
        /// </summary>
        /// <returns>An instance of GameRunning </returns>
        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }
        
        public void GameLoop() {
            
        }

        public void InitializeGameState() {
        }

        /// <summary>
        /// Updates game logic. The method is called in Game.gameLoop(). 
        /// </summary>
        public void UpdateGameLogic() {
            // Making the player move
            player.Move();
            // Detecting collision with obstacles 
            levelDrawer.LevelEntities.Iterate(player.Iterator);
            // Shows Game Over Screen if player has died
            if (player.Entity.IsDeleted()) {
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE",
                        "GAME_OVER", ""));
            }
            // Changes the level if the player flies through the portal 
            if (player.Entity.Shape.Position.Y > 1.0f) {
                if (currentLevel == "the-beach.txt") {
                    currentLevel = "short-n-sweet.txt";
                } else {
                    currentLevel = "the-beach.txt";
                }
                // Sends a message to the TaxiBus to register an event, and further send a message 
                // to the eventprocessors to change the level. 
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE",
                        "GAME_RUNNING", currentLevel));
                // Sets the players position and speed to an approriate amount after the level
                // changes 
                player.SetPosition(0.5f, 0.85f);
                player.velocityX = new Vec2F(0f, 0f);
                player.velocityY = new Vec2F(0f, -0.001f);
            }
        }

        /// <summary>
        /// Renders the different aspects of the game. For instance the player or the explosion when
        /// the player collides with an obstacle. Also renders the levels. 
        /// </summary>
        public void RenderState() {
            
            // Renders the explosions
            player.explosionContainer.explosions.RenderAnimations();
            
            // Renders the player
            if (player.Entity.IsDeleted()) {
                player.SetPosition(20f,20f);
            } else {
                player.RenderPlayer();
            }

            // Render level images
            levelDrawer.LevelEntities.RenderEntities();
            levelDrawer.PlatformEntities.RenderEntities();
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
                        GameEventType.PlayerEvent, this, "BOOSTER_UPWARDS", "", ""));
                break;
            case "KEY_LEFT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_LEFT", "", ""));
                break;
            case "KEY_RIGHT":
                TaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_RIGHT", "", ""));
                break;
            
            }
        }

        /// <summary>
        /// Making sure that when a key is released the event is registered to the TaxiBus - is to
        /// be used in HandleKeyEvent
        /// </summary>
        /// <param name="key"> A string with the name of the key that was released by the user
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
                KeyPress(keyValue);
            } else {
                KeyRelease(keyValue);
            }
        }

        /// <summary>
        /// Handles GameStateEvents received from the TaxiBus. Specifically level changing-events. 
        /// </summary>
        /// <param name="eventType"> A GameEventType, which is only relevant if it is
        /// a GameStateEvent</param>
        /// <param name="gameEvent"> A GameEvent containing a message with a string and potentially
        /// a string in the Parameter2-variable</param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            switch (eventType) {
                case GameEventType.GameStateEvent:
                    if (gameEvent.Parameter1 == "GAME_RUNNING") {
                        currentLevel = gameEvent.Parameter2;
                        // GetLevel
                        levelGetter = new GetLevel(currentLevel);

                        // ProcessLevel
                        levelProcessor = new ProcessLevel(levelGetter);

                        // DrawLevel
                        levelDrawer = new DrawLevel(levelProcessor);
                    }
                    break;
            }
        }
    }
        
}