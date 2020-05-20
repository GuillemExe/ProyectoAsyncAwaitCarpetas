using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProyectoAsyncAwaitCarpetas
{
    public class Lectura
    {
        // Use el modificador async para especificar que un método es asincrónico.
        // Este método o una expresión, hace referencia al mismo como un método asincrónico.
        static List<string> ListaDeDirectorios;

        // En su mayoría he usado el sistema que aportaba por defecto el “System”.
        // El file y el folders para guardar las carpetas y los documentos.
        static DirectoryInfo di = new DirectoryInfo(@"C:\Program Files");
        static List<FileInfo> files = new List<FileInfo>();
        static List<DirectoryInfo> folders = new List<DirectoryInfo>();

        public async Task<List<string>> ReadFolder()
        {
            FullDirList(di, "");

            ListaDeDirectorios = new List<string>();

            Parallel.Invoke(() =>
            {
                foreach (var folder in folders)
                {
                    ListaDeDirectorios.Add(folder.FullName);
                }
            }, () => {
                foreach (var file in files)
                {
                    ListaDeDirectorios.Add(file.FullName);
                }
            });

            return ListaDeDirectorios;
        }

        static void FullDirList(DirectoryInfo dir, string searchPattern)
        {
            Parallel.Invoke(() =>
            {
                try
                {
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        files.Add(file);
                    }
                }
                catch
                {
                    // Ignored
                }
            }, () => {
                try
                {
                    foreach (DirectoryInfo directorio in dir.GetDirectories())
                    {
                        folders.Add(directorio);
                        FullDirList(directorio, searchPattern);
                    }
                }
                catch
                {
                    // Ignored
                }
            });
        }
    }
}
