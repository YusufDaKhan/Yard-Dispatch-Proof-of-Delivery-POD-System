using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicaDispatch.Api.Migrations;

/// <inheritdoc />
public partial class _20261127140003_RenameMaxLoadCapacityToKg : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "MaxLoadCapacity",
            table: "Vehicles",
            newName: "MaxLoadCapacityKg");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "MaxLoadCapacityKg",
            table: "Vehicles",
            newName: "MaxLoadCapacity");
    }
}
