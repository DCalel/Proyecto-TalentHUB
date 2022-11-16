using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaGenerica
{
	public class AVLRepetidos<T> : NodoLista<T> where T : IComparable
    {
		NodoLista<T> NodoPadre = new NodoLista<T>();
		NodoLista<T> NodoBorrar = new NodoLista<T>();
		public int AuxProfundidadIz = 0;
		public int AuxProfundidadDe = 0;
		public int contador = 0;

		//Recibe el valor a insertar
		public void Agregar(T valor, Delegate delegado)
		{
			Insercion(NodoPadre, valor, delegado);//Manda a llamar la funcion que insertara el elemento
			NodoPadre = Equilibrio(NodoPadre);
		}

		public void Borrar(T valor, Delegate delegado, Delegate delegado2)
		{
			Eliminacion(NodoPadre, valor, delegado, delegado2);//Manda a llamar la funcion que eliminara el elemento
		}

		public void Actualizar(T valor, Delegate delegado, Delegate delegado2)
        {
			Modificar(NodoPadre, valor, delegado, delegado2);
        }


        public List<List<T>> Recorrido()
        {
            Profundidad = 0;
            List<List<T>> ListaArbol = new List<List<T>>();
            R(NodoPadre, ListaArbol);
            Profundidad++;
            return ListaArbol;
        }

        private void R(NodoLista<T> padre, List<List<T>> Lista)
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

        public List<T> Busqueda(T valor, Delegate delegado)
        {
            NodoLista<T> aux = NodoPadre;
            NodoLista<T> noexiste = new NodoLista<T>();
            ContadorComparaciones = 0;
            while (aux.Valor != null)
            {
                if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor[0])) == 0) //(valor == aux.valor)//Encontro el valor buscado
                {
                    ContadorComparaciones++;
                    return aux.Valor;//Devulve el valor buscado
                }
                else if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor[0])) == -1)//(valor < aux.valor)//Compara para saber si se tiene que mover a la izquierda
                {
                    if (aux.Izquierda.Valor != null)//Verifica que no se encuentre en una hoja
                    {
                        ContadorComparaciones++;
                        aux = aux.Izquierda;//Se mueve a la izquierda
                    }
                    else //Si se encuentra en una hoja entonces no existe el valor buscado
                    {
                        return noexiste.Valor;//Devuelve un valor vacio porque no encontro lo solicitado
                    }
                }
                else if (Convert.ToInt32(delegado.DynamicInvoke(valor, aux.Valor[0])) == 1) //(valor > aux.valor)//Compara para saber si se tiene que mover a la derecha
                {
                    if (aux.Derecha.Valor != null)//Verfica que no se encuentre en una hoja
                    {
                        ContadorComparaciones++;
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

        public void Insercion(NodoLista<T> padre, T valor, Delegate delegado)
		{
			padre.FactorEquilibrio = 0;
			//Si el nodo esta vacio entonces se insertara en ese lugar, si no se mueve
			if (padre.Valor == null)
			{
				padre.Valor = new List<T> { valor };//Inserta el elemento

				//Inicializa los hijos del nodo insertado
				padre.Izquierda = new NodoLista<T>();
				padre.Derecha = new NodoLista<T>();
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == -1)//(valor < padre.valor)//Verfica si se tiene que mover a la izquierda, si no se mueve a la derecha
			{
				padre.AlturaIzquierda = 0;
				Insercion(padre.Izquierda, valor, delegado);//Se mueve a la izquierda					
				if (padre.Izquierda.Valor != null)
				{
					padre.AlturaIzquierda = Math.Max(padre.Izquierda.AlturaDerecha, padre.Izquierda.AlturaIzquierda) + 1;
				}
				padre.Izquierda = Equilibrio(padre.Izquierda);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == 1) //(valor > padre.valor)
			{
				padre.AlturaDerecha = 0;
				Insercion(padre.Derecha, valor, delegado);//Se mueve a la derecha
				if (padre.Derecha.Valor != null)
				{
					padre.AlturaDerecha = Math.Max(padre.Derecha.AlturaDerecha, padre.Derecha.AlturaIzquierda) + 1;
				}
				padre.Derecha = Equilibrio(padre.Derecha);
			}
            else
            {
				padre.Valor.Add(valor);
            }

			padre.FactorEquilibrio = padre.AlturaDerecha - padre.AlturaIzquierda;
		}

		public NodoLista<T> Equilibrio(NodoLista<T> Padre)
		{
			//Rotacion Doble a al Inzquierda(+2,-1)
			if (Padre.FactorEquilibrio == 2 && Padre.Derecha.FactorEquilibrio == -1)
			{
				ContadorRotaciones++;
				//Rotacion Derecha, luego Rotacion Izquierda(2,-1)
				NodoLista<T> Aux = Padre.Derecha;
				NodoLista<T> Aux2 = Padre.Derecha.Izquierda;
				Aux.Izquierda = Aux2.Derecha;
				Padre.Derecha = Aux2;
				Aux2.Derecha = Aux;
			}
			//Rotacion Doble a al Derecha(-2,+1)
			else if (Padre.FactorEquilibrio == -2 && Padre.Izquierda.FactorEquilibrio == 1)
			{
				ContadorRotaciones++;
				//Rotacion Izquierda, luego Rotacion Derecha
				NodoLista<T> Aux = Padre.Izquierda;
				NodoLista<T> Aux2 = Padre.Izquierda.Derecha;
				Aux.Derecha = Aux2.Izquierda;
				Padre.Izquierda = Aux2;
				Aux2.Izquierda = Aux;
			}
			//Rotacion Simple a la Izquierda
			if (Padre.FactorEquilibrio == 2)
			{
				ContadorRotaciones++;
				NodoLista<T> Aux = Padre.Derecha;
				NodoLista<T> Aux2 = Padre.Derecha.Izquierda;
				Padre.Derecha = Aux2;
				Aux.Izquierda = Padre;
				ActualizarAltura(Aux);
				Padre = Aux;
			}
			//Rotacion Simple a la Derecha

			else if (Padre.FactorEquilibrio == -2)
			{
				ContadorRotaciones++;
				NodoLista<T> Aux = Padre.Izquierda;
				NodoLista<T> Aux2 = Padre.Izquierda.Derecha;
				Padre.Izquierda = Aux2;
				Aux.Derecha = Padre;
				ActualizarAltura(Aux);
				Padre = Aux;
			}
			return Padre;
		}

		public void ActualizarAltura(NodoLista<T> Padre)
		{
			Padre.AlturaIzquierda = 0;
			Padre.AlturaDerecha = 0;
			Padre.FactorEquilibrio = 0;
			if (Padre.Izquierda.Valor != null)
			{
				ActualizarAltura(Padre.Izquierda);
				if (Padre.Izquierda.Valor != null)
				{
					Padre.AlturaIzquierda = Math.Max(Padre.Izquierda.AlturaDerecha, Padre.Izquierda.AlturaIzquierda) + 1;
					Padre.FactorEquilibrio = Padre.AlturaDerecha - Padre.AlturaIzquierda;
				}
			}
			if (Padre.Derecha.Valor != null)
			{
				ActualizarAltura(Padre.Derecha);
				if (Padre.Derecha.Valor != null)
				{
					Padre.AlturaDerecha = Math.Max(Padre.Derecha.AlturaDerecha, Padre.Derecha.AlturaIzquierda) + 1;
					Padre.FactorEquilibrio = Padre.AlturaDerecha - Padre.AlturaIzquierda;
				}
			}
		}

		public void Eliminacion(NodoLista<T> padre, T valor, Delegate delegado, Delegate delegado2)
		{
			if (padre.Valor == null)
			{
				return;
			}
			//Busca el elemento a eliminar con recursividad
			if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == -1)//(valor < padre.valor)
			{
				Eliminacion(padre.Izquierda, valor, delegado, delegado2);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == 1)//(valor > padre.valor)
			{
				Eliminacion(padre.Derecha, valor, delegado, delegado2);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == 0)//(valor == padre.valor)//Encontro el elemento a eliminar
			{
				NodoBorrar = padre;
				NodoLista<T> aux = new NodoLista<T>();

				for (int i = 0; i < padre.Valor.Count; i++)
				{
					if (Convert.ToInt32(delegado2.DynamicInvoke(valor, padre.Valor[i])) == 0)
					{
						padre.Valor.RemoveAt(i);
					}
				}

				if (padre.Valor.Count == 0)//Comprueba si la lista quedo vacía o si tiene más nombres repetidos
				{
					//Comprueba si tiene hijos
					if (padre.Izquierda.Valor == null && padre.Derecha.Valor == null)//Si cumple entonces no tiene hijos, si no avanza y comprueba de que lado esta el hijo
					{
						if (padre.Valor.Count == 0)
						{
							NodoBorrar = new NodoLista<T>();
							padre.Valor = NodoBorrar.Valor;//Se elimina la lista vacia
						}
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
				ActualizarAltura(padre);
			}
			padre.FactorEquilibrio = padre.AlturaDerecha - padre.AlturaIzquierda;
		}

		public void Modificar(NodoLista<T> padre, T valor, Delegate delegado, Delegate delegado2)
        {
			//Busca el elemento a eliminar con recursividad
			if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == -1)//(valor < padre.valor)
			{
				Modificar(padre.Izquierda, valor, delegado, delegado2);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == 1)//(valor > padre.valor)
			{
				Modificar(padre.Derecha, valor, delegado, delegado2);
			}
			else if (Convert.ToInt32(delegado.DynamicInvoke(valor, padre.Valor[0])) == 0)//(valor == padre.valor)//Encontro el elemento a modificar
			{
				NodoBorrar = padre;

				for (int i = 0; i < padre.Valor.Count; i++)
				{
					if (Convert.ToInt32(delegado2.DynamicInvoke(valor, padre.Valor[i])) == 0)
					{
						padre.Valor.RemoveAt(i);
						padre.Valor.Insert(i, valor);
					}
				}

			}
		}



	}
}
