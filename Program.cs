﻿using static GenerarTablero;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using Casilla = GenerarTablero.Casilla;

partial class Program
{
    public const int Ancho = 44;
    public const int Alto = 25;
    public const int CantidadTrampas = 35;
    public const int CantidadObstaculos = 200;
    public const int CantidadObjetos = 8;

    public static List<Ficha> fichasSeleccionadas = new();
    public static List<Ficha> fichasDisponibles = new()
    {
        new Ficha("Ficha Morada", -1, -1, ConsoleColor.Magenta, "Teletransportación Aleatoria", 4),
        new Ficha("Ficha Verde", -1, -1, ConsoleColor.Green, "Inmunidad Temporal", 3),
        new Ficha("Ficha Azul", -1, -1, ConsoleColor.Blue, "Paso Fantasma", 3),
        new Ficha("Ficha Amarilla", -1, -1, ConsoleColor.Yellow, "Curación Rápida", 3),
        new Ficha("Ficha AzulCeleste", -1, -1, ConsoleColor.Cyan, "Avance Triple", 3)
    };

    static void Main()
    {
        System.Console.WriteLine("El primer jugador en alcanzar 3 `Objetos` amarillos gana la partida ");
        System.Console.WriteLine("Evite las trampas(Casillas Rojas) asi evitara´ perder vida o que aumente el cooldown de su habilidad");
        System.Console.WriteLine("Enjoy the game!");
        System.Console.WriteLine("");
        // Generar tablero y trampas
        var (tablero, trampas) = Generar(Ancho, Alto, CantidadTrampas, CantidadObstaculos);
        Random random = new();

        // Limpiar la selección de fichas al inicio
        fichasSeleccionadas.Clear();

        // Seleccionar fichas para los jugadores
        SeleccionarFichas(tablero, fichasSeleccionadas);
        
        // Colocar objetos en el tablero
        ColocarObjetos(tablero,CantidadObjetos,random); // Coloca 6 objetos


        // Dibujar el tablero inicial
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);

        // Inicializar información del juego
        Informacion.Inicializar();

        int turnoActual = 0;

        while (fichasSeleccionadas.Count > 0)
        {
            Console.Clear();
            TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
            Informacion.MostrarInformacion(fichasSeleccionadas);
            Console.WriteLine($"\nTurno del Jugador {turnoActual + 1} ({fichasSeleccionadas[turnoActual].Nombre})");

            // Menú para seleccionar acción
            var accion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Elige tu acción:")
                .AddChoices(new[] { "Mover Ficha", "Usar Habilidad" })
            );

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
                    TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
                    fichasSeleccionadas[turnoActual].FinalizarTurno();
                    turnoActual = (turnoActual + 1) % fichasSeleccionadas.Count; // Avanzar al siguiente turno*/
                     
                     
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
                /*turnoActual = (turnoActual + 1) % fichasSeleccionadas.Count;*/
            }
            
            // Verificar si la ficha perdió todos los puntos
            if (fichasSeleccionadas[turnoActual].Puntos == 0)
            {
                Console.WriteLine($"{fichasSeleccionadas[turnoActual].Nombre} ha perdido todos sus puntos. ¡El juego ha terminado!");
            break; // Sale del bucle y termina el juego
            }

            // Verificar si la ficha perdió todos los puntos
            if (!VerificarPuntos(fichasSeleccionadas[turnoActual]))
            {
                fichasSeleccionadas.RemoveAt(turnoActual);
                if (turnoActual >= fichasSeleccionadas.Count)
                turnoActual = 0; // Reiniciar el turno si excede el número de fichas
                
            }
            /*else
            {
                turnoActual = (turnoActual + 1) % fichasSeleccionadas.Count;
            }*/

            foreach (var ficha in fichasSeleccionadas)
            {
                // Reducir cooldown si es necesario
                /*if (ficha.Cooldown > 0)
                {
                    ficha.Cooldown--;
                }*/

                // Reducir inmunidad si está activa
                if (ficha.TurnosInmunidad > 0)
                {
                    ficha.TurnosInmunidad--;
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
        // Crear el prompt con detalles adicionales sobre cada ficha
        var fichaElegida = AnsiConsole.Prompt(new SelectionPrompt<Ficha>()
            .Title($"Jugador {i}, elija su ficha:")
            .UseConverter(ficha => $"{ficha.Nombre} - Habilidad: {ficha.Habilidad} - Cooldown: {ficha.Cooldown} turnos")
            .AddChoices(fichasDisponibles));

        // Remover la ficha elegida de la lista de disponibles
        fichasDisponibles.Remove(fichaElegida);

        // Obtener una posición aleatoria para colocar la ficha en el tablero
        var posicion = ObtenerPosicionAleatoria(tablero, random);
        fichaElegida.Fila = posicion.Item1;
        fichaElegida.Columna = posicion.Item2;

        // Agregar la ficha elegida a la lista de fichas seleccionadas
        fichasSeleccionadas.Add(fichaElegida);

        // Mostrar un mensaje con la información de la ficha seleccionada
        AnsiConsole.MarkupLine($"[bold]{fichaElegida.Nombre}[/] con la habilidad [italic]{fichaElegida.Habilidad}[/] fue seleccionada por Jugador {i} y colocada en ({posicion.Item1}, {posicion.Item2}).");
    }
}


