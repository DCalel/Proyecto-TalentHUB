using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibliotecaGenerica;

namespace TalentHUB.Models.Data
{
    public sealed class Singleton
    {
        private readonly static Singleton _instance = new Singleton();

        //-------------Search---------------------
        public List<Register> ListRegisters;
        public List<Register> RegisterFound;
        public List<List<Register>> ListOfList;
        public AVLRepetidos<Register> AVLName;

        //-------------Huffman--------------------
        public ArbolAVL<Register> AVLDPI;
        public List<Huffman<Register>> ListHuffman;
        public Huffman<Register> huffman;

        //-------------LZW------------------------
        public List<string> EncryptLetter;
        public List<string> DecryptLetter;

        //-------------DES------------------------
        public List<string> Encrypt;
        public List<string> Decrypt;

        //-------------RSA------------------------
        public string MessageSend;
        public List<long> MessageEncrypt;
        public string MessageDecrypt;

        private Singleton()
        {
            //-------------Search---------------------
            ListRegisters = new List<Register>();
            RegisterFound = new List<Register>();
            ListOfList = new List<List<Register>>();
            AVLName = new AVLRepetidos<Register>();

            //-------------Huffman--------------------
            AVLDPI = new ArbolAVL<Register>();
            ListHuffman = new List<Huffman<Register>>();
            huffman = new Huffman<Register>();

            //-------------LZW------------------------
            EncryptLetter = new List<string>();
            DecryptLetter = new List<string>();

            //-------------DES------------------------
            Encrypt = new List<string>();
            Decrypt = new List<string>();

            //-------------RSA------------------------
            MessageSend = new string("");
            MessageEncrypt = new List<long>();
            MessageDecrypt = new string("");
        }

        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
