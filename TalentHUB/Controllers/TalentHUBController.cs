using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentHUB.Models;
using TalentHUB.Models.Data;
using Newtonsoft.Json;
using System.IO;
using BibliotecaGenerica;
using BibliotecaGenerica.DES;
using BibliotecaGenerica.DES.Cypher;
using BibliotecaGenerica.RSA;
using System.Text;
using System.Diagnostics;


namespace TalentHUB.Controllers
{
    public class TalentHUBController : Controller
    {
        // GET: TalentHUBController
        public ActionResult Index()
        {
            return View(Singleton.Instance.ListRegisters);
        }

        public ActionResult Details()
        {
            ViewBag.Encrypt = string.Join("", Singleton.Instance.Encrypt.ToArray());
            ViewBag.Decrypt = string.Join("", Singleton.Instance.Decrypt.ToArray());

            ViewBag.EncryptLetter = string.Join("", Singleton.Instance.EncryptLetter.ToArray());
            ViewBag.DecryptLetter = string.Join("", Singleton.Instance.DecryptLetter.ToArray());

            Singleton.Instance.Encrypt.Clear();
            Singleton.Instance.Decrypt.Clear();
            Singleton.Instance.EncryptLetter.Clear();
            Singleton.Instance.DecryptLetter.Clear();

            return View(Singleton.Instance.RegisterFound);
        }

        public ActionResult Name()
        {
            return View(Singleton.Instance.RegisterFound);
        }

