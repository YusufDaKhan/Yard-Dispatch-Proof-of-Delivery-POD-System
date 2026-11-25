using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicaDispatch.Api.Migrations;

/// <inheritdoc />
public partial class _20261125125007_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                MaxLoadCapacity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DeliveryTickets",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DispatchOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: true),
                DeliveryAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CustomerInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryTickets", x => x.Id);
                table.ForeignKey(
                    name: "FK_DeliveryTickets_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "PodRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProofOfDelivery = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ReceivedSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                DeliveryTicketId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PodRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_PodRecords_DeliveryTickets_DeliveryTicketId",
                    column: x => x.DeliveryTicketId,
                    principalTable: "DeliveryTickets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TicketItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                WeightKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DeliveryTicketId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TicketItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_TicketItems_DeliveryTickets_DeliveryTicketId",
                    column: x => x.DeliveryTicketId,
                    principalTable: "DeliveryTickets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DeliveryTickets_VehicleId",
            table: "DeliveryTickets",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_PodRecords_DeliveryTicketId",
            table: "PodRecords",
            column: "DeliveryTicketId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TicketItems_DeliveryTicketId",
            table: "TicketItems",
            column: "DeliveryTicketId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PodRecords");

        migrationBuilder.DropTable(
            name: "TicketItems");

        migrationBuilder.DropTable(
            name: "DeliveryTickets");

        migrationBuilder.DropTable(
            name: "Vehicles");
    }
}
