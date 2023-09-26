using System;

namespace SpaceTaxi_3.States {
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

            /// <summary>
            /// Takes a state and returns the state as a string. Throws an exception if the
            /// state is invalid.
            /// </summary>
            /// <param name="state">A GameStateType</param>
            /// <returns> Returns state as a string </returns>
            /// <exception cref="ArgumentException">If the state is not one of the states
            /// contained in the enum GameStateTypes an exception is thrown</exception>
            public static string TransformStateToString(GameStateTypes state) {
                switch (state) {
                case GameStateTypes.GameRunning:
                    return "GAME_RUNNING";
                case GameStateTypes.GamePaused:
                    return "GAME_PAUSED";
                case GameStateTypes.MainMenu:
                    return "MAIN_MENU";
                case GameStateTypes.GameOver:
                    return "GAME_OVER";
                case GameStateTypes.LevelState:
                    return "LEVEL_STATE";
                default:
                    throw new ArgumentException("Invalid argument - no match");
                }
            }
        }
    }
}