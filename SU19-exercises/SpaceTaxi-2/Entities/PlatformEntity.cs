using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace SpaceTaxi_2.Entities {
    public class PlatformEntity : Entity {
        public int platformNumber = 0;

        public PlatformEntity(Shape shape, IBaseImage image, int platform) : base(shape, image) {
            platformNumber = platform;
        }
    }
}