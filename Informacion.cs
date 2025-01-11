using Spectre.Console;
using System.Collections.Generic;

public class Informacion
{
    // Tabla para mostrar la información de las fichas
    private static Table tablaInformacion = new Table();

    // Inicializa la tabla con las columnas necesarias
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

    // Muestra la información actualizada de las fichas en la consola
    public static void MostrarInformacion(List<Ficha> fichas)
    {
        // Limpia las filas anteriores antes de agregar nueva información
        tablaInformacion.Rows.Clear();

        foreach (var ficha in fichas)
        {
            // Agrega una nueva fila con la información de cada ficha
            tablaInformacion.AddRow(
                ficha.Nombre,
                ficha.Puntos.ToString(),
                ficha.Habilidad,
                ficha.Cooldown > 0 ? ficha.Cooldown.ToString() : "Listo");
        }

        // Refresca la tabla para mostrar la información actualizada en la consola
        AnsiConsole.Live(tablaInformacion).Start(ctx =>
        {
            ctx.Refresh();
        });
    }
}
