//using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text.Json;
using Lab4;

namespace Lab4.Model;

public class Database : IDatabase
{
    private String connString = GetConnectionString();
    private static readonly String cockroachDBUsername = "alexsa";
    private static readonly String cockroachDBPassword = "G_ITuPeScqFHiNZ5Q40PrQ";

    ObservableCollection<Airport> airports = new();

    public Database()
    {
        SelectAllAirports();
    }

    /// <summary>
    /// Builds a ConnectionString, which is used to connect to the database
    /// </summary>
    /// <returns></returns>
    static String GetConnectionString()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = "petty-frog-13034.5xj.cockroachlabs.cloud",
            Port = 26257,
            SslMode = SslMode.VerifyFull,
            Username = cockroachDBUsername,
            Password = cockroachDBPassword,
            Database = "defaultdb",
            IncludeErrorDetail = true
        };
        return connStringBuilder.ConnectionString;
    }

    /// <summary>
    /// Updates the ObservableCollection airports with data from the datebase
    /// </summary>
    /// <returns></returns>
    public ObservableCollection<Airport> SelectAllAirports()
    {
        airports.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open(); // opens connection to the database
        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports", conn);
        using var reader = cmd.ExecuteReader(); 
        while (reader.Read()) // reads one line from the database at a time
        {
            String id = reader.GetString(0);
            String city = reader.GetString(1);
            String dateVisited = reader.GetString(2);
            Int32 rating = reader.GetInt32(3);
            Airport airportToAdd = new(id, city, dateVisited, rating); // creates a new airport
            airports.Add(airportToAdd);
            Console.WriteLine(airportToAdd);
        }
        return airports;
    }

    /// <summary>
    /// Returns one airport from the database
    /// </summary>
    /// <param name="id"> airport id </param>
    /// <returns></returns>
    public Airport SelectAirport(string id)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = ("SELECT city, date_visited, rating FROM airports WHERE id = @id");
        cmd.Parameters.AddWithValue("id", id); // gets an airport from the database given the id
        using var reader = cmd.ExecuteReader(); 
        while (reader.Read()) 
        {
            String city = reader.GetString(0);
            String dateVisited = reader.GetString(1);
            Int32 rating = reader.GetInt32(2);
            Airport airport = new(id, city, dateVisited, rating);
            return airport;
        }
        return null;
    }

    /// <summary>
    /// Adds an airport to the database, returns an error if unsuccessful
    /// </summary>
    /// <param name="airport"> airport object </param>
    /// <returns></returns>
    public AirportAdditionError InsertAirport(Airport airport)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); 
            conn.Open(); 
            var cmd = new NpgsqlCommand(); 
            cmd.Connection = conn; 
            cmd.CommandText = "INSERT INTO airports (id, city, date_visited, rating) VALUES (@id, @city, @dateVisited, @rating)";
            cmd.Parameters.AddWithValue("id", airport.Id);
            cmd.Parameters.AddWithValue("city", airport.City);
            cmd.Parameters.AddWithValue("dateVisited", airport.DateVisited);
            cmd.Parameters.AddWithValue("rating", airport.Rating);
            cmd.ExecuteNonQuery(); 
            SelectAllAirports();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return AirportAdditionError.DBAdditionError;
        }
        return AirportAdditionError.NoError;
    }

    /// <summary>
    /// Deletes an airport from the database, returns an error if unsuccessful
    /// </summary>
    /// <param name="airport"> airport object </param>
    /// <returns></returns>
    public AirportDeletionError DeleteAirport(Airport airport)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM airports WHERE id = @id";
        cmd.Parameters.AddWithValue("id", airport.Id);
        int numDeleted = cmd.ExecuteNonQuery();
        if (numDeleted > 0)
        {
            SelectAllAirports(); // updates the SQL database
            return AirportDeletionError.NoError;
        }
        return AirportDeletionError.DBDeletionError;
    }

    /// <summary>
    /// Updates an airport in the database, returns an airport if unsuccessful
    /// </summary>
    /// <param name="airport"> airport object </param>
    /// <param name="city"> airport city </param>
    /// <param name="dateVisited"> date airport was visited </param>
    /// <param name="rating"> airport rating </param>
    /// <returns></returns>
    public AirportEditError UpdateAirport(Airport airport, string city, DateTime dateVisited, int rating)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); 
            conn.Open(); 
            var cmd = new NpgsqlCommand(); 
            cmd.Connection = conn; 
            cmd.CommandText = "UPDATE airports SET city = @city, date_visited = @dateVisited, rating = @rating WHERE id = @id;";
            cmd.Parameters.AddWithValue("id", airport.Id);
            cmd.Parameters.AddWithValue("city", city);
            cmd.Parameters.AddWithValue("dateVisited", dateVisited);
            cmd.Parameters.AddWithValue("rating", rating);
            var numAffected = cmd.ExecuteNonQuery();
            SelectAllAirports();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Update failed, {0}", pe);
            return AirportEditError.DBEditError;
        }
        return AirportEditError.NoError;
    }

    /// <summary>
    /// Returns the number of airports currently in the database
    /// </summary>
    /// <returns></returns>
    public int CountAirports()
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            var cmd = new NpgsqlCommand("SELECT id FROM airports", conn);
            int numAirports = 0;
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) // for every entry in the table, add one to numAirports
            {
                numAirports++;
            }
            return numAirports;
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Count failed, {0}", pe);
            return -1;
        }
    }

 }
