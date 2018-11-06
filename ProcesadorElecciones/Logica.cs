using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcesadorElecciones
{
    public enum Modo { TOTAL, PORCENTAJE, SIN_ASIGNAR }

    class Logica
    {
		

        ManejadorXML manejador;

		public Logica(ManejadorXML manejador)
        {
            this.manejador = manejador;
        }

		public void GenerarGraficoGeneral(Modo modo)
        {
            IDictionary<string, int> dict = new Dictionary<string, int>();
            var datos = manejador.DatosBase//.GroupBy(d => d.Element("partido").Value)
                .Aggregate(dict, (dictSeed, d) =>
                {
                    if (dictSeed.ContainsKey(d.Element("partido").Value))
                    {
                        dictSeed[d.Element("partido").Value] += Convert.ToInt32(d.Element("num_votos").Value);
                    }
                    else
                    {
                        dictSeed.Add(d.Element("partido").Value, Convert.ToInt32(d.Element("num_votos").Value));
                    }
                    return dictSeed;
                }).OrderByDescending(d =>  d.Value);
			
            int totalVotos = (int)datos.Where(p => IsNombrePartido(p.Key)).Sum(votos => votos.Value);

            var datosExtra = ObtenerDatosExtra(datos);

            switch (modo)
            {
                case Modo.PORCENTAJE:
                    IDictionary<string, double> datosPorcentaje = new Dictionary<string, double>();
                    foreach (var d in datos)
                    {
                        datosPorcentaje.Add(d.Key, d.Value * 100d / totalVotos);
                    }
                    manejador.GenerarSVGPorcentaje(datosPorcentaje, datosExtra);
                    break;
                case Modo.TOTAL:
                    manejador.GenerarSVGTotal(datos, datosExtra);
                    break;
            }
			


            
        }

        /**
         * Obtiene total de votos, abstenciones, votantes y blancos
         * */
        private IDictionary<string, int> ObtenerDatosExtra(IOrderedEnumerable<KeyValuePair<string, int>> datos)
        {
            IDictionary<string, int> datosExtra = new Dictionary<string, int>();

            int totalVotosPartidos = (int)datos.Where(p => IsNombrePartido(p.Key)).Sum(votos => votos.Value);
            datosExtra.Add("VotosPartidos", totalVotosPartidos);
            int totalVotos = (int)datos.Sum(votos => votos.Value);
            datosExtra.Add("Votos", totalVotos);
            int abstenciones = (int)datos.Where(p => p.Key.Equals("Abstenciones")).Sum(votos => votos.Value);
            datosExtra.Add("Abstenciones", abstenciones);
            int votantes = (int)datos.Where(p => p.Key.Equals("Votantes")).Sum(votos => votos.Value);
            datosExtra.Add("Votantes", votantes);
            int blancos = (int)datos.Where(p => p.Key.Equals("Blancos")).Sum(votos => votos.Value);
            datosExtra.Add("Blancos", blancos);

            return datosExtra;
        }

        private static void Show<T>(IEnumerable<T> colección)
        {
            foreach (var item in colección)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Elementos en la colección: {0}.", colección.Count());
        }
		
        /**
         * distingue si el string pasado es un nombre de un partido (Todo mayúsculas) o no (por ejemplo, votos en blanco)
         */
		public static bool IsNombrePartido(string a)
        {
            foreach (char c in a)
                if (char.IsLower(c))
                    return false;
            return true;
        }

    }

	public class PriorizarPartidos: IComparer<string>
    {
		/**
		 * Si la cadenan es todo mayúsculas, es un partido político. Priorizar estos sobre cadenas que no son partidos (P. ej, En blanco)
		 */
        public int Compare(string a, string b)
        {
            bool isAMayus, isBMayus;
            isAMayus = isBMayus = true;

            foreach (char c in a)
            {
                if (Char.IsLower(c))
                    isAMayus = false;
            }

            foreach (char c in b)
            {
                if (Char.IsLower(c))
                    isBMayus = false;
            }

            if (isAMayus && !isBMayus)
                return -1;
            if (!isAMayus && isBMayus)
                return 1;
            else
                return a.CompareTo(b);
        }
    }
}
