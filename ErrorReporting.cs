namespace Lab4.Model;

public enum AirportAdditionError
{
    InvalidIdLength,
    InvalidCityLength,
    InvalidRating,
    InvalidDate,
    DuplicateAirportId,
    DBAdditionError,
    NoError
}

public enum AirportDeletionError
{
    AirportNotFound,
    AirportNotSelected,
    DBDeletionError,
    NoError
}

public enum AirportEditError
{
    AirportNotFound,
    InvalidRating,
    InvalidDate,
    DBEditError,
    NoError
}
