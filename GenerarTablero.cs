public class GenerarTablero
{
    // Tipos de casillas en el tablero
    public enum Casilla
    {
        Camino,    // Casilla transitable
        Obstaculo, // Casilla con un obstáculo
        Trampa     // Casilla con una trampa
    }

    // Genera el tablero con obstáculos y trampas
    public static (Casilla[,], List<(int fila, int columna, Trampa.Tipo tipo)>) Generar(int ancho, int alto, int cantidadTrampas, int cantidadObstaculos)
    {
        var tablero = new Casilla[alto, ancho]; // Inicializa el tablero 2D
        var random = new Random(); // Generador de números aleatorios

        InicializarTablero(tablero); // Llena el tablero con obstáculos
        GenerarCamino(tablero, 1, 1, random); // Genera caminos a partir de la posición inicial (1,1)
        ColocarObstaculos(tablero, cantidadObstaculos, random); // Coloca obstáculos
        List<(int fila, int columna, Trampa.Tipo tipo)> trampas = new(); // Lista para trampas
        ColocarTrampas(tablero, cantidadTrampas, random, trampas); // Coloca trampas

        return (tablero, trampas); // Retorna el tablero y las trampas
    }

    // Llena el tablero con obstáculos
    private static void InicializarTablero(Casilla[,] tablero)
{
    for (int fila = 0; fila < tablero.GetLength(0); fila++)
    {
        for (int columna = 0; columna < tablero.GetLength(1); columna++)
        {
            // Asigna Obstáculo a los bordes
            if (fila == 0 || fila == tablero.GetLength(0) - 1 || columna == 0 || columna == tablero.GetLength(1) - 1)
            {
                tablero[fila, columna] = Casilla.Obstaculo;
            }
            else
            {
                tablero[fila, columna] = Casilla.Camino; // Resto del tablero como Camino
            }
        }
    }
}



    // Genera caminos en el tablero
    private static void GenerarCamino(Casilla[,] tablero, int fila, int columna, Random random)
    {
        tablero[fila, columna] = Casilla.Camino; // Marca como camino

        var direcciones = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }; // Direcciones posibles
        direcciones = direcciones.OrderBy(_ => random.Next()).ToArray(); // Orden aleatorio

        foreach (var (df, dc) in direcciones)
        {
            int nuevaFila = fila + df * 2;
            int nuevaColumna = columna + dc * 2;

            if (EsPosicionValida(tablero, nuevaFila, nuevaColumna) && tablero[nuevaFila, nuevaColumna] == Casilla.Obstaculo)
            {
                tablero[fila + df, columna + dc] = Casilla.Obstaculo; // Conecta camino
                GenerarCamino(tablero, nuevaFila, nuevaColumna, random); // Llamada recursiva
            }
        }
    }

    // Verifica si la posición es válida
    private static bool EsPosicionValida(Casilla[,] tablero, int fila, int columna)
    {
        return fila > 0 && fila < tablero.GetLength(0) - 1 && columna > 0 && columna < tablero.GetLength(1) - 1;
    }

    // Coloca obstáculos aleatorios
    private static void ColocarObstaculos(Casilla[,] tablero, int cantidadObstaculos, Random random)
    {
        int obstaculosColocados = 0;
        while (obstaculosColocados < cantidadObstaculos)
        {
            var (fila, columna) = ObtenerPosicionAleatoria(tablero, random);
            if (tablero[fila, columna] == Casilla.Camino)
            {
                tablero[fila, columna] = Casilla.Obstaculo; // Coloca obstáculo
                obstaculosColocados++;
            }
        }
    }

    // Coloca trampas aleatorias
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
                tablero[fila, columna] = Casilla.Trampa; // Coloca trampa
                trampasColocadas++;
            }
        }
    }

    // Genera un tipo de trampa aleatoria
    private static Trampa.Tipo GenerarTipoTrampa(Random random)
    {
        int tipo = random.Next(Enum.GetValues(typeof(Trampa.Tipo)).Length); // Selección aleatoria
        return (Trampa.Tipo)tipo;
    }

    // Obtiene una posición aleatoria que sea un camino
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
