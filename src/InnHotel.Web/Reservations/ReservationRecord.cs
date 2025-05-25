using System;

namespace InnHotel.Web.Reservations;

/// <summary>
/// Represents the data returned by the API after creating or fetching a Reservation.
/// </summary>
public record ReservationRecord(
    int Id,
    int GuestId,
    string GuestFirstName,
    string GuestLastName,
    int RoomId,
    string RoomNumber,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice,
    string ReservationStatus
);
