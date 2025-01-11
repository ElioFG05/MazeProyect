using Spectre.Console;
using System.Collections.Generic;

public class Informacion
{
    private static Table tablaInformacion = new Table();


    public static void Inicializar()
    {
        tablaInformacion = new Table()
            .Border(TableBorder.Rounded)
            .Title("Información de Fichas")
            .AddColumn("Nombre")
            .AddColumn("Puntos")
            .AddColumn("Habilidad")
            .AddColumn("Cooldown");
    }

    public static void MostrarInformacion(List<Ficha> fichas)
    {
        tablaInformacion.Rows.Clear();

        foreach (var ficha in fichas)
        {
            tablaInformacion.AddRow(
                ficha.Nombre,
                ficha.Puntos.ToString(),
                ficha.Habilidad,
                ficha.Cooldown > 0 ? ficha.Cooldown.ToString() : "Listo");
        }

        AnsiConsole.Live(tablaInformacion).Start(ctx =>
        {
            ctx.Refresh();
        });
    }
}
