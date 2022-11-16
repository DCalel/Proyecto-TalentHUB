using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica.RSA
{
    public class Keys
    {
        public long[] GeneratorKeys(int p, int q)
        {
            //n
            int n = p * q;
            //fi
            int fi = (p - 1) * (q - 1);

            //e
            int e = 0;
            int Contador = 1;
            bool Estado = true;
            while (Estado && Contador < fi)
            {
                ++Contador;
                if (EncontrarPrimo(Contador) && fi % Contador != 0)
                {
                    e = Contador;
                    Estado = false;
                }
            }

            //d
            int d = 0;
            for (int i = e + 1; i < 1000000; i++)
            {
                int auxd = i * e;
                if (auxd % fi == 1)
                {
                    d = i;
                    break;
                }
            }

            long[] keys = { e, d, n };
            return keys;
        }


        // Recursive function to
        // return gcd of a and b
        static int __gcd(int a, int b)
        {
            // Everything divides 0
            if (a == 0 || b == 0)
            {
                return 0;
            }

            // base case
            if (a == b)
            {
                return a;
            }

            // a is greater
            if (a > b)
            {
                return __gcd(a - b, b);
            }

            return __gcd(a, b - a);
        }

        // function to check and print if
        // two numbers are co-prime or not
        public bool Coprime(int a, int b)
        {

            if (__gcd(a, b) == 1)
            {
                //"Co-Prime"
                return true;
            }
            else
            {
                //"Not Co-Prime"
                return false;
            }
        }

        public static bool EncontrarPrimo(int n)
        {
            for (int i = 2; i < n; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
