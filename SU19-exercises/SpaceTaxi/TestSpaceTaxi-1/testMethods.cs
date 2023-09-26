using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxi_1.LevelParser;

namespace TestSpaceTaxi {
    public class testMethods {
        public static Dictionary<char, Image> KeyDictionary;
        public static EntityContainer LevelEntities;
        public static string[] KeyLegend;
        public char[,] Map2DArray;
        public string MapString;
        public string MetaString;

        public testMethods(string filename, ProcessLevel level) {
            MapString = testMethods.StringArrayToString(
                File.ReadAllLines(testMethods.GetLevelFilePath(filename)), 0, 22);
            Map2DArray = testMethods.StringTo2dArray(MapString);
            KeyLegend = File.ReadAllLines(testMethods.GetLevelFilePath(filename));
            testMethods.KeyDictionary = new Dictionary<char, Image>();
            testMethods.KeyLegendToDict(KeyLegend);
            testMethods.LevelEntities = new EntityContainer();
            testMethods.entityContainer(level.Map2DArray, level.KeyDictionary,
                testMethods.LevelEntities);
        }

        public static string GetLevelFilePath(string filename) {
            // Find base path.
            var dir = new DirectoryInfo(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location));

            while (dir.Name != "bin") {
                dir = dir.Parent;
            }

            dir = dir.Parent;

            // Find level file.
            var path = Path.Combine(dir.FullName, "Levels", filename);

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Error: The file \"{path}\" does not exist.");
            }

            return path;
        }


        public static string StringArrayToString(string[] stringArray, int lineStart, int lineEnd) {
            var finalString = "";
            for (var i = lineStart; i <= lineEnd; i++) {
                finalString += stringArray[i];
            }

            return finalString;
        }

        /// <summary>
        ///     Converts a string to a char 2D-Array.
        /// </summary>
        /// <param name="map"> The string converted in StringArrayToString</param>
        /// <returns> A char 2D-Array</returns>
        public static char[,] StringTo2dArray(string map) {
            var finalArray = new char[41, 23];
            for (var i = 0; i < map.Length; i++) {
                finalArray[i % 40, i / 40] = map[i];
            }

            return finalArray;
        }

        /// <summary>
        ///     Creates a dictionary that matches the appropriate key (character) with its string
        ///     (proper filename)
        /// </summary>
        /// <param name="stringArray"> The whole txt-file as a string array</param>
        public static Dictionary<char, Image> KeyLegendToDict(string[] stringArray) {
            Dictionary<char, Image> finalDict = new Dictionary<char, Image>();
            foreach (var line in stringArray) {
                if (line.Contains("png")) {
                    finalDict.Add(line[0], new Image(Path.Combine("Assets", "Images"
                        , line.Remove(0, 3))));
                }
            }

            return finalDict;
        }

        public static void entityContainer(char[,] array, Dictionary<char, Image> dict,
            EntityContainer entities) {
            var rowLength = array.GetLength(0);
            var numberOfRows = array.GetLength(1);
            for (var i = 0; i < rowLength; i++) {
                for (var j = 0; j < numberOfRows; j++) {
                    Vec2F position = new Vec2F(1.0f / rowLength * i,
                        -1f / numberOfRows * j + (numberOfRows - 1) / (float) numberOfRows);
                    Vec2F extend = new Vec2F
                        (1.0f / rowLength, 1.0f / numberOfRows);
                    // Checks each coordinate in the array to see if it is a space-symbol.         
                    if (array[i, j] != 32) {
                        // Checks that the key is contained in the dictionary                      
                        if (dict.ContainsKey(array[i, j])) {
                            var entityImage = dict[array[i, j]];
                            entities.AddStationaryEntity(new StationaryShape
                            (position, extend), entityImage);
                        }
                    }
                }
            }
        }
    }
}