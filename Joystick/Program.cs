using System;

namespace JoystickCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            JoystickCompiler compiler = new JoystickCompiler();

            Console.WriteLine("🎮 Compilador de Lenguaje Joystick");
            Console.WriteLine("===================================\n");

            // Ejecutar programas de prueba
            string[] testFiles = { "test1.jsk", "test2.jsk", "test3.jsk" };

            foreach (string testFile in testFiles)
            {
                if (System.IO.File.Exists(testFile))
                {
                    Console.WriteLine($"\n🔧 Ejecutando: {testFile}");
                    Console.WriteLine(new string('-', 40));
                    compiler.Compile(testFile);
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}