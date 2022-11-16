using System;
using System.Collections.Generic;

namespace BibliotecaGenerica
{
    public class Heap<T> : Nodo<T> where T : IComparable    
    {

        Nodo<T> NodoPadre = new Nodo<T>();
		Nodo<T> AuxNodo = new Nodo<T>();
		Nodo<T> AuxUltimo = new Nodo<T>();
		int PosicionNodo = 0;
		bool Esta_Insertado;
		bool Eliminado;


		public void Agregar(T valor, Delegate delegado)
		{
			PosicionNodo++;
			Esta_Insertado = false;
			Insercion(NodoPadre, valor, delegado);//Manda a llamar la funcion que insertara el elemento
		}

		public void Insercion(Nodo<T> padre, T valor, Delegate delegado)
		{
			//Si el nodo esta vacio entonces se insertara en ese lugar, si no se mueve
			if (padre.Valor == null)
			{
				padre.Valor = valor;//Inserta el elemento
				padre.Posicion = PosicionNodo;
				Esta_Insertado = true;

				//Inicializa los hijos del nodo insertado
				padre.Izquierda = new Nodo<T>();
				padre.Derecha = new Nodo<T>();
			}
			else if(padre.Derecha.Valor != null && padre.Izquierda.Valor != null)//Forma Invariante
			{
				Insercion(padre.Izquierda, valor, delegado);
				
				if (Esta_Insertado == false)
				{
					Insercion(padre.Derecha, valor, delegado);
				}
			}
			else if (padre.Izquierda.Valor == null && (PosicionNodo / 2) == padre.Posicion)
			{
				Insercion(padre.Izquierda, valor, delegado);
			}
			else if (padre.Derecha.Valor == null && ((PosicionNodo - 1) / 2) == padre.Posicion)
			{
				Insercion(padre.Derecha, valor, delegado);
			}

			OrdenInvariante(padre, delegado);
		}

		
		public T Borrar(Delegate delegado)
		{
			Eliminado = false;
			return Eliminacion(NodoPadre, delegado);//Manda a llamar la funcion que eliminara el elemento
		}

		public T Eliminacion(Nodo<T> padre, Delegate delegado)
		{
			AuxNodo = NodoPadre;
			if (PosicionNodo == 1)
			{
				PosicionNodo--;
				return NodoPadre.Valor;
			}
			else
			{
				if (padre.Posicion == PosicionNodo )
				{
					AuxUltimo.Valor = padre.Valor;
					NodoPadre.Valor = AuxUltimo.Valor;
					padre.Valor = padre.Derecha.Valor;
					OrdenInvariante(NodoPadre, delegado);
					Eliminado = true;
					PosicionNodo--;
					return AuxNodo.Valor;
				}
				else if(padre.Derecha.Valor != null && padre.Izquierda.Valor != null)//Forma Invariante
				{
					Eliminacion(padre.Izquierda, delegado);
					
					if(Eliminado == false)
					{
						Eliminacion(padre.Derecha, delegado);
					}
					
				}
				else if (padre.Izquierda.Valor != null && (PosicionNodo / 2) == padre.Posicion)
				{
					Eliminacion(padre.Izquierda, delegado);
				}
				else if (padre.Derecha.Valor != null && ((PosicionNodo - 1) / 2) == padre.Posicion)
				{
					Eliminacion(padre.Derecha, delegado);
				}
				R(NodoPadre, delegado);
				return AuxNodo.Valor;
			}
		}
		
		public void OrdenInvariante(Nodo<T> padre, Delegate delegado)
		{
			Nodo<T> AuxNodo = new Nodo<T>();
			AuxNodo.Valor = padre.Valor;
			if (padre.Izquierda.Valor != null && Convert.ToInt32(delegado.DynamicInvoke(padre.Izquierda.Valor, padre.Valor)) == 1)
			{
				padre.Valor = padre.Izquierda.Valor;
				padre.Izquierda.Valor = AuxNodo.Valor;
			}
			else if (padre.Derecha.Valor != null && Convert.ToInt32(delegado.DynamicInvoke(padre.Derecha.Valor, padre.Valor)) == 1)
			{
				padre.Valor = padre.Derecha.Valor;
				padre.Derecha.Valor = AuxNodo.Valor;
			}
		}

		private void R(Nodo<T> padre, Delegate delegado)
		{
			if (padre.Valor != null)
			{
				R(padre.Izquierda, delegado);
				OrdenInvariante(padre, delegado);
				R(padre.Derecha, delegado);
			}
		}






































		
    }
}