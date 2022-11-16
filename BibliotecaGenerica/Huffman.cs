using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica
{
    public class Huffman<T> where T : IComparable
    {
        public string Caracter { get; set; }
        public int Frecuencia { get; set; }
        public string Codigo { get; set; }
        public Huffman<T> Padre { get; set; }
        public Huffman<T> Izquierda { get; set; }
        public Huffman<T> Derecha { get; set; }
        public bool Hoja { get; set; }

        public void Agregar(string valor)
        {
            Caracter = valor;
            Frecuencia = 1;
            Codigo = "";
            Hoja = true;

            Derecha = null;
            Izquierda = null;
            Padre = null;

        }

        public void UnirNodos(Huffman<T> nodo1, Huffman<T> nodo2)
        {
            Codigo = "";
            Hoja = false;
            Padre = null;

            Izquierda = nodo1;
            Derecha = nodo2;

           // nodo1.Padre = nodo2.Padre = this;

            Frecuencia = nodo1.Frecuencia + nodo2.Frecuencia;
            Caracter = nodo1.Caracter + nodo2.Caracter;            
        }


        public int OrdernarMenorMayor(int frecuencia1, int frecuencia2)
        {
            return frecuencia1.CompareTo(frecuencia2);
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public void IncrementarFrecuencia()
        {
            Frecuencia++;
        }
    }
}
