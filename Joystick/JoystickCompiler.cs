using System;
using System.Collections.Generic;

namespace JoystickCompiler
{
    public class JoystickCompiler
    {
        private Dictionary<string, bool> buttonStates = new Dictionary<string, bool>
        {
            {"A", false}, {"B", false}, {"X", false}, {"Y", false},
            {"START", false}, {"SELECT", false}
        };

        public void ExecuteMovement(string direction)
        {
            switch (direction.ToUpper())
            {
                case "UP":
                    Console.WriteLine("   Movimiento hacia ARRIBA");
                    break;
                case "DOWN":
                    Console.WriteLine("   Movimiento hacia ABAJO");
                    break;
                case "LEFT":
                    Console.WriteLine("   Movimiento hacia IZQUIERDA");
                    break;
                case "RIGHT":
                    Console.WriteLine("   Movimiento hacia DERECHA");
                    break;
            }
        }

        public void ExecuteButton(string button, string action)
        {
            bool newState = action.ToUpper() == "PRESS";
            buttonStates[button.ToUpper()] = newState;

            string stateText = newState ? "PRESIONADO" : "LIBERADO";
            Console.WriteLine("   Estado actual: " + button + " = " + stateText);
        }

        public void Compile(string inputFile)
        {
            try
            {
                Scanner scanner = new Scanner(inputFile);
                Parser parser = new Parser(scanner);

                // Necesitamos pasar la referencia a esta instancia para que el parser
                // pueda llamar a ExecuteMovement y ExecuteButton
                // Esto requiere modificar el parser o usar un enfoque diferente
                parser.SetCompiler(this);
                parser.Parse();

                Console.WriteLine("EJECUCION COMPLETADA EXITOSAMENTE");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }
    }
}