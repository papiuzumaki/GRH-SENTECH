using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GRH_SENTECH.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    budget = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "postes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    intitule = table.Column<string>(type: "text", nullable: false),
                    niveau_hierarchique = table.Column<int>(type: "integer", nullable: false),
                    salaire_min = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    salaire_max = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    matricule = table.Column<string>(type: "text", nullable: false),
                    nom = table.Column<string>(type: "text", nullable: false),
                    prenom = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    date_naissance = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    genre = table.Column<int>(type: "integer", nullable: false),
                    departement_id = table.Column<int>(type: "integer", nullable: false),
                    poste_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employes", x => x.id);
                    table.ForeignKey(
                        name: "FK_employes_departements_departement_id",
                        column: x => x.departement_id,
                        principalTable: "departements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employes_postes_poste_id",
                        column: x => x.poste_id,
                        principalTable: "postes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conges",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_conge = table.Column<int>(type: "integer", nullable: false),
                    date_debut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    statut = table.Column<int>(type: "integer", nullable: false),
                    motif = table.Column<string>(type: "text", nullable: true),
                    employe_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conges", x => x.id);
                    table.ForeignKey(
                        name: "FK_conges_employes_employe_id",
                        column: x => x.employe_id,
                        principalTable: "employes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contrats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_contrat = table.Column<int>(type: "integer", nullable: false),
                    date_debut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    salaire_base = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    periode_essai = table.Column<bool>(type: "boolean", nullable: false),
                    employe_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contrats", x => x.id);
                    table.ForeignKey(
                        name: "FK_contrats_employes_employe_id",
                        column: x => x.employe_id,
                        principalTable: "employes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "evaluations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    periode = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    date_evaluation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    employe_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluations", x => x.id);
                    table.ForeignKey(
                        name: "FK_evaluations_employes_employe_id",
                        column: x => x.employe_id,
                        principalTable: "employes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "departements",
                columns: new[] { "id", "budget", "code", "nom" },
                values: new object[,]
                {
                    { 1, 5000000m, "DG", "Direction Générale" },
                    { 2, 2000000m, "RH", "Ressources Humaines" },
                    { 3, 3000000m, "INFO", "Informatique" }
                });

            migrationBuilder.InsertData(
                table: "postes",
                columns: new[] { "id", "intitule", "niveau_hierarchique", "salaire_max", "salaire_min" },
                values: new object[,]
                {
                    { 1, "Directeur Général", 5, 1500000m, 800000m },
                    { 2, "Responsable RH", 4, 700000m, 400000m },
                    { 3, "Développeur Senior", 3, 600000m, 300000m },
                    { 4, "Développeur Junior", 2, 300000m, 150000m },
                    { 5, "Stagiaire", 1, 150000m, 50000m }
                });

            migrationBuilder.InsertData(
                table: "employes",
                columns: new[] { "id", "date_naissance", "departement_id", "email", "genre", "matricule", "nom", "poste_id", "prenom" },
                values: new object[,]
                {
                    { 1, new DateTime(1985, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, "amadou.diallo@sentech.sn", 1, "EMP001", "Diallo", 1, "Amadou" },
                    { 2, new DateTime(1990, 7, 22, 0, 0, 0, 0, DateTimeKind.Utc), 2, "fatou.ndiaye@sentech.sn", 2, "EMP002", "Ndiaye", 2, "Fatou" },
                    { 3, new DateTime(1988, 11, 5, 0, 0, 0, 0, DateTimeKind.Utc), 3, "ibrahima.sow@sentech.sn", 1, "EMP003", "Sow", 3, "Ibrahima" },
                    { 4, new DateTime(1995, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), 3, "mariama.ba@sentech.sn", 2, "EMP004", "Ba", 4, "Mariama" },
                    { 5, new DateTime(1992, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), 2, "moussa.fall@sentech.sn", 1, "EMP005", "Fall", 2, "Moussa" },
                    { 6, new DateTime(1998, 9, 8, 0, 0, 0, 0, DateTimeKind.Utc), 3, "aissatou.gueye@sentech.sn", 2, "EMP006", "Gueye", 5, "Aissatou" },
                    { 7, new DateTime(1987, 4, 20, 0, 0, 0, 0, DateTimeKind.Utc), 1, "cheikh.kane@sentech.sn", 1, "EMP007", "Kane", 1, "Cheikh" },
                    { 8, new DateTime(1993, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), 2, "rokhaya.mbaye@sentech.sn", 2, "EMP008", "Mbaye", 2, "Rokhaya" },
                    { 9, new DateTime(1991, 8, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "oumar.cisse@sentech.sn", 1, "EMP009", "Cisse", 3, "Oumar" },
                    { 10, new DateTime(1996, 12, 3, 0, 0, 0, 0, DateTimeKind.Utc), 3, "khady.toure@sentech.sn", 2, "EMP010", "Toure", 4, "Khady" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_conges_employe_id",
                table: "conges",
                column: "employe_id");

            migrationBuilder.CreateIndex(
                name: "IX_contrats_employe_id",
                table: "contrats",
                column: "employe_id");

            migrationBuilder.CreateIndex(
                name: "IX_departements_code",
                table: "departements",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employes_departement_id",
                table: "employes",
                column: "departement_id");

            migrationBuilder.CreateIndex(
                name: "IX_employes_email",
                table: "employes",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employes_matricule",
                table: "employes",
                column: "matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employes_poste_id",
                table: "employes",
                column: "poste_id");

            migrationBuilder.CreateIndex(
                name: "IX_evaluations_employe_id",
                table: "evaluations",
                column: "employe_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conges");

            migrationBuilder.DropTable(
                name: "contrats");

            migrationBuilder.DropTable(
                name: "evaluations");

            migrationBuilder.DropTable(
                name: "employes");

            migrationBuilder.DropTable(
                name: "departements");

            migrationBuilder.DropTable(
                name: "postes");
        }
    }
}
