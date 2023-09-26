using System;

namespace SpaceTaxi.States {
    public class GameStateType {
        public enum GameStateTypes {
            GameRunning,
            GamePaused,
            MainMenu,
            GameOver,
            LevelState
        }

        
        // Takes a string and returns the appropriate state of the GameStateTypes. 
        // If the string doesn't fit any of the states an exception is thrown 
        public class StateTransformer {
            public static GameStateTypes TransformStringToState(string state) {
                switch (state) {
                case "GAME_RUNNING":
                    return GameStateTypes.GameRunning;
                case "GAME_PAUSED":
                    return GameStateTypes.GamePaused;
                case "MAIN_MENU":
                    return GameStateTypes.MainMenu;
                case "GAME_OVER":
                    return GameStateTypes.GameOver;
                case "LEVEL_STATE":
                    return GameStateTypes.LevelState;
                default:
                    throw new ArgumentException("Invalid argument - no match");
                }
            }
        }
    }
}