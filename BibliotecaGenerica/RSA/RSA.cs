using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica.RSA
{
    public class RSA
    {
        readonly Keys processkeys = new Keys();
        
        public List<long> Encrypt(long[] keys,string message)
        {
            List<long> messageCrypted = new List<long>();
            if (message == null)
            {
                return messageCrypted;
            }
            for (int i = 0; i < message.Length; i++)
            {
                messageCrypted.Add(ExpoBinary(message[i], keys[0], keys[1]));
            }
            return messageCrypted;
        }

        public string Decrypt(long[] keys,List<long> message)
        {
            List<char> charMessageDecrypted = new List<char>();
            for (int i = 0; i < message.Count; i++)
            {
                long m = ExpoBinary(message[i], keys[0], keys[1]);
                charMessageDecrypted.Add((char)m);
            }

            return string.Join("", charMessageDecrypted);
        }

        public long[] GetKeys()
        {
            Random rnd = new Random();
            // 153 prime numbers
            int[] ListPrime = { 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069 };
            int a = ListPrime[rnd.Next(0, 153)];
            int b = ListPrime[rnd.Next(0, 153)];
            while (processkeys.Coprime(a, b) != true)
            {
                a = ListPrime[rnd.Next(0, 153)];
                b = ListPrime[rnd.Next(0, 153)];
            }
            //0:e, 1:d, 2:n
            long[] keys = processkeys.GeneratorKeys(a, b);

            return keys;
        }

        public static long ExpoBinary(long a, long b, long m)
        {
            a %= m;
            long res = 1;
            while (b > 0)
            {
                if (!((b % 2) == 0))
                {
                    res = res * a % m;
                }
                a = a * a % m;
                b >>= 1;
            }
            return res;
        }
    }
}