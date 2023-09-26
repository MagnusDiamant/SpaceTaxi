using System.IO;
using System.Threading;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.Entities;
using SpaceTaxi.Taxi;

namespace SpaceTaxiTests {
    public class PlayerMovementTests {
        private Customer customer;
        private Game game;
        private GameTimer GameTimer;
        private GameEvent<object> MoveLeftEvent;
        private GameEvent<object> MoveRightEvent;
        private GameEvent<object> MoveUpEvent;
        private Player player;
        private GameEvent<object> StopLeftEvent;
        private GameEvent<object> StopRightEvent;
        private GameEvent<object> StopUpEvent;
        private double tol = 1E-8;

        [SetUp]
        public void InitiateGame() {
            GlobalSettings.DefaultFloatingPointTolerance = tol;
            game = new Game();
            player = new Player();
            MoveUpEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_UPWARDS", "", "");
            MoveRightEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_TO_RIGHT", "", "");
            MoveLeftEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_TO_LEFT", "", "");
            StopUpEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "STOP_ACCELERATE_UP", "", "");
            StopRightEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "STOP_ACCELERATE_RIGHT", "", "");
            StopLeftEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "STOP_ACCELERATE_LEFT", "", "");
            customer = new Customer("Cami", -1, '1',
                "^J", 10, 100, new StationaryShape(
                    new Vec2F(0.8f, 0.9f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png")));
            Thread.Sleep(1000);
        }

        /// <summary>
        ///     Tests that the players velocity upwards changes after player.ProcessEvent has processed
        ///     an event wit the message "BOOSTER_UPWARDS"
        /// </summary>
        [Test]
        public void TestProcessEventMoveUp() {
            player.PickedUpCustomer.Add(customer);
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveUpEvent);
            player.Move();
            Assert.AreEqual(0.000129f, player.VelocityY.Y);
        }

        /// <summary>
        ///     Tests that the players velocity sideways changes after player.ProcessEvent has processed
        ///     an event wit the message "BOOSTER_TO_RIGHT"
        /// </summary>
        [Test]
        public void TestProcessEventMoveRight() {
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveRightEvent);
            player.Move();
            Assert.AreNotEqual(0f, player.VelocityX.X);
        }

        /// <summary>
        ///     Tests that the players velocity sideways changes after player.ProcessEvent has processed
        ///     an event wit the message "BOOSTER_TO_LEFT"
        /// </summary>
        [Test]
        public void TestProcessEventMoveLeft() {
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveLeftEvent);
            player.Move();
            Assert.AreNotEqual(0f, player.VelocityX.X);
        }

        /// <summary>
        ///     Tests that the velocity upwards has stopped increasing (and been affected by the gravity)
        ///     after player.processEvent has processed an event with the message "STOP_ACCELERATE_UP"
        /// </summary>
        [Test]
        public void TestProcessEventStopUp() {
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveUpEvent);
            player.Move();
            player.ProcessEvent(GameEventType.PlayerEvent, StopUpEvent);
            player.Move();
            Assert.AreEqual(0.000128f, player.VelocityY.Y);
        }

        /// <summary>
        ///     Tests that the velocity sideways has stopped increasing (and been affected by the gravity)
        ///     after player.processEvent has processed an event with the message "STOP_ACCELERATE_RIGHT"
        /// </summary>
        [Test]
        public void TestProcessEventStopRight() {
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveRightEvent);
            player.Move();
            player.ProcessEvent(GameEventType.PlayerEvent, StopRightEvent);
            player.Move();
            Assert.AreEqual(0.000002f * 0.995f, player.VelocityX.X);
        }

        /// <summary>
        ///     Tests that the velocity sideways has stopped increasing (and been affected by the gravity)
        ///     after player.processEvent has processed an event with the message "STOP_ACCELERATE_LEFT"
        /// </summary>
        [Test]
        public void TestProcessEventStopLeft() {
            Game.GameTimer.MeasureTime();
            Game.GameTimer.ShouldUpdate();
            Game.GameTimer.ShouldReset();
            player.ProcessEvent(GameEventType.PlayerEvent, MoveLeftEvent);
            player.Move();
            player.ProcessEvent(GameEventType.PlayerEvent, StopLeftEvent);
            player.Move();
            Assert.AreEqual(-0.000002f * 0.995f, player.VelocityX.X);
        }
    }
}