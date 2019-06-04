using System;
using openview;
namespace Generating
{
    class Tree
    {
        static Random doblstn = new Random();
        double Randomize(double value)
        {
            double weight = Math.Sqrt(doblstn.NextDouble());
            return value * (1 - weight) + doblstn.NextDouble() * weight;
        }
        public void Grow(Blocks blcs, int x, int y, int z)
        {//https://minecraft.gamepedia.com/Tree
            double initheight = branches.Minlength + doblstn.NextDouble() * (branches.Maxlength - branches.Minlength);
            Grow(blcs, x, y, z, initheight);
        }
        public void Grow(Blocks blcs, int x, int y, int z,double InitHeight)
        {//https://minecraft.gamepedia.com/Tree
            if (blcs.Getblock(x, y, z) == branches.growin)
            {
                Setbranch(maxsplits, branches.leaves.fill, branches.Lengthfactor, InitHeight, x, y, z, doblstn.NextDouble() * 360, 90, branches.leaves.chance, branches.Minbranches, branches.Maxbranches, branches.growtoground.GetValue(), branches.growin, branches.leaves.width, branches.leaves.height, branches.leaveradius, blcs, branches.branchradius, InitHeight, branches.togroundfactor, branches.branchfill);
                double rootheight = roots.Minlength + doblstn.NextDouble() * (roots.Maxlength - roots.Minlength);
                Setbranch(maxsplits, roots.leaves.fill, branches.Lengthfactor, rootheight, x, y, z, doblstn.NextDouble() * 360, -90, roots.leaves.chance, roots.Minbranches, branches.Maxbranches, roots.growtoground.GetValue(), branches.growin, roots.leaves.width, roots.leaves.height, roots.leaveradius, blcs, branches.branchradius, InitHeight, branches.togroundfactor, branches.branchfill);
            }
        }
        void Setleaves(byte whichblock, byte branchfill, double radius, byte growin, double leavechance, double leavewidth, double leaveheight, double x, double y, double z, Blocks blocks)
        {
            int setx, sety, setz, i, j, k, mwidth = (int)Math.Max(radius, leavewidth), mheight = (int)Math.Max(radius, leaveheight);
            double dis;
            for (i = -mwidth; i <= mwidth; i++)
            {
                for (j = -mwidth; j <= mwidth; j++)
                {
                    for (k = (int)-radius; k <= mheight; k++)
                    {
                        setx = (int)x + i;
                        sety = (int)y + j;
                        setz = (int)z + k;
                        if (
                            (!(setx < 0 || setx > blocks.lw || sety < 0 || sety > blocks.ll || setz < 0 || setz > blocks.lh))
                            && blocks.Block[setx + sety * blocks.lw + setz * blocks.lwl] == growin)
                        {
                            dis = Math.Sqrt(i * i + j * j + k * k);
                            if (dis <= radius)
                            {
                                blocks.Block[setx + sety * blocks.lw + setz * blocks.lwl] = branchfill;
                            }
                            else if (k >= 0 && (doblstn.NextDouble() < leavechance))
                            {
                                dis = (Math.Sqrt((i / leavewidth) * (i / leavewidth) + (j / leavewidth) * (j / leavewidth) + (k / leaveheight) * (k / leaveheight)));
                                if (dis <= 1)
                                {
                                    blocks.Block[setx + sety * blocks.lw + setz * blocks.lwl] = whichblock;
                                }
                            }
                        }
                    }
                }
            }
        }
        void Setbranch(int maxsplits, byte whichblock, double Lengthfactor, double length, double x, double y, double z, double rotationx, double rotationy, double leavechance, int Minbranches, int Maxbranches, double afplatting, byte growin, double leavewidth, double leaveheight, double leaveradius, Blocks blocks, double branchradius, double initheight, double afplattingsfactor, byte branchfill)
        {
            int i, j;
            double fx = 0, fy = -1, fz = 0;
            Bigmath.Turn(fz, fy, rotationy, out fz, out fy);
            Bigmath.Turn(fx, fy, rotationx, out fx, out fy);
            double lengthverh = length / initheight;
            double initialwidth = lengthverh/ Math.Sqrt(lengthverh) *initheight*0.4;
            if (initheight > 0) {
            for (i = 0; i < length; i++)
            {
                if (z + i * fz + 1 < blocks.lh)
                {
                    initialwidth = lengthverh/Math.Sqrt(((i+length)/length)*lengthverh) * initheight * 0.4;
                    Setleaves(whichblock, branchfill, branchradius * initialwidth, growin, Math.Pow(leavechance, initialwidth), leaveradius * initialwidth, leaveradius * initialwidth, x + i * fx, y + i * fy, z + i * fz, blocks);
                }
            }
                initialwidth = lengthverh / Math.Sqrt(((i + length) / length) * lengthverh) * initheight * 0.4;
                Setleaves(whichblock, branchfill, branchradius * initialwidth, growin, Math.Pow(leavechance, initialwidth), leavewidth * initialwidth, leaveheight * initialwidth, x + i * fx, y + i * fy, z + i * fz, blocks);
            int bcount = doblstn.Next(Minbranches, Maxbranches + 1);
            if (length > 1 && maxsplits > 0)
            {
                    for (j = 0; j < bcount; j++)
                    {
                        Setbranch(maxsplits - 1, whichblock, Lengthfactor, length * Randomize(Lengthfactor), x + i * fx, y + i * fy, z + i * fz, rotationx + doblstn.NextDouble() * (4 * rotationy) - 2 * rotationy, 90 * afplatting*Bigmath.Sgn(rotationy), leavechance, Minbranches, Maxbranches, (afplatting + afplattingsfactor) / 2, growin, leavewidth, leaveheight, leaveradius, blocks, branchradius, initheight, afplattingsfactor, branchfill);
                    }
                }
            }
        }
        public static Tree[] Totrees(params Tree[] args) => args;
        public void Save(string path)
        {
            string[] a = new string[8];
            a[0] = Convert.ToString(branches.growtoground);
            a[1] = Convert.ToString(branches.leaves.chance);
            System.IO.File.WriteAllLines(path, a);
        }
        public Consts.Biomes biome = Consts.Biomes.Grassland;
        public double Notgrowradius=1;
        public int maxsplits = 3, minwaterheight = 0, maxwaterheight = 0;
        public Branch branches = new Branch(), roots = new Branch { leaves = new Leave { chance = 0 }, growin = 3 };
        public string Name;
    }
    class Dbetween {
        public double Minimum = 0;
        public double Maximum = 1;
        public double GetValue() => Minimum + Consts.Tools.doblstn.NextDouble() * (Maximum - Minimum);
    }
    class Ibetween
    {
        public int Minimum = 0;
        public int Maximum = 1;
        public int GetValue() => Consts.Tools.doblstn.Next(Minimum ,Maximum+1) ;
    }
    class Leave
    {
        public byte fill = Consts.Blockfill.acacialeavesgreen;
        public double chance = 1, width = 2, height = 2;
    }
    class Branch
    {
        public Leave leaves = new Leave();
        public byte growin = Consts.Blockfill.air;
        public int Minbranches = 1, Maxbranches = 3;
        public byte branchfill = 5;
        public double Minlength = 4, Maxlength = 10, Lengthfactor = 0.7, leaveradius = 0, branchradius = 1, togroundfactor = 0.5;
        public Dbetween growtoground = new Dbetween {Minimum=0,Maximum= 1 };
    }
    class Terrain {

    }
    class Biome
    {
        public static Consts.Biomes GetBiome(double e, double m)
        {
            if (e < 0.1) return Consts.Biomes.Ocean;
            if (e < 0.12) return Consts.Biomes.Beach;
            if (e > 0.8)
            {
                if (m < 0.5) return Consts.Biomes.Tundra;
                return Consts.Biomes.Snow;
            }
            if (e > 0.6)
            {
                if (m < 0.33) return Consts.Biomes.TemperateDesert;
                if (m < 0.66) return Consts.Biomes.Shrubland;
                return Consts.Biomes.Taiga;
            }
            if (e > 0.3)
            {
                if (m < 0.16) return Consts.Biomes.TemperateDesert;
                if (m < 0.50) return Consts.Biomes.Grassland;
                if (m < 0.83) return Consts.Biomes.TemperateDeciduousForest;
                return Consts.Biomes.TemperateRainForest;
            }
            if (m < 0.16) return Consts.Biomes.SubtropicalDesert;
            if (m < 0.33) return Consts.Biomes.Grassland;
            if (m < 0.66) return Consts.Biomes.TropicSeasonalForest;
            return Consts.Biomes.TropicRainforest;
        }
        public static double GetTreeChance(Consts.Biomes biome) {
            switch (biome)
            {
                case Consts.Biomes.Ocean:
                    return 0.1;
                case Consts.Biomes.Beach:
                    return 0.1;
                    break;
                case Consts.Biomes.Scorched:
                    return 0.5;
                    break;
                case Consts.Biomes.Bare:
                    return 0.5;
                    break;
                case Consts.Biomes.Tundra:
                    return 0.15;
                    break;
                case Consts.Biomes.TemperateDesert:
                    return 0.05;
                    break;
                case Consts.Biomes.Shrubland:
                    return 0.5;
                    break;
                case Consts.Biomes.Grassland:
                    return 0.2;
                    break;
                case Consts.Biomes.TemperateDeciduousForest:
                    return 0.5;
                    break;
                case Consts.Biomes.Snow:
                    return 0.1;
                    break;
                case Consts.Biomes.Taiga:
                    return 0.1;
                    break;
                case Consts.Biomes.TemperateRainForest:
                    return 0.6;
                    break;
                case Consts.Biomes.SubtropicalDesert:
                    return 0.07;
                    break;
                case Consts.Biomes.TropicSeasonalForest:
                    return 0.4;
                    break;
                case Consts.Biomes.TropicRainforest:
                    return 0.7;
                    break;
                default:return 0.5;
                    break;
            }
        }
    }

    namespace Consts
    {
        public static class Tools {public static Random doblstn = new Random(); }
        public static class Blockfill { public const byte air = 0, water = 1, stone = 2, sand = 3, grass = 4, wood = 5, ice = 6, brownground = 7, acacialeavesgreen = 8, acacaleavesorange = 9, dinamite = 10, whiteflowers = 11, purpleflowers = 12, orangeflowers = 13, blueflowers = 14, redflowers = 15,Tapijt=17, lava = 24, coral = 25; }
        public enum Biomes {Ocean=0,Beach,Scorched,Bare,Tundra,TemperateDesert,Shrubland,Grassland,TemperateDeciduousForest,Snow,Taiga,TemperateRainForest,SubtropicalDesert,TropicSeasonalForest,TropicRainforest }
        public enum Sunstate { sunrise,day,sunset,night }
    }
}