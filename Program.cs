using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace vhex
{
    class Program
    {

        static string indir;
        static string outdir;


        static void usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("vhex <dir> <out>");
            Console.WriteLine();
            Console.WriteLine("Unpacks VRChat HTTP cache files");
        }

        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }




        static string filenameFromPath(string path)
        {
            var poss = path.Split((char)0x5C);

            return poss[poss.Length - 1];

        }

        static string findExtension(byte[] data)
        {
            switch (data[0])
            {
                case 0x89:
                    return ".png";

                case 0x55:
                    return ".ufs";


                default:
                    return "";
            }
        }
      


        static int Main(string[] args)
        {
            if ( args.Length < 2 )
            {
                usage();
                return -1; 
            }

            indir = args[0];
            outdir = args[1];

            if (!Directory.Exists(indir))
            {
                Console.WriteLine("Directory {0} is inaccessible or does not exist.", indir);
                return -1;
            }

            Console.WriteLine("Building tree...");
            var alldirs = Directory.GetFiles(indir, "*", SearchOption.AllDirectories);


            var match = new byte[4] { 0x0d,0x0a,0x0d,0x0a };
            foreach (string cfile in alldirs)
            {
                var data = File.ReadAllBytes(cfile);
                {
                    var name = filenameFromPath(cfile);
                    var wut = PatternAt(data, match);
                    var startpos = wut.First() + 4;
                    var len = data.Length - startpos;

                    byte[] pdata = new byte[len];

                    Array.Copy(data, startpos, pdata,0,len);
                    Console.WriteLine("OK {0}", name);
                    File.WriteAllBytes(outdir + "/" + name + findExtension(pdata),pdata);

                   



                }

            }



                return 0; 
        }
    }
}
