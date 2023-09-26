using DIKUArcade;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi.Taxi;

namespace SpaceTaxiTests {
    public class ScoreTests {
        private Score score;
        private Text text;

        [SetUp]
        public void SetUpScore() {
            Window.CreateOpenGLContext();
            score = new Score();
            text = new Text("Score: 1", new Vec2F(0.75f, 0.7f),
                new Vec2F(0.2f, 0.2f));
        }

        /// <summary>
        ///     Should test that string contained in score has been changed after points has been
        ///     updated, but this does not seem to work, because we cannot compare the text to the score
        /// </summary>
        [Test]
        public void TestPoints() {
            score.Points = 1;
            score.RenderScore();
            Assert.AreSame(text, score);
        }
    }
}