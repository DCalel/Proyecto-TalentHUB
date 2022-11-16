using System;
using System.Collections.Generic;

namespace BibliotecaGenerica
{
	public class ArbolABB<T> : Nodo<T> where T : IComparable
	{
		Nodo<T> NodoPadre = new Nodo<T>();
		Nodo<T> NodoBorrar = new Nodo<T>();
		public int AuxProfundidadIz = 0;
		public int AuxProfundidadDe = 0;

		//Recibe el valor a insertar
		public void Agregar(T valor, Delegate delegado)
		{
			Insercion(NodoPadre, valor, delegado);//Manda a llamar la funcion que insertara el elemento
		}

		public void Borrar(T valor, Delegate delegado)
		{
			Eliminacion(NodoPadre, valor, delegado);//Manda a llamar la funcion que eliminara el elemento
		}

		public List<T> Recorrido()
		{
			Profundidad = 0;
			List<T> ListaArbol = new List<T>();
			R(NodoPadre, ListaArbol);
			Profundidad++;
			return ListaArbol;
		}

		private void R(Nodo<T> padre, List<T> Lista)
		{
			if (padre.Valor != null)
			{
				if (AuxProfundidadIz > Profundidad)
				{
					Profundidad++;
				}
				if (AuxProfundidadDe > Profundidad)
				{
					Profundidad++;
				}
				AuxProfundidadIz++;

				R(padre.Izquierda, Lista);
				Lista.Add(padre.Valor);

				AuxProfundidadIz = 0;
				AuxProfundidadDe++;

				R(padre.Derecha, Lista);

				AuxProfundidadDe = 0;
			}
		}

		public T Busqueda(T valor, Delegate delegado)
		{

			Nodo<T> aux = NodoPadre;
			Nodo<T> noexiste = new Nodo<T>();
			while (aux.Valor != null)
			{
				if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor)) == 0) //(valor == aux.valor)//Encontro el valor buscado
				{
					ContadorComparaciones += 1;
					return aux.Valor;//Devulve el valor buscado
				}
				else if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor)) == -1)//(valor < aux.valor)//Compara para saber si se tiene que mover a la izquierda
				{
					if (aux.Izquierda.Valor != null)//Verifica que no se encuentre en una hoja
					{
						ContadorComparaciones += 1;
						aux = aux.Izquierda;//Se mueve a la izquierda
					}
					else //Si se encuentra en una hoja entonces no existe el valor buscado
					{
						return noexiste.Valor;//Devuelve un valor vacio porque no encontro lo solicitado
					}
				}
				else if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor)) == 1) //(valor > aux.valor)//Compara para saber si se tiene que mover a la derecha
				{
					if (aux.Derecha.Valor != null)//Verfica que no se encuentre en una hoja
					{
						ContadorComparaciones += 1;
						aux = aux.Derecha;//Se mueve a la derecha
					}
					else //Si se encuentra en una hoja entonces no existe el valor buscado
					{
						return noexiste.Valor;//Devuelve un valor vacio porque no encontro lo solicitado
					}
				}
			}
			return aux.Valor;//El arbol esta vacio
		}


		public void Insercion(Nodo<T> padre, T valor, Delegate delegado)
		{
			//Si el nodo esta vacio entonces se insertara en ese lugar, si no se mueve
			if (padre.Valor == null)
			{
				padre.Valor = valor;//Inserta el elemento

				//Inicializa los hijos del nodo insertado
				padre.Izquierda = new Nodo<T>();
				padre.Derecha = new Nodo<T>();
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor)) == -1)//(valor < padre.valor)//Verfica si se tiene que mover a la izquierda, si no se mueve a la derecha
			{
				Insercion(padre.Izquierda, valor, delegado);//Se mueve a la izquierda					
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor)) == 1) //(valor > padre.valor)
			{
				Insercion(padre.Derecha, valor, delegado);//Se mueve a la derecha						
			}
		}


		public void Eliminacion(Nodo<T> padre, T valor, Delegate delegado)
		{
			//Busca el elemento a eliminar con recursividad
			if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor)) == -1)//(valor < padre.valor)
			{
				Eliminacion(padre.Izquierda, valor, delegado);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor)) == 1)//(valor > padre.valor)
			{
				Eliminacion(padre.Derecha, valor, delegado);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor)) == 0)//(valor == padre.valor)//Encontro el elemento a eliminar
			{
				NodoBorrar = padre;
				Nodo<T> aux = new Nodo<T>();

				//Comprueba si tiene hijos
				if (padre.Izquierda.Valor == null && padre.Derecha.Valor == null)//Si cumple entonces no tiene hijos, si no avanza y comprueba de que lado esta el hijo
				{
					NodoBorrar = new Nodo<T>();
					padre.Valor = NodoBorrar.Valor;
				}

				//Verifica si es el hijo izquierdo si no entonces es el hijo derecho
				else if (padre.Derecha.Valor == null)
				{
					padre = padre.Izquierda;// Se mueve al sub-arbol izquierdo del nodo a eliminar

					//Se busca la hoja con el valor con el que se va a reemplazar el valor eliminado
					while (padre.Derecha.Valor != null)
					{
						aux = padre;//Se guarda al padre del hijo que va a reemplazar
						padre = padre.Derecha;//Se guarda el valor más grande del lado izquierdo
					}
					NodoBorrar.Valor = padre.Valor;//Se reemplaza el nodo elliminado con el nuevo valor para mantener coherencia en el arbol
					NodoBorrar = padre.Izquierda;//El NodoBorrar se vuelve null
					aux.Derecha = NodoBorrar;//Se borra la hoja ya que ahora esta en otro lugar				
				}
				//Entrara aqui por dos razones
				//1. Solo tiene un hijo y es el derecho
				//2. Tiene los dos hijos y se buscará el elemento más a la izquierda del sub-derecho para reemplazar el elemento eliminado		
				else
				{
					padre = padre.Derecha;//Se mueve al sub-arbol derecho del nodo a eliminar

					//Se busca la hoja con el valor con el que se va a reemplazar el valor eliminado
					while (padre.Izquierda.Valor != null)
					{
						aux = padre;//Se guarda al padre del hijo que va a reemplazar
						padre = padre.Izquierda;//Se guarda el valor más pequeño del lado derecho
					}
					NodoBorrar.Valor = padre.Valor;//Se reemplaza el nodo elliminado con el nuevo valor para mantener coherencia en el arbol
					NodoBorrar = padre.Derecha;//El NodoBorrar se vuelve null
					aux.Izquierda = NodoBorrar;//Se borra la hoja ya que ahora esta en otro lugar



				}
			}
		}


	}
}