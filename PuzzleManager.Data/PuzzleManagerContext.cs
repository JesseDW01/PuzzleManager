
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// The Microsoft.EntityFrameworkCore namespace contains the core functionalities 
// for building data models, configuring DbContexts, and performing database operations 
// in Entity Framework Core.

using PuzzleManager.Domain;
// "PuzzleManager.Domain" is where our entity classes (Puzzle, PuzzleMaker, etc.) are defined.
// This context class needs to reference them so EF Core knows how to map them to database tables.

namespace PuzzleManager.Data
{
	/// <summary>
	/// The PuzzleManagerContext class serves as the primary interface between 
	/// our domain model and the underlying database. It inherits from DbContext 
	/// (part of EF Core) and manages querying, saving, and configuring the model.
	/// </summary>
	public class PuzzleManagerContext : DbContext
	{
		/// <summary>
		/// The constructor takes DbContextOptions, which includes configuration data 
		/// (like the connection string). Passing it up to the base DbContext class 
		/// allows EF Core to know how to connect to and manage the database.
		/// </summary>
		/// <param name="options">The options to configure the context, including connection info.</param>
		public PuzzleManagerContext(DbContextOptions options) : base(options)
		{
		}

		/// <summary>
		/// Each DbSet property represents a table in the database. 
		/// 'Puzzle' is our domain model that will map to a 'Puzzles' table by convention.
		/// </summary>
		public DbSet<Puzzle> Puzzles { get; set; } = default!;

		/// <summary>
		/// This DbSet represents puzzle makers (companies or individuals who create puzzles).
		/// EF Core will map it to a table for 'PuzzleMakers'.
		/// </summary>
		public DbSet<PuzzleMaker> PuzzleMakers { get; set; } = default!;

		/// <summary>
		/// This DbSet represents the people (family members/friends) 
		/// who check out puzzles. EF Core maps it to a 'PuzzleHolders' table.
		/// </summary>
		public DbSet<PuzzleHolder> PuzzleHolders { get; set; } = default!;

		/// <summary>
		/// Records of puzzle checkouts—linking a Puzzle and a PuzzleHolder 
		/// along with timing, completion metrics, etc.
		/// </summary>
		public DbSet<PuzzleCheckout> PuzzleCheckouts { get; set; } = default!;

		/// <summary>
		/// OnModelCreating is where we can configure or override EF Core conventions 
		/// using the Fluent API (e.g. relationships, column mappings, table names).
		/// Calling the base method ensures default configurations are applied first.
		/// </summary>
		/// <param name="modelBuilder">An object for building the EF Core model.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// Example: modelBuilder.Entity<Puzzle>().HasOne(...) 
			// Additional Fluent API configurations go here if needed.
			// Essentially, Fluent API is a powerful alternative or complement to data annotations,
			// giving you full control over your entity-to-database mappings in EF Core.

			// Configure the relationship between PuzzleHolder and IdentityUser via UserId, to avoid introducing framework-specific classes into your domain layer.
			modelBuilder.Entity<PuzzleHolder>()
				.HasOne<IdentityUser>() // No navigation property in PuzzleHolder
				.WithMany() // IdentityUser can have multiple PuzzleHolders if desired
				.HasForeignKey(ph => ph.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			// Configure PuzzleCheckout relationships
			modelBuilder.Entity<PuzzleCheckout>()
				.HasOne(pc => pc.Puzzle)
				.WithMany(p => p.PuzzleCheckouts)
				.HasForeignKey(pc => pc.PuzzleId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<PuzzleCheckout>()
				.HasOne(pc => pc.PuzzleHolder)
				.WithMany(ph => ph.PuzzleCheckouts)
				.HasForeignKey(pc => pc.PuzzleHolderId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
