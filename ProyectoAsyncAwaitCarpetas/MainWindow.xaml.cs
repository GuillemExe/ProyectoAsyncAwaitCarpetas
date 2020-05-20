using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoAsyncAwaitCarpetas
{
    public partial class MainWindow : Window
    {
        string TimeGet;
        List<string> RoutesOne = new List<string>();
        List<string> RoutesTwo = new List<string>();
        DirectoryInfo di = new DirectoryInfo(@"C:\Program Files");

        static List<FileInfo> files = new List<FileInfo>();
        static List<DirectoryInfo> folders = new List<DirectoryInfo>();

        public MainWindow()
        {
            InitializeComponent();

            var lectura = new Lectura();

            // Una región es algo que ayuda a empaquetar un grupo de funciones o parámetros,
            // no tiene peso, para ver el codigo que contiene solo haz doble click en donde pone REGION.
            #region ---> EVENTOS DE LA INTERFAZ
            // Evento o lisener del boton uno. Tipo CLICK.
            ButtonUno.Click += (sender, args) =>
            {
                // Limpiamos la lista
                RoutesOne.Clear();

                // Empezamos con toda la operacion.
                var timingCode1 = System.Diagnostics.Stopwatch.StartNew();

                // Cargamos los ficheros
                FullDirList(di, "");

                Parallel.Invoke(() =>
                {
                    foreach (var folder in folders)
                    {
                        RoutesOne.Add(folder.FullName);
                    }
                }, () => { 
                    foreach (var file in files)
                    {
                        RoutesOne.Add(file.FullName);
                    }
                });

                // Set list on listbox
                ListBoxTablaUno.ItemsSource = RoutesOne;

                // Set time on textblock
                TextBlockUno.Text = "Time: " + ConvertMillisecondsToSeconds(timingCode1.ElapsedMilliseconds) + " seconds";
            };

            // Evento o lisener del boton dos. Tipo CLICK.
            ButtonDos.Click += (sender, args) =>
            {
                // Limpiamos la lista.
                RoutesTwo.Clear();

                // Empezamos con toda la operacion.
                var timingCode2 = System.Diagnostics.Stopwatch.StartNew();

                RoutesTwo = lectura.ReadFolder().Result;

                timingCode2.Stop();

                ListBoxTablaDos.ItemsSource = RoutesTwo;

                TextBlockDos.Text = "Time: " + ConvertMillisecondsToSeconds(timingCode2.ElapsedMilliseconds) + " seconds";
            };

            // Evento o lisener del boton tress. Tipo CLICK.
            ButtonTres.Click += (sender, args) =>
            {
                // Recogemos la fecha actual por el System.
                TimeGet = DateTime.Now.ToString("HH:mm:ss");

                TextBlockTres.Text = "Actual time: " + TimeGet;
            };
            #endregion
        }


        // Una función simple que hice para que retornada el valor en segundos.
        public static double ConvertMillisecondsToSeconds(double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds).TotalSeconds;
        }


        // Solo se encarga de guardarlo en las listas de “file” y “folders”.
        // Tiene un try catch por si el documento o carpeta es inaccesible. Para evitar el error.
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
                } catch
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
                } catch
                {
                    // Ignored
                }
            });
        }


        // Esto es lo que tenia de la anterior práctica, que no la termine por un problema en la
        // forma en la que lo hacía, usando las librerías y las clases que da el propio sistema,
        // pude agilizar mucho el tiempo de lectura y poder hacer el AsyncAwait. Por el formato
        // en el que lo estaba haciendo se me hacía imposible llevarlo a cabo.

        // No es necesario mirarlo.

        // Una región es algo que ayuda a empaquetar un grupo de funciones o parámetros,
        // no tiene peso, para ver el codigo que contiene solo haz doble click en donde pone REGION.
        #region ---> REGION ---> Parámetros para el lector explorador.
        private void ReadFolderVersionStable(string route)
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
                        RoutesOne.Add(folder);
                        ReadFolderVersionStable(folder);
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
                        RoutesOne.Add(document);
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            });
        }

        private void ReadFolderDefaultVersionDeprecated(string route)
        {
            // 2 Medio.
            // Las diferentes pruebas de rendimiento han portado
            // que este tenga una 95% de eficacia respecto al
            // tiempo dado de las tres tipos de lectura.

            // Tiempo: 32 a 36 segundos.
            // Procesado por un Intel i7-6 de 4.0Ghz.
            // Volumen ocupado para el escáner 370Gb.

            try
            {
                foreach (string folder in Directory.GetDirectories(@route))
                {
                    RoutesOne.Add(folder);

                    try
                    {
                        foreach (string document in Directory.GetFiles(folder))
                        {
                            RoutesOne.Add(document);
                        }
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                    ReadFolderDefaultVersionDeprecated(folder);
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        private void ReadFolderVersionSimpleDeprecated(string route)
        {
            // 3 Pesimo.
            // Las diferentes pruebas de rendimiento han portado
            // que este tenga una 75% de eficacia respecto al
            // tiempo dado de las tres tipos de lectura.

            // Tiempo: 34 a 42 segundos.
            // Procesado por un Intel i7-6 de 4.0Ghz.
            // Volumen ocupado para el escáner 370Gb.

            try
            {
                foreach (string folder in Directory.GetDirectories(@route))
                {
                    RoutesOne.Add(folder);

                    Parallel.Invoke(() =>
                    {
                        try
                        {
                            foreach (string document in Directory.GetFiles(folder))
                            {
                                RoutesOne.Add(document);
                            }
                        }
                        catch (Exception)
                        {
                            // Ignored
                        }
                    }, () => { ReadFolderVersionSimpleDeprecated(folder); });
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }
        #endregion
    }
}