using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ticket_Manager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditBookingDialogBox : Window
    {
        List<Tickets> allbookings;
        List<string> newBookings = new List<string>();
        List<Tickets> oldBookings;
        List<Tickets> bookings;
        MainWindow ownerReference;
        public EditBookingDialogBox(List<Tickets> inBookings, List<Tickets> inAllBookings, MainWindow inOwnerReference)
        {
            //on construction, get all booking data and add a reference to the main window
            InitializeComponent();
            bookings = inBookings;
            oldBookings = new List<Tickets>(bookings);
            oldBookings.AsReadOnly();
            allbookings = inAllBookings;
            ownerReference = inOwnerReference;
            ImportBookingData();
        }

        private void BSeatingChartStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                string seatName = ((TextBlock)e.OriginalSource).Name; //if the user clicks on a textbox
                if (bookings.Exists(element => element.GetTicketID() == seatName)) //and it exists i the booking list
                {
                    int targetIndex = bookings.FindIndex(seat => seat.GetTicketID() == seatName);
                    bookings.RemoveAt(targetIndex); //remove the booking from the user's list and the global list
                    targetIndex = allbookings.FindIndex(seat => seat.GetTicketID() == seatName);
                    allbookings.RemoveAt(targetIndex);
                    ImportBookingData();
                }
                else if (newBookings.Exists(element => element == seatName)) //if the user is removing a new booking
                {
                    int targetIndex = newBookings.FindIndex(element => element == seatName); //remove it from the list
                    newBookings.RemoveAt(targetIndex);
                    OutputBookingInfo();
                }
                else if (allbookings.Exists(element => element.GetTicketID() == seatName))
                {
                    MessageBox.Show("Seat already reserved.");
                    return;
                }
                else if (newBookings.Count + 1 > oldBookings.Count)
                {
                    MessageBox.Show("Too many seats reserved, please ensure only 6 seats are selected.");
                    return;
                }
                else
                {
                    newBookings.Add(seatName);
                    ColorSeat(seatName, Brushes.HotPink);
                    OutputBookingInfo();
                }
            }
        }
        /// <summary>
        /// Takes a provided TextBlock name, searches MainWindow.xaml
        /// for the element and if found will colour the element with
        /// the brush colour provided
        /// </summary>
        /// <param name="blockName">the TextBlock that will be coloured</param>
        /// <param name="chosenColour">the colour that the TextBlock will be printed</param>
        private void ColorSeat(string blockName, Brush inColour)
        {
            TextBlock chosenBlock = this.FindName(blockName) as TextBlock;
            if (object.ReferenceEquals(chosenBlock, null) == false)
            {
                chosenBlock.Foreground = inColour;
            }
        }
        private void ImportBookingData()
        {
            newBookings.Clear();
            for (int i = 0; i < bookings.Count; i++)
            {
                newBookings.Add(bookings[i].GetTicketID());
            }
            ResetChairColours(USeatingChart.Children, Brushes.Blue);
            ResetChairColours(DSeatingChart.Children, Brushes.Red);
            ResetChairColours(SSeatingStack.Children, Brushes.Gold);
            foreach (Tickets item in allbookings)
            {
                ColorSeat(item.GetTicketID(), Brushes.Gray);
            }
            foreach (Tickets item in bookings)
            {
                ColorSeat(item.GetTicketID(), Brushes.HotPink);
            }
            OutputBookingInfo();
        }

        /// <summary>
        /// recurses through a UI Element Collection, and stops recursing if it finds
        /// a TextBlock in order to colour the elements, used to return the seating
        /// charts to their original Blue, Red, Yellow colour scheme
        /// </summary>
        /// <param name="childrenList">the current Element Collection being sorted through</param>
        /// <param name="chosenColour">the chosen reset colour</param>
        private void ResetChairColours(UIElementCollection childrenList, Brush chosenColour)
        {
            foreach (UIElement item in childrenList)
            {
                if (item is TextBlock && item.Uid != null)
                {
                    TextBlock itemSuccess = (TextBlock)item;
                    itemSuccess.Foreground = chosenColour;
                }
                else
                {
                    StackPanel itemFail = (StackPanel)item;
                    ResetChairColours(childrenList: itemFail.Children, chosenColour: chosenColour);
                }
            }
        }
        private void OutputBookingInfo()
        {
            string oldBookingsString = "Original booking: ";
            for (int i = 0; i < oldBookings.Count; i++)
            {
                oldBookingsString += oldBookings[i].GetTicketID() + " ";
            }

            string newBookingsString = "New booking: ";
            for (int i = 0; i < newBookings.Count; i++)
            {
                newBookingsString += newBookings[i] + " ";
            }
            OriginaBookingBlock.Text = oldBookingsString;
            NewBookingBlock.Text = newBookingsString;
        }

        private void resetBookingButton_Click(object sender, RoutedEventArgs e)
        {
            bookings = oldBookings;
            ImportBookingData();
        }

        private void saveBookingButton_Click(object sender, RoutedEventArgs e)
        {
            //return the new booking data to the parent window and close the page
            ownerReference.ReturnEditInfo(inNewBookings: newBookings, confirm: false, cancel: false, inPlayID: oldBookings[0].PlayID, inCustomerID: oldBookings[0].customerID);
            this.Close();
        }

        private void cancelBookingButton_Click(object sender, RoutedEventArgs e)
        {
            //return the new booking data to the parent window and close the page
            ownerReference.ReturnEditInfo(inNewBookings: null, confirm: false, cancel: true, inPlayID: oldBookings[0].PlayID, inCustomerID: oldBookings[0].customerID);
            this.Close();
        }

        private void confirmBookingButton_Click(object sender, RoutedEventArgs e)
        {
            //return the new booking data to the parent window and close the page
            MessageBoxResult result = MessageBox.Show("Warning! Confirming a booking will not save any changes made to it.", "Alert!", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            ownerReference.ReturnEditInfo(inNewBookings: null, confirm: true, cancel: false, inPlayID: oldBookings[0].PlayID, inCustomerID: oldBookings[0].customerID);
            this.Close();
        }
    }
}
