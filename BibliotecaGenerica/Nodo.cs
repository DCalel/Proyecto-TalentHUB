using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica
{
    public class Nodo<T>
    {
        public T Valor { get; set; }
        public Nodo<T> Izquierda { get; set; }
        public Nodo<T> Derecha { get; set; }
        public int Posicion{get; set;}
        public int FactorEquilibrio { get; set; }
        public int AlturaIzquierda { get; set; }
        public int AlturaDerecha { get; set; }
        public int ContadorComparaciones { get; set; }
        public int ContadorRotaciones { get; set; }
        public int Profundidad { get; set; }

    }
}