static bool MoverFicha(Ficha ficha, int nuevaFila, int nuevaColumna, Casilla[,] tablero, List<(int fila, int columna, Trampa.Tipo tipo)> trampas)
{
    // Verificar si el movimiento es válido o si la habilidad Paso Fantasma está activa
    if (EsMovimientoValido(tablero, nuevaFila, nuevaColumna) || ficha.turnosPasoFantasma > 0)
    {
        // Mover la ficha a la nueva posición
        ficha.Mover(nuevaFila, nuevaColumna, tablero);

        // Manejar trampas en la nueva posición
        ManejarTrampa(nuevaFila, nuevaColumna, ficha, tablero, trampas);

        
        // Verificar los puntos de la ficha después de mover
        if (!VerificarPuntos(ficha))
        {
            return false;
        }

        // Actualizar el tablero y notificar el movimiento
        ActualizarTableroYNotificar(ficha, tablero, nuevaFila, nuevaColumna);

        // Reducir el contador de Paso Fantasma si está activo
        if (ficha.turnosPasoFantasma > 0)
        {
            ficha.turnosPasoFantasma--;
            if (ficha.turnosPasoFantasma == 0)
            {
                Console.WriteLine($"{ficha.Nombre} ya no puede atravesar obstáculos.");
            }
        }
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

        if (trampa != default((int fila, int columna, Trampa.Tipo tipo)))
        {
            switch (trampa.tipo)
            {
                case Trampa.Tipo.Quita1Punto:
                    ficha.PerderPuntos(1);
                    Console.WriteLine($"Trampa activada,Ficha {ficha.Nombre} perdió 10% de vida .");
                    Thread.Sleep(3000); // Pausa de 3 segundos
                    break;

                case Trampa.Tipo.Quita2Puntos:
                    ficha.PerderPuntos(2);
                    Console.WriteLine($"Trampa activada,Ficha {ficha.Nombre} perdió 20% de vida .");
                    Thread.Sleep(3000); // Pausa de 3 segundos
                    break;

                case Trampa.Tipo.AumentarCooldown:
                    ficha.AumentarCooldown(5);
                    Console.WriteLine($"Trampa AumentarCooldown activada en ({fila}, {columna}). Cooldown de {ficha.Nombre} aumentado en 5 turnos.");
                    Thread.Sleep(3000); // Pausa de 3 segundos
                    break;

                // Aquí puedes agregar más tipos de trampas si lo deseas
            }

            // Cambiar el estado de la casilla de Trampa a Camino
            tablero[fila, columna] = Casilla.Camino;

            // Eliminar la trampa de la lista
            trampas.Remove(trampa);

            Console.WriteLine($"Casilla en ({fila}, {columna}) ahora es Camino.");
        }
    }
}


    static bool EsMovimientoValido(Casilla[,] tablero, int fila, int columna)
{
    return tablero[fila, columna] == Casilla.Camino || 
           tablero[fila, columna] == Casilla.Trampa || 
           tablero[fila, columna] == Casilla.Objeto; // Permitir movimiento a objetos
}


    static void ActualizarTableroYNotificar(Ficha ficha, Casilla[,] tablero, int fila, int columna)
    {
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
        Console.WriteLine($"{ficha.Nombre} se movió a ({fila}, {columna}).");
    }
}