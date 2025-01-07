using static GenerarTablero;
using Spectre.Console;
using System.Collections.Generic;


partial class Program
{ 
    // Dimensiones del tablero
    public const int Ancho = 25;
    public const int Alto = 24;

    // Cantidades definidas para trampas y obstáculos
   public const int CantidadObstaculos = 225;
   public const int CantidadTrampas = 15;

   static List<Ficha> fichasSeleccionadas = new(); 
 


    // Lista de fichas disponibles para los jugadores
    static List<Ficha> fichasDisponibles = new() 
    { 
        new Ficha("Ficha Morada", -1, -1, ConsoleColor.Magenta,""), 
        new Ficha("Ficha Verde", -1, -1, ConsoleColor.Green,""), 
        new Ficha("Ficha Azul", -1, -1, ConsoleColor.Blue,""), 
        new Ficha("Ficha Amarilla", -1, -1, ConsoleColor.Yellow,""),
        new Ficha("Ficha Negra", -1, -1, ConsoleColor.Black,"s")
    }; 


  static void Main()
{
   
    // Generar el tablero
    Casilla[,] tablero = Generar(Ancho, Alto, CantidadObstaculos, CantidadTrampas);

    // Generar las trampas
    List<(int fila, int columna, Trampa.Tipo tipo)> trampas = new List<(int fila, int columna, Trampa.Tipo tipo)>();

    Random random = new();
    
    // Pasar la lista trampas a la función ColocarTrampas
    GenerarTablero.ColocarTrampas(tablero, CantidadTrampas, random, trampas); // Ahora la lista trampas es pasada como argumento
    
    // Selección de fichas por los jugadores
    fichasSeleccionadas.Clear(); // Reinicia la lista global de fichas seleccionadas
    
    SeleccionarFichas(tablero, fichasSeleccionadas); // Permite a los jugadores seleccionar sus fichas

    // Dibujar tablero con las fichas seleccionadas
    TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas); // Llamada al método de la clase separada

    
    // Turno del jugador 1 (0) o jugador 2 (1)
    int turnoActual = 0;
    while (true)
    {
        // Mostrar el turno actual
        Console.WriteLine($"\nTurno del Jugador {turnoActual + 1} ({fichasSeleccionadas[turnoActual].Nombre})");

        // Esperar y procesar la entrada del usuario
        Console.WriteLine("Usa las teclas de dirección para mover la ficha.");
        ConsoleKey teclaPresionada = Console.ReadKey(true).Key;

        // Determinar el movimiento basado en la tecla presionada
        int nuevaFila = fichasSeleccionadas[turnoActual].Fila;
        int nuevaColumna = fichasSeleccionadas[turnoActual].Columna;

        switch (teclaPresionada)
        {
            case ConsoleKey.UpArrow:
                nuevaFila--;
                break;
            case ConsoleKey.DownArrow:
                nuevaFila++;
                break;
            case ConsoleKey.LeftArrow:
                nuevaColumna--;
                break;
            case ConsoleKey.RightArrow:
                nuevaColumna++;
                break;
            default:
                Console.WriteLine("Tecla inválida. Usa las flechas de dirección.");
                continue;
        }

        // Intentar mover la ficha, ahora pasando la lista de trampas
        if (MoverFicha(fichasSeleccionadas[turnoActual], nuevaFila, nuevaColumna, tablero, trampas))
        {
            // Redibujar el tablero si el movimiento fue exitoso
            TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);

            // Cambiar de turno
            turnoActual = (turnoActual + 1) % fichasSeleccionadas.Count;
        }
        else
        {
            Console.WriteLine("Movimiento no permitido. Intenta nuevamente.");
        }
    }
}

    // Método para permitir a los jugadores seleccionar sus fichas
    static void SeleccionarFichas(Casilla[,] tablero, List<Ficha> fichasSeleccionadas)
    {
        var random = new Random();
        for (int i = 1; i <= 2; i++)
        {
            // Mostrar menú para seleccionar ficha al jugador actual
            var fichaElegida = AnsiConsole.Prompt(new SelectionPrompt<Ficha>()
                .Title($"Jugador {i}, elija su ficha:")
                .AddChoices(fichasDisponibles));

            fichasDisponibles.Remove(fichaElegida); // Remover la ficha elegida de la lista disponible

            var posicion = ObtenerPosicionAleatoria(tablero, random); // Obtener posición aleatoria en el camino

            fichaElegida.Fila = posicion.Item1;
            fichaElegida.Columna = posicion.Item2;

            fichasSeleccionadas.Add(fichaElegida); // Agregar la ficha seleccionada al juego

            AnsiConsole.MarkupLine($"[bold]{fichaElegida.Nombre}[/] fue seleccionada por Jugador {i} y colocada en ({posicion.Item1}, {posicion.Item2}).");
        }
    }
// Método MoverFicha ajustado
static bool MoverFicha(Ficha ficha, int nuevaFila, int nuevaColumna, Casilla[,] tablero, List<(int fila, int columna, Trampa.Tipo tipo)> trampas)
{
    if (tablero[nuevaFila, nuevaColumna] == Casilla.Camino || tablero[nuevaFila, nuevaColumna] == Casilla.Trampa)
    {
        ficha.Mover(nuevaFila, nuevaColumna); // Mueve la ficha a la nueva posición

        // Lógica para perder puntos si cae en una trampa
        if (tablero[nuevaFila, nuevaColumna] == Casilla.Trampa)
        {
            // Buscar la trampa en la lista de trampas usando las coordenadas
            var trampa = trampas.FirstOrDefault(t => t.fila == nuevaFila && t.columna == nuevaColumna);

            if (trampa != default)
            {
                // Crear una instancia de la trampa usando el tipo de trampa encontrado
                var trampaObjeto = new Trampa(trampa.tipo);
                trampaObjeto.AplicarTrampa(ficha); // Aplica el efecto de la trampa

                // Elimina la trampa de la lista de trampas
                trampas.RemoveAll(t => t.fila == nuevaFila && t.columna == nuevaColumna);

                // Desactivar la trampa en el tablero (ponerla como Camino)
                tablero[nuevaFila, nuevaColumna] = Casilla.Camino;
            }
        }

        // Verificar si los puntos llegan a 0
        if (ficha.Puntos <= 0)
        {
            Console.WriteLine($"{ficha.Nombre} ha perdido todos sus puntos. ¡El juego termina para esta ficha!");
            return false; // Detiene el movimiento ya que la ficha está fuera del juego
        }

        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas); // Redibujar el tablero con la nueva posición
        Console.WriteLine($"{ficha.Nombre} se movió a ({nuevaFila}, {nuevaColumna}).");
        return true; // Movimiento realizado con éxito
    }
    else
    {
        Console.WriteLine("Movimiento inválido. Esa casilla no es transitable."); // Movimiento no permitido
        return false;
    }
}


}
