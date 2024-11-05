﻿using System.Collections.ObjectModel;

namespace Lab6_Starter.Model
{
    public partial interface IDatabase
    {
        ObservableCollection<Airport> SelectAllAirports();
        Airport SelectAirport(String id);
        AirportAdditionError InsertAirport(Airport airport);
        AirportDeletionError DeleteAirport(Airport airport);
        AirportEditError UpdateAirport(Airport replacementAirport);
    }
}