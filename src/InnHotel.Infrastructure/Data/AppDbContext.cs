using InnHotel.Core.BranchAggregate;
using InnHotel.Core.EmployeeAggregate;
using InnHotel.Core.ReservationAggregate;
using InnHotel.Core.ServiceAggregate;
using InnHotel.Core.RoomAggregate;
using InnHotel.Core.GuestAggregate;
using InnHotel.Core.AuthAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace InnHotel.Infrastructure.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options,
  IDomainEventDispatcher? dispatcher) : IdentityDbContext<ApplicationUser>(options)
{
  private readonly IDomainEventDispatcher? _dispatcher = dispatcher;
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<Branch> Branches { get; set; }
  public DbSet<Guest> Guests { get; set; }
  public DbSet<Reservation> Reservations { get; set; }
  public DbSet<RoomType> RoomTypes { get; set; }
  public DbSet<Room> Rooms { get; set; }
  public DbSet<Employee> Employees { get; set; }
  public DbSet<Service> Services { get; set; }
  public DbSet<ReservationRoom> ReservationRooms { get; set; }
  public DbSet<ReservationService> ReservationServices { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
