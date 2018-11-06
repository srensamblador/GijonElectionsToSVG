using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcesadorElecciones
{
    class Programa
    {
        private static ManejadorXML manejador;
        private static Logica logica;

        static void Main(string[] args)
        {
            Creditos();
            //Si se ejecuta desde cmd se salta el proceso de introducción del archivo manualmente
            if (args.Length > 0)
                Initialize(args[0]);
            else
            {
                Console.WriteLine("\nIntroduzca la ruta al fichero que quiere procesar: ");

                string nombreFichero = "";
                //Carga XML
                do
                {
                    nombreFichero = Console.ReadLine();
                } while (!Initialize(nombreFichero));
            }

            //Usuario selecciona si quiere datos en número total o porcentajes
            int seleccion = -1;
            Console.WriteLine("\nIntroduzca el modo en que desea obtener los datos:\n" +
                    "\t0 -> Número total\n" +
                    "\t1-> Porcentaje");
            do
            {
                try
                {
                    seleccion = int.Parse(Console.ReadLine());
                } catch (FormatException e){ }
            } while (seleccion != 0 && seleccion != 1);

            Modo modo = Modo.SIN_ASIGNAR;
            switch (seleccion)
            {
                case 0:
                    modo = Modo.TOTAL;
                    break;
                case 1:
                    modo = Modo.PORCENTAJE;
                    break;
            }
            if (modo == Modo.SIN_ASIGNAR)
            {
                throw new ArgumentException("Modo incorrecto");
            }

            logica.GenerarGraficoGeneral(modo);

            Console.WriteLine("Se ha generado el .svg");
            Console.ReadLine();
            

            


        }

        private static void Creditos()
        {
            Console.WriteLine("############################################\n" +
                "Procesador Electoral - Versión 1.0\n" +
                "Programa creado por Samuel Cifuentes García\n" +
                "############################################");
        }

        private static bool Initialize(string nombre_programa)
        {
            logica = null;
            try
            {
                manejador = new ManejadorXML(nombre_programa);
                logica = new Logica(manejador);
                return true;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No existe el fichero.");
                return false;
            }
        }
    }
}
