using System.IO;
using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi_2;


namespace TestSpaceTaxi2 {
    public class TestMovement {
        private Game game;
        private TestMethods player;
        private GameEvent<object> MoveRightEvent;
        private GameEvent<object> MoveLeftEvent;
        private GameEvent<object> StopUpEvent;

        [SetUp]
        public void InitiateGame() {
            
            game = new Game();
            player = new TestMethods();
            MoveRightEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_TO_RIGHT", "", "");
            MoveLeftEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_TO_LEFT", "", "");
            StopUpEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "STOP_ACCELERATE_UP", "", "");
        }

        // Tests if the player image changes when the direction is changed 
        [Test]
        public void TestPlayerImage() {
            player.upThruster = new Vec2F(0f, 0.2f);
            player.RenderPlayer();
            Assert.AreSame(player.taxiBoosterBottomLeft, player.Entity.Image);
        }
        // Tests if the player image changes when the direction is changed 
        [Test]
        public void TestPlayerImage2() {
            player.upThruster = new Vec2F(0f, 0f);
            player.RenderPlayer();
            Assert.AreSame(player.taxiBoosterNoneLeft, player.Entity.Image);
        }
        // Tests if the player image changes when the direction is changed 
        [Test]
        public void TestPlayerImage3() {
            player.upThruster = new Vec2F(0f, 0.2f);
            player.sideThruster = new Vec2F(0.2f, 0f);
            player.RenderPlayer();
            Assert.AreSame(player.taxiBoosterBottomBackLeft, player.Entity.Image);
        }

        // Tests if the orientation of the taxi is changed after an event has been processed with 
        // the message to move in that direction 
        [Test]
        public void TestProcessEventRight() {
            player.ProcessEvent(GameEventType.PlayerEvent, MoveRightEvent);
            Assert.AreEqual(Orientation.Right, player.taxiOrientation);
        }
        // Tests if the orientation of the taxi is changed after an event has been processed with 
        // the message to move in that direction 
        [Test]
        public void TestProcessEventLeft() {
            player.taxiOrientation = Orientation.Right;
            player.ProcessEvent(GameEventType.PlayerEvent, MoveLeftEvent);
            Assert.AreEqual(Orientation.Left, player.taxiOrientation);
        }
        // Tests if the orientation of the taxi is changed after an event has been processed with 
        // the message to move in that direction 
        [Test]
        public void TestProcessEventStop() {
            player.upThruster = new Vec2F(0.0f, 0.4f);
            player.ProcessEvent(GameEventType.PlayerEvent, StopUpEvent);
            Assert.AreEqual(0f, player.upThruster.Y);
        }
    }
}

