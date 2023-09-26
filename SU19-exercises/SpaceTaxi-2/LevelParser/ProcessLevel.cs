using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DIKUArcade.Graphics;

namespace SpaceTaxi_2.LevelParser {
    public class ProcessLevel {
        public Dictionary<char, Image> KeyDictionary;
        public string[] KeyLegend;
        public char[,] Map2DArray;
        public string MapString;
        public string MetaString;
        public List<char> PlatformList;

        /// <summary>
        ///     Constructor that saves the string from the file in a variable
        /// </summary>
        /// <param name="filename"> The file used to make the string </param>
        public ProcessLevel(GetLevel level) {
            MapString = StringArrayToString(
                File.ReadAllLines(level.path), 0, 22);
            Map2DArray = StringTo2dArray(MapString);
            KeyLegend = File.ReadAllLines(level.path);
            KeyDictionary = new Dictionary<char, Image>();
            PlatformList = new List<char>();
            KeyLegendToDict(KeyLegend);
            PlatformToList(KeyLegend);
        }

        
        /// <summary>
        /// Converts a string array to a string
        /// </summary>
        /// <param name="stringArray"> Is a string array generated in the constructor using
        /// System.ReadAllLines </param>
        /// <param name="lineStart"> The line where the string should start</param>
        /// <param name="lineEnd"> The line where the string should end</param>
        /// <returns> A string </returns>
        private string StringArrayToString(string[] stringArray, int lineStart, int lineEnd) {
            var finalString = "";
            for (var i = lineStart; i <= lineEnd; i++) {
                finalString += stringArray[i];
            }

            return finalString;
        }

        /// <summary>
        /// Converts a string to a char 2D-Array.
        /// </summary>
        /// <param name="map"> The string converted in StringArrayToString</param>
        /// <returns> A char 2D-Array</returns>
        private char[,] StringTo2dArray(string map) {
            var finalArray = new char[41, 23];
            for (var i = 0; i < map.Length; i++) {
                finalArray[i % 40, i / 40] = map[i];
            }

            return finalArray;
        }

        /// <summary>
        /// Creates a dictionary that matches the appropriate key (character) with its image
        /// </summary>
        /// <param name="stringArray"> The whole txt-file as a string array</param>
        private void KeyLegendToDict(string[] stringArray) {
            foreach (var line in stringArray) {
                if (line.Contains("png")) {
                    KeyDictionary.Add(line[0], new Image(Path.Combine("Assets", "Images"
                        , line.Remove(0, 3))));
                }
            }
        }
        
        /// <summary>
        /// Creates a list of the characters that symbolize platforms. 
        /// </summary>
        /// <param name="stringArray"> The string array containing all the lines in the txt-
        /// fil </param>
        private void PlatformToList(string[] stringArray) {
            var tempString = "";
            foreach (var line in stringArray) {
                if (line.Contains("Platforms:")) {
                    tempString = line.Substring(11);
                    foreach (char x in tempString) {
                        if (x != ',' && x != ' ') {
                            PlatformList.Add(x);
                        }
                    }
                }
                
            }
        } 
    }
}