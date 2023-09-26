using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace SpaceTaxi_2 {
    public class Explosions {
        // Creating fields for explosions
        private readonly int explosionLength = 500;
        public AnimationContainer explosions;
        private readonly List<Image> explosionStrides;

        public Explosions() {
            // Added snippet of code from the assignment description
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(1);
        }
        
        /// <summary>
        /// AddExplosion adds an explosion animation to the animation container
        /// (added from the assignment description).
        /// </summary>
        /// <param name="posX"> The x-coordinate in the position of the explosion </param>
        /// <param name="posY"> The y-coordinate in the position of the explosion </param>
        /// <param name="extendX">The x-coordinate in the extent of the explosion </param>
        /// <param name="extendY"> The y-coordinate in the extent of the explosion </param>
        public void AddExplosion(float posX, float posY, float extendX, float extendY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extendX, extendY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
        }
    }
}