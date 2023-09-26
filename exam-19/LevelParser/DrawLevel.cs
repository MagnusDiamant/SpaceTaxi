using System;
using System.Collections.Generic;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi.Entities;

namespace SpaceTaxi.LevelParser {
    public class DrawLevel {
        private ProcessLevel Level;
        public EntityContainer<LevelEntities> LevelEntities;

        public DrawLevel(ProcessLevel level) {
            LevelEntities = new EntityContainer<LevelEntities>();
            CreateLevelEntityContainer(level.Map2DArray, level.KeyDictionary, level.PlatformList);
            Level = level;
        }

        /// <summary>
        ///     Goes through the given array and uses the dictionary to create an entity with an image
        ///     that fits the key situated in the coordinate of the array. The entity is given an
        ///     appropriate position and extent that is calculated from the number of keys in the map.
        ///     Finally the entity is added to the entity container. 
        /// </summary>
        /// <param name="array"> A 2d array containing the keys that map the level</param>
        /// <param name="dict"> The dictionary created from the KeyLegend in ProcessLevel.cs</param>
        /// <param name="platformList">A list containing all the characters symbolizing platforms</param>
        public void CreateLevelEntityContainer(Char[,] array, Dictionary<Char, Image> dict,
            List<char> platformList) {
            var rowLength = array.GetLength(0) - 1;
            var numberOfRows = array.GetLength(1);
            for (var i = 0; i < rowLength; i++) {
                for (var j = 0; j < numberOfRows; j++) {
                    // Checks each coordinate in the array to see if it is a space-symbol. 
                    if (array[i, j] != 32) {
                        var position = new Vec2F(1.0f / rowLength * i,
                            -1f / numberOfRows * j + (numberOfRows - 1) / (float) numberOfRows);
                        var extend = new Vec2F(1.0f / rowLength, 1.0f / numberOfRows);
                        // Checks that the key is contained in the dictionary 
                        if (dict.ContainsKey(array[i, j])) {
                            var entityImage = dict[array[i, j]];
                            // If the key is contained in our list of characters symbolizing 
                            // platforms, the entity is added with an isPlatform-bool = true.
                            // This is used later on for collision detection. 
                            if (platformList.Contains(array[i, j])) {
                                LevelEntities.AddStationaryEntity(new LevelEntities(
                                    new StationaryShape(
                                        position, extend), entityImage,
                                    array[i, j], true));
                            } else {
                                LevelEntities.AddStationaryEntity(new LevelEntities(
                                    new StationaryShape
                                        (position, extend), entityImage,
                                    array[i, j], false));
                            }
                        }
                    }
                }
            }
        }
    }
}