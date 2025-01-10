using System;
using System.Collections.Generic;
using System.Linq;

public class GenerarTablero
{
    public enum Casilla
    {
        Camino,
        Obstaculo,
        Trampa
    }

    public static (Casilla[,], List<(int fila, int columna, Trampa.Tipo tipo)>) Generar(int ancho, int alto, int cantidadTrampas, int cantidadObstaculos)
    {
        var tablero = new Casilla[alto, ancho];
        var random = new Random();

        InicializarTablero(tablero);
        GenerarCamino(tablero, 1, 1, random); // Genera caminos desde (1,1)

        if (!HayCaminosConectados(tablero)) // Verifica que los caminos no estén bloqueados
        {
            Console.WriteLine("Error: Los caminos no están completamente conectados.");
            return (tablero, new List<(int, int, Trampa.Tipo)>());
        }

        ColocarObstaculos(tablero, cantidadObstaculos, random);
        List<(int fila, int columna, Trampa.Tipo tipo)> trampas = new();
        ColocarTrampas(tablero, cantidadTrampas, random, trampas);

        return (tablero, trampas);
    }

    public static void InicializarTablero(Casilla[,] tablero)
    {
        for (int fila = 0; fila < tablero.GetLength(0); fila++)
        {
            for (int columna = 0; columna < tablero.GetLength(1); columna++)
            {
                if (fila == 0 || fila == tablero.GetLength(0) - 1 || columna == 0 || columna == tablero.GetLength(1) - 1)
                {
                    tablero[fila, columna] = Casilla.Obstaculo; // Bordes como obstáculos
                }
                else
                {
                    tablero[fila, columna] = Casilla.Camino; // Inicialmente todo es camino
                }
            }
        }
    }

  public static void GenerarCamino(Casilla[,] tablero, int fila, int columna, Random random)
{
    tablero[fila, columna] = Casilla.Camino; // Marca la posición actual como camino

    var direcciones = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
    direcciones = direcciones.OrderBy(_ => random.Next()).ToArray(); // Direcciones aleatorias

    foreach (var (df, dc) in direcciones)
    {
        int nuevaFila = fila + df * 2;
        int nuevaColumna = columna + dc * 2;

        if (EsPosicionValida(tablero, nuevaFila, nuevaColumna) && tablero[nuevaFila, nuevaColumna] == Casilla.Obstaculo)
        {
            tablero[fila + df, columna + dc] = Casilla.Camino; // Conecta el camino intermedio
            GenerarCamino(tablero, nuevaFila, nuevaColumna, random); // Llamada recursiva
        }
    }
}


    private static bool EsPosicionValida(Casilla[,] tablero, int fila, int columna)
    {
        return fila > 0 && fila < tablero.GetLength(0) - 1 && columna > 0 && columna < tablero.GetLength(1) - 1;
    }

   private static void ColocarObstaculos(Casilla[,] tablero, int cantidadObstaculos, Random random)
{
    int obstaculosColocados = 0;
    while (obstaculosColocados < cantidadObstaculos)
    {
        var (fila, columna) = ObtenerPosicionAleatoria(tablero, random);
        if (tablero[fila, columna] == Casilla.Camino) // Solo coloca el obstáculo en una casilla de camino
        {
            tablero[fila, columna] = Casilla.Obstaculo; // Coloca el obstáculo
            obstaculosColocados++;

            // Verifica la conectividad del tablero después de colocar un obstáculo
            if (!HayCaminosConectados(tablero))
            {
                // Si los caminos no están conectados, deshace la colocación
                tablero[fila, columna] = Casilla.Camino;
                obstaculosColocados--;
            }
        }
    }
}



    public static void ColocarTrampas(Casilla[,] tablero, int cantidadTrampas, Random random, List<(int fila, int columna, Trampa.Tipo tipo)> trampas)
    {
        int trampasColocadas = 0;
        while (trampasColocadas < cantidadTrampas)
        {
            var (fila, columna) = ObtenerPosicionAleatoria(tablero, random);
            if (tablero[fila, columna] == Casilla.Camino)
            {
                var tipoTrampa = GenerarTipoTrampa(random);
                trampas.Add((fila, columna, tipoTrampa));
                tablero[fila, columna] = Casilla.Trampa; // Coloca la trampa
                trampasColocadas++;
            }
        }
    }

    private static Trampa.Tipo GenerarTipoTrampa(Random random)
    {
        int tipo = random.Next(Enum.GetValues(typeof(Trampa.Tipo)).Length);
        return (Trampa.Tipo)tipo;
    }

    public static (int fila, int columna) ObtenerPosicionAleatoria(Casilla[,] tablero, Random random)
    {
        while (true)
        {
            int fila = random.Next(tablero.GetLength(0));
            int columna = random.Next(tablero.GetLength(1));
            if ((tablero[fila, columna] == Casilla.Camino || tablero[fila, columna] == Casilla.Obstaculo) &&
                !(fila == 0 || fila == tablero.GetLength(0) - 1 || columna == 0 || columna == tablero.GetLength(1) - 1))
            {
                return (fila, columna);
            }
        }
    }

    private static bool HayCaminosConectados(Casilla[,] tablero)
{
    bool[,] visitado = new bool[tablero.GetLength(0), tablero.GetLength(1)];

    // Encuentra el primer camino para iniciar DFS
    bool encontradoCamino = false;
    for (int i = 0; i < tablero.GetLength(0); i++)
    {
        for (int j = 0; j < tablero.GetLength(1); j++)
        {
            if (tablero[i, j] == Casilla.Camino)
            {
                DFS(tablero, visitado, i, j);
                encontradoCamino = true;
                break;
            }
        }
        if (encontradoCamino) break;
    }

    // Si encontramos caminos, revisamos que todos estén conectados
    for (int i = 0; i < tablero.GetLength(0); i++)
    {
        for (int j = 0; j < tablero.GetLength(1); j++)
        {
            if (tablero[i, j] == Casilla.Camino && !visitado[i, j])
            {
                return false; // Hay caminos no conectados
            }
        }
    }

    return true; // Todos los caminos están conectados
}


    private static void DFS(Casilla[,] tablero, bool[,] visitado, int fila, int columna)
    {
        if (!EsPosicionValida(tablero, fila, columna) || visitado[fila, columna] || tablero[fila, columna] != Casilla.Camino)
            return;

        visitado[fila, columna] = true;

        DFS(tablero, visitado, fila - 1, columna); // Arriba
        DFS(tablero, visitado, fila + 1, columna); // Abajo
        DFS(tablero, visitado, fila, columna - 1); // Izquierda
        DFS(tablero, visitado, fila, columna + 1); // Derecha
    }
}
