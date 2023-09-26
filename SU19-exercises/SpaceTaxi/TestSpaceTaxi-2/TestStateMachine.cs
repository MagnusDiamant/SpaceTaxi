using System.Collections.Generic;
using DIKUArcade.EventBus;
using NUnit.Framework;
using SpaceTaxi_3;
using SpaceTaxi_3.States;

namespace TestSpaceTaxi2 {
    public class TestStateMachine {
        private StateMachine stateMachine;
        private GameEventBus<object> taxiBus;
        
        [SetUp]
        public void InitiateStateMachine() {
            DIKUArcade.Window.CreateOpenGLContext();
            // Initializes a GalagaBus 
            taxiBus = TaxiBus.GetBus();
            taxiBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window
                GameEventType.PlayerEvent, // Message from the player
                GameEventType.GameStateEvent // Message about the GameStateEvent
            });
            // 3.2.1 - Intantiates a StateMachine
            stateMachine = new StateMachine();
            // 3.2.1 - Subscribes said stateMachine to the galagaBus 
            taxiBus.Subscribe(GameEventType.GameStateEvent, stateMachine);
             
 
        }
         
        // Tests that the initial ActiveState is MainMenu
        [Test]
        public void TestInitialState() {
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }
         
        // Tests that the ActiveState can be changed from MainMenu to LevelState
        [Test]
        public void TestEventGamePaused() {
            taxiBus.RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "LEVEL_STATE", ""));
            taxiBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<LevelState>());
        }
    } 
}