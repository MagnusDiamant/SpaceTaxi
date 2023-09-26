using DIKUArcade.EventBus;

namespace SpaceTaxi_3 {
    public static class TaxiBus {
        private static GameEventBus<object> eventBus;


        /// <summary>
        /// A method that returns an eventBus if it has already instantiated, otherwise instantiates
        /// it and returns it. 
        /// </summary>
        /// <returns> An instance of the eventbus </returns>
        public static GameEventBus<object> GetBus() {
            return TaxiBus.eventBus ?? (TaxiBus.eventBus =
                       new GameEventBus<object>());
        }
    }
}