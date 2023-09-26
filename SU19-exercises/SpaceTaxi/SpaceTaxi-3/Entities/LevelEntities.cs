using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace SpaceTaxi_3.Entities {
    public class LevelEntities : Entity {
        public char Character;
        public IBaseImage Image;
        public bool IsPlatform;
        public Shape Shape;

        public LevelEntities(Shape shape, IBaseImage image, char character, bool isPlatform)
            : base(shape, image) {
            Shape = shape;
            Image = image;
            Character = character;
            IsPlatform = isPlatform;
        }
    }
}