        public ActionResult Message()
        {
            ViewBag.MessageSend = Singleton.Instance.MessageSend;
            ViewBag.MessageEncrypt = string.Join("", Singleton.Instance.MessageEncrypt.ToArray());
            ViewBag.MessageDecrypt = Singleton.Instance.MessageDecrypt;

            return View(Singleton.Instance.ListRegisters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReadFile(IFormFile FileUploaded)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            if (FileUploaded == null)
            {
                return RedirectToAction(nameof(Index));
            }
            string message = "";
            try
            {
                var file = new StreamReader(FileUploaded.OpenReadStream());
                {
                    string info = file.ReadToEnd();
                    foreach (string Row in info.Split("\n"))
                    {
                        if (string.IsNullOrEmpty(Row))
                        {
                            TempData["Message"] = "Los Registros han sido cargados correctamente aunque se encontro una o varias lineas en blanco";
                            Singleton.Instance.ListRegisters = Singleton.Instance.AVLDPI.Recorrido();
                            return RedirectToAction(nameof(Index));
                        }

                        string JsonAction = Row.Split(";")[0];
                        string JsonString = Row.Split(";")[1];


                        Register NewRegister = JsonConvert.DeserializeObject<Register>(JsonString);

                        if (JsonAction == "INSERT")
                        {
                            message = "1";
                            ValidationRecluiter(NewRegister);
                            HuffmanEncode(NewRegister);
                            Insert(NewRegister);
                            GetLetter(NewRegister);
                        }
                        else if (JsonAction == "PATCH")
                        {
                            message = "2";
                            HuffmanEncode(NewRegister);
                            Patch(NewRegister);
                        }
                        else
                        {
                            message = "3";
                            Delete(NewRegister);
                        }
                    }
                }
                TempData["Message"] = "Los Registros han sido cargados correctamente";
                Singleton.Instance.ListRegisters = Singleton.Instance.AVLDPI.Recorrido();
                if (Singleton.Instance.ListRegisters.Count == 0)
                {
                    Singleton.Instance.ListOfList = Singleton.Instance.AVLName.Recorrido();
                    foreach (var item in Singleton.Instance.ListOfList)
                    {
                        foreach (var item2 in item)
                        {
                            Singleton.Instance.ListRegisters.Add(item2);
                        }
                    }
                }

                timer.Stop();
                String time = timer.Elapsed.ToString("mm\\:ss\\.fff");
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {
                TempData["Message"] = message;
                Singleton.Instance.ListRegisters = Singleton.Instance.AVLDPI.Recorrido();
                return RedirectToAction(nameof(Index));
            }
        }

        public static void Insert(Register NewRegister)
        {
            Singleton.Instance.AVLName.Agregar(NewRegister, NewRegister.InsertByName);
            Singleton.Instance.AVLDPI.Agregar(NewRegister, NewRegister.InsertByDpi);

        }

        public static void Patch(Register NewRegister)
        {
            Singleton.Instance.AVLName.Actualizar(NewRegister, NewRegister.InsertByName, NewRegister.InsertByDpi);
            Singleton.Instance.AVLDPI.Borrar(NewRegister, NewRegister.InsertByDpi);
            Singleton.Instance.AVLDPI.Agregar(NewRegister, NewRegister.InsertByDpi);
        }

        public static void Delete(Register NewRegister)
        {
            Singleton.Instance.AVLName.Borrar(NewRegister, NewRegister.InsertByName, NewRegister.InsertByDpi);
            Singleton.Instance.AVLDPI.Borrar(NewRegister, NewRegister.InsertByDpi);
        }

        public void ValidationRecluiter(Register NewRegister)
        {
            RSA rsa = new RSA();

            if (!Directory.Exists(@"RSA"))
            {
                Directory.CreateDirectory("RSA");
            }

            for (int i = 0; i < NewRegister.Companies.Length; i++)
            {
                string path = @"RSA/" + NewRegister.recluiter + "-" + NewRegister.Companies[i];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!(System.IO.File.Exists(path + "/private.txt") && System.IO.File.Exists(path + "/public.txt")))
                {
                    long[] keys = rsa.GetKeys();
                    System.IO.File.AppendAllText(path + "/private.txt", keys[1] + "," + keys[2]);
                    System.IO.File.AppendAllText(path + "/public.txt", keys[0] + "," + keys[2]);
                }
            }
        }

        public ActionResult SendMessage(string recluiter, string companie, string message)
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                Singleton.Instance.MessageSend = message;
                string[] keysFound = Directory.GetDirectories(@"RSA/");

                for (int i = 0; i < keysFound.Length; i++)
                {
                    keysFound[i] = Path.GetFileName(keysFound[i]);
                }

                string[] keys = { "", "" };
                for (int i = 0; i < keysFound.Length; i++)
                {
                    if (keysFound[i].Split("-")[0] == recluiter && keysFound[i].Split("-")[1] == companie)
                    {
                        keys = Directory.GetFiles(@"RSA/" + keysFound[i] + "/");
                    }
                }

                var file = new StreamReader(keys[1]);
                {
                    string info = file.ReadToEnd();
                    RSA rsa = new RSA();
                    string[] auxkesy = info.Split(",");
                    long[] keyPublic = { long.Parse(auxkesy[0]), long.Parse(auxkesy[1]) };
                    Singleton.Instance.MessageEncrypt = rsa.Encrypt(keyPublic, message);
                }

                var file2 = new StreamReader(keys[0]);
                {
                    string info = file2.ReadToEnd();

                    RSA rsa = new RSA();
                    string[] auxkesy = info.Split(",");
                    long[] keyPrivate = { long.Parse(auxkesy[0]), long.Parse(auxkesy[1]) };
                    Singleton.Instance.MessageDecrypt = rsa.Decrypt(keyPrivate, Singleton.Instance.MessageEncrypt);
                }

                timer.Stop();
                String time = timer.Elapsed.ToString("mm\\:ss\\.fff");
                return RedirectToAction(nameof(Message));
            }
            catch (Exception)
            {
                TempData["Message"] = "Nombre del reclutador o campañia incorrecto, intente de nuevo por favor";
                return RedirectToAction(nameof(Message));
            }
        }

        public ActionResult SearchByName(string IdName)
        {
            try
            {
                Register NewRegister = new Register()
                {
                    Name = IdName
                };

                List<Register> Found = new List<Register>();
                
                Stopwatch timer = new Stopwatch();
                timer.Start();
                GetRegister(Found, NewRegister, NewRegister.InsertByName, true);
                timer.Stop();
                String time = timer.Elapsed.ToString("mm\\:ss\\.fff");

                if (Singleton.Instance.RegisterFound == null)
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return RedirectToAction(nameof(Index));
                }

                if (Singleton.Instance.RegisterFound.Contains(null))
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return RedirectToAction(nameof(Index));
                }

