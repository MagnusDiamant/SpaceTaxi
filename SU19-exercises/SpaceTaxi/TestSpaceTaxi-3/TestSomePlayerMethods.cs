using System;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using NUnit.Framework;
using SpaceTaxi_3;
using SpaceTaxi_3.Entities;
using SpaceTaxi_3.Taxi;

namespace TestSpaceTaxi_3 {
    public class TestSomePlayerMethods {
        public Player player;
        private EntityContainer<LevelEntities> levelEntities;
        private Customer customer;
        private string level;
        private GameEvent<object> MoveRightEvent;
        private Game game; 
        

        [SetUp]
        public void PlayerSetUp() {
            Window.CreateOpenGLContext();
            player = new Player();
            levelEntities = new EntityContainer<LevelEntities>();
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                    new Vec2F(0.8f,0.8f),new Vec2F(0.2f,0.1f)),
                new Image(Path.Combine("Assets","Images","neptune-square.png")),'1',
                true));
            levelEntities.AddStationaryEntity(new LevelEntities(new StationaryShape(
                    new Vec2F(0.3f,0.3f),new Vec2F(0.2f,0.1f)),
                new Image(Path.Combine("Assets","Images","neptune-square.png")),'l',
                false));
            customer = new Customer("Cami",-1,'1',
                "^J",10,100,new StationaryShape(
                    new Vec2F(0.5f,0.5f),new Vec2F(0.1f,0.1f)),
                new Image(Path.Combine("Assets", "Images","CustomerStandRight.png")));
            level = "the-beach.txt";
            MoveRightEvent = GameEventFactory<object>.CreateGameEventForAllProcessors(
                GameEventType.PlayerEvent, this,
                "BOOSTER_TO_RIGHT", "", "");
            game = new Game();
            
        }

        // When levelCollision is called the player is given a direction based on its velocity,
        // which is why we first set its direction before calling LevelCollision. Then we test 
        // if the direction has been changed. 
        [Test]
        public void TestLevelCollisionDirectionChange() {
            player.SetPosition(0.5f,0.5f);
            player.Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.1f,0.0f);
            player.VelocityX = new Vec2F(0.2f,0.0f);
            player.VelocityY = new Vec2F(0.0f,-0.001f);
            levelEntities.Iterate(player.LevelCollision);
            Assert.IsTrue( 0.2f == player.Entity.Shape.AsDynamicShape().Direction.X 
                           && -0.001f == player.Entity.Shape.AsDynamicShape().Direction.Y); 
        }

        // The collision detection seems to be failing in this test as it never enters the if-
        // statement, causing this test to fail. 
        [Test]
        public void TestLevelCollisionPlatformLanding() {
            player.SetPosition(0.8f,0.8f);
            player.VelocityX = new Vec2F(0.2f,0.0f);
            player.VelocityY = new Vec2F(0.0f,-0.001f);
            levelEntities.Iterate(player.LevelCollision);
            player.Move();
            Assert.IsTrue( 0.0f == player.Entity.Shape.AsDynamicShape().Direction.X 
                           && 0.0f == player.Entity.Shape.AsDynamicShape().Direction.Y);
        }

        // Cannot be tested because we can't access the bool landed, which is private. 
        [Test]
        public void TestCustomerCollision() {
            
        }

        // This test also fails, because the first if-statement in changeLevel is never entered. 
        // But the level is changed, when the game is played and the rest of the tests for this
        // method work, which is why we can't figure out why this test fails.
        [Test]
        public void TestChangeLevel() {
            player.ChangeLevel(level);
            Assert.AreEqual("short-n-sweet.txt",level);
        }

        // When the level is changed the player is given a position, so it won't die immediately 
        // after changing the level. This method tests if the player is given the correct position 
        // after a level change. 
        [Test]
        public void TestChangeLevelPlayerPosition() {
            player.ChangeLevel(level);
            Assert.IsTrue( 0.5f == player.Entity.Shape.Position.X 
                           && 0.85f == player.Entity.Shape.Position.Y);
        }

        // Just as the position is changed after a level change, so is the velocity. This tests the 
        // players velocity after a changing of the level. 
        [Test]
        public void TestChangeLevelPlayerVelocity() {
            player.ChangeLevel(level);
            Assert.IsTrue( 0.0f == player.VelocityX.X && -0.001f == player.VelocityY.Y);
        }

        [Test]
        public void TestSetExtent() {
            player.SetExtent(0.5f, 0.4f);
            Assert.AreEqual(0.5f, player.Entity.Shape.Extent.X);
            Assert.AreEqual(0.4f, player.Entity.Shape.Extent.Y);
        }
        [Test]
        public void TestSetPosition() {
            player.SetPosition(0.5f, 0.4f);
            Assert.AreEqual(0.5f, player.Entity.Shape.Position.X);
            Assert.AreEqual(0.4f, player.Entity.Shape.Position.Y);
        }
 
        [Test]
        public void TestMoveRight() {
            player.ProcessEvent(GameEventType.PlayerEvent, MoveRightEvent);
            Console.WriteLine(Game.GameTimer.CapturedUpdates);
            player.Move();
            Assert.AreNotEqual(0f, player.VelocityX.X);
        }
    }
}