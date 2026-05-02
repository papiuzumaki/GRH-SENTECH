using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Departement> Departements { get; set; }
        public DbSet<Poste> Postes { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Contrat> Contrats { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Conge> Conges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Departement
            modelBuilder.Entity<Departement>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.HasIndex(d => d.Code).IsUnique();
                entity.Property(d => d.Nom).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Code).IsRequired().HasMaxLength(6);
                entity.Property(d => d.Budget).HasColumnType("decimal(18,2)");
            });

            // Poste
            modelBuilder.Entity<Poste>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Intitule).IsRequired();
                entity.Property(p => p.SalaireMin).HasColumnType("decimal(18,2)");
                entity.Property(p => p.SalaireMax).HasColumnType("decimal(18,2)");
            });

            // Employe
            modelBuilder.Entity<Employe>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Matricule).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Matricule).IsRequired();
                entity.Property(e => e.Email).IsRequired();

                entity.HasOne(e => e.Departement)
                      .WithMany(d => d.Employes)
                      .HasForeignKey(e => e.DepartementId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Poste)
                      .WithMany(p => p.Employes)
                      .HasForeignKey(e => e.PosteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Contrat
            modelBuilder.Entity<Contrat>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.SalaireBase).HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.Employe)
                      .WithMany(e => e.Contrats)
                      .HasForeignKey(c => c.EmployeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Evaluation
            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.HasKey(ev => ev.Id);
                entity.Property(ev => ev.Note).HasColumnType("decimal(4,2)");

                entity.HasOne(ev => ev.Employe)
                      .WithMany(e => e.Evaluations)
                      .HasForeignKey(ev => ev.EmployeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Conge
            modelBuilder.Entity<Conge>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Employe)
                      .WithMany(e => e.Conges)
                      .HasForeignKey(c => c.EmployeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            modelBuilder.Entity<Departement>().HasData(
                new Departement { Id = 1, Nom = "Direction Générale", Code = "DG", Budget = 5000000 },
                new Departement { Id = 2, Nom = "Ressources Humaines", Code = "RH", Budget = 2000000 },
                new Departement { Id = 3, Nom = "Informatique", Code = "INFO", Budget = 3000000 }
            );

            modelBuilder.Entity<Poste>().HasData(
                new Poste { Id = 1, Intitule = "Directeur Général", NiveauHierarchique = 5, SalaireMin = 800000, SalaireMax = 1500000 },
                new Poste { Id = 2, Intitule = "Responsable RH", NiveauHierarchique = 4, SalaireMin = 400000, SalaireMax = 700000 },
                new Poste { Id = 3, Intitule = "Développeur Senior", NiveauHierarchique = 3, SalaireMin = 300000, SalaireMax = 600000 },
                new Poste { Id = 4, Intitule = "Développeur Junior", NiveauHierarchique = 2, SalaireMin = 150000, SalaireMax = 300000 },
                new Poste { Id = 5, Intitule = "Stagiaire", NiveauHierarchique = 1, SalaireMin = 50000, SalaireMax = 150000 }
            );

            modelBuilder.Entity<Employe>().HasData(
                new Employe { Id = 1, Matricule = "EMP001", Nom = "Diallo", Prenom = "Amadou", Email = "amadou.diallo@sentech.sn", DateNaissance = new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Homme, DepartementId = 1, PosteId = 1 },
                new Employe { Id = 2, Matricule = "EMP002", Nom = "Ndiaye", Prenom = "Fatou", Email = "fatou.ndiaye@sentech.sn", DateNaissance = new DateTime(1990, 7, 22, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Femme, DepartementId = 2, PosteId = 2 },
                new Employe { Id = 3, Matricule = "EMP003", Nom = "Sow", Prenom = "Ibrahima", Email = "ibrahima.sow@sentech.sn", DateNaissance = new DateTime(1988, 11, 5, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Homme, DepartementId = 3, PosteId = 3 },
                new Employe { Id = 4, Matricule = "EMP004", Nom = "Ba", Prenom = "Mariama", Email = "mariama.ba@sentech.sn", DateNaissance = new DateTime(1995, 1, 30, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Femme, DepartementId = 3, PosteId = 4 },
                new Employe { Id = 5, Matricule = "EMP005", Nom = "Fall", Prenom = "Moussa", Email = "moussa.fall@sentech.sn", DateNaissance = new DateTime(1992, 6, 12, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Homme, DepartementId = 2, PosteId = 2 },
                new Employe { Id = 6, Matricule = "EMP006", Nom = "Gueye", Prenom = "Aissatou", Email = "aissatou.gueye@sentech.sn", DateNaissance = new DateTime(1998, 9, 8, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Femme, DepartementId = 3, PosteId = 5 },
                new Employe { Id = 7, Matricule = "EMP007", Nom = "Kane", Prenom = "Cheikh", Email = "cheikh.kane@sentech.sn", DateNaissance = new DateTime(1987, 4, 20, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Homme, DepartementId = 1, PosteId = 1 },
                new Employe { Id = 8, Matricule = "EMP008", Nom = "Mbaye", Prenom = "Rokhaya", Email = "rokhaya.mbaye@sentech.sn", DateNaissance = new DateTime(1993, 2, 14, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Femme, DepartementId = 2, PosteId = 2 },
                new Employe { Id = 9, Matricule = "EMP009", Nom = "Cisse", Prenom = "Oumar", Email = "oumar.cisse@sentech.sn", DateNaissance = new DateTime(1991, 8, 25, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Homme, DepartementId = 3, PosteId = 3 },
                new Employe { Id = 10, Matricule = "EMP010", Nom = "Toure", Prenom = "Khady", Email = "khady.toure@sentech.sn", DateNaissance = new DateTime(1996, 12, 3, 0, 0, 0, DateTimeKind.Utc), Genre = Genre.Femme, DepartementId = 3, PosteId = 4 }
            );
        }
    }
}
