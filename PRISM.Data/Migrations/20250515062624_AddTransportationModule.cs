using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRISM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransportationPlanId",
                table: "ContractDeliveries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OriginLocationId = table.Column<int>(type: "int", nullable: false),
                    DestinationLocationId = table.Column<int>(type: "int", nullable: false),
                    TransportMode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EffectiveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    BaseRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FuelSurchargePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AdditionalFees = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Carrier = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRates_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRates_Locations_OriginLocationId",
                        column: x => x.OriginLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransportationPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlanNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OriginLocationId = table.Column<int>(type: "int", nullable: false),
                    DestinationLocationId = table.Column<int>(type: "int", nullable: false),
                    Carrier = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransportMode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedCostPerUnit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportationPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportationPlans_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportationPlans_Locations_OriginLocationId",
                        column: x => x.OriginLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransportationRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TransportationPlanId = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EstimatedTransitTime = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TransportMode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Carrier = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ScheduledDeparture = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActualDeparture = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ScheduledArrival = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActualArrival = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportationRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportationRoutes_Locations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportationRoutes_Locations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportationRoutes_TransportationPlans_TransportationPlanId",
                        column: x => x.TransportationPlanId,
                        principalTable: "TransportationPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDeliveries_TransportationPlanId",
                table: "ContractDeliveries",
                column: "TransportationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_DestinationLocationId",
                table: "ShippingRates",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_OriginLocationId_DestinationLocationId_Transpo~",
                table: "ShippingRates",
                columns: new[] { "OriginLocationId", "DestinationLocationId", "TransportMode", "EffectiveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportationPlans_DestinationLocationId",
                table: "TransportationPlans",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationPlans_OriginLocationId",
                table: "TransportationPlans",
                column: "OriginLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationPlans_PlanNumber",
                table: "TransportationPlans",
                column: "PlanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportationRoutes_FromLocationId",
                table: "TransportationRoutes",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationRoutes_ToLocationId",
                table: "TransportationRoutes",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationRoutes_TransportationPlanId",
                table: "TransportationRoutes",
                column: "TransportationPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDeliveries_TransportationPlans_TransportationPlanId",
                table: "ContractDeliveries",
                column: "TransportationPlanId",
                principalTable: "TransportationPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractDeliveries_TransportationPlans_TransportationPlanId",
                table: "ContractDeliveries");

            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropTable(
                name: "TransportationRoutes");

            migrationBuilder.DropTable(
                name: "TransportationPlans");

            migrationBuilder.DropIndex(
                name: "IX_ContractDeliveries_TransportationPlanId",
                table: "ContractDeliveries");

            migrationBuilder.DropColumn(
                name: "TransportationPlanId",
                table: "ContractDeliveries");
        }
    }
}
