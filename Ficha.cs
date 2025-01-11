using Casilla = GenerarTablero.Casilla;

public class Ficha
{
    public string Nombre { get; set; }  // Nombre de la ficha
    public int Fila { get; set; }       // Posición de la ficha en la fila
    public int Columna { get; set; }    // Posición de la ficha en la columna
    public ConsoleColor Color { get; set; }  // Color de la ficha
    public string Habilidad { get; set; }    // Habilidad especial de la ficha
    public int Cooldown { get;  set; }  // Turnos restantes para reutilizar la habilidad
    public int Puntos { get; set; }    // Puntos actuales de la ficha
    public int TurnosInmunidad { get;  set; } = 0;  // Turnos restantes de inmunidad
    public int turnosPasoFantasma;  // Contador para la habilidad Paso Fantasma

    // Constructor para inicializar una ficha
    public Ficha(string nombre, int fila, int columna, ConsoleColor color, string habilidad, int cooldown = 0, int puntosIniciales = 1)
    {
        Nombre = nombre;
        Fila = fila;
        Columna = columna;
        Color = color;
        Habilidad = habilidad;
        Cooldown = cooldown;
        Puntos = puntosIniciales;
    }

        // Mueve la ficha a una nueva posición, permitiendo atravesar obstáculos con "Paso Fantasma"
    public bool Mover(int nuevaFila, int nuevaColumna, Casilla[,] tablero)
    {
        // Verificar si la casilla es un obstáculo
        Casilla casillaDestino = tablero[nuevaFila, nuevaColumna];

        // Si la ficha tiene la habilidad "Paso Fantasma", podrá atravesar obstáculos
        if (turnosPasoFantasma > 0 && casillaDestino == Casilla.Obstaculo)
        {
            Console.WriteLine($"{Nombre} atraviesa un obstáculo debido a la habilidad Paso Fantasma.");
            Fila = nuevaFila;
            Columna = nuevaColumna;
            return true; // El movimiento fue exitoso
        }

        // Si no tiene la habilidad o no es un obstáculo, comprobar si la casilla es libre
        if (casillaDestino != Casilla.Obstaculo)
        {
            Fila = nuevaFila;
            Columna = nuevaColumna;
            return true; // El movimiento fue exitoso
        }

        Console.WriteLine($"{Nombre} no puede moverse a una casilla con obstáculo.");
        return false; // Movimiento no permitido
    }

    // Mueve la ficha a una nueva posición
    public void Mover(int nuevaFila, int nuevaColumna)
    {
        Fila = nuevaFila;
        Columna = nuevaColumna;
    }

    // Reduce puntos de la ficha, si no está en inmunidad
    public void PerderPuntos(int puntos)
    {
        if (TurnosInmunidad > 0)
        {
            Console.WriteLine($"{Nombre} es inmune a la pérdida de puntos este turno.");
        }
        else
        {
            Puntos -= puntos;
            Console.WriteLine($"¡{Nombre} perdió {puntos} puntos! Puntos restantes: {Puntos}");
        }
    }
    // Finaliza el turno de la ficha, gestionando la inmunidad, cooldown y Paso Fantasma
    public void FinalizarTurno()
    {
        if (TurnosInmunidad > 0)
        {
            TurnosInmunidad--;  // Reducir el contador de inmunidad
            if (TurnosInmunidad == 0)
            {
                Console.WriteLine($"{Nombre} ya no es inmune a la pérdida de puntos.");
            }
        }

        if (Cooldown > 0)
        {
            Cooldown--;  // Reducir el contador de cooldown de habilidad
        }

        // Decrementar los turnos de la habilidad Paso Fantasma si está activa
        if (turnosPasoFantasma > 0)
        {
            turnosPasoFantasma--;  // Reducir el contador de turnos de Paso Fantasma
            if (turnosPasoFantasma == 0)
            {
                Console.WriteLine($"{Nombre} ya no puede atravesar obstáculos.");
            }
        }
    }


    

    // Usa la habilidad especial de la ficha, si no está en cooldown
    public void UsarHabilidad(Casilla[,] tablero, Random random, List<Ficha> fichasSeleccionadas)
    {
        if (Cooldown > 0)
        {
            Console.WriteLine($"La habilidad {Habilidad} está en enfriamiento por {Cooldown} turnos más.");
            return;
        }

        switch (Habilidad)
        {
            case "Teletransportación Aleatoria":
                TeletransportarAleatoriamente(tablero, random, fichasSeleccionadas);
                Cooldown = 3;  // Establece cooldown de 3 turnos
                break;
            case "Inmunidad Temporal":
                InmunidadTemporal();
                Cooldown = 2;  // Establece cooldown de 2 turnos
                break;
            case "Paso Fantasma":
                PasoFantasma();
                Cooldown = 4;  // Establece cooldown de 4 turnos
                break;
            case "Curación Rápida":
                Curar();
                Cooldown = 3;  // Establece cooldown de 3 turnos
                break;
            case "Avance Doble":
                AvanceDoble(tablero, fichasSeleccionadas);
                Cooldown = 2;  // Establece cooldown de 2 turnos
                break;
            default:
                Console.WriteLine("Habilidad no reconocida.");
                break;
        }
    }

