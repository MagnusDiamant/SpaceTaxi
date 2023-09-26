using DIKUArcade.EventBus;

namespace SpaceTaxi_2 {
    public static class TaxiBus {
        private static GameEventBus<object> eventBus;
        

        // A method that returns an eventBus if it has already instantiated, otherwise 
        // instantiates it and returns it. 
        public static GameEventBus<object> GetBus() {
            return TaxiBus.eventBus ?? (TaxiBus.eventBus =
                       new GameEventBus<object>());
        }
    }
}
