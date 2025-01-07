public class Trampa
{
    public enum Tipo
    {
        Quita1Punto,
        Quita2Puntos,
        RetrocedeAlLugar // Tipo de trampa que hace retroceder a la ficha
    }

    public Tipo TipoDeTrampa { get; set; }

    public Trampa(Tipo tipo)
    {
        TipoDeTrampa = tipo;
    }

    // Método para aplicar el efecto de la trampa sobre la ficha
    public void AplicarTrampa(Ficha ficha)
    {
        switch (TipoDeTrampa)
        {
            case Tipo.Quita1Punto:
                ficha.PerderPuntos(1);
                Console.WriteLine($"¡{ficha.Nombre} cayó en una trampa que le quitó 1 punto!");
                break;
            case Tipo.Quita2Puntos:
                ficha.PerderPuntos(2);
                Console.WriteLine($"¡{ficha.Nombre} cayó en una trampa que le quitó 2 puntos!");
                break;
           
        }
    }
}