using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ProyectoAsyncAwaitCarpetas
{
    class Lectura
    {
        public Lectura()
        {
            StatusRead = false;
            ListaDeDirectorios = new List<string>();
        }

        // Use el modificador async para especificar que un método es asincrónico.
        // Este método o una expresión, hace referencia al mismo como un método asincrónico.

        // Para determinar el status de incio fin empleo una flag.
        public bool StatusRead { get; set; }
        public List<string> ListaDeDirectorios { get; set; }

        public async void ReadFolder()
        {
            await Task.Run(() =>
            {
                StatusRead = true;
                ReadFolderVersionStable(@"C:\Program Files");
            });
        }

        internal void ReadFolderVersionStable(string route)
        {
            while (StatusRead)
            {
                ReadFolder(route);
            }
        }

        private void ReadFolder(string route)
        {
            // 1 Mejor.
            // Las diferentes pruebas de rendimiento han portado
            // que este tenga una 100% de eficacia respecto al
            // tiempo dado de las tres tipos de lectura.

            // Tiempo: 30 a 34 segundos.
            // Procesado por un Intel i7-6 de 4.0Ghz.
            // Volumen ocupado para el escáner 370Gb.

            Parallel.Invoke(() =>
            {
                try
                {
                    foreach (string folder in Directory.GetDirectories(@route))
                    {
                        ListaDeDirectorios.Add(folder);
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            }, () =>
            {
                try
                {
                    foreach (string document in Directory.GetFiles(@route))
                    {
                        ListaDeDirectorios.Add(document);
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            });
            StatusRead = false;
        }
    }
}
