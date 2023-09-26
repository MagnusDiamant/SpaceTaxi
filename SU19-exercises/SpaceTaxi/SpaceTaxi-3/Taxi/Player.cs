using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using SpaceTaxi_3.Entities;
using SpaceTaxi_3.States;

namespace SpaceTaxi_3.Taxi {
    public class Player : IGameEventProcessor<object> {
        private readonly DynamicShape shape;
        private readonly ImageStride taxiBoosterBackLeft;
        private readonly ImageStride taxiBoosterBackRight;
        private readonly ImageStride taxiBoosterBottomBackLeft;
        private readonly ImageStride taxiBoosterBottomBackRight;
        private readonly ImageStride taxiBoosterBottomLeft;

        private readonly ImageStride taxiBoosterBottomRight;

        // Fields for the taxi's images
        private readonly Image taxiBoosterNoneLeft;
        private readonly Image taxiBoosterNoneRight;

        // Field for explosions
        public Explosions ExplosionContainer;
        private float friction;

        private Vec2F gravity;

        // Has the taxi landed 
        private bool landed;

        // CustomerList for the customers that have been picked up
        public List<Customer> PickedUpCustomer;
        private Vec2F sideThruster;
        private Orientation taxiOrientation;

        private Vec2F upThruster;

        // Fields for the taxi's velocity, thrusters and friction
        public Vec2F VelocityX;
        public Vec2F VelocityY;

