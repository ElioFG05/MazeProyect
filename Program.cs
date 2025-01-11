using static GenerarTablero;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using Casilla = GenerarTablero.Casilla;


partial class Program
{ 
    public const int Ancho = 34;
    public const int Alto = 27;
    public const int CantidadTrampas = 35;
    public const int CantidadObstaculos = 200;

    public static List<Ficha> fichasSeleccionadas = new(); 
    public static List<Ficha> fichasDisponibles = new() 
    { 
        new Ficha("Ficha Morada", -1, -1, ConsoleColor.Magenta,"Teletransportación Aleatoria", 4), 
        new Ficha("Ficha Verde", -1, -1, ConsoleColor.Green,"Inmunidad Temporal" , 2), 
        new Ficha("Ficha Azul", -1, -1, ConsoleColor.Blue, "Paso Fantasma", 2), 
        new Ficha("Ficha Amarilla", -1, -1, ConsoleColor.Yellow, "Curación Rápida", 2),
        new Ficha("Ficha Negra", -1, -1, ConsoleColor.Black, "Avance Doble", 2)
    };

   static void Main()
    {
        var (tablero, trampas) = Generar(Ancho, Alto, CantidadTrampas, CantidadObstaculos);
        Random random = new();

        fichasSeleccionadas.Clear();
        SeleccionarFichas(tablero, fichasSeleccionadas);
       
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
        Informacion.Inicializar();

        int turnoActual = 0;
        while (fichasSeleccionadas.Count > 0)
        {
            // Mostrar la información actual de las fichas
            Console.Clear();
            TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
            Informacion.MostrarInformacion(fichasSeleccionadas);
            Console.WriteLine($"\nTurno del Jugador {turnoActual + 1} ({fichasSeleccionadas[turnoActual].Nombre})");

            // Menú para seleccionar acción
            var accion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Elige tu acción:")
                    .AddChoices(new[] { "Mover Ficha", "Usar Habilidad" }));

            if (accion == "Mover Ficha")
            {
                Console.WriteLine("Usa las teclas de dirección para mover la ficha.");
                ConsoleKey teclaPresionada = Console.ReadKey(true).Key;

                int nuevaFila = fichasSeleccionadas[turnoActual].Fila;
                int nuevaColumna = fichasSeleccionadas[turnoActual].Columna;

                switch (teclaPresionada)
                {
                    case ConsoleKey.UpArrow: nuevaFila--; break;
                    case ConsoleKey.DownArrow: nuevaFila++; break;
                    case ConsoleKey.LeftArrow: nuevaColumna--; break;
                    case ConsoleKey.RightArrow: nuevaColumna++; break;
                    default:
                        Console.WriteLine("Tecla inválida. Usa las flechas de dirección.");
                        continue;
                }

                


                if (MoverFicha(fichasSeleccionadas[turnoActual], nuevaFila, nuevaColumna, tablero, trampas))
                {
                    /*/ Actualizar información después de cada acción
                    Informacion.MostrarInformacion(fichasSeleccionadas);*/
                    TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
                   
                }
                else
                {
                    Console.WriteLine("Movimiento no permitido. Intenta nuevamente.");  
                    continue;
                }
            }
            else if (accion == "Usar Habilidad")
            {
                Random rndm = new();
                fichasSeleccionadas[turnoActual].UsarHabilidad(tablero, random, fichasSeleccionadas);
            }

            // Verificar si la ficha perdió todos los puntos
            if (!VerificarPuntos(fichasSeleccionadas[turnoActual]))
            {
                fichasSeleccionadas.RemoveAt(turnoActual);
                if (turnoActual >= fichasSeleccionadas.Count)
                    turnoActual = 0; // Si el turno actual excede el número de fichas, reiniciarlo
            }
            else
            {
                turnoActual = (turnoActual + 1) % fichasSeleccionadas.Count;
            }

            foreach (var ficha in fichasSeleccionadas)
            {
                // Reducir cooldown si es necesario
                if (ficha.Cooldown > 0)
                {
                    ficha.Cooldown--;
                }

                // Verificar si la ficha tiene inmunidad activa
                if (ficha.TurnosInmunidad > 0)
                {
                    ficha.TurnosInmunidad--; // Reducir inmunidad solo si está activa

                    // Mostrar mensaje de inmunidad solo después de reducir los turnos
                    if (ficha.TurnosInmunidad == 0)
                    {
                        Console.WriteLine($"{ficha.Nombre} ya no tiene inmunidad temporal.");
                    }
                    
                }
            }
        }
    }

     public static void SeleccionarFichas(Casilla[,] tablero, List<Ficha> fichasSeleccionadas)
    {
        var random = new Random();
        for (int i = 1; i <= 2; i++)
        {
            var fichaElegida = AnsiConsole.Prompt(new SelectionPrompt<Ficha>()
                .Title($"Jugador {i}, elija su ficha:")
                .AddChoices(fichasDisponibles));

            fichasDisponibles.Remove(fichaElegida);

            var posicion = ObtenerPosicionAleatoria(tablero, random);
            fichaElegida.Fila = posicion.Item1;
            fichaElegida.Columna = posicion.Item2;

            fichasSeleccionadas.Add(fichaElegida);
            AnsiConsole.MarkupLine($"[bold]{fichaElegida.Nombre}[/] fue seleccionada por Jugador {i} y colocada en ({posicion.Item1}, {posicion.Item2}).");
        }
    }


    static bool MoverFicha(Ficha ficha, int nuevaFila, int nuevaColumna, Casilla[,] tablero, List<(int fila, int columna, Trampa.Tipo tipo)> trampas)
    {
        if (EsMovimientoValido(tablero, nuevaFila, nuevaColumna))
        {
            ficha.Mover(nuevaFila, nuevaColumna);
        
            // Llamada a ManejarTrampa con los argumentos correctos
            ManejarTrampa(nuevaFila, nuevaColumna, ficha, tablero, trampas);
            
            if (!VerificarPuntos(ficha))
            {
            return false;
            }
            ActualizarTableroYNotificar(ficha, tablero, nuevaFila, nuevaColumna);
            return true;
        }
        
        else
        {
        Console.WriteLine("Movimiento inválido. Esa casilla no es transitable.");
        return false;
        }
    }

    static bool VerificarPuntos(Ficha ficha)
    {
        if (ficha.Puntos <= 0)
        {
            Console.WriteLine($"{ficha.Nombre} ha perdido todos sus puntos. ¡El juego termina para esta ficha!");
            return false;
        }
        return true;
    }

    
    public static void ManejarTrampa(int fila, int columna, Ficha ficha, Casilla[,] tablero, List<(int fila, int columna, Trampa.Tipo tipo)> trampas)
    {
        if (tablero[fila, columna] == Casilla.Trampa)
        {
        // Encuentra la trampa correspondiente
        var trampa = trampas.FirstOrDefault(t => t.fila == fila && t.columna == columna);

        // Verificar si la trampa no es la tupla predeterminada
        if (trampa != default((int fila, int columna, Trampa.Tipo tipo)))
        {
            int puntosDeLaTrampa = (int)trampa.tipo;

            // Aplicar efectos de la trampa a la ficha
            ficha.PerderPuntos(puntosDeLaTrampa);

            // Cambiar el estado de la casilla de Trampa a Camino
            tablero[fila, columna] = Casilla.Camino;

            // Eliminar la trampa de la lista
            trampas.Remove(trampa);

            // Mensaje de depuración
            Console.WriteLine($"Trampa activada en ({fila}, {columna}). Ficha ha perdido puntos. Casilla ahora es Camino.");
        }
        }
    }



    static bool EsMovimientoValido(Casilla[,] tablero, int fila, int columna)
    {
        return tablero[fila, columna] == Casilla.Camino || tablero[fila, columna] == Casilla.Trampa;
    }

    static void ActualizarTableroYNotificar(Ficha ficha, Casilla[,] tablero, int fila, int columna)
    {
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
        Console.WriteLine($"{ficha.Nombre} se movió a ({fila}, {columna}).");
    }
}