                if (Singleton.Instance.RegisterFound.Count == 0)
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Name));
            }
            catch (Exception)
            {
                TempData["Message"] = "Coloque Datos Para Buscar";
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult SearchByDpi(string IdDpi, string password)
        {
            try
            {
                Singleton.Instance.EncryptLetter.Clear();
                Singleton.Instance.DecryptLetter.Clear();
                if (password == null)
                {
                    TempData["Message"] = "Contraseña invalida";
                    Singleton.Instance.Encrypt.Clear();
                    Singleton.Instance.Decrypt.Clear();
                    Singleton.Instance.RegisterFound.Clear();
                    return RedirectToAction(nameof(Details));
                }
                if (password.Length != 8)
                {
                    TempData["Message"] = "Contraseña invalida";
                    Singleton.Instance.Encrypt.Clear();
                    Singleton.Instance.Decrypt.Clear();
                    Singleton.Instance.RegisterFound.Clear();
                    return RedirectToAction(nameof(Details));
                }

                Register NewRegister = new Register
                {
                    Dpi = IdDpi
                };

                List<Register> Found = new List<Register>();


                GetRegister(Found, NewRegister, NewRegister.InsertByDpi, false);

                if (Found == null)
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return RedirectToAction(nameof(Details));
                }


                if (Found.Contains(null))
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return RedirectToAction(nameof(Details));
                }

                GetLetter(NewRegister);

                Stopwatch timer = new Stopwatch();
                timer.Start();
                GetConversation(NewRegister, password);
                Descompresedletter(NewRegister);
                timer.Stop();
                String time = timer.Elapsed.ToString("mm\\:ss\\.fff");

                Singleton.Instance.RegisterFound = Found;
                return RedirectToAction(nameof(Details));
            }
            catch (Exception)
            {
                TempData["Message"] = "Coloque Datos Para Buscar";
                return RedirectToAction(nameof(Details));
            }
        }

        public void GetConversation(Register newRegister, string password)
        {
            string[] LettersFound = Directory.GetFiles(@"inputs/");
            List<string> ConvOfaPerson = new List<string>();

            for (int i = 0; i < LettersFound.Length; i++)
            {
                LettersFound[i] = Path.GetFileName(LettersFound[i]);
            }

            for (int i = 0; i < LettersFound.Length; i++)
            {
                if (LettersFound[i].Split("-")[1] == newRegister.Dpi && LettersFound[i].Split("-")[0] == "CONV")
                {
                    ConvOfaPerson.Add(LettersFound[i]);
                }
            }

            for (int i = 0; i < ConvOfaPerson.Count; i++)
            {
                var file = new StreamReader("inputs/" + ConvOfaPerson[i]);
                {
                    string info = file.ReadToEnd();
                    foreach (string Row in info.Split("\n"))
                    {
                        DES des = new DES();

                        if (!Directory.Exists(@"crypted"))
                        {
                            Directory.CreateDirectory("crypted");
                        }

                        List<string> message = new List<string>();
                        int index = 0;
                        int aux = Row.Length;
                        while (aux > 0)
                        {
                            if (aux >= 8)
                            {
                                message.Add(Row.Substring(index, 8));
                                aux -= 8;
                                index += 8;
                            }
                            else
                            {
                                int d = 8 - aux;
                                message.Add(Row.Substring(index, aux));

                                for (int j = 0; j < d; j++)
                                {
                                    message[message.Count - 1] += "z";
                                }
                                aux = 0;
                            }

                        }
                        List<string> compressed = new List<string>();
                        List<string> compressed2 = new List<string>();
                        foreach (string item in message)
                        {
                            string encoded = des.Encrypt(item, password);
                            string textenconded = Operations.BitsToString(encoded);
                            compressed.Add(textenconded);
                        }

                        if (i == 0)
                        {
                            Singleton.Instance.Encrypt.Add(string.Join("", compressed.ToArray()) + "\n");
                        }

                        foreach (var item in compressed)
                        {
                            string encoded = des.Decrypt(item, password);
                            string textenconded = Operations.BitsToString(encoded);
                            compressed2.Add(textenconded);
                        }


                        if (i == 0)
                        {
                            if (compressed2[compressed2.Count-1].Contains("z"))
                            {
                                compressed2[compressed2.Count - 1] = compressed2[compressed2.Count - 1].Split("\r")[0];
                            }

                            Singleton.Instance.Decrypt.Add(string.Join("", compressed2.ToArray()) + "\n");
                        }


                        string path = @"crypted/" + ConvOfaPerson[i];

                        string json = string.Join(",", compressed.ToArray());
                        System.IO.File.AppendAllText(path, json);
                    }
                }

            }
        }

        public void GetLetter(Register newRegister)
        {
            Singleton.Instance.EncryptLetter.Clear();
            string[] LettersFound = Directory.GetFiles(@"inputs/");
            List<string> LettersOfaPerson = new List<string>();

            for (int i = 0; i < LettersFound.Length; i++)
            {
                LettersFound[i] = Path.GetFileName(LettersFound[i]);
            }

            for (int i = 0; i < LettersFound.Length; i++)
            {
                if (LettersFound[i].Split("-")[1] == newRegister.Dpi && LettersFound[i].Split("-")[0] == "REC")
                {
                    LettersOfaPerson.Add(LettersFound[i]);
                }
            }

            for (int i = 0; i < LettersFound.Length; i++)
            {
                LettersFound[i] = Path.GetFileName(LettersFound[i]);
            }


            for (int i = 0; i < LettersOfaPerson.Count; i++)
            {
                bool aux = true;
                var file = new StreamReader("inputs/" + LettersOfaPerson[i]);
                {
                    string info = file.ReadToEnd();

                    LZW lzw = new LZW();
                    List<int> compressed = lzw.Compress(info);

                    if (!Directory.Exists(@"compressed"))
                    {
                        Directory.CreateDirectory("compressed");
                    }

                    string path = @"compressed/" + LettersOfaPerson[i];
                    
                    string json = string.Join(",", compressed.ToArray());
                    Singleton.Instance.EncryptLetter.Add(json);

                    if (!System.IO.File.Exists(path))
                    {
                        if (aux)
                        {
                            System.IO.File.AppendAllText(path, json);
                            aux = false;
                        }
                        else
                        {
                            System.IO.File.AppendAllText(path, "," + json);
                        }
                    }
                }
            }

        }

        public void Descompresedletter(Register newRegister)
        {
            string[] LettersFound = Directory.GetFiles(@"compressed/");
            List<string> LettersOfaPerson = new List<string>();

            for (int i = 0; i < LettersFound.Length; i++)
            {
                LettersFound[i] = Path.GetFileName(LettersFound[i]);
            }

            for (int i = 0; i < LettersFound.Length; i++)
            {
                if (LettersFound[i].Split("-")[1] == newRegister.Dpi)
                {
                    LettersOfaPerson.Add(LettersFound[i]);
                }
            }

            for (int i = 0; i < LettersOfaPerson.Count; i++)
            {
                var file = new StreamReader("compressed/" + LettersOfaPerson[i]);
                {
                    string info = file.ReadToEnd();
                    foreach (string Row in info.Split("\n"))
                    {
                        LZW lzw = new LZW();
                        List<int> compressed = new List<int>();

                        foreach (string item in Row.Split(","))
                        {
                            compressed.Add(Convert.ToInt32(item));
                        }

                        string decompressed = lzw.Decompress(compressed);
                        Singleton.Instance.DecryptLetter.Add(decompressed +"\n");

                        if (!Directory.Exists(@"decompressed"))
                        {
                            Directory.CreateDirectory("decompressed");
                        }
                        string path = @"decompressed/" + LettersOfaPerson[i];
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.File.AppendAllText(path, decompressed);
                        }
                    }
                }
            }




        }

        public void GetRegister(List<Register> Found, Register NewRegister, Delegate InsertBy, bool verficador)
        {
            try
            {
                if (verficador == true)
                {
                    Found = Singleton.Instance.AVLName.Busqueda(NewRegister, InsertBy);
                    if (Found != null)
                    {
                        Singleton.Instance.RegisterFound = Found;
                    }
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                }
                else
                {
                    Found.Add(Singleton.Instance.AVLDPI.Busqueda(NewRegister, InsertBy));
                }

                if (Found == null)
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return;
                }


                if (Found.Contains(null))
                {
                    TempData["Message"] = "La Persona No Existe o Coloco Mal Los Datos";
                    return;
                }

                string path = GetPath(Found);
                CreateFile(path, Found);
                TempData["Message"] = "La busqueda tuvo exito y el resultado se almaceno en la carpeta outputs";
            }
            catch (Exception)
            {
                TempData["Message"] = "Coloque Datos Para Buscar";
                return;
            }
        }

        public void CreateFile(string path, List<Register> Found)
        {
            System.IO.File.Delete(path);
            for (int i = 0; i < Found.Count; i++)
            {
                //HuffamDecode(Found[i]);
                string json = JsonConvert.SerializeObject(Found[i]) + "\n";
                System.IO.File.AppendAllText(path, json);
            }
        }

        public string GetPath(List<Register> Found)
        {
            if (!Directory.Exists(@"outputs"))
            {
                Directory.CreateDirectory("outputs");
            }
            string path = @"outputs/" + Found[0].Name + ".csv";

            return path;
        }

        public void HuffmanEncode(Register NewRegister)
        {
            for (int i = 0; i < NewRegister.Companies.Length; i++)
            {
                List<string> CodedMessage = new List<string>();
                List<string> ID = new List<string>();
                char[] auxDpi = NewRegister.Dpi.ToCharArray();

                GetTableFrequency(NewRegister.Companies[i], CodedMessage);
                CreateHuffmanTree(Singleton.Instance.ListHuffman);
                CreateHuffmanCode("", Singleton.Instance.ListHuffman[0], CodedMessage);

                if (auxDpi.Length > CodedMessage.Count)
                {
                    DpiIsHigher(auxDpi, CodedMessage, ID);
                }
                else
                {
                    DpiIsLess(auxDpi, CodedMessage, ID);
                }
                string DpiEncoded = string.Join("", ID);
                NewRegister.Companies[i] += ":" + DpiEncoded;
            }
        }

        public void GetTableFrequency(string message, List<string> CodedMessage)
        {
            Singleton.Instance.ListHuffman.Clear();
            string[] table = new string[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                string aux = message.Substring(i, 1);
                table[i] = aux;
                CodedMessage.Add(aux);
            }

            for (int i = 0; i < table.Length; i++)
            {
                Huffman<Register> auxHuffman = new Huffman<Register>();

                if (Singleton.Instance.ListHuffman.Exists(x => x.Caracter == table[i]))
                {
                    Singleton.Instance.ListHuffman.Find(x => x.Caracter == table[i]).IncrementarFrecuencia();
                }
                else
                {
                    auxHuffman.Agregar(table[i]);
                    Singleton.Instance.ListHuffman.Add(auxHuffman);
                }
            }

            Singleton.Instance.ListHuffman = Singleton.Instance.ListHuffman.OrderBy(x => x.Frecuencia).ToList();
        }

        public void CreateHuffmanTree(List<Huffman<Register>> huffman)
        {
            while (huffman.Count > 1)
            {
                Huffman<Register> auxHuffman = new Huffman<Register>();
                Huffman<Register> node1 = huffman[0];
                huffman.RemoveAt(0);
                Huffman<Register> node2 = huffman[0];
                huffman.RemoveAt(0);

                auxHuffman.UnirNodos(node1, node2);

                huffman.Add(auxHuffman);
                huffman = huffman.OrderBy(x => x.Frecuencia).ToList();
                Singleton.Instance.ListHuffman = huffman;
            }
        }

        public void CreateHuffmanCode(string code, Huffman<Register> node, List<string> CodedMessage)
        {
            if (node == null)
            {
                return;
            }

            if (node.Izquierda == null && node.Derecha == null)
            {
                node.Codigo = code;

                for (int i = 0; i < CodedMessage.Count; i++)
                {
                    if (CodedMessage[i] == node.Caracter)
                    {
                        CodedMessage[i] = node.Codigo;
                    }
                }
            }
            CreateHuffmanCode(code + "0", node.Izquierda, CodedMessage);
            CreateHuffmanCode(code + "1", node.Derecha, CodedMessage);
        }

        public void DpiIsHigher(char[] auxDpi, List<string> CodedMessage, List<string> ID)
        {
            for (int i = 0; i < CodedMessage.Count; i++)
            {
                CreateID(auxDpi, CodedMessage, ID, i);
            }
            for (int i = 0; i < (auxDpi.Length - CodedMessage.Count); i++)
            {
                string aux = auxDpi[CodedMessage.Count + i].ToString();
                ID.Add(aux);
            }
        }

        public void DpiIsLess(char[] auxDpi, List<string> CodedMessage, List<string> ID)
        {
            for (int i = 0; i < auxDpi.Length; i++)
            {
                CreateID(auxDpi, CodedMessage, ID, i);
            }
        }

        public void CreateID(char[] auxDpi, List<string> CodedMessage, List<string> ID, int i)
        {
            int aux = Convert.ToInt32(auxDpi[i].ToString());
            int auxId = Convert.ToInt32(CodedMessage[i]);
            string auxNum = "";
            if (auxId == 0 || CodedMessage[i].Length != Convert.ToString(auxId).Length)
            {
                for (int j = 0; j < CodedMessage[i].Length; j++)
                {
                    auxNum += "1";
                    auxId = Convert.ToInt32(auxNum);
                }
            }
            auxId = Convert.ToInt32(auxId.ToString(), 2);//Conversion de base binaria a base decimal
            ID.Add(Convert.ToString(aux + auxId));
        }

        public void HuffamDecode(Register NewRegister)
        {
            for (int i = 0; i < NewRegister.Companies.Length; i++)
            {
                string company = NewRegister.Companies[i].Split(":")[0];
                string ID = NewRegister.Companies[i].Split(":")[1];

                List<string> CodedMessage = new List<string>();
                GetTableFrequency(company, CodedMessage);
                CreateHuffmanTree(Singleton.Instance.ListHuffman);
                CreateHuffmanCode("", Singleton.Instance.ListHuffman[0], CodedMessage);

                List<string> Decoder = new List<string>();
                int aux = 0;
                if (CodedMessage.Count < 13)
                {
                    CodedMessageIsLess(CodedMessage, Decoder, ID, aux);
                }
                else
                {
                    CodedMessageIsHigher(CodedMessage, Decoder, ID, aux);
                }

                List<string> DecodedDpi = new List<string>();

                if (CodedMessage.Count < 13)
                {
                    DecoderIsHigher(CodedMessage, Decoder, DecodedDpi);
                }
                else
                {
                    DecoderIsLess(CodedMessage, Decoder, DecodedDpi);
                }

                string EncondedDpi = string.Join("", DecodedDpi);

                //Evitan que se duplique el dpi decodificado en la compania
                //+1 es debido a que no existe el ":" en company o ID
                if (NewRegister.Companies[i].Length > (company.Length + ID.Length + 1))
                {
                    //NewRegister.Companies[i].Split(":")[2] = ":" + EncondedDpi;
                }
                else
                {
                    //NewRegister.Companies[i] += ":" + EncondedDpi;
                }

            }
        }

        public void CodedMessageIsLess(List<string> CodedMessage, List<string> Decode, string ID, int aux)
        {
            //El id tiene el mismo error de guardar tres ceros(000) solo guarda uno (0), esto provoca que se pierda la codificación original
            for (int i = 0; i < CodedMessage.Count; i++)
            {
                CodedMessage[i] = Convert.ToString(Convert.ToInt32(CodedMessage[i].ToString(), 2));
            }

            for (int i = 0; i < CodedMessage.Count; i++)
            {
                Decode.Add(ID.Substring(aux, CodedMessage[i].Length));
                aux += CodedMessage[i].Length;
            }
            for (int i = 0; i < (13 - CodedMessage.Count); i++)
            {
                Decode.Add(ID.Substring(aux, 1));
                aux += 1;
            }
        }

        public void CodedMessageIsHigher(List<string> CodedMessage, List<string> Decode, string ID, int aux)
        {
            for (int i = 0; i < CodedMessage.Count; i++)
            {
                CodedMessage[i] = Convert.ToString(Convert.ToInt32(CodedMessage[i].ToString(), 2));
            }
            for (int i = 0; i < 13; i++)
            {
                Decode.Add(ID.Substring(aux, CodedMessage[i].Length));
                aux += CodedMessage[i].Length;
            }
        }

        public void DecoderIsHigher(List<string> CodedMessage, List<string> Decoder, List<string> DecodedDpi)
        {
            for (int i = 0; i < CodedMessage.Count; i++)
            {
                CreateDpi(CodedMessage, Decoder, DecodedDpi, i);
            }
            for (int i = 0; i < (13 - CodedMessage.Count); i++)
            {
                DecodedDpi.Add(Decoder[CodedMessage.Count + i]);
            }
        }

        public void DecoderIsLess(List<string> CodedMessage, List<string> Decoder, List<string> DecodedDpi)
        {
            for (int i = 0; i < Decoder.Count; i++)
            {
                CreateDpi(CodedMessage, Decoder, DecodedDpi, i);
            }
        }

        public void CreateDpi(List<string> CodedMessage, List<string> Decoder, List<string> DecodedDpi, int i)
        {
            int aux1 = Convert.ToInt32(Decoder[i]);
            int aux2 = Convert.ToInt32(CodedMessage[i]);
            string auxNum = "";

            if (aux2 == 0 || CodedMessage[i].Length != Convert.ToString(aux2).Length)
            {
                for (int j = 0; j < CodedMessage[i].Length; j++)
                {
                    auxNum += "1";
                    aux2 = Convert.ToInt32(auxNum);
                }
            }
            DecodedDpi.Add(Convert.ToString(aux1 - aux2));
        }


    }
}