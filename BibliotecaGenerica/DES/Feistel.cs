using System;
using System.Collections.Generic;
using System.Text;
using BibliotecaGenerica.DES.Boxes;

namespace BibliotecaGenerica.DES
{
    public class Feistel
    {
        public string FeistelProcess(string left, string right, string key)
        {
            string AuxRight = right;
            string Feistelresult = GetResult(right, key);

            right = Operations.Xor(Feistelresult, left);
            left = AuxRight;

            return left + right;
        }

        public string GetResult(string right, string key)
        {
            string expanded = Operations.MatrixOperation(Pbox.E, right);
            string xor = Operations.Xor(expanded, key);
            string sbox = Operations.FeistelSbox(xor);
            return Operations.MatrixOperation(Pbox.P, sbox);
        }
    }
}
