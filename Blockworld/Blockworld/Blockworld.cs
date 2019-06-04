using openview;
using Screen;
using Microsoft.VisualBasic;
using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
//using System.Net;
using System.Diagnostics;
using Generating;
using System.Text;
using AviFile;
using Generating.Consts;
namespace BlockWorld
{
    class User
    {
        Gui.Object prompt = new Gui.Object { Left = 20, Width = 200, Height = 50, backcolor = Color.FromArgb(150, Color.Black), font = new Font("Tahoma", 8), textcolor = Brushes.White };
        void Init()
        {
            //Plaatje.Width = CurrentScreen.view.Width;
            CurrentScreen.view.changeresolution = true;
            CurrentScreen.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
            CurrentScreen.KeyUp += new KeyEventHandler(TextBox1_KeyUp);
            CurrentScreen.MouseDown += new MouseEventHandler(PictureBox1_MouseDown);
            CurrentScreen.MouseMove += new MouseEventHandler(Beweeg_muis);
            CurrentScreen.MouseEnter += new System.EventHandler(Muis_op);
            CurrentScreen.MouseLeave += new System.EventHandler(Muis_af);
            CurrentScreen.FormClosing += new FormClosingEventHandler(Rondlopen_Closing);
            prompt.Top = CurrentScreen.Height - prompt.Height - prompt.Left;
            //Newworld.objects = Gui.Object.Combine(make);
            //CurrentScreen.view.objects = Gui.Object.Combine(Newworld, daynight, prompt);
            CurrentScreen.view.objects = Gui.Object.Combine(prompt);
            //daynight.Clickormove += new Gui.Clickeventargs(ToolStripButton4_Click);
            CurrentScreen.view.Hideall();
            CurrentScreen.view.Backimage = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "other\\huislicht.bmp");
        }
        int Filmwidth, Filmheight;
        string SaveDirectory, TextureDirectory;
        private int ViewWidth = 100, Mist = 100;
        private openview.View curview = new openview.View();
        private openview.View.Light FlashLight = new openview.View.Light { straal = 20 };
        private Viewscreen CurrentScreen = new Viewscreen();
        private double TimeShift;
        private int filewidth = 100, FrameRate = 10;
        private bool Visor = false, Showlines = false, Gravity = false;
        public User(string Directory, string Texturedirectory)
        {
            Init();
            trees = Tree.Totrees(sugarspin, acacia, cactusplant, bloem, coral, jungletree, bush, den, braambos);
            Map_laden();
            autovg = true;
            lb = 1000;
            lh = 300;
            lbe = lb - 1;
            lhe = lh - 1;
            lb2 = lb * lb;
            l[1] = l[2] = lb;
            l[3] = lh;
            position[3] = lh;
            position[1] = position[2] = lb / 2;
            if (curview.Block.Block.Length == 1)
            {
                curview.Rescale(lb, lb, lh);
            }
            schaal[1] = 100; schaal[2] = 50;
            WW = "WereldWijd";
            kijkwijdte = 100;
            pi180 = Math.PI / 180;
            zfactor = 1;
            nietgeplaatst = new bool[lb, lb];
            nietgemaakt = new bool[lb, lb];
            SaveDirectory = Directory;
            TextureDirectory = Texturedirectory;
            Load_colors();
            curview.Rescale(ViewWidth, ViewWidth, 300);
            curview.Addlight(FlashLight);

        }
        void Load_Terrain(double x, double y)
        {
            for (int px = (int)(position[1] - ViewWidth) / filewidth; px <= (position[1] + ViewWidth); px++)
            {

            }
        }
        string[] commands;
        public bool Executecmd(string cmd)
        {
            try
            {
                commands = Strings.Split(cmd);
                switch (commands[0])
                {
                    case "TRANSLATE":
                        position[1] = Convert.ToDouble(commands[1]);
                        position[2] = Convert.ToDouble(commands[2]);
                        position[3] = Convert.ToDouble(commands[3]);
                        break;
                    case "FLASHLIGHT":
                        FlashLightOn = Convert.ToBoolean(commands[1]);
                        break;
                    case "VGI":
                        Vgi(Convert.ToDouble(commands[1]));
                        break;
                    case "GRAVITY":
                        Gravity = Convert.ToBoolean(commands[1]);
                        break;
                    case "LINE":
                        lijn = Convert.ToBoolean(commands[1]);
                        break;
                    case "VISOR":
                        Visor = Convert.ToBoolean(commands[1]);
                        break;
                    case "RECORD":
                        Opnemen();
                        break;
                    case "WATCH":
                        Film_kijken();
                        break;
                    case "SETHOUR":
                        TimeShift = (((((Convert.ToDouble(commands[1]) / 24) * 1800) + 1800) - (DateAndTime.Timer % 1800)) % 1800);
                        break;
                    case "VIEWWIDTH":
                        ViewWidth = Convert.ToInt32(commands[1]);
                        break;
                    case "FPS":
                        FrameRate = Convert.ToInt32(commands[1]);
                        break;
                    case "MAP":
                        CurrentScreen.view.Backimage = curview.Getmap();
                        CurrentScreen.Dothings();
                        double sec = DateAndTime.Timer + 2;
                        do { } while (DateAndTime.Timer < sec);
                        break;
                    case "MIST":
                        Mist = Convert.ToInt32(commands[1]);
                        break;
                    case "OPEN":
                        //Openen();
                        break;
                }
                return true;
            }
            catch { return false; }//MessageBox.Show("this was an command, but it caused an error"); 
        }
        public void Run()
        {
            CurrentScreen.Show();
            do
            {
                do { CurrentScreen.Dothings(); if (!CurrentScreen.Visible) { return; } } while (!herhalen);
                Indrukken();

            } while (CurrentScreen.Visible);
        }

