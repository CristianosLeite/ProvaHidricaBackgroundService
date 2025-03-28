﻿using System.Windows;
using System.Windows.Controls;
using ProvaHidrica.Types;

namespace ProvaHidrica.Interfaces
{
    public interface IDoorService
    {
        void SetDoors(List<Button> doors);
        void SubscribeDoors();
        void UnsubscribeAll(object sender, RoutedEventArgs e);
        void StartFlashing(Control control, Command command, int door);
        void FlashGreen(Control control);
        void FlashOrange(Control control);
        void SetOrange(Control control);
        void ManualDoorOpen(object sender, RoutedEventArgs e);
        Task<bool> CheckForOpenDoors(List<int> doors);
        void SetOpen(Control control, int door);
        void CloseDoor(Control control, int door);
        Task<bool> IsDoorOpen(int door);
    }
}
