using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APBDprojekt.Migrations
{
    /// <inheritdoc />
    public partial class creatingtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientPK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    telephoneNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    has5PercentDiscount = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientPK);
                });

            migrationBuilder.CreateTable(
                name: "Software",
                columns: table => new
                {
                    SoftwarePk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    currentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    purchasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    subscriptionPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    canBuy = table.Column<bool>(type: "bit", nullable: false),
                    canSubscribe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Software", x => x.SoftwarePk);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hashed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ClientCompany",
                columns: table => new
                {
                    ClientPK = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KRS = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCompany", x => x.ClientPK);
                    table.ForeignKey(
                        name: "FK_ClientCompany_Client_ClientPK",
                        column: x => x.ClientPK,
                        principalTable: "Client",
                        principalColumn: "ClientPK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientPerson",
                columns: table => new
                {
                    ClientPK = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    peselNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPerson", x => x.ClientPK);
                    table.ForeignKey(
                        name: "FK_ClientPerson_Client_ClientPK",
                        column: x => x.ClientPK,
                        principalTable: "Client",
                        principalColumn: "ClientPK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    ContractPK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    priceWithDiscounts = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    alreadyPaid = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paymentStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    paymentEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endOfSupport = table.Column<DateTime>(type: "datetime2", nullable: false),
                    signed = table.Column<bool>(type: "bit", nullable: false),
                    currentSoftwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientFK = table.Column<int>(type: "int", nullable: false),
                    SoftwareFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.ContractPK);
                    table.ForeignKey(
                        name: "FK_Contract_Client_ClientFK",
                        column: x => x.ClientFK,
                        principalTable: "Client",
                        principalColumn: "ClientPK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contract_Software_SoftwareFK",
                        column: x => x.SoftwareFK,
                        principalTable: "Software",
                        principalColumn: "SoftwarePk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    DiscountPK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    percentValue = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    dateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoftwareFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.DiscountPK);
                    table.ForeignKey(
                        name: "FK_Discount_Software_SoftwareFK",
                        column: x => x.SoftwareFK,
                        principalTable: "Software",
                        principalColumn: "SoftwarePk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionPK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    priceWithDiscounts = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subscriptionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    subscriptionEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    currentSoftwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientFK = table.Column<int>(type: "int", nullable: false),
                    SoftwareFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionPK);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Client_ClientFK",
                        column: x => x.ClientFK,
                        principalTable: "Client",
                        principalColumn: "ClientPK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Software_SoftwareFK",
                        column: x => x.SoftwareFK,
                        principalTable: "Software",
                        principalColumn: "SoftwarePk",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FK_userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_FK_userId",
                        column: x => x.FK_userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractPayment",
                columns: table => new
                {
                    ContractPaymentPK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DateOfPayment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientFK = table.Column<int>(type: "int", nullable: false),
                    ContractFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPayment", x => x.ContractPaymentPK);
                    table.ForeignKey(
                        name: "FK_ContractPayment_Client_ClientFK",
                        column: x => x.ClientFK,
                        principalTable: "Client",
                        principalColumn: "ClientPK");
                    table.ForeignKey(
                        name: "FK_ContractPayment_Contract_ContractFK",
                        column: x => x.ContractFK,
                        principalTable: "Contract",
                        principalColumn: "ContractPK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Software",
                columns: new[] { "SoftwarePk", "canBuy", "canSubscribe", "category", "currentVersion", "Description", "name", "purchasePrice", "subscriptionPrice" },
                values: new object[] { 1, true, false, "contract", "1.0", "C#", "Rider", 1000m, null });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "DiscountPK", "dateFrom", "dateTo", "name", "percentValue", "SoftwareFK", "type" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Student", 50.0m, 1, "contract" },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sale", 30.0m, 1, "contract" },
                    { 3, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Summer", 80.0m, 1, "contract" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contract_ClientFK",
                table: "Contract",
                column: "ClientFK");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_SoftwareFK",
                table: "Contract",
                column: "SoftwareFK");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPayment_ClientFK",
                table: "ContractPayment",
                column: "ClientFK");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPayment_ContractFK",
                table: "ContractPayment",
                column: "ContractFK");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_SoftwareFK",
                table: "Discount",
                column: "SoftwareFK");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FK_userId",
                table: "RefreshTokens",
                column: "FK_userId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ClientFK",
                table: "Subscriptions",
                column: "ClientFK");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SoftwareFK",
                table: "Subscriptions",
                column: "SoftwareFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientCompany");

            migrationBuilder.DropTable(
                name: "ClientPerson");

            migrationBuilder.DropTable(
                name: "ContractPayment");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Software");
        }
    }
}
