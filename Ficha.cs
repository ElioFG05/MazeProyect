using Casilla = GenerarTablero.Casilla;

public class Ficha
{
    public string Nombre { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
    public ConsoleColor Color { get; set; }
    public string Habilidad { get; set; }
    public int Cooldown { get; internal set; }
    public int Puntos { get; private set; }

    public Ficha(string nombre, int fila, int columna, ConsoleColor color, string habilidad, int cooldown = 0, int puntosIniciales = 4)
    {
        Nombre = nombre;
        Fila = fila;
        Columna = columna;
        Color = color;
        Habilidad = habilidad;
        Cooldown = cooldown;
        Puntos = puntosIniciales;
    }

    public void Mover(int nuevaFila, int nuevaColumna)
    {
        Fila = nuevaFila;
        Columna = nuevaColumna;
    }

    public void PerderPuntos(int puntos)
    {
        Puntos -= puntos;
        Console.WriteLine($"¡{Nombre} perdió {puntos} puntos! Puntos restantes: {Puntos}");
    }

    public void UsarHabilidad(Casilla[,] tablero, Random random,List<Ficha> fichasSeleccionadas)
    {
        if (Cooldown > 0)
        {
            Console.WriteLine($"La habilidad {Habilidad} está en enfriamiento por {Cooldown} turnos más.");
            return;
        }

        switch (Habilidad)
        {
            case "Teletransportación Aleatoria":
                TeletransportarAleatoriamente(tablero, random,fichasSeleccionadas);
                Cooldown = 3;
                break;
            case "Inmunidad Temporal":
                InmunidadTemporal();
                Cooldown = 2;
                break;
            case "Paso Fantasma":
                PasoFantasma();
                Cooldown = 4;
                break;
            case "Curación Rápida":
                Curar();
                Cooldown = 3;
                break;
            case "Avance Doble":
                AvanceDoble(tablero,fichasSeleccionadas);
                Cooldown = 2;
                break;
            default:
                Console.WriteLine("Habilidad no reconocida.");
                break;
        }
    }

    private void TeletransportarAleatoriamente(Casilla[,] tablero, Random random,List<Ficha> fichasSeleccionadas)
    {
        int fila, columna;
        do
        {
            fila = random.Next(tablero.GetLength(0));
            columna = random.Next(tablero.GetLength(1));
        } while (tablero[fila, columna] != Casilla.Camino);
        Mover(fila, columna);
        Console.WriteLine($"{Nombre} se teletransportó a la posición ({fila}, {columna}).");
        TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);

    }

    private void InmunidadTemporal()
    {
        Console.WriteLine($"{Nombre} es inmune a la pérdida de puntos por 1 turno.");
    }

    private void PasoFantasma()
    {
        Console.WriteLine($"{Nombre} puede atravesar obstáculos por 2 turnos.");
    }

    private void Curar()
    {
        Puntos += 2;
        if (Puntos > 4) Puntos = 4;
        Console.WriteLine($"{Nombre} se ha curado. Puntos actuales: {Puntos}");
    }

    public void AvanceDoble(Casilla[,] tablero,List<Ficha> fichasSeleccionadas)
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
                    i--;
                    continue;
            }

            if (IsMovimientoValido(nuevaFila, nuevaColumna, tablero))
            {
                Mover(nuevaFila, nuevaColumna);
                Console.WriteLine($"{Nombre} se ha movido a la posición ({nuevaFila}, {nuevaColumna}).");
                // Llamar a DibujarTablero para actualizar la visualización
                TableroDrawer.DibujarTablero(tablero, fichasSeleccionadas);
            }
            else
            {
                Console.WriteLine("Movimiento no permitido. Intenta nuevamente.");
                i--;
            }
        }
    }

    public static bool IsMovimientoValido(int nuevaFila, int nuevaColumna, Casilla[,] tablero)
    {
        if (nuevaFila < 0 || nuevaFila >= tablero.GetLength(0) || nuevaColumna < 0 || nuevaColumna >= tablero.GetLength(1))
        {
            return false;
        }

        if (tablero[nuevaFila, nuevaColumna] == Casilla.Obstaculo)
        {
            return false;
        }

        return true;
    }
}
