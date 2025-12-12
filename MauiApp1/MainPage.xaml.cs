using MauiApp1.Data;
using MauiApp1.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;


namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private readonly LocationDatabase _db;

    private CancellationTokenSource? _cts;
    private bool _isTracking;

    private const int IntervalMs = 5000; // every 5 seconds

    public MainPage(LocationDatabase db)
    {
        InitializeComponent();
        _db = db;
    }

    // ================================
    // START TRACKING
    // ================================
    private async void OnStartTrackingClicked(object sender, EventArgs e)
    {
        try
        {
            if (_isTracking)
                return;

            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlertAsync(
                    "Permission Denied",
                    "Location permission is required to track movement.",
                    "OK");

                return;
            }

            _isTracking = true;
            _cts = new CancellationTokenSource();

            await DisplayAlertAsync(
                "Tracking Started",
                "Location will be saved every 5 seconds.",
                "OK");

            _ = Task.Run(() => TrackLoopAsync(_cts.Token));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error starting tracking", ex.ToString(), "OK");
        }
    }

    // ================================
    // STOP TRACKING
    // ================================
    private void OnStopTrackingClicked(object sender, EventArgs e)
    {
        StopTracking();
    }

    private void StopTracking()
    {
        if (!_isTracking)
            return;

        _cts?.Cancel();
        _cts = null;
        _isTracking = false;
    }

    // ================================
    // GPS TRACKING LOOP
    // ================================
    private async Task TrackLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var request = new GeolocationRequest(
                    GeolocationAccuracy.Best,
                    TimeSpan.FromSeconds(10));

                var location = await Geolocation.GetLocationAsync(request, token);

                if (location != null)
                {
                    var entry = new LocationEntry
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Timestamp = DateTime.UtcNow
                    };

                    await _db.SaveLocationAsync(entry);

                    System.Diagnostics.Debug.WriteLine(
                        $"Saved: {entry.Latitude}, {entry.Longitude}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Tracking error: {ex}");
            }

            // Wait before collecting again
            try
            {
                await Task.Delay(IntervalMs, token);
            }
            catch
            {
                // ignore cancellation exceptions
            }
        }
    }
    // ================================
    // SHOW SAVED POINTS ON MAP
    // ================================
    private void OnShowSavedPointsClicked(object sender, EventArgs e)
    {
        DrawHeatMapAsync();
    }

    private async Task DrawHeatMapAsync()
    {
        var points = await _db.GetLocationsAsync();

        MainMap.Pins.Clear();
        MainMap.MapElements.Clear();

        foreach (var p in points)
        {
            var circle = new Circle
            {
                Center = new Location(p.Latitude, p.Longitude),
                Radius = Distance.FromMeters(80),
                StrokeColor = Colors.Transparent,
                FillColor = Color.FromRgba(255, 0, 0, 70) // translucent red
            };

            MainMap.MapElements.Add(circle);
        }

        if (points.Count > 0)
        {
            var last = points[^1];
            MainMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    new Location(last.Latitude, last.Longitude),
                    Distance.FromMeters(300)
                )
            );
        }
    }


}
