public class Ficha
{
    public string Nombre { get; set; }
    public int Fila { get; set; } // Fila actual de la ficha
    public int Columna { get; set; } // Columna actual de la ficha
    public ConsoleColor Color { get; set; }
    public string Habilidad { get; set; } // Propiedad para habilidad
    public int Cooldown { get; internal set; } // Propiedad de enfriamiento
    public int Puntos { get; private set; } // Puntos o vida de la ficha

    public Ficha(string nombre, int fila, int columna, ConsoleColor color, string habilidad, int cooldown = 0, int puntosIniciales = 5)
    {
        Nombre = nombre;
        Fila = fila;
        Columna = columna;
        Color = color;
        Habilidad = habilidad; // Inicializa la habilidad
        Cooldown = cooldown; // Inicializa el cooldown
        Puntos = 5; // Inicializa los puntos
    }

    public void Mover(int nuevaFila, int nuevaColumna)
    {
        Fila = nuevaFila;
        Columna = nuevaColumna;
    }

    // Método para reducir los puntos
    public void PerderPuntos(int cantidad)
    {
        Puntos = Math.Max(0, Puntos - cantidad); // Evita que los puntos sean negativos
        Console.WriteLine($"{Nombre} ha perdido {cantidad} punto(s). Puntos restantes: {Puntos}");
    }

}