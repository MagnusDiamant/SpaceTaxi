using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi.Entities;
using SpaceTaxi.LevelParser;

namespace SpaceTaxiTests {
    public class DrawLevelTests {
        private char[,] array2;
        private char[,] array3;
        private DrawLevel drawLevel;
        private char[,] emptyArray;
        private GetLevel getLevel;
        private List<char> platFormList;
        private ProcessLevel processLevel;

        private char[,] shortArray;
        private char[,] spaceArray;
        private EntityContainer<LevelEntities> testContainer;
        private Dictionary<char, Image> testDict;

        [SetUp]
        public void TestSetup() {
            Window.CreateOpenGLContext();

            shortArray = new char[4, 1] {{'#'}, {'%'}, {'J'}, {'Y'}};
            array2 = new char[2, 1] {{'#'}, {'#'}};
            array3 = new char[2, 1] {{'J'}, {'J'}};
            spaceArray = new char[1, 1] {{' '}};
            emptyArray = new char[0, 0];
            testContainer = new EntityContainer<LevelEntities>();
            testDict = new Dictionary<char, Image>();
            testContainer.AddStationaryEntity(new LevelEntities(new StationaryShape(new Vec2F(0, 0),
                new Vec2F(0.5f, 1f)), new Image(Path.Combine("Assets", "Images",
                "ironstone-square.png")), '#', false));
            testContainer.AddStationaryEntity(new LevelEntities(new StationaryShape(new Vec2F(0, 0),
                new Vec2F(0.5f, 1f)), new Image(Path.Combine("Assets", "Images",
                "ironstone-square.png")), '%', false));
            testContainer.AddStationaryEntity(new LevelEntities(new StationaryShape(new Vec2F(0, 0),
                new Vec2F(0.5f, 1f)), new Image(Path.Combine("Assets", "Images",
                "ironstone-square.png")), 'J', true));
            testDict.Add('#', new Image(Path.Combine("Assets", "Images", "ironstone-square.png")));
            testDict.Add('%', new Image(Path.Combine("Assets", "Images", "ironstone-square.png")));
            testDict.Add('J', new Image(Path.Combine("Assets", "Images", "ironstone-square.png")));
            getLevel = new GetLevel("short-n-sweet.txt");
            processLevel = new ProcessLevel(getLevel);
            drawLevel = new DrawLevel(processLevel);
            platFormList = new List<char>();
            platFormList.Add('J');
        }


        /// <summary>
        ///     Tests that a LevelEntityContainer is created with the right amount of entities with a
        ///     call to CreateLevelEntityContainer
        /// </summary>
        [Test]
        public void TestCreateLevelEntityContainer1() {
            drawLevel.LevelEntities.ClearContainer();
            drawLevel.CreateLevelEntityContainer(shortArray, testDict, platFormList);
            Assert.AreEqual(3, drawLevel.LevelEntities.CountEntities());
        }

        /// <summary>
        ///     Tests that the levelEntities in the LevelEntityContainer have the correct boolean
        ///     isPlatform.
        /// </summary>
        [Test]
        public void TestCreateLevelEntityContainer2() {
            drawLevel.LevelEntities.ClearContainer();
            drawLevel.CreateLevelEntityContainer(array2, testDict, platFormList);
            foreach (LevelEntities levelEntity in drawLevel.LevelEntities) {
                Assert.AreEqual(false, levelEntity.IsPlatform);
            }
        }

        /// <summary>
        ///     Tests that the levelEntities in the LevelEntityContainer have the correct boolean
        ///     isPlatform.
        /// </summary>
        [Test]
        public void TestCreateLevelEntityContainer3() {
            drawLevel.LevelEntities.ClearContainer();
            drawLevel.CreateLevelEntityContainer(array3, testDict, platFormList);
            foreach (LevelEntities levelEntity in drawLevel.LevelEntities) {
                Assert.AreEqual(true, levelEntity.IsPlatform);
            }
        }

        /// <summary>
        ///     Tests that the method can differentiate between space and other characters
        /// </summary>
        [Test]
        public void TestCreateLevelEntityContainerSpace() {
            drawLevel.LevelEntities.ClearContainer();
            drawLevel.CreateLevelEntityContainer(spaceArray, testDict, platFormList);
            Assert.AreEqual(0, drawLevel.LevelEntities.CountEntities());
        }

        /// <summary>
        ///     Tests that the method can handle an empty array.
        /// </summary>
        [Test]
        public void TestCreateLevelEntityContainerEmpty() {
            drawLevel.LevelEntities.ClearContainer();
            drawLevel.CreateLevelEntityContainer(emptyArray, testDict, platFormList);
            Assert.AreEqual(0, drawLevel.LevelEntities.CountEntities());
        }
    }
}