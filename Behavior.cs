using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;


namespace BugCatching
{
    public class Behavior
    {
        public string Classification { get; set; } = "Flying";
        public int NumFrames { get; set; } = 3;

        public AnimatedSprite sprite = new AnimatedSprite();
        public Vector2 startingPosition;

        public Behavior() { }
        public Behavior(AnimatedSprite sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.startingPosition = position;
            //if (Classification == "Flying")
                

        }
        

        public class Flight : StardewValley.BellsAndWhistles.Butterfly
        {
            private bool summerButterfly;
            
            public Flight(Behavior behavior)
                :base(behavior.startingPosition)
            {
                this.sprite = behavior.sprite;
                this.position = behavior.startingPosition;
                this.startingPosition = this.position;
                this.baseFrame = this.sprite.currentFrame;
                if (behavior.NumFrames == 4)
                    this.summerButterfly = true;
            }


        }
    }
}
