using System.Collections.ObjectModel;

namespace Lab4.Model;

public class BusinessLogic : IBusinessLogic
{
    private IDatabase Database { get; set; }

    public BusinessLogic()
    {
        Database = new Database();
    }

    public ObservableCollection<Airport> Airports { get { return Database.SelectAllAirports(); } }

    /// <summary>
    /// Checks for errors in the entry, if there are errors, return an error.
    /// If there aren't errors, add the airport to the database.
    /// </summary>
    /// <param name="id"> airport id </param>
    /// <param name="city"> airport city </param>
    /// <param name="dateVisited"> date airport was visited </param>
    /// <param name="rating"> airport rating </param>
    /// <returns></returns>
    public AirportAdditionError AddAirport(String id, String city, String dateVisited, String rating)
    {
        int minIdLength = 3;
        int maxIdLength = 4;
        int maxCityLength = 25;
        int maxRating = 5;

        if (Database.SelectAirport(id) != null)
        {
            return AirportAdditionError.DuplicateAirportId;
        }

        if (id.Length < minIdLength || id.Length > maxIdLength)
        {
            return AirportAdditionError.InvalidIdLength;
        }

        if (city.Length > maxCityLength || city.Length == 0)
        {
            return AirportAdditionError.InvalidCityLength;
        }

        if (!DateTime.TryParse(dateVisited, out DateTime result))
        {
            return AirportAdditionError.InvalidDate;
        }

        if (!int.TryParse(rating, out int num) || int.Parse(rating) <= 0 || int.Parse(rating) > maxRating)
        {
            return AirportAdditionError.InvalidRating;
        }

        Database.InsertAirport(new Airport(id, city, result, num));
        return AirportAdditionError.NoError;
    }

    /// <summary>
    /// Deletes an airport from the database
    /// </summary>
    /// <param name="airport"> airport object </param>
    /// <returns></returns>
    public AirportDeletionError DeleteAirport(Airport airport)
    {
        if (airport == null)
        {
            return AirportDeletionError.AirportNotSelected;
        }

        Airport currAirport = Database.SelectAirport(airport.Id);
        if (currAirport != null)
        {
            return Database.DeleteAirport(airport);
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }

    /// <summary>
    /// Edits a current airport in the database, leaving the id untouched.
    /// </summary>
    /// <param name="id"> airport id </param>
    /// <param name="city"> airport city </param>
    /// <param name="dateVisited"> date airport was visited </param>
    /// <param name="rating"> airport rating </param>
    /// <returns></returns>
    public AirportEditError EditAirport(String id, String city, String dateVisited, String rating)
    {
        int maxRating = 5; 

        Airport airport = Database.SelectAirport(id);
        if (airport == null) 
        {
            return AirportEditError.AirportNotFound;
        }
        
        if (!DateTime.TryParse(dateVisited, out DateTime result))
        {
            return AirportEditError.InvalidDate;
        }

        if (!int.TryParse(rating, out int num) || int.Parse(rating) <= 0 || int.Parse(rating) > maxRating)
        {
            return AirportEditError.InvalidRating;
        }

        return Database.UpdateAirport(airport, city, result, num);
    }

    /// <summary>
    /// Returns an airport object from its id
    /// </summary>
    /// <param name="id"> airport id </param>
    /// <returns></returns>
    public Airport SelectAirport(String id)
    {
        return Database.SelectAirport(id);
    }

    /// <summary>
    /// Returns the number of entries in the database (number of airports visited)
    /// </summary>
    /// <returns></returns>
    public int CountAirports()
    {
        return Database.CountAirports();
    }
}
