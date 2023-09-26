using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using SpaceTaxi_2;

namespace TestSpaceTaxi2 {
    public class TestMethods {
        public readonly DynamicShape shape;
        // Fields for the taxi's images
        public Image taxiBoosterNoneLeft;
        public Image taxiBoosterNoneRight;
        public ImageStride taxiBoosterBackLeft;
        public ImageStride taxiBoosterBackRight;
        public ImageStride taxiBoosterBottomBackLeft;
        public ImageStride taxiBoosterBottomBackRight;
        public ImageStride taxiBoosterBottomLeft;
        public ImageStride taxiBoosterBottomRight;
        public Orientation taxiOrientation;
        // Fields for the taxi's velocity, thrusters and friction
        public Vec2F velocityX;
        public Vec2F velocityY;
        public Vec2F upThruster;
        public Vec2F sideThruster;
        public float friction;
        
        // Field for explosions
        public Explosions explosionContainer;

        public TestMethods() {
            shape = new DynamicShape(new Vec2F(), new Vec2F());
            // Saving the different images for the taxi in different fields
            taxiBoosterNoneLeft =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
            taxiBoosterNoneRight =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));
            taxiBoosterBackLeft = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Back.png")));
            taxiBoosterBackRight = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Back_Right.png")));
            taxiBoosterBottomBackLeft = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Bottom_Back.png")));
            taxiBoosterBottomBackRight = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Bottom_Back_Right.png")));
            taxiBoosterBottomLeft = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Bottom.png")));
            taxiBoosterBottomRight = new ImageStride(80,ImageStride.CreateStrides
                (2,Path.Combine("Assets","Images","Taxi_Thrust_Bottom_Right.png")));
            
            Entity = new Entity(shape, taxiBoosterNoneLeft);
            // Instantiating the fields for velocity and thrusters as well as setting the friction
            // to 0.995f.
            velocityY = new Vec2F(0,0);
            velocityX = new Vec2F(0,0);
            upThruster = new Vec2F(0,0);
            sideThruster = new Vec2F(0,0);
            friction = 0.995f;
            
            // Explosions
            explosionContainer = new Explosions();
        }
        

        public Entity Entity { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="gameEvent"></param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message) {
                case "BOOSTER_UPWARDS": 
                    MoveUp();
                    break;
                case "BOOSTER_TO_RIGHT":
                    taxiOrientation = Orientation.Right;
                    MoveRight();
                    break;
                case "BOOSTER_TO_LEFT":
                    taxiOrientation = Orientation.Left;
                    MoveLeft();
                    break;
                case "STOP_ACCELERATE_UP":
                    upThruster = new Vec2F(0,0);
                    break;
                case "STOP_ACCELERATE_RIGHT":
                    sideThruster = new Vec2F(0,0);
                    break;
                case "STOP_ACCELERATE_LEFT":
                    sideThruster = new Vec2F(0,0);
                    break;
                }
            }
        }

        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        public void SetExtent(float width, float height) {
            shape.Extent.X = width;
            shape.Extent.Y = height;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RenderPlayer() {
            Vec2F vec = new Vec2F(0, 0);
            switch (taxiOrientation) {
                case Orientation.Left:
                    if (sideThruster.X == vec.X && upThruster.Y == vec.Y) {
                        Entity.Image = taxiBoosterNoneLeft;
                        
                    }
                    else if (sideThruster.X != vec.X && upThruster.Y == vec.Y) {
                        Entity.Image = taxiBoosterBackLeft;
                    } else if (sideThruster.X != vec.X && upThruster.Y != vec.Y){
                        Entity.Image = taxiBoosterBottomBackLeft;
                    } else {
                        Entity.Image = taxiBoosterBottomLeft;
                    }
                    break;
                case Orientation.Right:
                    if (sideThruster.X == vec.X && upThruster.Y == vec.Y) {
                        Entity.Image = taxiBoosterNoneRight;
                    }
                    else if (sideThruster.X != vec.X && upThruster.Y == vec.Y) {
                        Entity.Image = taxiBoosterBackRight;
                    } else if (sideThruster.X != vec.X && upThruster.Y != vec.Y){
                        Entity.Image = taxiBoosterBottomBackRight;
                    } else {
                        Entity.Image = taxiBoosterBottomRight;
                    }
                    break;
            }
            Entity.RenderEntity();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Move() {
            velocityY = upThruster + (new Vec2F(0f, -0.000001f)) * 
                        Game.gameTimer.CapturedUpdates + velocityY;
            velocityX = sideThruster * Game.gameTimer.CapturedUpdates + velocityX * friction;
            Entity.Shape.Move(velocityY);
            Entity.Shape.Move(velocityX);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveUp() {
            upThruster = new Vec2F(0f,0.000002f) * Game.gameTimer.CapturedUpdates;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void MoveRight() {
            sideThruster = new Vec2F(0.000002f,0f);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void MoveLeft() {
            sideThruster = new Vec2F(-0.000002f,0f);
        }
        
        
        // Method Iterator iterates through an EntityContainer and deletes the relevant entities
        // if there has been a collision.
        public void Iterator(Entity entity) {
            // Checking if the player has collided with an obstacle.
            // If a collision has happened player is deleted and an explosion is
            // shown.
            Entity.Shape.AsDynamicShape().Direction = new Vec2F(velocityX.X,velocityY.Y);
            if (CollisionDetection.Aabb(Entity.Shape.AsDynamicShape(),entity.Shape)
                .Collision) {
                explosionContainer.AddExplosion(shape.Position.X, shape.Position.Y,
                    0.1f, 0.1f);
                Entity.DeleteEntity();
            }
        }
    }
}