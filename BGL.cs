using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        public Form frmIzbornik;
        private string igrac;
        public string level;

        public string Igrac
        {
            get { return igrac; }
            set
            {
                if (value == "")
                {
                    igrac = "Nepoznat";
                }
                else
                    igrac = value;
            }
        }

        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            this.frmIzbornik.Hide();
            START = true;
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL(int b)
        {
            brzina = b;
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */

        Sprite gun;
        Balon balon;
        Sprite target;
        Sprite p, p1, p2;
        Sprite voda;
        int bodovi = 0;
        int brzina;

        /* Initialization */

        List<Stickman> listaAktivnih;
        List<Stickman> listaAktivnih2;
        List<Stickman> listaCekanja;
        List<Stickman> listaCekanja2;
        DateTime pocetak = DateTime.Now;


        private void SetupGame()
        {
            RemoveSprites();
            //1. setup stage
            //SetStageTitle("PMF");
            SetStageTitle("Water war");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\bckg.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            balon = new Balon("sprites//balon.png");
            balon.SetSize(50);
            Game.AddSprite(balon);

            p = new Sprite("sprites//podloga.png", 0, 0);
            p.SetY(450);
            p.Width = 1067;
            Game.AddSprite(p);

            p1 = new Sprite("sprites//podloga.png", 0, 0);
            p1.SetY(300);
            p1.Width = 450;
            p1.SetVisible(false);
            Game.AddSprite(p1);

            p2 = new Sprite("sprites//podloga.png", 0, 0);
            p2.SetY(250);
            p2.SetX(650);
            p2.Width = 450;
            p2.SetVisible(false);
            Game.AddSprite(p2);

            voda =new Sprite("sprites//voda.png", 0, 0);
            voda.SetVisible(false);
            Game.AddSprite(voda);

            gun = new Sprite("sprites//gun.png", 0, 0);
            int dno = GameOptions.DownEdge - gun.Heigth;
            gun.SetX(GameOptions.RightEdge / 2 - gun.Width); //sredina
            gun.SetY(dno);
            Game.AddSprite(gun);

            target=new Sprite("sprites//target.png", 0, 0);
            Game.AddSprite(target);

            balon.oduzmi += _padBalona;            
            
            //3. scripts that start
            
            listaAktivnih = new List<Stickman>();
            listaAktivnih2 = new List<Stickman>();
            listaCekanja = new List<Stickman>();
            listaCekanja2 = new List<Stickman>();

            for (int i = 0; i < 20; i++)
            {
                Stickman novi = new Stickman("sprites//stickman.png", 0, 0, brzina);
                listaCekanja.Add(novi);
                Game.AddSprite(novi);
            }

            for (int i = 0; i < 20; i++)
            {
                Stickman novi = new Stickman("sprites//stickman2.png", 0, 0, brzina);
                listaCekanja2.Add(novi);
                Game.AddSprite(novi);
            }

            Game.StartScript(Gun);
            Game.StartScript(Target);
            Game.StartScript(Klik);
            Game.StartScript(Pucanje);
            Game.StartScript(Stickman);            
            Game.StartScript(Stickman2);
            Game.StartScript(Aktivacija);
            Game.StartScript(Aktivacija2);
            Game.StartScript(Balon);
           

            if (brzina == 7)
            {
                level = "Level 1";
            }
            else if (brzina == 11)
            {
                level = "Level 2";
            }
            else if (brzina == 18)
            {
                level = "Level 3";
            }
            else if (brzina == 22)
            {
                level = "Level 4";
            }
            
        }


        /* Scripts */

        private int Aktivacija()
        {
            Random g = new Random();

            while (START)
            {
                //pozicija
                int x = p.X;
                int y = p.Y - p.Heigth * 2;
                int[] y_poz = { p1.Y - p1.Heigth * 2, y };
                if (brzina == 7)
                {
                    if (listaCekanja.Count > 0)
                    {
                        Stickman s = listaCekanja[0];
                        s.GotoXY(x, y);
                        s.SetVisible(true);
                        listaCekanja.RemoveAt(0);
                        listaAktivnih.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
                else if (brzina == 11)
                {
                    p1.SetVisible(true);
                    if (listaCekanja.Count > 0)
                    {
                        Stickman s = listaCekanja[0];
                        s.GotoXY(p.X, y_poz[g.Next(y_poz.Length)]);
                        s.SetVisible(true);
                        listaCekanja.RemoveAt(0);
                        listaAktivnih.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
                else if (brzina == 18)
                {
                    p1.SetVisible(true);
                    if (listaCekanja.Count > 0)
                    {
                        Stickman s = listaCekanja[0];
                        s.GotoXY(p.X, y_poz[g.Next(y_poz.Length)]);
                        s.SetVisible(true);
                        listaCekanja.RemoveAt(0);
                        listaAktivnih.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
                else if (brzina == 22)
                {
                    p1.SetVisible(true);
                    p2.SetVisible(true);
                    if (listaCekanja.Count > 0)
                    {
                        Stickman s = listaCekanja[0];
                        s.GotoXY(p.X, y_poz[g.Next(y_poz.Length)]);
                        s.SetVisible(true);
                        listaCekanja.RemoveAt(0);
                        listaAktivnih.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
            }
            return 0;
        }

        private int Aktivacija2()
        {
            Random g = new Random();


            while (START)
            {
                //pozicija
                int x = 1000;
                int y = p.Y - p.Heigth * 2;
                int[] y_poz2 = { p2.Y - p2.Heigth * 2, y};
                if (brzina == 7)
                {
                    if (listaCekanja2.Count > 0)
                    {
                        Stickman s = listaCekanja2[0];
                        s.GotoXY(x, y);
                        s.SetVisible(true);
                        listaCekanja2.RemoveAt(0);
                        listaAktivnih2.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
                else if (brzina == 11)
                {
                    if (listaCekanja2.Count > 0)
                    {
                        Stickman s = listaCekanja2[0];
                        s.GotoXY(x, y);
                        s.SetVisible(true);
                        listaCekanja2.RemoveAt(0);
                        listaAktivnih2.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
                else if (brzina == 18)
                {
                    if (listaCekanja2.Count > 0)
                    {
                        Stickman s = listaCekanja2[0];
                        s.GotoXY(p.X + p.Width, y);
                        s.SetVisible(true);
                        listaCekanja2.RemoveAt(0);
                        listaAktivnih2.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }

                else if (brzina == 22)
                {
                    if (listaCekanja2.Count > 0)
                    {
                        Stickman s = listaCekanja2[0];
                        s.GotoXY(p.X + p.Width, y_poz2[g.Next(y_poz2.Length)]);
                        s.SetVisible(true);
                        listaCekanja2.RemoveAt(0);
                        listaAktivnih2.Add(s);
                    }
                    else
                        break;
                    int cekaj = g.Next(1, 3);
                    Wait(cekaj);
                }
            }
            return 0;
        }
        private void RemoveSprites()
        {
            //vrati brojač na 0
            BGL.spriteCount = 0;
            //izbriši sve spriteove
            BGL.allSprites.Clear();
            //počisti memoriju
            GC.Collect();
        }

        private void BGL_FormClosed(object sender, FormClosedEventArgs e)
        {
            START = false;
            Wait(0.1);
            this.frmIzbornik.Show();
        }

        private int Gun()
        {
            while (START)
            {
                gun.X = sensing.Mouse.X - gun.Width/2;
            }
            return 0;
        }

        private int Target()
        {
            while (START)
            {
                target.X = sensing.Mouse.X- target.Width/2;
                target.Y = sensing.Mouse.Y- target.Heigth/2;
            }
            return 0;
        }
        private int Voda()
        {
            voda.SetVisible(true);
            Wait(0.1);
            voda.SetVisible(false);
            return 0;
        }

        private int Balon()
        {
            balon.SetVisible(true);
            while (START)
            {
                balon.Y += 20;
                
                if (balon.Y > GameOptions.DownEdge)
                {
                    break;
                }
                else if (target.TouchingSprite(balon) && sensing.MouseDown)
                {
                    bodovi += 5;
                    balon.SetVisible(false);
                    break;
                }
                Wait(0.2);
            }
            return 0;
        }

        private void _padBalona()
        {
            bodovi -= 4;
            ISPIS = bodovi.ToString();
        }

        private int Stickman()
        {
            int x = p.X;
            int y = p.Y - p.Heigth * 2;

            if (brzina == 7)
            {
                for (int i = 0; i < listaAktivnih.Count; i++)
                {
                    Stickman s = listaAktivnih[i];
                    s.GotoXY(x, y);
                    s.SetVisible(true);
                }

                while (START)
                {
                    //System.Diagnostics.Debug.WriteLine("lista: " + listaAktivnih.Count);
                    for (int i = 0; i < listaAktivnih.Count; i++)
                    {
                        Stickman s = listaAktivnih[i];
                        s.KretanjeDesno();
                    }
                    
                    Wait(0.2);
                }
            }
            else if (brzina == 11)
            {
                for (int i = 0; i < listaAktivnih.Count; i++)
                {
                    Stickman s = listaAktivnih[i];
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih.Count; i++)
                    {
                        Stickman s = listaAktivnih[i];

                        if (s.X < p1.X + p1.Width)
                        {
                            s.KretanjeDesno();
                        }
                        if (s.X >= p1.X + p1.Width && s.X!=600 && s.Y!=1500)
                        {
                            s.GotoXY(s.X, p.Y - p.Heigth * 2);
                            s.KretanjeDesno();
                        }
                    }
                    Wait(0.2);
                }
            }
            else if (brzina == 18)
            {
                for (int i = 0; i < listaAktivnih.Count; i++)
                {
                    Stickman s = listaAktivnih[i];
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih.Count; i++)
                    {
                        Stickman s = listaAktivnih[i];

                        if (s.X < p1.X + p1.Width)
                        {
                            s.KretanjeDesno();
                        }
                        if (s.X >= p1.X + p1.Width && s.X != 600 && s.Y != 1500)
                        {
                            s.GotoXY(s.X, p.Y - p.Heigth * 2);
                            s.KretanjeDesno();
                        }
                    }
                    Wait(0.2);
                }
            }
            else if (brzina == 22)
            {
                for (int i = 0; i < listaAktivnih.Count; i++)
                {
                    Stickman s = listaAktivnih[i];
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih.Count; i++)
                    {
                        Stickman s = listaAktivnih[i];

                        if (s.X < p1.X + p1.Width)
                        {
                            s.KretanjeDesno();
                        }
                        if (s.X >= p1.X + p1.Width && s.X != 600 && s.Y != 1500)
                        {
                            s.GotoXY(s.X, p.Y - p.Heigth * 2);
                            s.KretanjeDesno();
                        }
                    }
                    Wait(0.1);
                }
            }
            return 0;
        }

        private int Stickman2()
        {
            int x = 1000;
            int y = p.Y - p.Heigth * 2;
            int[] y_poz = { p1.Y - p1.Heigth * 2, y};
            Random g = new Random();

            if (brzina == 7)
            {
                for (int i = 0; i < listaAktivnih2.Count; i++)
                {
                    Stickman s = listaAktivnih2[i];
                    s.GotoXY(x, y);
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih2.Count; i++)
                    {
                        Stickman s = listaAktivnih2[i];
                        s.KretanjeLijevo();
                    }
                    Wait(0.1);
                }
                
            }
            else if (brzina == 11)
            {
                for (int i = 0; i < listaAktivnih2.Count; i++)
                {
                    Stickman s = listaAktivnih2[i];
                    s.GotoXY(x, y);
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih2.Count; i++)
                    {
                        Stickman s = listaAktivnih2[i];
                        s.KretanjeLijevo();
                    }
                    Wait(0.1);
                }

            }
            else if (brzina == 18)
            {
                for (int i = 0; i < listaAktivnih2.Count; i++)
                {
                    Stickman s = listaAktivnih2[i];
                    s.GotoXY(x, y);
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih2.Count; i++)
                    {
                        Stickman s = listaAktivnih2[i];
                        s.KretanjeLijevo();
                    }
                    Wait(0.1);
                }

            }

            else if (brzina == 22)
            {
                for (int i = 0; i < listaAktivnih2.Count; i++)
                {
                    Stickman s = listaAktivnih2[i];
                    s.SetVisible(true);
                }

                while (START)
                {
                    for (int i = 0; i < listaAktivnih2.Count; i++)
                    {
                        Stickman s = listaAktivnih2[i];

                        if (s.X > GameOptions.RightEdge - p2.Width)
                        {
                            s.KretanjeLijevo();
                        }
                        else if (s.X <= GameOptions.RightEdge - p2.Width && s.X != 600 && s.Y != 1500)
                        {
                            s.GotoXY(s.X, y);
                            s.KretanjeLijevo();
                            if (s.X < p1.X + p1.Width && s.X != 600 && s.Y != 1500)
                            {
                                s.GotoXY(s.X, y_poz[g.Next(y_poz.Length)]); //skakanje
                                s.KretanjeLijevo();
                            } 
                        }
                    }
                    Wait(0.2);
                }
            }
            return 0;
        }

        private int Klik()
        {
            while (START)
            {
                if (sensing.MouseDown)
                    sensing.MouseDown = false;
                Wait(0.05);
            }
            return 0;
        }
        private int Pucanje()
        {
            while (START)
            {
                for (int i = 0; i < listaAktivnih.Count; i++)
                {
                    if (target.TouchingSprite(listaAktivnih[i]) && sensing.MouseDown)
                    {
                        int x = listaAktivnih[i].X;
                        int y = listaAktivnih[i].Y;
                        listaAktivnih[i].GotoXY(600, 1500);
                        voda.GotoXY(x,y);
                        Voda();
                        listaAktivnih[i].SetVisible(false);
                        bodovi += 1;                      
                        break;
                    }
                }
                for (int i = 0; i < listaAktivnih2.Count; i++)
                {
                    if (target.TouchingSprite(listaAktivnih2[i]) && sensing.MouseDown)
                    {
                        int x = listaAktivnih2[i].X;
                        int y = listaAktivnih2[i].Y;
                        listaAktivnih2[i].GotoXY(600, 1500);
                        voda.GotoXY(x,y);                
                        Voda();
                        listaAktivnih2[i].SetVisible(false);
                        bodovi += 1;
                        break;
                    }
                }
                ISPIS = bodovi.ToString();
                DateTime kraj = DateTime.Now;
                var sekunde = (kraj- pocetak).TotalSeconds;

                if (sekunde >= 30)
                {
                    START = false;
                    MessageBox.Show("Isteklo je vrijeme, imate " + bodovi.ToString() + " bodova");
                    using (StreamWriter sw = File.AppendText("dat.txt"))
                    {
                        sw.WriteLine(Igrac +" 30s --> " + bodovi.ToString() + " "+ level );
                    }
                    break;
                }


                if (bodovi == 40)
                {
                    START = false;
                    using (StreamWriter sw = File.AppendText("dat.txt"))
                    {
                        sw.WriteLine(Igrac +" "+ Math.Round(sekunde,0).ToString()+ "s --> " + bodovi.ToString()+" "+ level);
                    }
                    MessageBox.Show("Kraj, imate 40 bodova");
                    break;
                }
            }
            return 0;
        }


        /* ------------ GAME CODE END ------------ */


    }
}
