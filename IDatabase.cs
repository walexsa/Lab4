using System.Collections.ObjectModel;

namespace Lab4.Model;

public interface IDatabase
{
    public ObservableCollection<Airport> SelectAllAirports();
    public Airport SelectAirport(String id);
    public AirportAdditionError InsertAirport(Airport airport);
    public AirportDeletionError DeleteAirport(Airport airport);
    public AirportEditError UpdateAirport(Airport airport, String city, DateTime dateVisited, int rating);
    public int CountAirports();
}
