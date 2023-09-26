using System.Collections.Generic;
using System.IO;
using DIKUArcade.Graphics;

namespace SpaceTaxi_3.LevelParser {
    public class ProcessLevel {
        public List<string> CustomerInfoList;
        public Dictionary<char, Image> KeyDictionary;
        public string[] KeyLegend;
        public char[,] Map2DArray;
        public string MapString;
        public string MetaString;
        public List<char> PlatformList;

        /// <summary>
        /// Constructor that saves the string from the file in a variable
        /// </summary>
        /// <param name="level"> An instance of GetLevel</param>
        public ProcessLevel(GetLevel level) {
            MapString = StringArrayToString(
                File.ReadAllLines(level.Path), 0, 22);
            Map2DArray = StringTo2DArray(MapString);
            KeyLegend = File.ReadAllLines(level.Path);
            KeyDictionary = new Dictionary<char, Image>();
            PlatformList = new List<char>();
            CustomerInfoList = new List<string>();
            KeyLegendToDict(KeyLegend);
            PlatformToList(KeyLegend);
            KeyLegendToCustomer(KeyLegend);
        }


        /// <summary>
        /// Converts a string array to a string
        /// </summary>
        /// <param name="stringArray">
        ///  Is a string array generated in the constructor using
        ///  System.ReadAllLines
        /// </param>
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
        ///     Converts a string to a char 2D-Array.
        /// </summary>
        /// <param name="map"> The string converted in StringArrayToString</param>
        /// <returns> A char 2D-Array</returns>
        private char[,] StringTo2DArray(string map) {
            var finalArray = new char[41, 23];
            for (var i = 0; i < map.Length; i++) {
                finalArray[i % 40, i / 40] = map[i];
            }

            return finalArray;
        }

        /// <summary>
        ///     Creates a dictionary that matches the appropriate key (character) with its image
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
        ///     Creates a list of the characters that symbolize platforms.
        /// </summary>
        /// <param name="stringArray">
        ///     The string array containing all the lines in the txt-
        ///     fil
        /// </param>
        private void PlatformToList(string[] stringArray) {
            foreach (var line in stringArray) {
                if (line.Contains("Platforms:")) {
                    var tempString = line.Substring(11);
                    foreach (var x in tempString) {
                        if (x != ',' && x != ' ') {
                            PlatformList.Add(x);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a list of the strings with information about the customers.
        /// </summary>
        /// <param name="stringArray">
        ///     The string array containing all the lines in the txt-
        ///     fil
        /// </param>
        private void KeyLegendToCustomer(string[] stringArray) {
            foreach (var line in stringArray) {
                if (line.Contains("Customer:")) {
                    CustomerInfoList.Add(line.Substring(10));
                }
            }
        }
    }
}