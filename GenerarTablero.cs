public class GenerarTablero
{
    public enum Casilla
    {
        Camino,       // Representa un camino transitable
        Obstaculo,    // Representa un obstáculo (pared)
        Trampa        // Representa una trampa oculta
    }

    public static Casilla[,] Generar(int ancho, int alto, int cantidadObstaculos, int cantidadTrampas)
    {
        var tablero = new Casilla[alto, ancho];
        var random = new Random();

        Inicializar(tablero);
        GenerarCamino(tablero, 1, 1, random); // Cambiar CrearCamino por GenerarCamino

        List<(int fila, int columna, Trampa.Tipo tipo)> trampas = new();
        ColocarTrampas(tablero, cantidadTrampas, random, trampas);

        return tablero;
    }

    private static void Inicializar(Casilla[,] tablero)
    {
        for (int fila = 0; fila < tablero.GetLength(0); fila++)
        {
            for (int columna = 0; columna < tablero.GetLength(1); columna++)
            {
                tablero[fila, columna] = Casilla.Obstaculo;
            }
        }
    }

    private static void GenerarCamino(Casilla[,] tablero, int fila, int columna, Random random)
    {
        tablero[fila, columna] = Casilla.Camino;

        var direcciones = new (int, int)[] { (-2, 0), (2, 0), (0, -2), (0, 2) };
        direcciones = direcciones.OrderBy(_ => random.Next()).ToArray();

        foreach (var (df, dc) in direcciones)
        {
            int nuevaFila = fila + df;
            int nuevaColumna = columna + dc;

            if (nuevaFila > 0 && nuevaFila < tablero.GetLength(0) - 1 && nuevaColumna > 0 && nuevaColumna < tablero.GetLength(1) - 1 && tablero[nuevaFila, nuevaColumna] == Casilla.Obstaculo)
            {
                tablero[fila + df / 2, columna + dc / 2] = Casilla.Camino;
                GenerarCamino(tablero, nuevaFila, nuevaColumna, random);
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
                tablero[fila, columna] = Casilla.Trampa;
                trampasColocadas++;
            }
        }
    }

    private static Trampa.Tipo GenerarTipoTrampa(Random random)
    {
        int tipo = random.Next(3); // Suponiendo 3 tipos de trampas
        return (Trampa.Tipo)tipo;
    }

    public static (int, int) ObtenerPosicionAleatoria(Casilla[,] tablero, Random random)
    {
        while (true)
        {
            int fila = random.Next(tablero.GetLength(0));
            int columna = random.Next(tablero.GetLength(1));

            if (tablero[fila, columna] == Casilla.Camino)
            {
                return (fila, columna);
            }
        }
    }
}
