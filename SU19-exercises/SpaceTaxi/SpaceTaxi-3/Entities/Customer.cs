using System.Diagnostics;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace SpaceTaxi_3.Entities {
    public class Customer : Entity {
        public bool ChangedLevel;
        public string DestinationPlatform;
        public int DropOffTime;
        public Stopwatch DropOffTimer;
        private Image Image;
        public bool IsDelivered;
        public bool IsRendered;

        public string Name;
        public bool PickedUp;
        public int Points;
        private Shape Shape;
        public char SpawnPlatform;
        public int SpawnTime;
        private Stopwatch spawnTimer;


        public Customer(string name, int spawnTime, char spawnPlatform, string destinationPlatform,
            int dropOffTime, int points, Shape shape, Image image) : base(shape, image) {
            Name = name;
            SpawnTime = spawnTime;
            SpawnPlatform = spawnPlatform;
            DestinationPlatform = destinationPlatform;
            DropOffTime = dropOffTime;
            Points = points;
            spawnTimer = new Stopwatch();
            spawnTimer.Start();
            DropOffTimer = new Stopwatch();
            PickedUp = false;
            Image = image;
            Shape = shape;
            IsDelivered = false;
            IsRendered = false;
            ChangedLevel = false;
        }

        /// <summary>
        ///     Renders the customer if its spawntime has been elapsed, and if it has not been picked up
        /// </summary>
        public void RenderEntity() {
            if (SpawnTime < spawnTimer.ElapsedMilliseconds / 1000 && !PickedUp) {
                Image.Render(Shape);
                IsRendered = true;
            }
        }

        /// <summary>
        ///     Starts the timer determining how much time the taxi has to drop off the customer. Also
        ///     changes PickedUp to true.
        /// </summary>
        public void BeingPickedUp() {
            DropOffTimer.Start();
            PickedUp = true;
        }
    }
}