        AviManager am;
        VideoStream video;
        //Noise.Blocknoise heightmap;
        //Noise.Blocknoise temperaturemap;
        Perlin.SimplexNoise heightmap, temperaturemap, biomemap,hillmap,treemap;
        Bitmap[] v = new Bitmap[5];
        Tree jungletree = new Tree { Name = "JUNGLETREE", maxsplits = 3, branches = new Branch { branchradius = 1, Lengthfactor = 0.5, Maxbranches = 5, leaveradius = 0, growtoground = new Dbetween { Minimum = 0.4, Maximum = 0.6 }, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 0.8, height = 3, width = 3 }, Minlength = 3, Maxlength = 5, Minbranches = 2 }, roots = new Branch { branchradius = 3, Lengthfactor = 0.5, leaveradius = 1, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 0, height = 1, width = 1 }, Minlength = 1, Maxlength = 2, Minbranches = 1, Maxbranches = 3, growtoground = new Dbetween { Minimum = 0.1, Maximum = 0.3 } }, biome = Generating.Consts.Biomes.TropicRainforest };
        Tree sugarspin = new Tree { Name = "SUGARSPIN", maxsplits = 3, branches = new Branch { branchradius = 1, Lengthfactor = 0.75, Maxbranches = 3, leaveradius = 0, growtoground = new Dbetween { Minimum = 0.6, Maximum = 0.7 }, leaves = new Leave { fill = Blockfill.acacaleavesorange, chance = 0.85, height = 2, width = 2 }, Minlength = 4, Maxlength = 6, Minbranches = 2 }, roots = new Branch { branchradius = 3, Lengthfactor = 0.5, leaveradius = 1, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 0, height = 1, width = 1 }, Minlength = 1, Maxlength = 2, Minbranches = 2, Maxbranches = 3, growtoground = new Dbetween { Minimum = 0.1, Maximum = 0.3 } }, biome = Biomes.Taiga };
        Tree acacia = new Tree { Name = "ACACIA", maxsplits = 3, branches = new Branch { branchradius = 1, Lengthfactor = 0.75, Maxbranches = 2, leaveradius = 0, growtoground = new Dbetween { Minimum = 0.6, Maximum = 1 }, togroundfactor = 0.8, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 1, height = 2, width = 3 }, Minlength = 2, Maxlength = 30, Minbranches = 1 }, roots = new Branch { branchradius = 1, Lengthfactor = 0.5, leaveradius = 0, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 0, height = 0, width = 0 }, Minlength = 0, Maxlength = 1, Minbranches = 1, Maxbranches = 3, growtoground = new Dbetween { Minimum = 0, Maximum = 0.2 } }, biome = Biomes.Taiga };
        Tree cactusplant = new Tree { Name = "CACTUSPLANT", roots = new Branch { Minlength = 0, Maxlength = 0 }, branches = new Branch { branchradius = -1, leaveradius = 1, growtoground = new Dbetween { Minimum = 0, Maximum = 0 }, togroundfactor = 1, Lengthfactor = 1, Minlength = 2, Maxlength = 3, Maxbranches = 4, Minbranches = 1, leaves = new Leave { chance = 1, width = 0, height = 0 } }, biome = Biomes.TemperateDesert, maxsplits = 2 };
        Tree palm = new Tree { Name = "PALM", biome = Biomes.Beach, branches = new Branch { Minlength = 2, Maxlength = 4, branchradius = 1, leaves = new Leave { width = 3, height = 2, chance = 0.8 }, leaveradius = 2 } };
        Tree bloem = new Tree { Name = "FLOWER", biome = Biomes.Grassland, maxsplits = 1, branches = new Branch { Minlength = 1, Maxlength = 2, leaves = new Leave { width = 2, height = 2, fill = Blockfill.purpleflowers }, branchradius = 1, growtoground = new Dbetween { Minimum = 1, Maximum = 1 }, togroundfactor = 1 }, roots = new Branch { Maxlength = 0 } };
        Tree coral = new Tree { Name = "CORAL", biome = Biomes.Ocean, maxwaterheight = 100, minwaterheight = 10, branches = new Branch { growin = 1, Minlength = 1, Maxlength = 5, branchfill = Blockfill.coral, leaves = new Leave { chance = 0 } } };
        Tree bush = new Tree { Name = "BUSH", biome = Biomes.TropicSeasonalForest, maxsplits = 1, branches = new Branch { Minlength = 0, Maxlength = 1, leaves = new Leave { width = 5, height = 3, chance = 1 }, growtoground = new Dbetween { Minimum = 1, Maximum = 1 } }, roots = new Branch { Maxlength = 0, Minlength = 0 } };
        Tree den = new Tree { Name = "DEN", Notgrowradius = 2, biome = Biomes.Tundra, maxsplits = 5, maxwaterheight = 0, minwaterheight = 0, branches = new Branch { leaves = new Leave { width = 8, height = 1.6, chance = 1, fill = Blockfill.acacialeavesgreen }, growtoground = new Dbetween { Minimum = 1, Maximum = 1 }, togroundfactor = 1, Lengthfactor = 0.8, Minbranches = 1, Maxbranches = 1, Maxlength = 10, Minlength = 5 } };
        Tree braambos = new Tree { maxsplits = 4, biome = Biomes.Grassland, Name = "BRAAMBOS", branches = new Branch { Lengthfactor = 0.5, branchfill = Blockfill.wood, Minlength = 5, Maxlength = 10, Maxbranches = 7, Minbranches = 2, branchradius = 0.5, growtoground = new Dbetween { Minimum = 0, Maximum = 1 }, togroundfactor = 0, leaves = new Leave { fill = Blockfill.acacialeavesgreen, chance = 0.5, width = 3, height = 1 } } };
        public Tree GetTreeByName(string Name)
        {
            foreach (Tree tr in trees)
            {
                if (tr.Name == Name)
                {
                    return tr;
                }
            }
            return acacia;
        }

        Tree[] trees;
        int lb, lh, lbe, lhe, lb2;
        double[] position = new double[4];
        string WW;
        long[] l = new long[4];
        bool[,] nietgemaakt, nietgeplaatst;
        int[,,] px = new int[1388, 1001, 8];
        int[] blok = new int[7];
        int[] pos = new int[4];
        int[,] at = new int[101, 6];
        byte[,,,,] klr = new byte[101, 101, 101, 7, 4];
        bool[] toets = new bool[256];
        int[,] d = new int[101, 5];
        int[] schaal = new int[3], blokplaats = new int[4];
        int[,] druppel;
        string[] mz = new string[101];
        double[] r = new double[4], snelh = new double[4];
        int opv, max, hoogte, vlkgt, wh, gemiddeld;
        double stli;
        Sunstate stage;
        Random doblstn = new Random();
        double[] stlh = new double[4], vlakgt = new double[4];
        Stopwatch timedo = Stopwatch.StartNew();
        double vaagheid = 12, zfactor, pi180, frames, tijdmuis = DateAndTime.Timer + 0.01, ReloadingTime, LastFrametime, savetime = DateAndTime.Timer + 300, filmsnelheid = 20, doelsnelheid = 1;
        byte voorraad = 0;
        int xp, yp, adn, nut, kijkwijdte, atlz = 100, totdruppels = 0;
        bool kijken, film, herhalen, sluiten, lijn, klik = false, autovg, bezigplaatsen = false, regenend = false, Creating = false, settingmouse = false, blokkeren = false;
        int minuut, maakvlak = 100;
        string pad, filmpad, beginpad;
        const byte vol = 255;
        const double sa = 0.5;
        bool gewijzigd = true;
        int[,,] noisekaart = new int[0, 0, 0];
        Generating.Consts.Biomes[,] biomenkaart = new Generating.Consts.Biomes[0, 0];
        double[,] treechanceheight = new double[0, 0];
        bool FlashLightOn = false;

        private void TextBox1_KeyUp(object sender, KeyEventArgs e) => toets[(int)e.KeyCode] = false;

        private void KleurenLadenToolStripMenuItem_Click(double x, double y) => Load_colors();
        double Gsin(double getal) => Math.Sin(getal * pi180);
        double Gcos(double getal) => Math.Cos(getal * pi180);

        private void Snelheid_klik(double x, double y)
        {
            doelsnelheid = Getalin("Hoe snel wil je gaan?\n0 tot 10\nNu: " + doelsnelheid + ".", 0, 10, doelsnelheid);
        }
        private void Kaart_klik(double x, double y) => Kaart();
        void Inventaris() => voorraad = (byte)(Getalin("Welk soort blokje(1 tot 30) wil je gebruiken?", 0, 30, (double)(voorraad)));
        private void ToolStripButton3_Click(double x, double y) => Informatie();

        /*            private void ToolStripButton9_Click(double x, double y)
                    {
                        string[] filesopened = new string[0];
                        Rondlopen newscreen = new Rondlopen(filesopened);
                        newscreen.CurrentScreen.Show();
                    }
                    */
        private void Plaatje_Click(double x, double y)
        {

        }

        private void AsToolStripMenuItem_Click(double x, double y) => Verplaatsing();
        private void ToolStripButton5_Click(double x, double y) => kijkwijdte = (int)Getalin("Hoe ver kan je maximaal kijken?\nNu: " + kijkwijdte, 1, 1000, kijkwijdte);
        private void Openen_klik(double x, double y) => Openen();
        private void Opslaan_klik(double x, double y) => Opslaan();
        string Padn(string pad) => beginpad + pad;
        int Schaalx(double ex) => (int)(ex / CurrentScreen.view.Width * schaal[1]);
        int Schaaly(double ey) => (int)(ey / CurrentScreen.view.Height * schaal[2]);
        private void Muis_op(object sender, EventArgs e) => Cursor.Hide();
        private void Muis_af(object sender, EventArgs e) => Cursor.Show();
        private void Transport_klik(double x, double y) => Verplaatsing();

        double Getalin(string vraag, double minim, double maxim, double standrd)
        {
            double antw = 0;
            string aw;
            aw = (Microsoft.VisualBasic.Interaction.InputBox(vraag, WW));
            if (aw.Length > 0)
            {
                try
                {
                    antw = Convert.ToDouble(aw);
                }
                catch
                { return standrd; }
            }
            else
            {
                return standrd;
            }
            if ((int)antw < minim || antw > maxim) { antw = standrd; }
            return antw;
        }

        private void ToolStripButton6_Click(double x, double y)
        {
            if (lijn)
            {
                lijn = false;
                MessageBox.Show("Lijn is uit", WW);
            }
            else
            {
                lijn = true;
                MessageBox.Show("Lijn is aan", WW);
            }
        }

        private void Rondlopen_Closing(object sender, FormClosingEventArgs e)
        {
            if (sluiten) { return; }
            sluiten = true;
            switch (MessageBox.Show("Wilt u de wereld opslaan?", WW, MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Cancel:
                    sluiten = false;
                    e.Cancel = true;
                    return;
                case DialogResult.Yes:
                    Opslaan();
                    break;
            }
            herhalen = false;
            kijken = false;
        }
        private void Nieuwewereld_klik()
        {
            curview.Rescale(lb, lb, lh);
            /*            Nieuwewereld zien = new Nieuwewereld(lb, lh);
                        zien.Show();
                        do
                        {
                            CurrentScreen.Dothings();
                        }
                        while (!Nieuwewereld.maken);
                        zien.Hide();
                        boom = zien.BM.Value;
                        stli = zien.TrackBar1.Value;
                        wh = zien.TrackBar2.Value;
                        max = zien.TrackBar4.Value;
                        opv = zien.TrackBar3.Value;
                        vlkgt = zien.TrackBar6.Value;
              */
            stli = 4;
            opv = 10;
            max = 280;
            wh = (int)Bigmath.Lerp(opv, max, 0.1);
            vlkgt = 100;
            Nieuwe_wereld();

            //Nieuwewereld.maken = false;
            Creating = true;
        }
        private void Regenen_klik(double x, double y)
        {
            if (regenend) { regenend = false; }
            else
            {
                regenend = true;
                druppel = new int[10000000, 4];
            }
        }
        void Regenen(int hoeveeldruppels, int x, int y, int omtrek)
        {
            int i, j;
            int homtrek = omtrek / 2;
            totdruppels += hoeveeldruppels;
            for (i = totdruppels - hoeveeldruppels; i < totdruppels; i++)
            {
                druppel[i, 1] = (int)(position[1] + (doblstn.NextDouble() * omtrek) - homtrek);
                druppel[i, 2] = (int)(position[2] + (doblstn.NextDouble() * omtrek) - homtrek);
                for (j = 300; j > 0; j--)
                {
                    if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2], j) == 0) { break; }
                }
                druppel[i, 3] = j;
                curview.Block.Setblock(druppel[i, 1], druppel[i, 2], druppel[i, 3], 1);
            }
        }
        void Doorregenen()
        {
            int i, opt;
            int[,] optie;
            for (i = 0; i < totdruppels; i++)
            {
                if (druppel[i, 3] != 0)
                {
                    opt = 0;
                    optie = new int[5, 4];
                    curview.Block.Setblock(druppel[i, 1], druppel[i, 2], druppel[i, 3], 0);
                    if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2], druppel[i, 3] - 1) == 0)
                    {
                        druppel[i, 3]--;
                    }
                    else
                    {


                        if (curview.Block.Getblock(druppel[i, 1] - 1, druppel[i, 2], druppel[i, 3] - 1) == 0)
                        {
                            opt++;
                            optie[opt, 3] = -1;
                            optie[opt, 1] = -1;
                        }

                        if (curview.Block.Getblock(druppel[i, 1] + 1, druppel[i, 2], druppel[i, 3] - 1) == 0)
                        {
                            opt++;
                            optie[opt, 3] = -1;
                            optie[opt, 1] = +1;
                        }
                        if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2] - 1, druppel[i, 3] - 1) == 0)
                        {
                            opt++;
                            optie[opt, 3] = -1;
                            optie[opt, 2] = -1;
                        }
                        if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2] + 1, druppel[i, 3] - 1) == 0)
                        {
                            opt++;
                            optie[opt, 3] = -1;
                            optie[opt, 2] = 1;
                        }
                        if (opt == 0)
                        {
                            if (curview.Block.Getblock(druppel[i, 1] - 1, druppel[i, 2], druppel[i, 3]) == 0)
                            {
                                opt++;
                                optie[opt, 1] = -1;
                            }
                            else if (curview.Block.Getblock(druppel[i, 1] + 1, druppel[i, 2], druppel[i, 3]) == 0)
                            {
                                opt++;
                                optie[opt, 1] = 1;
                            }
                            else if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2] - 1, druppel[i, 3]) == 0)
                            {
                                opt++;
                                optie[opt, 2] = -1;
                            }
                            else if (curview.Block.Getblock(druppel[i, 1], druppel[i, 2] + 1, druppel[i, 3]) == 0)
                            {
                                opt++;
                                optie[opt, 2] = 1;
                            }
                        }
                        int kies = 1 + (int)(doblstn.NextDouble() * opt);
                        druppel[i, 1] += optie[kies, 1];
                        druppel[i, 2] += optie[kies, 2];
                        druppel[i, 3] += optie[kies, 3];
                    }
                    curview.Block.Setblock(druppel[i, 1], druppel[i, 2], druppel[i, 3], 1);
                }
            }
        }


        private void Printscreen()
        {
            string pad = Nieuw_bestand("bmp");
            try
            {
                CurrentScreen.view.Backimage.Save(pad);
            }
            catch { MessageBox.Show("het pad is niet gevonden"); }
        }
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!blokkeren)
            {
                if (!klik)
                {
                    klik = true;
                    blok = curview.Getblockpointed;
                    byte plts = 0, dims = 0;
                    Double b5;
                    int[] afs = new int[4];
                    afs[1] = 1;
                    afs[2] = lb;
                    afs[3] = lb2;
                    curview.Reloadview();
                    CurrentScreen.view.Backimage = curview.getview;
                    xp = schaal[1] / 2;
                    yp = schaal[2] / 2;
                    if (blok[0] == 0)
                    {
                        switch (e.Button)
                        {
                            case MouseButtons.Left when curview.Block.Block[blok[1] + blok[2] * curview.Block.lw + blok[3] * curview.Block.lwl] == Blockfill.lava:
                                if (toets[17])
                                {//Control
                                    curview.SwitchLight(blok[1], blok[2], blok[3]);
                                    goto buitenber;
                                }
                                else
                                {
                                    curview.Removelight(blok[1], blok[2], blok[3]);
                                }

                                break;
                            case MouseButtons.Right:
                                {
                                    b5 = blok[4] - 3.5;
                                    dims = (byte)(Math.Abs(b5) + 0.5);
                                    blok[dims] = (int)(blok[dims] + (Math.Abs(b5) / b5));
                                    if (!curview.Block.Checkwithinreach(blok[1], blok[2], blok[3])) { goto buitenber; }
                                    plts = voorraad;
                                    if (toets[16])
                                    {
                                        if (bezigplaatsen)
                                        {
                                            for (int i = 1; i < 4; i++)
                                            {
                                                if (blokplaats[i] > blok[i])
                                                {
                                                    Bigmath.Switchints(blokplaats[i], blok[i], out blok[i], out blokplaats[i]);
                                                }
                                            }
                                            if (commands.Length == 1 && commands[0] == "FILL")
                                            {
                                                curview.Block.Setblockrange(Blocks.Shapes.rectangle, blokplaats[1], blokplaats[2], blokplaats[3], blok[1], blok[2], blok[3], voorraad);
                                            }
                                            //                                    dowithshift = "";
                                            gewijzigd = false;
                                            blokkeren = false;
                                            bezigplaatsen = false;
                                            toets[16] = false;
                                        }
                                        else
                                        {


                                            if (commands.Length > 1 && commands[0] == "GROW")
                                            {
                                                Tree tr = GetTreeByName(commands[1]);
                                                try
                                                {
                                                    double initheight = Convert.ToDouble(commands[2]);
                                                    tr.Grow(curview.Block, blok[1], blok[2], blok[3], initheight);
                                                }
                                                catch
                                                {
                                                    tr.Grow(curview.Block, blok[1], blok[2], blok[3]);
                                                }
                                            }
                                            else { bezigplaatsen = true; blokplaats[1] = blok[1]; blokplaats[2] = blok[2]; blokplaats[3] = blok[3]; }
                                        }
                                        goto buitenber;
                                    }
                                    if (voorraad == 10)
                                    {
                                        if (adn > 99) { MessageBox.Show("Teveel dynamiet", "WereldWijd"); return; }
                                        adn++;
                                        d[adn, 1] = blok[1];
                                        d[adn, 2] = blok[2];
                                        d[adn, 3] = blok[3];
                                        d[adn, 4] = 1;
                                    }
                                    else if (voorraad == Blockfill.lava)
                                    {
                                        double[] li = { 0, doblstn.NextDouble(), doblstn.NextDouble(), doblstn.NextDouble() };
                                        curview.Addlight(new openview.View.Light { straal = 10, x = blok[1], y = blok[2], z = blok[3], Brightness = li });
                                    }

                                    break;
                                }
                        }

                        if (blok[dims] > l[dims] || blok[dims] < 0) { if (dims < 3) { goto buitenber; } else { if (blok[3] < 0 || blok[3] > lhe) { goto buitenber; } } }
                        if (nut < atlz) { nut += 1; } else { nut = 0; }
                        at[nut, 1] = blok[1];
                        at[nut, 2] = blok[2];
                        at[nut, 3] = blok[3];
                        at[nut, 4] = curview.Block.Getblock(blok[1], blok[2], blok[3]);
                        at[nut, 5] = 1;
                        gewijzigd = true;
                        curview.Block.Setblock(blok[1], blok[2], blok[3], plts);
                        buitenber:;
                    }
                    klik = false;
                }
            }
        }
        string Welke_map()
        {
            FolderBrowserDialog FolderBrowserDialog1 = new FolderBrowserDialog();
            string wm = Padn("");
            FolderBrowserDialog1.ShowDialog();
            wm = FolderBrowserDialog1.SelectedPath;
            return wm;
        }
        private void Beweeg_muis(object sender, MouseEventArgs e)
        {
            if (tijdmuis < DateAndTime.Timer && !settingmouse && !blokkeren)
            {
                if (herhalen)
                {
                    Cursor.Hide();
                    tijdmuis = DateAndTime.Timer + 0.01;
                    //65a,68d,87w,83s
                    int mx, my;

                    mx = CurrentScreen.Width / 2;
                    my = CurrentScreen.Height / 2;
                    r[1] += (e.X - mx) / 5 * zfactor;
                    r[2] -= (e.Y - my) / 5 * zfactor;
                    settingmouse = true;
                    curview.Setmousetomidpicturebox(CurrentScreen, CurrentScreen.Cursor, false);
                    CurrentScreen.Dothings();
                    settingmouse = false;
                    if (r[2] > 80) { r[2] = 90; }
                    else if (r[2] < -80) { r[2] = -90; }
                    if (r[1] > 360)
                    {
                        r[1] = (r[1]) % 360;
                    }
                    else if (r[1] < 0)
                    {
                        r[1] = (r[1] + 3600) % 360;
                    }
                }
                else
                {
                    //CurrentScreen.Cursor.s
                    Cursor.Show();
                }
            }
        }

        private void Autovaagheid_klik(double x, double y) { if (autovg) { autovg = false; } else { autovg = true; } }

        string Welk_bestand(string extensie)
        {
            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            if (extensie == "map") { OpenFileDialog1.RestoreDirectory = true; } else { OpenFileDialog1.RestoreDirectory = false; OpenFileDialog1.Filter = "WereldWijd bestanden(*." + extensie + "|*." + extensie; }
            OpenFileDialog1.InitialDirectory = Padn("");
            string wb;
            OpenFileDialog1.ShowDialog();
            wb = OpenFileDialog1.FileName;
            return wb;
        }
        string Nieuw_bestand(string extensie)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            extensie = "WereldWijd bestanden(*." + extensie + "|*." + extensie;
            SaveFileDialog1.InitialDirectory = Padn("");
            SaveFileDialog1.Filter = extensie;
            string wb;
            SaveFileDialog1.ShowDialog();
            wb = SaveFileDialog1.FileName;
            return wb;
        }
        void Frbijw()
        {
            //fPSToolStripMenuItem.Text = Convert.ToString(ltd);
            if (autovg)
            {
                //frames per sec.
                ReloadingTime *= FrameRate;
                frames = vaagheid * (Math.Sqrt(ReloadingTime));
                if (frames > 50) { frames = 50; } else if (frames < 1) { frames = 1; };
                //if (oframes > frames * 1.25 || oframes < frames * 0.8)
                {
                    Vgi(frames);
                    LastFrametime = frames;
                }
            }
        }
        void Openen()
        {
            string bpad;
            Creating = false;
            bpad = Welk_bestand("3Dwereld");
            if (bpad == null) { return; }
            try
            {
                curview.Block = Blocks.Fromfile(bpad);
                pad = bpad;
            }
            catch
            {
                MessageBox.Show("Het bestand bestaat niet.", WW);
            }
        }
        void Opnemen()
        {
            if (film)
            {
                am.Close();
                film = false;
                MessageBox.Show("film stopt", WW);
            }
            else
            {
                if (MessageBox.Show("Wilt u iets opnemen?", WW, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    film = true;
                    filmpad = beginpad + Interaction.InputBox("Hoe moet de film heten?", WW) + ".avi";
                    am = new AviManager(filmpad, false);
                    Filmwidth = CurrentScreen.view.Backimage.Width;
                    Filmheight = CurrentScreen.view.Backimage.Height;
                    video = am.AddVideoStream(true, 10, Conv(CurrentScreen.view.Backimage));
                    //am.Close();
                    //                    video = new VideoStream(1,new IntPtr());
                    //Directory.CreateDirectory(filmpad);
                    //filmpad = filmpad + @"\";
                }
            }
        }
        void Film_kijken()
        {
            double begintijd = DateAndTime.Timer;
            double nuframe = 0;
            kijken = true;
            filmpad = Welke_map();
            do
            {
                nuframe += ((DateAndTime.Timer - begintijd) * filmsnelheid);
                if (File.Exists(filmpad + @"\" + (int)nuframe + ".BMP"))
                {
                    CurrentScreen.view.Backimage = (Bitmap)Image.FromFile(filmpad + @"\" + (int)nuframe + ".BMP");
                    begintijd = DateAndTime.Timer;
                }
                else { goto klaar; }
                CurrentScreen.Dothings();
            } while (kijken);
            klaar:
            kijken = false;
        }
        void Opslaan()
        {
            if (pad == null || pad == "") { pad = Nieuw_bestand("3Dwereld"); }
            if (pad == null || pad == "") { MessageBox.Show("U hebt geen bestandsnaam opgegeven", WW); return; }

            curview.Block.Save(pad);
        }

        void Explosie()
        {
            int x, y, z, i, j;
            int[,] minmax = new int[3, 4];
            for (i = 1; i <= adn; i++)
            {
                if (d[i, 4] == 1)
                {
                    for (j = 1; j <= 3; j++)
                    {
                        if (d[i, j] - 10 < 0) { minmax[1, j] = 0; } else { minmax[1, j] = d[i, j] - 10; }
                        if (d[i, j] + 10 > l[j])
                        {
                            minmax[2, j] = (int)l[j];
                        }
                        else
                        {
                            minmax[2, j] = d[i, j] + 10;
                        }

                        for (x = minmax[1, 1]; x <= minmax[2, 1]; x++)

                            for (y = minmax[1, 2]; y <= minmax[2, 2]; y++)
                            {

                                for (z = minmax[1, 3]; z <= minmax[2, 3]; z++)
                                {
                                    //omtrek=10
                                    if (Bigmath.Pyt(x - d[i, 1], y - d[i, 2], z - d[i, 3]) < 10)
                                    {
                                        curview.Block.Setblock(x, y, z, 0);
                                    }
                                }
                            }
                    }
                    d[i, 1] = 0;
                    d[i, 2] = 0;
                    d[i, 3] = 0;
                    d[i, 4] = 0;
                }
            }
            adn = 0;
        }

        void Kaart()
        {
            CurrentScreen.view.Backimage = curview.Getmap();
            CurrentScreen.Dothings();
            double sec = DateAndTime.Timer + 2;
            do { } while (DateAndTime.Timer < sec);

        }
        void Verplaatsing()
        {
            //            r[2] = -90;
            curview.Reloadview();
            CurrentScreen.view.Backimage = curview.getview;
            position[1] = Getalin("X", -lb, lb * 2, lb * 0.5);
            position[2] = Getalin("Y", -lb, lb * 2, lb * 0.5);
            position[3] = Getalin("Z", -lh, lh * 2, lh * 0.5);
        }
        void Informatie()
        {
            string hlp = "positie:\nx: " + position[1] + "\ny: " + position[2] + "\nz: " + position[3] + "\nAantal dynamiet: " + adn + " staven." + "\ninventaris: " + voorraad;
            MessageBox.Show(hlp, "WereldWijd");
        }

        void Vgi(double inp)
        {
            schaal[1] = (int)(CurrentScreen.view.Width / inp);
            schaal[2] = (int)(CurrentScreen.view.Height / inp);
            vaagheid = inp;
        }
        void Klin()
        {
            double tijd = (DateAndTime.Timer + TimeShift) % 1800, roodtemp, groentemp, blauwtemp;
            double sec = tijd % 60;
            minuut = (int)(tijd / 60);
            if (minuut > 22 || minuut < 7)
            { //nacht
                stage = Sunstate.night;
                roodtemp = 0;
                groentemp = 0;
                blauwtemp = 0;
            }
            else if (minuut > 7 && minuut < 22)
            { //dag
                stage = Sunstate.day;
                roodtemp = 1;
                groentemp = 1;
                blauwtemp = 1;
            }
            else
            { //ondergang
                if (minuut == 7)
                {
                    sec = 60 - sec;
                    stage = Sunstate.sunrise;
                }
                else
                {
                    stage = Sunstate.sunset;
                }
                if (sec < 20)
                {
                    roodtemp = 1;
                    groentemp = 1 * (20 - sec) * 0.05;
                    blauwtemp = 1;
                }
                else if (sec < 40)
                {
                    roodtemp = 1;
                    groentemp = 0;
                    blauwtemp = 1 * (40 - sec) * 0.05;
                }
                else
                {
                    roodtemp = 1 * (60 - sec) * 0.05;
                    groentemp = 0;
                    blauwtemp = 0;
                }
            }        //diameter van de zon is 0,53 graden
                     //            curview.Setcolortemperature(roodtemp*0.9+0.1, groentemp*0.9+0.1, blauwtemp*0.9+0.1);
            curview.Setcolortemperature(roodtemp * 0.8 + 0.2, groentemp * 0.8 + 0.2, blauwtemp * 0.8 + 0.2);
        }
        void Terug()
        {
            if (at[nut, 5] == 1 && nut > 0)
            {
                switch (curview.Block.Getblock(at[nut, 1], at[nut, 2], at[nut, 3]))
                {
                    case Blockfill.dinamite: d[adn, 1] = d[adn, 2] = d[adn, 3] = d[adn, 4] = 0; adn--; break;
                    case Blockfill.lava: curview.Removelight(at[nut, 1], at[nut, 2], at[nut, 3]); break;
                }
                switch (at[nut, 4])
                {
                    case Blockfill.lava:
                        double[] br = { 0, doblstn.NextDouble(), doblstn.NextDouble(), doblstn.NextDouble() };
                        curview.Addlight(new openview.View.Light { x = at[nut, 1], y = at[nut, 2], z = at[nut, 3], straal = 5, Brightness = br }); break;
                }
                curview.Block.Setblock((int)at[nut, 1], (int)at[nut, 2], (int)at[nut, 3], (byte)(at[nut, 4]));
                at[nut, 1] = 0;
                at[nut, 2] = 0;
                at[nut, 3] = 0;
                at[nut, 4] = 0;
                nut--;
            }
        }
        void Bewegen(double richting)
        {
            snelh[1] += Gsin(r[1] + 90 * (richting - 1)) * (doelsnelheid * 0.25);
            snelh[2] += Gcos(r[1] + 90 * (richting - 1)) * (doelsnelheid * 0.25);
        }
        void Zwaartekracht()
        {
            if (curview.Block.Checkwithinreach((int)position[1], (int)position[2], (int)position[3] - 2))
            {
                if (curview.Block.Getblock(position[1], position[2], position[3] - 2) > 1)
                { return; }
            }
            snelh[3] -= 0.5;

        }
        void Snelhbijw()
        {
            double[] plus = new double[4], npa = new double[4], epa = new double[4];
            double afgelegd, versnelling = 1;

            if (curview.Block.Checkwithinreach(position[1], position[2], position[3] - 2))
            {
                if (curview.Block.Getblock(position[1], position[2], position[3] - 2) == 1)
                {
                    versnelling = 0.8;
                }
                else if (curview.Block.Getblock(position[1], position[2], position[3] - 2) == 6)
                {
                    versnelling = 0.98;
                }
                else
                {
                    versnelling = 0.95;
                }
            }
            else
            {
                versnelling = 0.95;
            }
            for (int i = 1; i < 4; i++)
            {
                snelh[i] *= versnelling;
                //                if (Math.Abs(snelh[i]) > 1) { plus[i] = snelh[i] / Math.Abs(snelh[i]); } else { plus[i] = snelh[i]; }
            }
            double afs = Math.Sqrt(snelh[1] * snelh[1] + snelh[2] * snelh[2] + snelh[3] * snelh[3]);
            if (afs > 0)
            {
                for (afgelegd = 0; afgelegd < (afs + 1); afgelegd++)
                {
                    if (afgelegd > afs) { afgelegd = afs; }
                    plus[1] = afgelegd * snelh[1] / afs;
                    plus[2] = afgelegd * snelh[2] / afs;
                    plus[3] = afgelegd * snelh[3] / afs;
                    npa[1] = position[1] + plus[1];
                    npa[2] = position[2] + plus[2];
                    npa[3] = position[3] + plus[3];
                    if (Gravity && afgelegd > 0)
                    {

                        for (int j = 0; j < 2; j++)
                        {
                            if (curview.Block.Checkwithinreach((int)npa[1], (int)npa[2], (int)npa[3] - j))
                            {
                                if (!(curview.Block.Getblock(npa[1], npa[2], npa[3] - j) < 2))
                                { snelh = new double[4]; goto botsing; };
                            }
                        }
                    }
                    epa[1] = position[1] + plus[1];
                    epa[2] = position[2] + plus[2];
                    epa[3] = position[3] + plus[3];
                }
                botsing:
                for (int i = 1; i < 4; i++)
                {
                    position[i] = epa[i];
                }
            }
        }
        void Indrukken()
        {
            CurrentScreen.view.Hideall();
            Stopwatch ctr = Stopwatch.StartNew();
            do
            {

                Klin();
                if (Gravity) { Zwaartekracht(); }
                Snelhbijw();
                if (toets[27])//escape
                {
                    /*CurrentScreen.view.Showall();
                    Newworld.visible = false;
                    prompt.visible = false;*/
                    curview.Setmouse(0, 0, CurrentScreen.Cursor, CurrentScreen, true);
                    toets[27] = false;
                    herhalen = false;
                    return;
                }
                if (toets[37])
                {
                    if (kijken) { filmsnelheid *= 0.8; herhalen = false; return; } else { Bewegen(4); }
                }

                else if (toets[39]) { if (kijken) { filmsnelheid /= 0.8; herhalen = false; return; } else { Bewegen(2); } }
                if (toets[38]) { Bewegen(1); }
                else if (toets[40]) { Bewegen(3); }
                if (toets[34])
                { //page-down
                    if (!Gravity) { snelh[3] -= doelsnelheid * 0.25; }
                }
                else if (toets[33])
                { //page-up
                    if (!Gravity) { snelh[3] += doelsnelheid * 0.25; }
                }
                if (toets[65])
                {  //a
                    r[1] = r[1] - 10 * zfactor;
                }
                else if (toets[68])
                { //d
                    r[1] = r[1] + 10 * zfactor;
                }
                if (toets[81])
                {  //q
                    r[3] -= 10;
                }
                else if (toets[69])
                { //e
                    r[3] += 10;
                }
                if (toets[78])
                {
                    Nieuwewereld_klik();
                    toets[78] = false;
                }

                if (toets[87])
                { //w
                    if (r[2] > 90 - 10 * zfactor) { r[2] = 90; } else { r[2] += 10 * zfactor; }
                }
                else if (toets[83])
                {  //s
                    if (r[2] < -80 + 10 * zfactor) { r[2] = -90; } else { r[2] -= 10 * zfactor; }
                }
                if (toets[70])
                {//f van fly
                    snelh[1] += Gsin(r[1]) * Gcos(r[2]) * (doelsnelheid * 0.25);
                    snelh[2] += Gcos(r[1]) * Gcos(r[2]) * (doelsnelheid * 0.25);
                    snelh[3] += Gsin(r[2]) * (doelsnelheid * 0.25);

                }
                if (toets[32])
                { //spatie
                    if (kijken) { kijken = false; }
                    else if (Gravity)
                    {
                        if (curview.Block.Checkwithinreach((int)position[1], (int)position[2], (int)position[3] - 2))
                        {
                            if (curview.Block.Getblock(position[1], position[2], position[3] - 2) > 0)
                            { snelh[3] += 2 * doelsnelheid; }
                        }

                    }
                }
                if (toets[36])
                { //home
                    zfactor *= 0.8;

                }
                else if (toets[35])
                { //end
                    zfactor /= 0.8;
                }
                if (toets[90])
                { //z
                    Terug(); toets[90] = false;
                }
                if (toets[13])
                { //enter
                    Explosie();
                    toets[13] = false;
                }
                if (toets[73])
                { //i van inventaris
                    Inventaris();
                    toets[73] = false;
                }
                if (toets[77])
                { //m van removeMem
                    bezigplaatsen = false;
                }
                if (toets[82])
                {//r
                    snelh = new double[4];
                }
                if (toets[80])
                {//p van opdrachtPrompt
                    Cmd();
                }
                if (r[1] > 360)
                {
                    r[1] %= 360;
                }
                else if (r[1] < 0)
                {
                    r[1] = (r[1] + 3600) % 360;
                }
                if (r[3] > 360)
                {
                    r[3] %= 360;
                }
                else if (r[3] < 0)
                {
                    r[3] = (r[3] + 3600) % 360;
                }
                if (regenend)
                {
                    Regenen(100, (int)position[1], (int)position[2], 100);
                    Doorregenen();
                }
                { Makeallaround(); }
                if (savetime < DateAndTime.Timer && gewijzigd)
                {
                    Opslaan(); savetime = DateAndTime.Timer + 300; gewijzigd = false;
                }
                if (zfactor > 1) { zfactor = 1; }
                FlashLight.x = (int)position[1];
                FlashLight.y = (int)position[2];
                FlashLight.z = (int)position[3];
                FlashLight.on = (stage == Sunstate.night && FlashLightOn);
                curview.Setwindowproperties(schaal[1], schaal[2], lijn, Visor, Mist, ViewWidth, zfactor, vaagheid);
                curview.Setposition(position[1], position[2], position[3], r[1], r[2], r[3]);
                //berToolStripMenuItem.Text = Convert.ToString(ctr.Elapsed.TotalMilliseconds / 1000);
                ctr = Stopwatch.StartNew();
                curview.Reloadview();
                ReloadingTime = ctr.Elapsed.TotalMilliseconds / 1000;
                Frbijw();
                Bitmap get = curview.getview;
                CurrentScreen.view.Backimage = get;
                if (film)
                {
                    if (video.CountFrames * video.Height * video.Width * 4 > 40000000)//40 mb
                    {
                        MessageBox.Show("Teveel geheugen: Film stopt");
                        am.Close();
                        film = false;
                    }
                    else
                    {
                        video.AddFrame(Conv(CurrentScreen.view.Backimage));
                    }
                    //                    Bitmap bp = new Bitmap(3,3);
                    //                    video.AddFrame(bp);
                    //                    bp.Dispose();
                }
                ctr = Stopwatch.StartNew();
                CurrentScreen.Dothings();
            } while (herhalen);
        }
        Bitmap Conv(Bitmap bmp)
        {
            Bitmap conv = new Bitmap(Filmwidth, Filmheight);
            using (Graphics grp = Graphics.FromImage(conv))
            { grp.DrawImage(bmp, 0, 0, Filmwidth, Filmheight); }
            return conv;
        }
        void Cmd()
        {
            prompt.visible = true;
            bool escape;
            do
            {
                CurrentScreen.Dothings();
                if (toets[13])
                {
                    if (Executecmd(prompt.text))
                    { prompt.text = ""; }
                    else { MessageBox.Show("this was an command, but it caused an error"); }
                }
                else if (toets[(int)Keys.Back])
                {
                    if (prompt.text.Length > 0)
                    {
                        prompt.text = Strings.Left(prompt.text, prompt.text.Length - 1);
                    }
                }
                else
                {
                    if (toets[32]) { prompt.text += " "; }
                    for (int i = 65; i < 91; i++)
                    {
                        byte[] t = { (byte)i };
                        if (toets[i]) { prompt.text += ASCIIEncoding.ASCII.GetString(t); }
                    }
                    for (int i = (int)Keys.D0; i < (int)Keys.D9 + 1; i++)
                    {
                        byte[] t = { (byte)i };
                        if (toets[i]) { prompt.text += ASCIIEncoding.ASCII.GetString(t); }
                    }
                }
                escape = toets[27];
                toets = new bool[256];
            } while (!escape);
            prompt.visible = false;
            toets[27] = false;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            herhalen = true;
            toets[(int)e.KeyCode] = true;
        }

        void Map_laden()
        {
            string[] c = new string[0];
            byte b;
            char a;
            string scf;
            scf = @"\";

            for (b = 90; b > 64; b--)
            {
                a = (char)b;
                try
                {
                    c = (Directory.GetDirectories(a + @":\"));
                    try
                    {
                        c = Directory.GetDirectories(a + ":" + scf + "Blokkenwereld");
                        beginpad = a + @":\" + "Blokkenwereld" + scf;
                        return;
                    }
                    catch
                    {
                        try
                        {
                            Directory.CreateDirectory(a + @":\" + "Blokkenwereld");
                            beginpad = a + @":\" + "Blokkenwereld" + scf;
                            return;
                        }
                        catch { }
                    }

                }
                catch { }
            }
        }
        bool notfilled(int x, int y, double radius)
        {
            int iradius = (int)radius;
            for (int i = -iradius; i <= radius; i++)
            {
                for (int j = -iradius; j <= radius; j++)
                {
                    if (Bigmath.Pyt(i, j, 0) <= radius)
                    {
                        if (!(nietgeplaatst[x + i, y + j])) { return false; }
                    }
                }
            }
            return true;
        }
        void Placebesttree(int x, int y)
        {
            int i = 0, counttreesgood = 0;
            int index;
            int temp = noisekaart[2, x, y];
            int waterheight = wh - noisekaart[1, x, y];
            if (waterheight < 0) { waterheight = 0; }
            int[] bomen = new int[trees.Length];
            double iheight;
            double[] initheight = new double[trees.Length];
            foreach (Tree tr in trees)
            {
                initheight[counttreesgood] = tr.branches.Minlength + doblstn.NextDouble() * (tr.branches.Maxlength - tr.branches.Minlength);
                if (tr.biome == biomenkaart[x, y]
                        && tr.minwaterheight <= waterheight && tr.maxwaterheight >= waterheight
                        )
                {
                    bomen[counttreesgood] = i;
                    counttreesgood++;
                }
                i++;
            }
            if (counttreesgood > 0)
            {
                index = doblstn.Next(0, counttreesgood);

                iheight = initheight[index];
                Boom_plaatsen(trees[bomen[index]], x, y, noisekaart[1, x, y] + 1, iheight);
            }
        }
        void Load_colors()
        {
            curview.Load_colors(TextureDirectory + "blokken\\", 29, 101);
            curview.Load_airtexture(TextureDirectory + "other\\wolken.bmp");
        }
        void Maakblokje(int x, int y)
        {
            gewijzigd = true;
            nietgemaakt[x, y] = false;
            byte onder = 3, boven, gras;
            double mheight, mtemp, mbiome,hilly;
            int wzh = wh + 3, temp, ijsbasishoogte, wtemp;
            mheight = heightmap.Getnoise(x, y);
            hilly = hillmap.Getnoise(x, y);
            treechanceheight[x, y] = treemap.Getnoise(x, y);
            mheight = Math.Pow(mheight, hilly*2+0.5);
            mbiome = biomemap.Getnoise(x, y);
            Biomes bioom = Biome.GetBiome(mheight, mbiome);
            biomenkaart[x, y] = bioom;
            mheight = mheight * (max - opv) + opv;
            mtemp = mbiome * 80 - 20;
            /*
                double vs, gem; 
                for (int i = 1; i < 4; i++)
                {
                    vs = mx[i] - mn[i];
                    gem = (mn[i] + mx[i]) / 2;
                    noisekaart[i, x, y] = (int)((tot[i] * vs * stlh[i]) + gem);
                }*/
            noisekaart[1, x, y] = (int)mheight;
            noisekaart[2, x, y] = (int)mtemp;
            hoogte = noisekaart[1, x, y];
            wtemp = noisekaart[2, x, y];
            noisekaart[2, x, y] -= (hoogte - wh);
            temp = noisekaart[2, x, y];
            switch (bioom)
            {
                case Biomes.Ocean:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, hoogte + 1, x, y, wh, Blockfill.water);
                    if (wtemp <= 0)
                    {
                        ijsbasishoogte = wh + wtemp;
                        if (ijsbasishoogte <= hoogte) { ijsbasishoogte = hoogte + 1; }
                        curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, ijsbasishoogte, x, y, wh, Blockfill.ice);
                    }
                    break;
                case Biomes.Beach:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.Scorched:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.Bare:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.Tundra:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.sand);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.ice);
                    break;
                case Biomes.TemperateDesert:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.Shrubland:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.Grassland:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.sand);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.grass);
                    break;
                case Biomes.TemperateDeciduousForest:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.sand);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.grass);
                    break;
                case Biomes.Snow:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.sand);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.ice);
                    break;
                case Biomes.Taiga:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.TemperateRainForest:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.brownground);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.grass);
                    break;
                case Biomes.SubtropicalDesert:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte, Blockfill.sand);
                    break;
                case Biomes.TropicSeasonalForest:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.brownground);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.grass);
                    break;
                case Biomes.TropicRainforest:
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, x, y, 0, x, y, hoogte - 1, Blockfill.brownground);
                    curview.Block.Setblock(x, y, hoogte, Blockfill.grass);
                    break;
            }
        }
        void Placetree(int x, int y)
        {
            double HeighestValue=0;
            int r = (int)(4 / Biome.GetTreeChance(biomenkaart[x, y])),treex=0,treey=0;
            int z;
            for (int ix = -r; ix <= r; ix++) {
                for (int iy = -r; iy <= r; iy++)
                {
                    if (treechanceheight[x + ix, y + iy] > HeighestValue) { HeighestValue = treechanceheight[x + ix, y + iy];treex = x + ix;treey = y + iy; }
                }
            }
            if (nietgeplaatst[treex, treey]) {
            z = noisekaart[1, x, y] + 1;
                if (z < lh - 20 && z > 0)
                {
                        Placebesttree(x, y);
                }
            }
        }
        void Nieuwe_wereld()
        {
            gemiddeld = (opv + max) / 2;
            l[1] = lbe;
            l[2] = lbe;
            l[3] = lh;
            //perlin noise
            Nieuwe_noise();
            l[3] = lh;
            r[2] = -90;
            Gravity = true;
            position[1] = lb / 2;
            position[2] = lb / 2;
            position[3] = lh;
            Makeallaround();
            //Instellen()
            curview.lights = new openview.View.Light[0];
            Huis();
            adn = 0;
            curview.Addlight(FlashLight);
        }
        void Makeallaround()
        {
            for (int x = -maakvlak; x <= maakvlak; x++)
            {
                for (int y = -maakvlak; y <= maakvlak; y++)
                {
                    if (position[1] + x < lb && position[1] + x >= 0 && (position[2] + y) < lb && position[2] + y >= 0)
                    {
                        if (nietgemaakt[(int)position[1] + x, (int)position[2] + y])
                        { Maakblokje((int)position[1] + x, (int)position[2] + y); }
                    }
                }
            }
            for (int x = -maakvlak + 20; x <= maakvlak - 20; x++)
            {
                for (int y = -maakvlak + 20; y <= maakvlak - 20; y++)
                {
                    if (position[1] + x < lb - 20 && position[1] + x >= 20 && (position[2] + y) < lb - 20 && position[2] + y >= 20)
                    {
                        if (nietgeplaatst[(int)position[1] + x, (int)position[2] + y])
                        { Placetree((int)position[1] + x, (int)position[2] + y); }
                    }
                }
            }
        }
        double Getoffset(int x, int y)
        {
            return 2 + 2000 * Math.Pow(noisekaart[2, x, y] + 30, -2);
        }
        void Huis()
        {
            bool huisopwater = false;
            int l2, grondhoogte, huishoogte = 11, huisbreedte = 10, dakhoogte = huisbreedte / 2, binnenbreedte = huisbreedte / 2 - 1, halfhuisbreedte = huisbreedte / 2, verdiepingshoogte = 5, verdiepingsdikte = 2, verdiepingsvloerhoogte = verdiepingshoogte + verdiepingsdikte - 1;
            int steigerbreedte = halfhuisbreedte + 2;
            int steigerbinnenbreedte = steigerbreedte - 1, deurtrapafstand = 3,trappositie=0,y,z;
            l2 = (int)(lb * 0.5);
            grondhoogte = noisekaart[1, l2, l2];
            if (grondhoogte < wh) { grondhoogte = wh; huisopwater = true; }
            if (grondhoogte < lh - (huishoogte + dakhoogte) && grondhoogte > 0)
            {
                if (huisopwater)
                {
                    //steiger
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - steigerbreedte, l2 - steigerbreedte, grondhoogte, l2 + steigerbreedte, l2 + steigerbreedte, grondhoogte + 1, Blockfill.wood);
                    //uitholling steiger
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - steigerbinnenbreedte, l2 - steigerbinnenbreedte, grondhoogte + 1, l2 + steigerbinnenbreedte, l2 + steigerbinnenbreedte, grondhoogte + 1, Blockfill.air);
                }
                //balkon
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - steigerbreedte, l2 - steigerbreedte, grondhoogte + verdiepingsvloerhoogte, l2 + steigerbreedte, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 1, Blockfill.wood);
                //uitholling balkon
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - steigerbinnenbreedte, l2 - steigerbinnenbreedte, grondhoogte + verdiepingsvloerhoogte + 1, l2 + steigerbinnenbreedte, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 1, Blockfill.air);
                //huis
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 - halfhuisbreedte, 0, l2 + halfhuisbreedte, l2 + halfhuisbreedte, grondhoogte + huishoogte, Blockfill.stone);
                if (huisopwater)
                {
                    //steiger onder de deur
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 - halfhuisbreedte, grondhoogte, l2 + 1, l2 - halfhuisbreedte, grondhoogte, Blockfill.wood);
                }
                else {//trap naar beneden
                    y = l2 - halfhuisbreedte;
                    z = grondhoogte;
                    do
                    {
                        curview.Block.Setblockrange(Blocks.Shapes.rectangle, y, l2 - 1, z, y, l2 + 1, z,Blockfill.stone);
                        if (trappositie % 2 == 0) { y--; } else { z--; }
                        trappositie++;
                    } while (z>(noisekaart[1,y,l2]));
                }
                //vloerbedekking
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - binnenbreedte, l2 - binnenbreedte, grondhoogte, l2 + binnenbreedte, l2 + binnenbreedte, grondhoogte, Blockfill.Tapijt);
                //dak
                curview.Block.Setblockrange(Blocks.Shapes.pyramid, l2 - halfhuisbreedte, l2 - halfhuisbreedte, grondhoogte + huishoogte + 1, l2 + halfhuisbreedte, l2 + halfhuisbreedte, grondhoogte + huishoogte + dakhoogte + 1, Blockfill.wood);
                //deur-uitholling,ook naar buiten
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 - halfhuisbreedte, grondhoogte + 1, l2 + 1, l2 - halfhuisbreedte - 2, grondhoogte + 4, Blockfill.air);
                //deur-uitholling 1e verd.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 1, l2 + 1, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 4, Blockfill.air);
                //raam tegenover deur-uitholling
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 + halfhuisbreedte, grondhoogte + 2, l2 + 1, l2 + halfhuisbreedte, grondhoogte + 4, Blockfill.water);
                //andere grote ramen
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 + 1, grondhoogte + 2, l2 + halfhuisbreedte, l2 - 1, grondhoogte + 4, Blockfill.water);
                //kleine ramen begane grond.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 + 4, grondhoogte + 2, l2 + halfhuisbreedte, l2 + 3, grondhoogte + verdiepingshoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 - 4, grondhoogte + 2, l2 + halfhuisbreedte, l2 - 3, grondhoogte + verdiepingshoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 + 4, l2 - halfhuisbreedte, grondhoogte + 2, l2 + 3, l2 + halfhuisbreedte, grondhoogte + verdiepingshoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 4, l2 - halfhuisbreedte, grondhoogte + 2, l2 - 3, l2 + halfhuisbreedte, grondhoogte + verdiepingshoogte - 1, Blockfill.water);
                //raam tegenover deur-uitholling 1e verd.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 + halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 2, l2 + 1, l2 + halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 4, Blockfill.water);
                //andere grote ramen 1e verd.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 + 1, grondhoogte + verdiepingsvloerhoogte + 2, l2 + halfhuisbreedte, l2 - 1, grondhoogte + verdiepingsvloerhoogte + 4, Blockfill.water);
                //kleine ramen 1e verd.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 + 4, grondhoogte + verdiepingsvloerhoogte + 2, l2 + halfhuisbreedte, l2 + 3, grondhoogte + huishoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - halfhuisbreedte, l2 - 4, grondhoogte + verdiepingsvloerhoogte + 2, l2 + halfhuisbreedte, l2 - 3, grondhoogte + huishoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 + 4, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 2, l2 + 3, l2 + halfhuisbreedte, grondhoogte + huishoogte - 1, Blockfill.water);
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 4, l2 - halfhuisbreedte, grondhoogte + verdiepingsvloerhoogte + 2, l2 - 3, l2 + halfhuisbreedte, grondhoogte + huishoogte - 1, Blockfill.water);
                //uitholling begane grond.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - binnenbreedte, l2 - binnenbreedte, grondhoogte + 1, l2 + binnenbreedte, l2 + binnenbreedte, grondhoogte + verdiepingshoogte - 1, Blockfill.air);
                //uitholling 1e verd.
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - binnenbreedte, l2 - binnenbreedte, grondhoogte + verdiepingsvloerhoogte + 1, l2 + binnenbreedte, l2 + binnenbreedte, grondhoogte + huishoogte - 1, Blockfill.air);
                //trapgat
                curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 - halfhuisbreedte + deurtrapafstand, grondhoogte + verdiepingshoogte, l2 + 1, l2 - halfhuisbreedte + deurtrapafstand + verdiepingshoogte - 1, grondhoogte + verdiepingsvloerhoogte, Blockfill.air);
                //trap
                for (int i = 0; i < verdiepingshoogte; i++)
                {
                    curview.Block.Setblockrange(Blocks.Shapes.rectangle, l2 - 1, l2 - halfhuisbreedte + deurtrapafstand + i, grondhoogte + 1 + i, l2 + 1, l2 - halfhuisbreedte + deurtrapafstand + verdiepingshoogte - 1, grondhoogte + 1 + i, Blockfill.wood);
                }
                double[] br = { 0, 1, 1, 1 };
                //lampen begane grond
                curview.Block.Setblock(l2, l2 - halfhuisbreedte, grondhoogte + 5, Blockfill.lava);
                curview.Addlight(new openview.View.Light { Brightness = br, on = true, straal = 20, x = l2, y = l2 - halfhuisbreedte, z = grondhoogte + 5 });
                curview.Block.Setblock(l2, l2 - halfhuisbreedte + deurtrapafstand + verdiepingshoogte, grondhoogte + 5, Blockfill.lava);
                curview.Addlight(new openview.View.Light { Brightness = br, on = true, straal = 20, x = l2, y = l2 - halfhuisbreedte + deurtrapafstand + verdiepingshoogte, z = grondhoogte + 5 });
                //lamp 1e verd.
                curview.Block.Setblock(l2, l2, grondhoogte + huishoogte - 1, Blockfill.lava);
                curview.Addlight(new openview.View.Light { Brightness = br, on = true, straal = 20, x = l2, y = l2, z = grondhoogte + huishoogte - 1 });
            }
        }
        void Boom_plaatsen(Tree tree, int x, int y, int z, double initheight)
        {//https://minecraft.gamepedia.com/Tree
         //stronk
            nietgeplaatst[x, y] = false;
            tree.Grow(curview.Block, x, y, z, initheight);
        }
        void Nieuwe_noise()
        {
            noisekaart = new int[4, lb, lb];
            biomenkaart = new Generating.Consts.Biomes[lb, lb];
            treechanceheight = new double[lb, lb];
            int x, y, vlkgt2;
            vlkgt2 = vlkgt * 2;
            for (x = 0; x < lb; x++)
            {
                for (y = 0; y < lb; y++)
                {
                    nietgemaakt[x, y] = nietgeplaatst[x, y] = true;
                }
            }
            heightmap = new Perlin.SimplexNoise(3, 10, 280, vlkgt, lb, lb);
            temperaturemap = new Perlin.SimplexNoise(1, lb, lb, 100, lb, lb);
            biomemap = new Perlin.SimplexNoise(1, 0, 1, 250, lb, lb);
            hillmap = new Perlin.SimplexNoise(1, 0, 1, 250, lb, lb);
            treemap = new Perlin.SimplexNoise(1, 0, 1, 20, lb, lb);
            //heightmap = new Noise.Blocknoise(lb, lb, vlkgt * 2, opv, max);
            //temperaturemap = new Noise.Blocknoise(lb, lb, 100, -20, 50);
        }
    }
    /*                    void Keuze_opslaan() {
                            string naam;
            string[] prop;
            prop = File.AppendAllLines(Nieuw_bestand("3dKZ"), prop);
            //       naam = Padn(InputBox("Hoe moet uw keuze heten?", WW) & ".3dKZ")
                            FileClose();
                            FileOpen(1, naam, OpenMode.Output)
                            PrintLine(1, boom)
                            PrintLine(1, stli)
                            PrintLine(1, opv)
                            PrintLine(1, wh)
                            PrintLine(1, max)
                            PrintLine(1, vlkgt)
                            FileClose(1)
                    }
                        void Open_keuze() {
                    FileClose();
                        string naam;
                        naam = Rondlopen.Welk_bestand(extensie:= "3dKZ")
                        If naam = vbNullString Then Exit Sub
                        FileOpen(FileNumber:= 1, FileName:= naam, Mode:= OpenMode.Input)
                        boom = LineInput(1)
                        stli = LineInput(1)
                        opv = LineInput(1)
                        wh = LineInput(1)
                        max = LineInput(1)
                        vlkgt = LineInput(1)
                        FileClose(1)
                }

      */


}