﻿using System.Collections.ObjectModel;
using Lab6_Starter.Model;

namespace FWAPPA.Model;

public partial class BusinessLogic : IBusinessLogic
{
    
    const int BRONZE_LEVEL = 42;
    const int SILVER_LEVEL = 84;
    const int GOLD_LEVEL = 128;
    
    IDatabase db;
    private readonly int MAX_RATING = 5;

    public ObservableCollection<Airport> Airports
    {
        get { return GetAirports(); }

    }

    public ObservableCollection<Airport> WisconsinAirports
    {
        get { return GetAirports(); }

    }

    public ObservableCollection<Airport> GetAllWisconsinAirports()
    {
        return db.GetAllWisconsinAirports();
    }

    public ObservableCollection<Airport> GetWisconsinAirportsWithinDistance(double userLatitude, double userLongitude, double maxDistanceKm)
    {
        return db.GetWisconsinAirportsWithinDistance(userLatitude, userLongitude, maxDistanceKm);
    }

    public Airport SelectAirportByCode(string airportCode)
    {
        return db.SelectAirportByCode(airportCode);
    }

    public ObservableCollection<Weather> Weathers
    {
        get { return GetWeathers(); }

    }
    
    partial void LoadAirportCoordinates();

    public BusinessLogic(IDatabase? db)
    {
        this.db = db;
        LoadAirportCoordinates();
    }
    

    public Airport FindAirport(String id)
    {
        return db.SelectAirport(id);
    }

    private AirportAdditionError CheckAirportFields(String? id, String? city, DateTime? dateVisited, int rating)
    {

        if (id == null || id.Length < 3 || id.Length > 4)
        {
            return AirportAdditionError.InvalidIdLength;
        }
        if (city == null || city.Length < 3)
        {
            return AirportAdditionError.InvalidCityLength;
        }
        if (rating < 1 || rating > MAX_RATING)
        {
            return AirportAdditionError.InvalidRating;
        }

        if (dateVisited == null)
        {
            return AirportAdditionError.InvalidDate;
        }

        return AirportAdditionError.NoError;
    }


    public AirportAdditionError AddAirport(String id, String city, DateTime? dateVisited, int rating)
    {

        var result = CheckAirportFields(id, city, dateVisited, rating);
        if (result != AirportAdditionError.NoError)
        {
            return result;
        }

        if (db.SelectAirport(id) != null)
        {
            return AirportAdditionError.DuplicateAirportId;
        }

        Airport airport = new Airport(id, city, (DateTime)dateVisited, rating); // this will never be null, we check in checkAirportFields
        db.InsertAirport(airport);

        return AirportAdditionError.NoError;
    }



    public AirportDeletionError DeleteAirport(String id)
    {

        var entry = db.SelectAirport(id);

        if (entry != null)
        {
            AirportDeletionError success = db.DeleteAirport(entry);
            if (success == AirportDeletionError.NoError)
            {
                return AirportDeletionError.NoError;

            }
            else
            {
                return AirportDeletionError.DBDeletionError;
            }
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clue"></param>
    /// <param name="answer"></param>
    /// <param name="difficulty"></param>
    /// <param name="date"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public AirportEditError EditAirport(String id, String city, DateTime dateVisited, int rating)
    {

        var fieldCheck = CheckAirportFields(id, city, dateVisited, rating);
        if (fieldCheck != AirportAdditionError.NoError)
        {
            return AirportEditError.InvalidFieldError;
        }

        var airport = db.SelectAirport(id);
        airport.Id = id;
        airport.City = city;
        airport.DateVisited = dateVisited;
        airport.Rating = rating;

        AirportEditError success = db.UpdateAirport(airport);
        if (success != AirportEditError.NoError)
        {
            return AirportEditError.DBEditError;
        }

        return AirportEditError.NoError;
    }


    public String CalculateStatistics()
    {
        FlyWisconsinLevel nextLevel;
        int numAirportsUntilNextLevel;

        int numAirportsVisited = db.SelectAllAirports().Count;
        if (numAirportsVisited < BRONZE_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Bronze;
            numAirportsUntilNextLevel = BRONZE_LEVEL - numAirportsVisited;
        }
        else if (numAirportsVisited < SILVER_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Silver;
            numAirportsUntilNextLevel = SILVER_LEVEL - numAirportsVisited;
        }
        else if (numAirportsVisited < GOLD_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Gold;
            numAirportsUntilNextLevel = GOLD_LEVEL - numAirportsVisited;
        }
        else
        {
            nextLevel = FlyWisconsinLevel.None;
            numAirportsUntilNextLevel = 0;
        }

        return String.Format("{0} airport{1} visited; {2} airports remaining until achieving {3}",
              numAirportsVisited, numAirportsVisited != 1 ? "s" : "", numAirportsUntilNextLevel, nextLevel);
    }

    public ObservableCollection<Airport> GetAirports()
    {
        return db.SelectAllAirports();
    }

    public ObservableCollection<Airport> GetWisconsinAirports()
    {
        return db.SelectAllWisconsinAirports();
    }

    public ObservableCollection<Weather> GetWeathers()
    {
        ObservableCollection<Weather> weathers = new ObservableCollection<Weather>();
        weathers.Add(new Weather("METAR KJFK 161853Z 21015G25KT 10SM -RA SCT020 BKN050", "TAF KJFK 161720Z 1618/1718 21015KT P6SM -RA BKN050"));
        return weathers;
    }

    public Route GetRoute()
    {
        var route = new Route("KATW", "KUBB");

        // Add edges 
        route.AddEdge("KATW", "Appleton", 0);
        route.AddEdge("KFLD", "Fond du Lac", 23);
        route.AddEdge("KUNN", "Dodge County", 28);
        route.AddEdge("KUBB", "Burlington", 47);
        route.AddEdge("KATW", "Appleton", 95);

        return route;
    }

}

