using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Stickman: Sprite
    {
        private int brzina;

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        public Stickman(string path, int x, int y, int brz) : base(path, x, y)
        {
            this.Brzina = brz;
            this.SetVisible(false);
        }

        public void KretanjeDesno()
        {
            this.X += this.Brzina;
        }

        public void KretanjeLijevo()
        {
            this.X -= this.Brzina;
        }
    }
}
