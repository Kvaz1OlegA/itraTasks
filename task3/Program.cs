using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    class Program
    {
        public static bool Check(string[] strings)
        {
            for(int i=0; i<strings.Length-1; i++)
            {
                for(int j=i+1; j<strings.Length;j++)
                {
                    if (strings[i] == strings[j])
                        return false;
                }
            }
            return true;
        }
        static void Main(string[] args)
        {
            if (args.Length % 2 == 0 || !Check(args))
            {
                Console.WriteLine("Error");
                return;
            }
            //------------------------------------------------------------------------------------------------------------------
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            HMACSHA512 hMACSHA = new HMACSHA512();

            byte[] key = new byte[32];
            byte[] rnd = new byte[4];

            random.GetBytes(key);
            random.GetBytes(rnd);

            int choise = BitConverter.ToInt32(rnd, 0);
            choise = (Math.Abs(choise) % args.Length) + 1;

            rnd = BitConverter.GetBytes(choise);
            hMACSHA.Key = key;
            Console.WriteLine("HMACK - " + BitConverter.ToString(hMACSHA.ComputeHash(rnd)));
            //------------------------------------------------------------------------------------------------------------------

            int yourChoise, size = args.Length;

            Console.WriteLine("Your choise\n");
            int num = 1;
            foreach(string s in args)
            {
                Console.WriteLine(num + " - " + s);
                num++;
            }
            Console.WriteLine("0 - Exit");

            yourChoise = int.Parse(Console.ReadLine());
            if(yourChoise == 0)
            {
                return;
            }

            if (choise == yourChoise)
            {
                Console.WriteLine("Draw");
            }
            else if ((choise > yourChoise && choise <= (choise + (size - 1) / 2)) || (choise < yourChoise && choise <= (yourChoise + (size - 1) / 2) % size))
            {
                Console.WriteLine("Win");
            }
            else
            {
                Console.WriteLine("Lost");
            }

            Console.WriteLine("Your choise - " + args[yourChoise - 1]);
            Console.WriteLine("Bot choise - " + args[choise - 1]);
            Console.WriteLine("HMAC key - " + BitConverter.ToString(key));
            //------------------------------------------------------------------------------------------------------------------
        }
    }
}
