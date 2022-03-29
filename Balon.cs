using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Balon: Sprite
    {
        Random g = new Random();
        public Balon(string path) : base(path)
        {
            this.SetVisible(false);
            int b1 = g.Next(0, GameOptions.RightEdge);
            this.GotoXY(b1, 0);
        }
        public delegate void EventHandler();
        public event EventHandler oduzmi;
        public override int Y
        {
            get { return base.Y; }
            set
            {
                if (value > GameOptions.DownEdge)
                {
                    int b1 = g.Next(0, GameOptions.RightEdge);
                    this.GotoXY(b1, 0);
                    oduzmi.Invoke();
                }
                else
                    base.Y = value;
            }
        }
    }
}
