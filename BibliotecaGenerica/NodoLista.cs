using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica
{
    public class NodoLista<T>
    {
        public List<T> Valor { get; set; }
        public NodoLista<T> Izquierda { get; set; }
        public NodoLista<T> Derecha { get; set; }
        public int Posicion { get; set; }
        public int FactorEquilibrio { get; set; }
        public int AlturaIzquierda { get; set; }
        public int AlturaDerecha { get; set; }
        public int ContadorComparaciones { get; set; }
        public int ContadorRotaciones { get; set; }
        public int Profundidad { get; set; }
    }
}
