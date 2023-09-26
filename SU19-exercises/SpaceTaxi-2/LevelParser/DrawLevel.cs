using System;
using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi_2.Entities;
using SpaceTaxi_2.States;

namespace SpaceTaxi_2.LevelParser {
    public class DrawLevel {
        public EntityContainer LevelEntities;
        private ProcessLevel Level;
        public EntityContainer PlatformEntities;
        public List<char> platformList;

        public DrawLevel(ProcessLevel level) {
            LevelEntities = new EntityContainer();
            PlatformEntities = new EntityContainer();
            entityContainer(level.Map2DArray, level.KeyDictionary);
            Level = level;
            platformList = new List<char>();

        }

        /// <summary>
        ///     Goes through the given array and uses the dictionary to create an entity with an image
        ///     that fits the key situated in the coordinate of the array. The entity is given an
        ///     appropriate position and extent that is calculated from the number of keys in the map.
        ///     Finally the entity is added to the entity container.
        /// </summary>
        /// <param name="array"> A 2d array containing the keys that map the level</param>
        /// <param name="dict"> The dictionary created from the KeyLegend in ProcessLevel.cs</param>
        /// <param name="level"> The level instantiated in Game.cs</param>
        public void entityContainer(Char[,] array, Dictionary<Char, Image> dict) {
            List<char> platformList = GameRunning.GetInstance().levelProcessor.PlatformList;
            var rowLength = array.GetLength(0) - 1;
            var numberOfRows = array.GetLength(1);
            for (var i = 0; i < rowLength; i++) {
                for (var j = 0; j < numberOfRows; j++) {
                    // Checks each coordinate in the array to see if it is a space-symbol. 
                    if (array[i, j] != 32) {
                        Vec2F position = new Vec2F(1.0f / rowLength * i,
                            -1f / numberOfRows * j + (numberOfRows - 1) / (float) numberOfRows);
                        Vec2F extend = new Vec2F(1.0f / rowLength, 1.0f / numberOfRows);
                        // Checks that the key is contained in the dictionary 
                        if (dict.ContainsKey(array[i, j])) {
                            var entityImage = dict[array[i, j]];
                            // If the key is contained in our list of characters symbolizing 
                            // platforms, the entity is added as a DynamicShape instead of a 
                            // stationary shape. This is used later on for collision detection. 
                            if (platformList.Contains(array[i, j])) {
                                LevelEntities.AddDynamicEntity(new DynamicShape(position,
                                    extend), entityImage);
                            } else {
                                LevelEntities.AddStationaryEntity(new StationaryShape(position,
                                    extend), entityImage);
                            }
                        }
                    }
                }
            }
        }
    }
}
