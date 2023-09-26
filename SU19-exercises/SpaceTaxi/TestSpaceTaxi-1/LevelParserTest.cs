using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;

namespace TestSpaceTaxi {
    [TestFixture]
    public class LevelParserTest {
        [SetUp]
        public void TestSetup() {
            Window.CreateOpenGLContext();
            testArray = new[] {
                new[] {"abc", "abc", "abc"},
                new[] {"/?#", "'*^", "€\n"},
                new[] {"æøå"}
            };
            longString = "oooooooooooooooooooooooooooooooooooooooooX";
            shortArray = new char[2, 1] {{'#'}, {'%'}};
            testContainer = new EntityContainer();
            generatedContainer = new EntityContainer();
            testDict = new Dictionary<char, Image>();
            testContainer.AddStationaryEntity(new StationaryShape(new Vec2F(0, 0),
                new Vec2F(0.5f, 1f)), new Image(Path.Combine("Assets", "Images",
                "ironstone-square.png")));
            testContainer.AddStationaryEntity(new StationaryShape(new Vec2F(0.5f, 0),
                new Vec2F(0.5f, 1f)), new Image(Path.Combine("Assets", "Images",
                "white-square.png")));
            testDict.Add('#', new Image(Path.Combine("Assets", "Images", "ironstone-square.png")));
            testDict.Add('%', new Image(Path.Combine("Assets", "Images", "ironstone-square.png")));
            KeyLegend = File.ReadAllLines(testMethods.
                GetLevelFilePath("short-n-sweet.txt"));
            testDictionary = new Dictionary<char, Image>();
            testDictionary = testMethods.KeyLegendToDict(KeyLegend);
        }

        private string[][] testArray;
        private string longString;
        private char[,] shortArray;
        private EntityContainer testContainer;
        private EntityContainer generatedContainer;
        private Dictionary<char, Image> testDict;
        private Dictionary<char, Image> testDictionary;
        private string[] KeyLegend;

        [Test]
        public void testEntityContainer() {
            testMethods.entityContainer(shortArray, testDict, generatedContainer);
            Assert.AreEqual(testContainer.CountEntities(),
                generatedContainer.CountEntities());
        }
        [Test]
        public void TestStringArrayToString1() {
            Assert.AreEqual("abcabcabc",
                testMethods.StringArrayToString(testArray[0], 0, 2));
        }

        [Test]
        public void TestStringArrayToString2() {
            Assert.AreEqual("/?#'*^€\n",
                testMethods.StringArrayToString(testArray[1], 0, 2));
        }

        [Test]
        public void TestStringArrayToString3() {
            Assert.AreEqual("æøå",
                testMethods.StringArrayToString(testArray[2], 0, 0));
        }

        [Test]
        public void TestStringTo2dArray1() {
            Assert.AreEqual('X', testMethods.StringTo2dArray(longString)[1, 1]);
        }

        [Test]
        public void TestDictionary() {
            foreach (KeyValuePair<char, Image> x in testDictionary) {
                Assert.NotNull(x.Value);
            }
        }
        [Test]
        public void TestDictionary1() {
            foreach (KeyValuePair<char, Image> x in testDictionary) {
                Assert.NotNull(x.Key);
            }
        }
        
    }
}