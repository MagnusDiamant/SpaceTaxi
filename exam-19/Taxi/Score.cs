using System.Drawing;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi.Taxi {
    public class Score {
        private Text score;

        public Score() {
            Points = 0;
            score = new Text("Score: " + Points, new Vec2F(0.75f, 0.7f),
                new Vec2F(0.2f, 0.2f));
            score.SetColor(Color.Red);
        }

        public int Points { get; set; }

        /// <summary>
        ///     Shows the total points-tally in the top-right corner, as well as updating the score.
        /// </summary>
        public void RenderScore() {
            score.SetText("Score: " + Points);
            score.RenderText();
        }
    }
}