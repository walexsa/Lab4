using System.ComponentModel;
using System.Drawing;

namespace Lab4.Model;

public class Airport : INotifyPropertyChanged
{
    String id;
    String city;
    DateTime dateVisited;
    int rating;

    public event PropertyChangedEventHandler PropertyChanged;

    public String Id
    {
        get { return id; }
        set
        {
            id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public String City
    {
        get { return city; }
        set
        {
            city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    public DateTime DateVisited
    {
        get { return dateVisited; }
        set
        {
            dateVisited = value;
            OnPropertyChanged(nameof(DateVisited));
        }
    }

    public int Rating
    {
        get { return rating; }
        set
        {
            if (value >= 1 && value <= 5)
            {
                rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Airport(String id, String city, DateTime dateVisited, int rating)
    {
        Id = id;
        City = city;
        DateVisited = dateVisited;
        Rating = rating;
    }

    public Airport(String id, String city, String dateVisited, int rating)
    {
        Id = id;
        City = city;
        if (!DateTime.TryParse(dateVisited, out DateTime result))
        {
            DateVisited = DateTime.Now; // if date entered is invalid, set date to now
        }
        DateVisited = result;
        Rating = rating;
    }

    public Airport()
    {
        Id = "KATW";
        City = "Appleton";
        DateVisited = DateTime.Now;
        Rating = 5;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Airport airport = (Airport)obj;
            return id.Equals(airport.id) && city.Equals(airport.city) && dateVisited.Equals(airport.dateVisited) && rating.Equals(airport.rating);
        }
    }
}