        public Player() {
            shape = new DynamicShape(new Vec2F(), new Vec2F());
            // Saving the different images for the taxi in different fields
            taxiBoosterNoneLeft =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
            taxiBoosterNoneRight =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));
            taxiBoosterBackLeft = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Back.png")));
            taxiBoosterBackRight = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Back_Right.png")));
            taxiBoosterBottomBackLeft = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back.png")));
            taxiBoosterBottomBackRight = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back_Right.png")));
            taxiBoosterBottomLeft = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom.png")));
            taxiBoosterBottomRight = new ImageStride(80, ImageStride.CreateStrides
                (2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Right.png")));
            Entity = new Entity(shape, taxiBoosterNoneLeft);
            // Instantiating the fields for velocity and thrusters as well as setting the friction
            // to 0.995f.
            VelocityY = new Vec2F(0, 0);
            VelocityX = new Vec2F(0, 0);
            gravity = new Vec2F(0f, -0.000001f);
            upThruster = new Vec2F(0, 0);
            sideThruster = new Vec2F(0, 0);
            friction = 0.995f;
            landed = false;

            // Explosions
            ExplosionContainer = new Explosions();

            // Instantiating customerList
            PickedUpCustomer = new List<Customer>();
        }

        public Entity Entity { get; }

        /// <summary>
        ///     Handles PlayerEvents received from the TaxiBus. Specifically movement-events.
        /// </summary>
        /// <param name="eventType">
        ///     A GameEventType, which is only relevant if it is
        ///     a PlayerEvent
        /// </param>
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
                    upThruster = new Vec2F(0, 0);
                    break;
                case "STOP_ACCELERATE_RIGHT":
                    sideThruster = new Vec2F(0, 0);
                    break;
                case "STOP_ACCELERATE_LEFT":
                    sideThruster = new Vec2F(0, 0);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sets the position of the taxi to the passed values x and y.
        /// </summary>
        /// <param name="x"> A float representing the horizontal position of the taxi</param>
        /// <param name="y">A float representing the vertical position of the taxi</param>
        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        /// <summary>
        ///     Sets the extent of the taxi to the passed values width and height
        /// </summary>
        /// <param name="width"> A float determining how wide the taxi should be</param>
        /// <param name="height"> A float determining how tall the taxi should be</param>
        public void SetExtent(float width, float height) {
            shape.Extent.X = width;
            shape.Extent.Y = height;
        }

        /// <summary>
        ///     Changes the image of the Player entity according to the players direction. For example,
        ///     if the if the orientation is left, and the sidethruster is turned on, but the
        ///     upthruster is not, the image will show a taxi with a thruster to its right
        /// </summary>
        public void RenderPlayer() {
            var vec = new Vec2F(0, 0);
            switch (taxiOrientation) {
            case Orientation.Left:
                if (sideThruster.X == vec.X && upThruster.Y == vec.Y) {
                    Entity.Image = taxiBoosterNoneLeft;
                } else if (sideThruster.X != vec.X && upThruster.Y == vec.Y) {
                    Entity.Image = taxiBoosterBackLeft;
                } else if (sideThruster.X != vec.X && upThruster.Y != vec.Y) {
                    Entity.Image = taxiBoosterBottomBackLeft;
                } else {
                    Entity.Image = taxiBoosterBottomLeft;
                }

                break;
            case Orientation.Right:
                if (sideThruster.X == vec.X && upThruster.Y == vec.Y) {
                    Entity.Image = taxiBoosterNoneRight;
                } else if (sideThruster.X != vec.X && upThruster.Y == vec.Y) {
                    Entity.Image = taxiBoosterBackRight;
                } else if (sideThruster.X != vec.X && upThruster.Y != vec.Y) {
                    Entity.Image = taxiBoosterBottomBackRight;
                } else {
                    Entity.Image = taxiBoosterBottomRight;
                }

                break;
            }

            Entity.RenderEntity();
        }

        /// <summary>
        ///     Changes the velocity of the taxi based on the thrusters and gravity. If the taxi has
        ///     landed on a surface, the movement (and gravity) is set to 0.
        /// </summary>
        public void Move() {
            if (!landed) {
                // This equation is the downforce from gravity subtracted from the force from the 
                // upthruster
                VelocityY = upThruster + gravity *
                            Game.GameTimer.CapturedUpdates + VelocityY;
                // This calculates the taxis speed moving sideways. We have added a variable called 
                // friciton, that slows down the taxi as the keys are released
                VelocityX = sideThruster * Game.GameTimer.CapturedUpdates + VelocityX * friction;
                Entity.Shape.Move(VelocityY);
                Entity.Shape.Move(VelocityX);

                if (PickedUpCustomer.Count != 0) {
                    PickedUpCustomer[0].Shape.Position = Entity.Shape.Position;
                }
            }
        }

        /// <summary>
        ///     Increases the amount of upthrust and makes sure that the taxi can fly again after
        ///     having landed
        /// </summary>
        private void MoveUp() {
            upThruster = new Vec2F(0f, 0.000002f) * Game.GameTimer.CapturedUpdates;
            landed = false;
        }

        /// <summary>
        ///     Increases the amount of sidethrust to the right and makes sure that the taxi can fly
        ///     again after having landed
        /// </summary>
        private void MoveRight() {
            if (!landed) {
                sideThruster = new Vec2F(0.000002f, 0f);
            }
        }

        /// <summary>
        ///     Increases the amount of sidethrust to the left and makes sure that the taxi can fly
        ///     again after having landed
        /// </summary>
        private void MoveLeft() {
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
        public void LevelCollision(LevelEntities entity) {
            // Checking if the player has collided with an obstacle.
            // If a collision has happened player is deleted and an explosion is
            // shown.
            Entity.Shape.AsDynamicShape().Direction = new Vec2F(VelocityX.X, VelocityY.Y);
            var colDetect = CollisionDetection.Aabb(Entity.Shape.AsDynamicShape(), entity.Shape);
            if (colDetect.Collision) {
                // If the player collides with a platform, the player is not deleted but its 
                // movement is simply stopped
                if (entity.IsPlatform && VelocityY.Y > -0.002) {
                    landed = true;
                    VelocityX = new Vec2F(0f, 0f);
                    VelocityY = new Vec2F(0f, 0f);
                    Entity.Shape.AsDynamicShape().Direction = new Vec2F(0f, 0f);
                    CustomerDelivered(entity);
                } else {
                    ExplosionContainer.AddExplosion(shape.Position.X, shape.Position.Y,
                        0.1f, 0.1f);
                    Entity.DeleteEntity();
                }
            }
        }

        /// <summary>
        /// Picks up a customer if it collides with the taxi. Also a few other conditions have to be
        /// met before a customer is picked up: the customer needs to be not already picked up, not
        /// already delivered and it needs to be rendered. Furthermore the taxi can not already have
        /// a customer inside.
        /// </summary>
        /// <param name="customer"> A customer which potentially collides with the taxi </param>
        public void CustomerCollision(Customer customer) {
            var tol = customer.Shape.Extent.X;
            // If the Taxi collides with a customer and if the Taxi has landed the customer is
            // picked up
            if (landed) {
                // Checks if taxi is within range of a customer
                if (Entity.Shape.Position.X - customer.Shape.Position.X < tol
                    && Entity.Shape.Position.X - customer.Shape.Position.X > -tol &&
                    !customer.PickedUp && !customer.IsDelivered && customer.IsRendered) {
                    if (PickedUpCustomer.Count == 0) {
                        customer.BeingPickedUp();
                        PickedUpCustomer.Add(customer);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the customer has been delivered in the right place at the right time. If so
        /// the player is awarded a certain amount of points.
        /// </summary>
        /// <param name="entity"> The platform in which the customer is dropped off </param>
        private void CustomerDelivered(LevelEntities entity) {
            // Checks if there is a customer in the taxi 
            if (PickedUpCustomer.Count > 0) {
                var customer = PickedUpCustomer[0];
                // If the first character in the string containing the destinationPlatform is '^'
                // and that is the only character and the level has been changed, the customer can 
                // be dropped off at any platform. 
                if (customer.DestinationPlatform.Contains("^")
                    && customer.DestinationPlatform.Length == 1 && customer.ChangedLevel) {
                    customer.IsDelivered = true;
                    customer.PickedUp = false;
                    // The customers position is set to be on top of the platform. 
                    customer.Shape.Position =
                        new Vec2F(entity.Shape.Position.X,
                            entity.Shape.Position.Y + customer.Shape.Extent.Y);
                    // If the player uses more time than is allotted by the customer, 1 point per 
                    // second more he uses is deducted from the total points awarded for the 
                    // delivery
                    if (customer.DropOffTime < customer.DropOffTimer.ElapsedMilliseconds / 1000) {
                        customer.Points += customer.DropOffTime -
                                           (int) customer.DropOffTimer.ElapsedMilliseconds / 1000;
                        // But the customer cannot take points from the taxi, only give less. 
                        if (customer.Points < 0) {
                            customer.Points = 0;
                        }
                    }

                    // Updates the total points after the delivery. 
                    GameRunning.GetInstance().Score.Points += customer.Points;
                }
                // If the destinationPlatform is a specific platform, the platform, on which the
                // taxi land is checked to see if it is the right platform. 
                else if (PickedUpCustomer.Exists(
                    x => x.DestinationPlatform.Contains(entity.Character.ToString()))) {
                    customer.IsDelivered = true;
                    customer.PickedUp = false;
                    customer.Shape.Position =
                        new Vec2F(entity.Shape.Position.X,
                            entity.Shape.Position.Y + customer.Shape.Extent.Y);
                    if (customer.DropOffTime < customer.DropOffTimer.ElapsedMilliseconds / 1000) {
                        customer.Points += customer.DropOffTime -
                                           (int) customer.DropOffTimer.ElapsedMilliseconds / 1000;
                        if (customer.Points < 0) {
                            customer.Points = 0;
                        }
                    }

                    GameRunning.GetInstance().Score.Points += customer.Points;
                }

                // The list of customers inside the taxi is "emptied". 
                PickedUpCustomer = new List<Customer>();
            }
        }

        /// <summary>
        ///     Changes the level of the game.
        /// </summary>
        /// <param name="level"> A string containing the file-name of the level</param>
        public void ChangeLevel(string level) {
            if (level == "the-beach.txt") {
                level = "short-n-sweet.txt";
            } else {
                level = "the-beach.txt";
            }

            // Sends a message to the TaxiBus to register an event, and further send a message 
            // to the eventprocessors to change the level. 
            TaxiBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent, this, "CHANGE_STATE",
                    "GAME_RUNNING", level));
            // Sets the players position and speed to an appropriate amount after the level
            // changes 
            SetPosition(0.5f, 0.85f);
            VelocityX = new Vec2F(0f, 0f);
            VelocityY = new Vec2F(0f, -0.001f);
            // If a customer is inside the taxi, and this method is called, the changeLevel field is 
            // changed to true. 
            if (PickedUpCustomer.Count > 0) {
                PickedUpCustomer[0].ChangedLevel = true;
            }
        }
    }
}