    // Teletransporta la ficha a una posición aleatoria en el tablero
    private void TeletransportarAleatoriamente(Casilla[,] tablero, Random random, List<Ficha> fichasSeleccionadas)
    {
        int fila, columna;
        do
        {
            fila = random.Next(tablero.GetLength(0));
            columna = random.Next(tablero.GetLength(1));
        } while (tablero[fila, columna] != Casilla.Camino);  // Busca una casilla válida
        Mover(fila, columna);
        Console.WriteLine($"{Nombre} se teletransportó a la posición ({fila}, {columna}).");
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
    }

    // Activa la inmunidad temporal para evitar pérdida de puntos
    private void InmunidadTemporal()
    {
        if (TurnosInmunidad == 0)  // Si la inmunidad no está activa
        {
            TurnosInmunidad = 6;  // Activar inmunidad por 6 turnos
            Console.WriteLine($"{Nombre} ha activado Inmunidad Temporal por 3 turnos.");
        }
        else
        {
            Console.WriteLine($"{Nombre} ya tiene Inmunidad Temporal activa.");
        }
    }

    // Permite a la ficha atravesar obstáculos durante 2 turnos
   private void PasoFantasma()
    {
        turnosPasoFantasma = 4;  // La habilidad dura 2 turnos
        Console.WriteLine($"{Nombre} puede atravesar obstáculos por 2 turnos.");
    }

    // Método para comprobar si la ficha puede atravesar un obstáculo
    public bool PuedeAtravesarObstaculo(Casilla casilla)
    {
        return turnosPasoFantasma > 0 && casilla == Casilla.Obstaculo;
    }

    // Incrementa los puntos de la ficha, hasta un máximo de 4
    private void Curar()
    {
        Puntos += 2;
        if (Puntos > 4) Puntos = 4;  // Asegura que los puntos no excedan 4
        Console.WriteLine($"{Nombre} se ha curado. Puntos actuales: {Puntos}");
    }

    // Permite a la ficha moverse dos veces en un turno
    public void AvanceDoble(Casilla[,] tablero, List<Ficha> fichasSeleccionadas)
    {
        Console.WriteLine($"{Nombre} puede moverse dos veces este turno.");

        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine($"Movimiento {i + 1} de 2: Usa las teclas de dirección para mover la ficha.");
            ConsoleKey teclaPresionada = Console.ReadKey(true).Key;

            int nuevaFila = Fila;
            int nuevaColumna = Columna;

            switch (teclaPresionada)
            {
                case ConsoleKey.UpArrow: nuevaFila--; break;
                case ConsoleKey.DownArrow: nuevaFila++; break;
                case ConsoleKey.LeftArrow: nuevaColumna--; break;
                case ConsoleKey.RightArrow: nuevaColumna++; break;
                default:
                    Console.WriteLine("Tecla inválida. Usa las flechas de dirección.");
                    i--;  // Repetir el movimiento si la tecla es inválida
                    continue;
            }

            if (IsMovimientoValido(nuevaFila, nuevaColumna, tablero))
            {
                Mover(nuevaFila, nuevaColumna);
                Console.WriteLine($"{Nombre} se ha movido a la posición ({nuevaFila}, {nuevaColumna}).");
                TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
            }
            else
            {
                Console.WriteLine("Movimiento no permitido. Intenta nuevamente.");
                i--;  // Repetir el movimiento si es inválido
            }
        }
    }

    // Verifica si un movimiento es válido, es decir, no fuera de los límites ni hacia un obstáculo
    public static bool IsMovimientoValido(int nuevaFila, int nuevaColumna, Casilla[,] tablero)
    {
        if (nuevaFila < 0 || nuevaFila >= tablero.GetLength(0) || nuevaColumna < 0 || nuevaColumna >= tablero.GetLength(1))
        {
            return false;  // Movimiento fuera de los límites del tablero
        }

        return tablero[nuevaFila, nuevaColumna] != Casilla.Obstaculo;  // Verifica que la casilla no sea un obstáculo
    }
}
