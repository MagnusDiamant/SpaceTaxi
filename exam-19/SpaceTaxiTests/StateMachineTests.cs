using System.Collections.Generic;
using DIKUArcade;
using DIKUArcade.EventBus;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.States;

namespace SpaceTaxiTests {
    public class StateMachineTests {
        private StateMachine stateMachine;
        private GameEventBus<object> taxiBus;

        [SetUp]
        public void InitiateStateMachine() {
            Window.CreateOpenGLContext();
            // Initializes a TaxiBus
            taxiBus = TaxiBus.GetBus();
            taxiBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window
                GameEventType.PlayerEvent, // Message from the player
                GameEventType.GameStateEvent // Message about the GameStateEvent
            });
            // Instantiates a StateMachine
            stateMachine = new StateMachine();
            // Subscribes said stateMachine to the TaxiBus 
            taxiBus.Subscribe(GameEventType.GameStateEvent, stateMachine);
        }

        /// <summary>
        ///     Tests that the initial ActiveState is MainMenu
        /// </summary>
        [Test]
        public void TestInitialState() {
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }

        /// <summary>
        ///     Tests that the ActiveState can be changed from MainMenu to LevelState
        /// </summary>
        [Test]
        public void TestEventLevelState() {
            taxiBus.RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "LEVEL_STATE", ""));
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<LevelState>());
        }

        /// <summary>
        ///     Tests that the MainMenu.HandleKeyEvent works and that StateMachine.ProcessEvent can
        ///     change the ActiveState from MainMenu to LevelState. Simultaneously tests that
        ///     GameStateType.TransformStringToState and StateMachine.SwitchState work.
        /// </summary>
        [Test]
        public void TestMainMenuHandleKeyEvent() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<LevelState>());
        }

        /// <summary>
        ///     Tests that the LevelState.HandleKeyEvent works and that StateMachine.ProcessEvent can
        ///     change the ActiveState from LevelState to GameRunning.
        /// </summary>
        [Test]
        public void TestLevelStateHandleKeyEvent() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GameRunning>());
        }

        /// <summary>
        ///     Tests that the LevelState.HandleKeyEvent works and that GameRunning is started with the
        ///     correct level.
        /// </summary>
        [Test]
        public void TestGameRunningCorrectLevel() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.AreEqual("short-n-sweet.txt", GameRunning.GetInstance().CurrentLevel);
        }

        /// <summary>
        ///     Tests that the GameRunning.HandleKeyEvent works and that StateMachine.ProcessEvent can
        ///     change the ActiveState from GameRunning to GamePaused.
        /// </summary>
        [Test]
        public void TestGameRunningHandleKeyEvent() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ESCAPE", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GamePaused>());
        }

        /// <summary>
        ///     Tests that the GamePaused.HandleKeyEvent works and that StateMachine.ProcessEvent can
        ///     change the ActiveState from GamePaused to GameRunning.
        /// </summary>
        [Test]
        public void TestGamePausedHandleKeyEvent() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ESCAPE", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GameRunning>());
        }

        /// <summary>
        ///     Tests that the GamePaused.HandleKeyEvent works and that StateMachine.ProcessEvent can
        ///     change the ActiveState from GamePaused to MainMenu.
        /// </summary>
        [Test]
        public void TestGamePausedToMainMenu() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ESCAPE", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_DOWN", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }

        /// <summary>
        ///     Tests that the LevelState.HandleKeyEvent works and that GameRunning is started with the
        ///     correct level.
        /// </summary>
        [Test]
        public void TestGameRunningLevel2() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_DOWN", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.AreEqual("the-beach.txt", GameRunning.GetInstance().CurrentLevel);
        }

        /// <summary>
        ///     Tests that you can go back from LevelState to MainMenu
        /// </summary>
        [Test]
        public void TestLevelStateToMainMenu() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_DOWN", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_DOWN", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }

        /// <summary>
        ///     Tests that you can press up and down in MainMenu and still proceed to LevelState
        /// </summary>
        [Test]
        public void TestMainMenuUpDown() {
            stateMachine.ActiveState.HandleKeyEvent("KEY_DOWN", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_UP", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            stateMachine.ActiveState.HandleKeyEvent("KEY_ENTER", "KEY_PRESS");
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<LevelState>());
        }
    }
}