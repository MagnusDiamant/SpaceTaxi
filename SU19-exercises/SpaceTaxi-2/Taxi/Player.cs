using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using SpaceTaxi_2.LevelParser;
using SpaceTaxi_2.States;

namespace SpaceTaxi_2 {
    public class Player : IGameEventProcessor<object> {
        private readonly DynamicShape shape;
        // Fields for the taxi's images
        private readonly Image taxiBoosterNoneLeft;
        private readonly Image taxiBoosterNoneRight;
        private readonly ImageStride taxiBoosterBackLeft;
        private readonly ImageStride taxiBoosterBackRight;
        private readonly ImageStride taxiBoosterBottomBackLeft;
        private readonly ImageStride taxiBoosterBottomBackRight;
        private readonly ImageStride taxiBoosterBottomLeft;
        private readonly ImageStride taxiBoosterBottomRight;
        private Orientation taxiOrientation;
        // Fields for the taxi's velocity, thrusters and friction
        public Vec2F velocityX;
        public Vec2F velocityY;
        public Vec2F Gravity; 
        private Vec2F upThruster;
        private Vec2F sideThruster;
        private float friction;
        private bool landed;
        
        // Field for explosions
        public Explosions explosionContainer;

        public Player() {
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
            Gravity = new Vec2F(0f, -0.000001f);
            upThruster = new Vec2F(0,0);
            sideThruster = new Vec2F(0,0);
            friction = 0.995f;
            landed = false;
            
            // Explosions
            explosionContainer = new Explosions();
            
            
        }
        

        public Entity Entity { get; }

        /// <summary>
        /// Handles PlayerEvents received from the TaxiBus. Specifically movement-events. 
        /// </summary>
        /// <param name="eventType"> A GameEventType, which is only relevant if it is
        /// a PlayerEvent</param>
        /// <param name="gameEvent"> A GameEvent containing a message with a string</param>
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

        /// <summary>
        /// Sets the position of the taxi to the passed values x and y. 
        /// </summary>
        /// <param name="x"> A float representing the horizontal position of the taxi</param>
        /// <param name="y">A float representing the vertical position of the taxi</param>
        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        /// <summary>
        /// Sets the extent of the taxi to the passed values width and height 
        /// </summary>
        /// <param name="width"> A float determining how wide the taxi should be</param>
        /// <param name="height"> A float determining how tall the taxi should be</param>
        public void SetExtent(float width, float height) {
            shape.Extent.X = width;
            shape.Extent.Y = height;
        }

        /// <summary>
        /// Changes the image of the Player entity according to the players direction. For example,
        /// if the if the orientation is left, and the sidethruster is turned on, but the
        /// upthruster is not, the image will show a taxi with a thruster to its right
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
        /// Changes the velocity of the taxi based on the thrusters and gravity. If the taxi has
        /// landed on a surface, the movement (and gravity) is set to 0. 
        /// </summary>
        public void Move() {
            if (!landed) {
                // This equation is the downforce from gravity subtracted from the force from the 
                // upthruster
                velocityY = upThruster + Gravity  *
                            Game.gameTimer.CapturedUpdates  + velocityY;
                // This calculates the taxis speed moving sideways. We have added a variable called 
                // friciton, that slows down the taxi as the keys are released
                velocityX = sideThruster * Game.gameTimer.CapturedUpdates + velocityX * friction;
                Entity.Shape.Move(velocityY);
                Entity.Shape.Move(velocityX);
            }
        }

        /// <summary>
        /// Increases the amount of upthrust and makes sure that the taxi can fly again after
        /// having landed
        /// </summary>
        public void MoveUp() {
            upThruster = new Vec2F(0f,0.000002f) * Game.gameTimer.CapturedUpdates;
            landed = false;
        }

        /// <summary>
        /// Increases the amount of sidethrust to the right and makes sure that the taxi can fly 
        /// again after having landed
        /// </summary>
        public void MoveRight() {
            if (!landed) {
                sideThruster = new Vec2F(0.000002f, 0f);
            }
        }

        /// <summary>
        /// Increases the amount of sidethrust to the left and makes sure that the taxi can fly 
        /// again after having landed
        /// </summary>
        public void MoveLeft() {
            if (!landed) {
                sideThruster = new Vec2F(-0.000002f, 0f);
            }
        }


        /// <summary>
        /// Method Iterator iterates through an EntityContainer and deletes the relevant entities
        /// if there has been a collision. If the collision is with a platform-entity (Dynamicshape)
        /// the taxi will not explode if its speed is an approriate amount. It will simply land
        /// on the surface. 
        /// </summary>
        /// <param name="entity"> An entity from the entitycontainer</param>
        public void Iterator(Entity entity) {
            // Checking if the player has collided with an obstacle.
            // If a collision has happened player is deleted and an explosion is
            // shown.
            Entity.Shape.AsDynamicShape().Direction = new Vec2F(velocityX.X,velocityY.Y);
            var ColDetect = CollisionDetection.Aabb(Entity.Shape.AsDynamicShape(), entity.Shape);
            if (ColDetect.Collision) {
                // If the player collides with a platform, the player is not deleted but its 
                // movement is simply stopped
                if (entity.Shape.GetType() == typeof(DynamicShape) && velocityY.Y > -0.002) {
                    landed = true;
                        velocityX = new Vec2F(0f, 0f);
                        velocityY = new Vec2F(0f, 0f);
                        Entity.Shape.AsDynamicShape().Direction = new Vec2F(0f, 0f);
                } else {
                    explosionContainer.AddExplosion(shape.Position.X, shape.Position.Y,
                        0.1f, 0.1f);
                    Entity.DeleteEntity();
                }
            }
        }
    }
}
