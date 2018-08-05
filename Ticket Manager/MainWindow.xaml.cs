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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Printing;
using System.IO;

namespace Ticket_Manager
{
    public class User
    {
        public User(string inUsername, ACCESS_RIGHTS inAccess)
        {
            username = inUsername;
            userRights = inAccess;
        }
        public enum ACCESS_RIGHTS
        {
            newsletterEditor,
            customerRep,
            bookingTeam,
            manager,
            admin
        }
        protected string username;
        protected ACCESS_RIGHTS userRights;

        public ACCESS_RIGHTS getAccessRights()
        {
            return userRights;
        }

        /// <summary>
        /// Basic authentication prototype, all it does it takes two string inputs and
        /// checks that they match both the expected a set of Username and Password inputs
        /// </summary>
        /// <param name="inUsername">the username as provided by the user</param>
        /// <param name="inPassword">the password as provided by the user</param>
        /// <returns></returns>
       public static ACCESS_RIGHTS? AuthenticateUser(string inUsername, string inPassword)
        {
            if (inUsername == "newsletter" && inPassword == "12345")
            {
                return ACCESS_RIGHTS.newsletterEditor;
            }
            else if (inUsername == "customer" && inPassword == "12345")
            {
                return ACCESS_RIGHTS.customerRep;
            }
            else if (inUsername == "booking" && inPassword == "12345")
            {
                return ACCESS_RIGHTS.bookingTeam;
            }
            else if (inUsername == "manager" && inPassword == "12345")
            {
                return ACCESS_RIGHTS.manager;
            }
            else if (inUsername == "admin" && inPassword == "admin")
            {
                return ACCESS_RIGHTS.admin;
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User currentUser = new User("admin", User.ACCESS_RIGHTS.admin); //construct a new admin user for testing purposes
        Business BusinessInstance = new Business(); //create a new Business Instance to handle all the Business Processing
        StackPanel activePanel;
        bool editDataMode = false; //declare a variable to check whether we're editing data or using new data
        List<string> chosenSeats = new List<string>(); 
        List<Brush> previousSeatColor = new List<Brush>();
        Play selectedPlay;
        List<string> customerPlayIDs = new List<string>();
        List<string> newBookings = new List<string>();
        PRICE_RANGE currentPlayPrices = 0;
        public MainWindow()
        {
            //On construction, reset all UI display elements and load up the home page
            InitializeComponent();
            customerSearchComboBox.SelectedIndex = 0;
            playSearchComboBox.SelectedIndex = 0;
            runningPeriodComboBox.SelectedIndex = 0;
            priceRangeComboBox.SelectedIndex = 0;
            activePanel = HomePanel;
            HomePageLoad();
            /* NavBarPanel.Visibility = Visibility.Collapsed;*/
        }

        private void ApplicationStart(object sender, EventArgs e)
        {
            //    Business customers = Business.LoadCustomer("Test.txt");
            try
            {
                Business Play22 = Business.LoadPlay("Play.txt");
            }
            catch 
            {
                BusinessInstance.SavePlay("Fail.txt"); 
        
            }
        }

        private void ApplicationExit(object sender, EventArgs e)
        {
            
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Check the user provided a username and password
            if (ValidationMethods.HasInput(UsernameBox.Text) == false || ValidationMethods.HasInput(PasswordBox.Text) == false)
            {
                MessageBox.Show("Please input valid login details!");
                return;
            }
            //check the user has valid access rights for the system
            User.ACCESS_RIGHTS? newUser = User.AuthenticateUser(inUsername: UsernameBox.Text, inPassword: PasswordBox.Text);
            if (newUser == null)
            {
                MessageBox.Show("Please input valid login details!");
                return;
            }
            //consturct a new user with provided details
            currentUser = new Ticket_Manager.User(inUsername: UsernameBox.Text, inAccess: (User.ACCESS_RIGHTS)newUser);

            //hide the login page and show the nav bar
            NavBarPanel.Visibility = Visibility.Visible;
            LoginStack.Visibility = Visibility.Collapsed;

            //use the user's access rights as a switch to decide where to start the system
            switch (currentUser.getAccessRights())
            {
                case User.ACCESS_RIGHTS.bookingTeam:
                    PlayPanel.Visibility = Visibility.Visible;
                    activePanel = PlayPanel;
                    PopulatePlayListView();
                    break;
                case User.ACCESS_RIGHTS.newsletterEditor:
                    PlayPanel.Visibility = Visibility.Visible;
                    activePanel = PlayPanel;
                    PopulatePlayListView();
                    break;
                case User.ACCESS_RIGHTS.customerRep:
                    HomePanel.Visibility = Visibility.Visible;
                    activePanel = HomePanel;
                    break;
                case User.ACCESS_RIGHTS.manager:
                    HomePanel.Visibility = Visibility.Visible;
                    activePanel = HomePanel;
                    break;
                case User.ACCESS_RIGHTS.admin:
                    HomePanel.Visibility = Visibility.Visible;
                    activePanel = HomePanel;
                    break;
            }
        }
        /// <summary>
        /// Takes a provided TextBlock name, searches MainWindow.xaml
        /// for the element and if found will colour the element with
        /// the brush colour provided
        /// </summary>
        /// <param name="blockName">the TextBlock that will be coloured</param>
        /// <param name="chosenColour">the colour that the TextBlock will be printed</param>
        private void ColourSeat(string blockName, Brush chosenColour)
        {
            TextBlock chosenBlock = this.FindName(blockName) as TextBlock;
            if (object.ReferenceEquals(chosenBlock, null) == false)
            {
                chosenBlock.Foreground = chosenColour;
            }
        }

        /// <summary>
        /// Takes the selected ListView and the Mouse Event Arguements
        /// to find which element has been selected in the list view
        /// </summary>
        /// <param name="parentList">the list view that has been clicked</param>
        /// <param name="e">the event delegate</param>
        /// <returns></returns>
        private int ListViewIndexCalculator(ListView parentList, MouseButtonEventArgs e)
        {
            DependencyObject item = (DependencyObject)e.OriginalSource;
            if (item.GetType() == typeof(TextBlock))
            {
                return parentList.SelectedIndex;
            }
            else
            {
                return -1;
            }
        }

        private void homescheduleListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //takes which play has been selected from the home page
            //and outputs it in the edit play field
            int index = ListViewIndexCalculator(homescheduleListView, e);
            if (index < 0)
            {
                return;
            }
            HomePanel.Visibility = Visibility.Collapsed;
            PlayPanel.Visibility = Visibility.Visible;
            activePanel = PlayPanel;
            PopulatePlayListView();
            PopulatePlayDetails(index);
        }

        private void expiringMembersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //takes which expiring gold member has been selected from the home page
            //and outputs it in the edit customer field
            int index = ListViewIndexCalculator(expiringMembersListView, e);
            if (index < 0 || ValidationMethods.HasInput(index.ToString()) == false)
            {
                MessageBox.Show("Please select a valid customer");
                return;
            }
            string customerTarget = ListViewNameParser(expiringMembersListView.SelectedItem.ToString());
            Customer targetCustomer = BusinessInstance.FetchCustomerByName(customerTarget);
            HomePanel.Visibility = Visibility.Collapsed;
            CustomerPanel.Visibility = Visibility.Visible;
            activePanel = CustomerPanel;
            PopulateCustomerListView();
            PopulateCustomerEditFields(targetCustomer);
        }

        private void homeButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.getAccessRights() == User.ACCESS_RIGHTS.bookingTeam || currentUser.getAccessRights() == User.ACCESS_RIGHTS.newsletterEditor)
            {
                MessageBox.Show("Current user logon does not have authentication for these features");
                return;
            }
            activePanel.Visibility = Visibility.Collapsed;
            HomePanel.Visibility = Visibility.Visible;
            activePanel = HomePanel;
            HomePageLoad();
        }

        private void bookButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.getAccessRights() == User.ACCESS_RIGHTS.bookingTeam || currentUser.getAccessRights() == User.ACCESS_RIGHTS.newsletterEditor)
            {
                MessageBox.Show("Current user logon does not have authentication for these features");
                return;
            }
            activePanel.Visibility = Visibility.Collapsed;
            BookingPanel.Visibility = Visibility.Visible;
            activePanel = BookingPanel;
            ResetBookingPage();
        }

        private void customerButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.getAccessRights() == User.ACCESS_RIGHTS.bookingTeam)
            {
                MessageBox.Show("Current user logon does not have authentication for these features");
                return;
            }
            activePanel.Visibility = Visibility.Collapsed;
            CustomerPanel.Visibility = Visibility.Visible;
            activePanel = CustomerPanel;
            ResetCustomerPage();
        }

        private void playButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.getAccessRights() == User.ACCESS_RIGHTS.customerRep)
            {
                MessageBox.Show("Current user logon does not have authentication for these features");
                return;
            }
            activePanel.Visibility = Visibility.Collapsed;
            PlayPanel.Visibility = Visibility.Visible;
            activePanel = PlayPanel;
            PopulatePlayListView();
            
        }

        /// <summary>
        /// Finds the play that has been selected from the list view
        /// and outputs all the relevant sales information to the booking page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bookingListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ListViewIndexCalculator(bookingListView, e);
            if (index == -1)
            {
                index = 0;
            }
            selectedPlay = BusinessInstance.GetPlayDictionaryItem(index.ToString());
            currentPlayPrices = selectedPlay.priceRange;
            List<Tickets> ticketList = BusinessInstance.FetchBookings(selectedPlay.playID);
            ResetChairColours(BUStack.Children, Brushes.Aqua);
            ResetChairColours(BDStack.Children, Brushes.Red);
            ResetChairColours(BSStack.Children, Brushes.Gold);
            foreach (Tickets ticket in ticketList)
            {
                ColourSeat(ticket.GetTicketID(), Brushes.Gray);
            }
        }

        private void BSeatingChartStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                string seatName = ((TextBlock)e.OriginalSource).Name; //if the object clicked is a valid TextBlock, get the element's Name
                for (int i = 0; i < chosenSeats.Count; i++) //loop through our list of chosen seats
                {
                    if (chosenSeats[i] == seatName) //if it exists already
                    {
                        ColourSeat(chosenSeats[i], previousSeatColor[i]); //reset the textblock and remove it from the list
                        chosenSeats.RemoveAt(i);
                        previousSeatColor.RemoveAt(i);
                        double prices = BookingPriceCalculator(chosenSeats, currentPlayPrices); //recalculate the prices
                        UpdateSelectionDisplay(prices);
                        return;
                    }
                }
                if (chosenSeats.Count >= 6)
                {
                    MessageBox.Show("Too many seats selected! Please ensure you have a maximum of 6 seats selected.", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
                else if (((TextBlock)e.OriginalSource).Foreground == Brushes.Gray)
                {
                    MessageBox.Show("Seat already reserved!");
                }
                else if (seatName.Length < 3) //if the user tries to click on the seat row textblocks, stop them
                {
                    return;
                }
                else
                {
                    //if seat selection is valid, add it to our list, colour the selection and work out the prices
                    chosenSeats.Add(seatName);
                    previousSeatColor.Add((((TextBlock)e.OriginalSource).Foreground));
                    ColourSeat(seatName, Brushes.Green);
                    double currentCost = BookingPriceCalculator(chosenSeats, currentPlayPrices);
                    UpdateSelectionDisplay(currentCost);
                }
            }
        }

        private void customerBookingsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //fetch the slected index
            int selectedPlay = ListViewIndexCalculator(customerBookingsListView, e);
            if (selectedPlay < 0) //return if it's invalid
            {
                return;
            }
            string playID = customerPlayIDs[selectedPlay]; //get the ID of the play
            List<Tickets> customerBookings = BusinessInstance.FetchTicketsByCustomerAndPlay(customerIDTextbox.Text, playID); //fetch the customers bookings
            List<Tickets> allBookings = BusinessInstance.FetchBookings(playID); //fetch all the bookings
            EditBookingDialogBox dialogBox = new EditBookingDialogBox(customerBookings, allBookings, this); //open up the edit bookings dialog box
            editDataMode = false; //reset the edit data variable
            dialogBox.Owner = this; //set the dialog box owner
            dialogBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialogBox.ShowDialog();
            if (editDataMode == false) //if nothing was changed, return
            {
                return;
            }
            editDataMode = false; //reset the edit variable
            bool success = BusinessInstance.CancelBookings(inPlayID: playID, inCustomerID: customerIDTextbox.Text); //attempt to cancel the existing bookings
            if (success == false)
            {
                MessageBox.Show("Error! Edit unsuccessful.");
                return;
            }
            success = BusinessInstance.AddBooking(incustomerID: customerIDTextbox.Text, inPlayID: playID, Amount: newBookings.Count, seatLocations: newBookings); //attempt to create a new booking
            if (success == false)
            {
                MessageBox.Show("Error! Edit unsuccessful.");
                return;
            }
            else
            {
                MessageBox.Show("Edit successful!");
                return;
            }

        }

        private void playSearchListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ListViewIndexCalculator(playSearchListView, e);
            if (index < 0)
            {
                Debug.WriteLine("Whoops, index is less than 0");
                return;
            }
            else if (index == 0) //if the user chose to add a new play
            {
                ResetPlayDetails(); //clear the edit fields
                Keyboard.Focus(playNameTextbox); //focus the keyboard input
            }
            else if (index > 0)
            {
                PopulatePlayDetails(index - 1); //populate the edit fields with the appropriate play details
                editDataMode = true; //set the system to edit mode
            }
        }

        private void reportPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (playSearchListView.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a play to produce a report for!");
                return;
            }
            PrintDialog printDialog = new PrintDialog(); //Create a new print dialog
            printDialog.ShowDialog(); //show the options dialogue
            printDialog.PrintVisual(PSeatingChartStack, "Sales Report"); //print the visual element containing the sales chart
        }

        private void warnExpiringButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("put word integration here");
        }
        private void ResetPlayDetails()
        {
            //clear and reset the UI elements
            playNameTextbox.Clear();
            playCompanyTextbox.Clear();
            runningPeriodComboBox.SelectedIndex = 0;
            startingDateCalander.DisplayDate = System.DateTime.Today;
            ResetChairsInfo(activePanel);
            priceRangeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// takes the active panel from the system,
        /// and builds a new string which contains the
        /// name of the container element which holds
        /// the seating chart
        /// </summary>
        /// <param name="currentPanel">the currently displayed XAML StackPanel</param>
        public void ResetChairsInfo(StackPanel currentPanel)
        {
            string stackHash = currentPanel.Name.ToString();
            if (stackHash == "HomePanel")
            {
                stackHash = "H";
            }
            else if (stackHash == "BookingPanel")
            {
                stackHash = "B";
            }
            else if (stackHash == "PlayPanel")
            {
                stackHash = "P";
            }

            StackPanel seatingChart = this.FindName(String.Concat(stackHash + "SeatingChartStack")) as StackPanel;
            ResetChairColours(childrenList: seatingChart.Children, chosenColour: Brushes.Black);
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

        private void playSearchTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) //if the user pressed enter whilst focused on the textbox
            {
                int searchType = playSearchComboBox.SelectedIndex; //get the search type
                if (searchType < 0 || searchType > 1) //if the user somehow gets an additional index
                {
                    return;
                }
                switch (searchType)
                {
                    case 0: //if the user is Searching by ID
                        {
                            string searchKey = playSearchTextbox.Text;
                            //validate our ID Input
                            if (ValidationMethods.IsNumeric(searchKey) == false || ValidationMethods.HasInput(searchKey) == false)
                            {
                                MessageBox.Show("The ID supplied is of invalid format, please ensure the ID you are searching for is numeric.");
                                return;
                            }
                            try
                            {
                                //Attempt to search for the target play and output it to the ListView
                                Play targetPlay = BusinessInstance.FetchPlayByIndex(searchKey);
                                playSearchListView.Items.Clear();
                                playSearchListView.Items.Add(targetPlay.GetName() + @"
" + targetPlay.playTime);
                            }
                            catch (Exception) //catch and return after any exceptions
                            {
                                MessageBox.Show("Please ensure the ID you searched for is of numerical format");
                                return;
                            }
                            break;
                        }

                    case 1: //if the user is searching by name
                        {
                            string searchKey = playSearchTextbox.Text;
                            //validate the input
                            if (ValidationMethods.IsNumeric(searchKey) == true || ValidationMethods.HasInput(searchKey) == false)
                            {
                                MessageBox.Show("The Name supplied is of invalid format");
                                return;
                            }
                            try //attempt to search and output the plays
                            {
                                List<Play> targetPlays = BusinessInstance.FetchPlayByName(searchKey);
                                playSearchListView.Items.Clear();
                                for (int i = 0; i < targetPlays.Count; i++)
                                {
                                    playSearchListView.Items.Add(targetPlays[i].GetName() + @"
" + targetPlays[i].playTime);
                                }
                            }
                            catch (Exception) //catch any exceptions and return
                            {
                                MessageBox.Show("Please ensure the ID you searched for is of numerical format");
                                return;
                            }
                            break;
                        }
                }
            }
        }
        private void PopulatePlayListView()
        {
            //on loading of the play page, get all the plays and output them to a listview
            playSearchListView.Items.Clear();
            int length = BusinessInstance.GetPlayDictionaryLength();
            List<Play> listViewOutput = new List<Play>();
            for (int i = 0; i < length; i++)
            {
                try
                {
                    listViewOutput.Add(BusinessInstance.GetPlayDictionaryItem(i.ToString()));
                }
                catch (Exception)
                {
                    continue;
                }
            }
            playSearchListView.Items.Add("Add New Play");
            for (int i = 0; i < length; i++)
            {
                try
                {
                    playSearchListView.Items.Add(listViewOutput[i].GetName() + @"
" + listViewOutput[i].playTime);
                }
                catch (Exception)
                {
                    continue;
                }

            }
        }
        private void PopulatePlayDetails(int playIndex)
        {
            //when the user selects a play, find the play and output all it's details to the edit fields
            selectedPlay = BusinessInstance.GetPlayDictionaryItem(playIndex.ToString());
            List<Tickets> sales = BusinessInstance.FetchBookings(selectedPlay.playID);
            List<string> saleSeats = new List<string>();
            ResetChairsInfo(activePanel);
            foreach (Tickets ticket in sales)
            {
                //create a new string by adding a P to the existing seat code, then colouring the existing UI element
                ColourSeat(("P" + ticket.GetTicketID().Substring(1, ticket.GetTicketID().Length - 1)), Brushes.Green);
                saleSeats.Add(ticket.GetTicketID());
            }
            double upperStalls;
            double dressStalls;
            double stalls;
            double price = TotalSalesCalculator(saleSeats, selectedPlay.priceRange, out upperStalls, out dressStalls, out stalls);
            UpperCircleBlock.Text = "Upper Circle Sales: £" + upperStalls.ToString();
            DressCircleBlock.Text = "Dress Circle Sales: £" + dressStalls.ToString();
            StallBlock.Text = "Stall Sales: £" + stalls.ToString();
            TotalSaleBlock.Text = "Total Sales: £" + price.ToString();
            playNameTextbox.Text = selectedPlay.GetName();
            playCompanyTextbox.Text = selectedPlay.GetCompany();
            startingDateCalander.DisplayDate = selectedPlay.playTime;
            priceRangeComboBox.SelectedIndex = (int)selectedPlay.GetPrice();
        }

        private void cancelPlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int toBeDeleted = playSearchListView.SelectedIndex - 1; //get the index of the play to be deleted
                MessageBoxResult userResponse = MessageBox.Show("Are you sure you want to delete " + BusinessInstance.GetPlayDictionaryItem(toBeDeleted.ToString()).GetName() + "?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (userResponse == MessageBoxResult.Yes) //check they want to go through
                {
                    BusinessInstance.CancelPlay(toBeDeleted.ToString()); //cancel the play and reset the UI elements
                    ResetPlayDetails();
                    PopulatePlayListView();
                }
            }
            catch
            {
                MessageBox.Show("No Play has been selected", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }
        }
        private void savePlayButton_Click(object sender, RoutedEventArgs e)
        {
            //validate the inputs for the new play
            if (ValidationMethods.IsNumeric(playNameTextbox.Text) == true || ValidationMethods.HasInput(playNameTextbox.Text) == false)
            {
                MessageBox.Show("Error! Please input a valid Play name!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (ValidationMethods.IsNumeric(playCompanyTextbox.Text) == true || ValidationMethods.HasInput(playCompanyTextbox.Text) == false)
            {
                MessageBox.Show("Error! Please input a valid Production Company", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (startingDateCalander.SelectedDate == null || ValidationMethods.ValidDate((DateTime)startingDateCalander.SelectedDate) == false)
            {
                MessageBox.Show("Error! Please input a valid starting date!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            //get the date and create a new time span for 8pm and merge the date and time together
            DateTime selectedDate = (DateTime)startingDateCalander.SelectedDate;
            TimeSpan setPlayTime = new TimeSpan(20, 0, 0);
            selectedDate = selectedDate.Date + setPlayTime;
            int runningPeriod = runningPeriodComboBox.SelectedIndex; //get the chosen running length
            if (editDataMode == false) //if we're not editing
            {
                int numberOfShows;
                switch (runningPeriod)
                {
                    case 0: //and we're showing a play for 3 weeks
                        numberOfShows = 24; //set the number of shows to 24
                        if (selectedDate.Month > 10 || selectedDate.Month < 5) //check they're in the summer season
                        {
                            MessageBox.Show("Error! Main Plays may only be staged between May and September");
                            return;
                        }
                        for (int i = 0; i < numberOfShows; i++) //loop through and create the new main plays
                        {
                            if (selectedDate.DayOfWeek == DayOfWeek.Tuesday || selectedDate.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                selectedDate = selectedDate.AddDays(1);
                            }
                            else if (selectedDate.DayOfWeek == DayOfWeek.Saturday) //create a mantinee showing for Saturdays
                            {
                                DateTime mantineeShowing = selectedDate;
                                TimeSpan afternoonTime = new TimeSpan(15, 0, 0);
                                mantineeShowing = mantineeShowing.Date + afternoonTime;
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: mantineeShowing);
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                                selectedDate = selectedDate.AddDays(1);
                            }
                            else
                            {
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                                selectedDate = selectedDate.AddDays(1);
                            }
                        }
                        break;
                    case 1: //do the same but with more shows for a 4 week run
                        numberOfShows = 32;
                        if (selectedDate.Month > 10 || selectedDate.Month < 5)
                        {
                            MessageBox.Show("Error! Main Plays may only be staged between May and September");
                            return;
                        }
                        for (int i = 0; i < numberOfShows; i++)
                        {
                            if (selectedDate.DayOfWeek == DayOfWeek.Tuesday || selectedDate.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                selectedDate = selectedDate.AddDays(1);
                            }
                            else if (selectedDate.DayOfWeek == DayOfWeek.Saturday)
                            {
                                DateTime mantineeShowing = selectedDate;
                                TimeSpan afternoonTime = new TimeSpan(15, 0, 0);
                                mantineeShowing = mantineeShowing.Date + afternoonTime;
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: mantineeShowing);
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                                selectedDate = selectedDate.AddDays(1);
                            }
                            else
                            {
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                                selectedDate = selectedDate.AddDays(1);
                            }
                        }
                        break;
                    case 2: //if it's a one off play
                        //check it's during summer season
                        if (selectedDate.Month > 10 || selectedDate.Month < 5)
                        {
                            MessageBox.Show("Error! Play added to off season");
                            return;
                        }
                        //check they're scheduled for the right day
                        if (selectedDate.DayOfWeek != DayOfWeek.Tuesday && selectedDate.DayOfWeek != DayOfWeek.Wednesday)
                        {
                            MessageBox.Show("Please place a one off play on a Tuesday or Wednesday");
                            return;
                        }
                        //add the play
                        BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                        break;
                    case 3: //if it's a christmas special
                        numberOfShows = 61;
                        //check it starts on the 1st of November
                        if (selectedDate.Day != 1 || selectedDate.Month != 11)
                        {
                            MessageBox.Show("Christmas performances can only begin from the 1st of November");
                            return;
                        }
                        else
                        {
                            //add plays to the list, skipping Christmas Eve, Christmas and Boxing Day
                            for (int i = 0; i < numberOfShows; i++)
                            {
                                if (i == 54 || i == 55 || i == 56)
                                {
                                    continue;
                                }
                                BusinessInstance.AddPlay(inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: selectedDate);
                                selectedDate = selectedDate.AddDays(1);
                            }
                        }
                        break;
                }
                BusinessInstance.SavePlay("Play.txt");
                MessageBox.Show("Creation successful");
            }
            else //if we're editing data, just call the edit play method
            {
                BusinessInstance.EditPlay(inplayID: (playSearchListView.SelectedIndex - 1).ToString(), inplayName: playNameTextbox.Text, inproductionCompany: playCompanyTextbox.Text, inpriceRange: (PRICE_RANGE)priceRangeComboBox.SelectedIndex, inplayTime: (DateTime)startingDateCalander.SelectedDate);
            }
            ResetPlayDetails();
            PopulatePlayListView();
        }
        private void ResetBookingPage()
        {
            //clear and reset the UI elements on the booking page
            bookingListView.Items.Clear();
            ResetChairsInfo(activePanel);
            bookingCustomersListView.Items.Clear();
            bookingCustomersListView.Items.Add("Add New Customer");
            BNameSearchBox.Clear();
            BNoSearchBox.Clear();
            chosenSeats.Clear();
            PopulateBookingPlayList();
            selectedSeatsTextblock.Text = "";

        }
        private void PopulateBookingPlayList()
        {
            //populate the select play listview on the booking page from the current Play Dictionary
            bookingListView.Items.Clear();
            int length = BusinessInstance.GetPlayDictionaryLength();
            List<Play> listViewOutput = new List<Play>();
            for (int i = 0; i < length; i++)
            {
                listViewOutput.Add(BusinessInstance.GetPlayDictionaryItem(i.ToString()));
                bookingListView.Items.Add(listViewOutput[i].GetName() + @"
" + listViewOutput[i].playTime);
            }
        }
        private void UpdateSelectionDisplay(double prices)
        {
            selectedSeatsTextblock.Text = "";
            string selectedSeats = "";
            for (int i = 0; i < chosenSeats.Count; i++)
            {
                selectedSeats += (chosenSeats[i] + ", ");
            }
            selectedSeatsTextblock.Foreground = Brushes.Red;
            selectedSeatsTextblock.Text = "Selected Seats: " + selectedSeats + @"
" + "Total Cost: £" + prices.ToString();
        }

        private void BCustomerSearchButton_Click(object sender, RoutedEventArgs e)
        {
            //validate the inputs for searching users
            if (ValidationMethods.IsNumeric(BNameSearchBox.Text) == true ^ ValidationMethods.HasInput(BNameSearchBox.Text) == false)
            {
                MessageBox.Show("Error! Please input a valid name to search!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (ValidationMethods.ValidPhoneNumber(BNoSearchBox.Text) == false ^ ValidationMethods.HasInput(BNoSearchBox.Text) == false)
            {
                MessageBox.Show("Error! Please input a valid number to search!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            //move the inputs over to variables
            string keyName = BNameSearchBox.Text;
            string keyNumber = BNoSearchBox.Text;

            List<Customer> targetCustomers = new List<Customer>();

            //if they searched by name
            if (keyName != "" && keyNumber == "")
            {
                targetCustomers.Add(BusinessInstance.FetchCustomerByName(keyName));
            }
            //if they searched by telephone number
            else if (keyName == "" && keyNumber != "")
            {
                targetCustomers.Add(BusinessInstance.FetchCustomerByNumber(keyNumber));
            }
            //if they didn't search
            else if (keyName == "" && keyNumber == "")
            {
                MessageBox.Show("Please enter search criteria!", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }
            PopulateBookingSearch(targetCustomers); //populate the Customer Listview on the Booking Page
        }
        private void PopulateBookingSearch(List<Customer> displayMember)
        {
            //loop through a list of customers and output them to a list view
            bookingCustomersListView.Items.Clear();
            bookingCustomersListView.Items.Add("Create New Customer");
            foreach (Customer item in displayMember)
            {try
                {
                    if (item.IsGold() == true)
                    {
                        bookingCustomersListView.Items.Add(item.GetName() + @"
" + ((GoldMember)item).GetAddress());
                    }
                    else
                    {
                        bookingCustomersListView.Items.Add(item.GetName() + @"
" + item.GetNumber());
                    }
                }
              catch
                {
                    MessageBox.Show("Customer Not Found", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
            }
        }

        private void bookingResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBookingPage();
        }

        private void bookingConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //get the element selected by the 
            int index = bookingCustomersListView.SelectedIndex;
            bool success;
            if (index == 0)
            {
                //validate the inputs
                if (ValidationMethods.HasInput(BNameSearchBox.Text) == false || ValidationMethods.IsNumeric(BNameSearchBox.Text) == true)
                {
                    MessageBox.Show("Please input a valid name for the new Customer");
                    return;
                }
                if (ValidationMethods.ValidPhoneNumber(BNoSearchBox.Text) == false || ValidationMethods.IsNumeric(BNoSearchBox.Text) == false)
                {
                    MessageBox.Show("Please input a valid number for the new Customer");
                    return;
                }
                //move the inputs over to variables
                string newName = BNameSearchBox.Text;
                string newNumber = BNoSearchBox.Text;
                int customerID = BusinessInstance.GetCustomerDictionaryLength();

                bool newCustomerSuccess = BusinessInstance.AddCustomer(newName, false, newNumber); //try and make a new customer
                if (newCustomerSuccess == false) //return if it fails
                {
                    MessageBox.Show("There was an error in creating your new customer, please try again");
                    return;
                }
                success = BusinessInstance.AddBooking(customerID.ToString(), selectedPlay.playID, chosenSeats.Count, chosenSeats);
                //attempt to add a new booking
            }
            else
            {
                //ensure the user has selected a customer
                if (index < 0 || ValidationMethods.HasInput(index.ToString()) == false)
                {
                    MessageBox.Show("Please select a valid customer");
                    return;
                }
                //parse the name of the customer from the list view and find them in the dictionary

                try
                {
                    string customerTarget = ListViewNameParser(bookingCustomersListView.SelectedItem.ToString());
                    Customer targetCustomer = BusinessInstance.FetchCustomerByName(customerTarget);
                    List<Tickets> existingBookings = BusinessInstance.FetchTicketsByCustomerAndPlay(targetCustomer.customerID, selectedPlay.playID);
             

                //if the user already has bookings for the show, return
                if (existingBookings.Count != 0)
                {
                    MessageBox.Show("Only one booking for each play per customer");
                    return;
                }
                //attempt to make a new booking
                success = BusinessInstance.AddBooking(targetCustomer.customerID, selectedPlay.playID, chosenSeats.Count, chosenSeats);
                }
                catch
                {
                    MessageBox.Show("Unable to process request, Please double click a play");
                    return;
                }
            }
            if (success == true)
            {
                MessageBox.Show("Booking succeded! Please confirm this booking by: " + DateTime.Now.AddDays(6));
            }
            else
            {
                MessageBox.Show("Booking failed! Please try again.");
            }
            ResetBookingPage(); //reset the page
        }
        /// <summary>
        /// loops through the selected item of a list view and catches anything before the first
        /// escape character
        /// </summary>
        /// <param name="inName">the item to be parsed</param>
        /// <returns></returns>
        private string ListViewNameParser(string inName)
        {
            string outName = "";
            int parseTest = 0;
            foreach (char letter in inName)
            {
                if (Regex.IsMatch(letter.ToString(), "\r") == true)
                {
                    break;
                }
                else if (int.TryParse(letter.ToString(), out parseTest) == false)
                {
                    outName += letter;
                }
            }
            return outName;
        }
        private void ResetCustomerPage()
        {
            customerSearchTextbox.Clear();
            customerSearchComboBox.SelectedIndex = 0;
            PopulateCustomerListView();
            ResetCustomerDetailInput();
            editDataMode = false;
            ExpiryMonthComboBox.SelectedIndex = 0;
        }
        private void ResetCustomerDetailInput()
        {
            customerIDTextbox.Clear();
            customerIDTextbox.IsEnabled = false;
            customerNameTextbox.Clear();
            customerTelephoneTextbox.Clear();
            customerGoldCheckBox.IsChecked = false;
            customerRenewalDateTextBox.Clear();
            customerRenewalDateTextBox.IsEnabled = false;
            customerAddressTextBox.Clear();
            customerAddressTextBox.IsEnabled = false;
            customerBookingsListView.Items.Clear();
        }
        private void PopulateCustomerListView()
        {
            List<Customer> customerDisplay = new List<Customer>();
            customerListView.Items.Clear();
            int length = BusinessInstance.FetchBiggestCustomerIndex();
            for (int i = 0; i < length + 1; i++)
            {
                Customer catcher = BusinessInstance.FetchCustomerByIndex(i.ToString());
                if (catcher == null)
                {
                    continue;
                }
                customerDisplay.Add(catcher);
                if (customerDisplay[i].IsGold() == true)
                {
                    customerListView.Items.Add(catcher.GetName() + @"
" + ((GoldMember)catcher).GetAddress());
                }
                else
                {
                    customerListView.Items.Add(catcher.GetName() + @"
" + catcher.GetNumber());
                }
            }
        }

        private void customerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //get the selected index
            int index = ListViewIndexCalculator(customerListView, e);
            if (index < 0) //return if it's less than 0
            {
                Debug.WriteLine("selected customer index is less than 0");
                return;
            }
            else //if not, move all the data to the edit fields
            {
                Customer targetCustomer = BusinessInstance.FetchCustomerByIndex(index.ToString());
                editDataMode = true;
                PopulateCustomerEditFields(targetCustomer);
            }
        }
        private void PopulateCustomerEditFields(Customer inCustomer)
        {
            customerIDTextbox.Text = inCustomer.customerID;
            customerNameTextbox.Text = inCustomer.GetName();
            customerTelephoneTextbox.Text = inCustomer.GetNumber();
            if (inCustomer.IsGold() == true)
            {
                customerGoldCheckBox.IsChecked = true;
                customerAddressTextBox.Text = ((GoldMember)inCustomer).GetAddress();
                customerAddressTextBox.IsEnabled = true;
                customerRenewalDateTextBox.Text = ((GoldMember)inCustomer).GetRenewalDate().ToString();
                customerRenewalDateTextBox.IsEnabled = true;
            }
            PopulateCustomerBookingsList(int.Parse(inCustomer.customerID));
        }

        private void customerGoldCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (customerGoldCheckBox.IsChecked == true)
            {
                customerAddressTextBox.IsEnabled = true;
                customerRenewalDateTextBox.IsEnabled = true;
            }
            else
            {
                customerAddressTextBox.IsEnabled = false;
                customerRenewalDateTextBox.IsEnabled = false;
            }
        }

        private void deleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            //Check the user isn't trying to delete the hardcoded Create New Customer button
            if (customerListView.SelectedIndex == -1)
            {
                MessageBox.Show("Cannot delete this record!", "Alert");
                return;
            }
            //if not, get the Customer index and remove it from the Customer Dictionary
            string customerTarget = ListViewNameParser(customerListView.SelectedItem.ToString());
            Customer targetCustomer = BusinessInstance.FetchCustomerByName(customerTarget);
            bool success = BusinessInstance.RemoveCustomer(targetCustomer.customerID);
            if (success == false)
            {
                MessageBox.Show("Deletion failed, please try again!");
                return;
            }
            else
            {
                MessageBox.Show("Deletion successful!");
                ResetCustomerPage();
            }
        }

        private void customerSaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            //if we're editing a customer
            if (editDataMode == true)
            {
                MessageBoxResult result = MessageBox.Show("Warning! Editing a customer's details will result in the deletion of their bookings, are you sure you wish to proceed?", "Alert", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
                //get their name and a reference to their data
                string customerTarget = ListViewNameParser(customerListView.SelectedItem.ToString());
                Customer targetCustomer = BusinessInstance.FetchCustomerByName(customerTarget);
                editDataMode = false; //reset the edit variable
                BusinessInstance.RemoveCustomer(targetCustomer.customerID); //remove the customer

                //validate the new inputs
                if (ValidationMethods.IsNumeric(customerNameTextbox.Text) == true || ValidationMethods.HasInput(customerNameTextbox.Text) == false)
                {
                    MessageBox.Show("Error! Please input a valid Name!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                if (ValidationMethods.ValidPhoneNumber(customerTelephoneTextbox.Text) == false || ValidationMethods.HasInput(customerTelephoneTextbox.Text) == false)
                {
                    MessageBox.Show("Error! Please input a valid Telephone Number!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
                //get the new inputs, check if the user is gold and attempt to build a new customer
                string newName = customerNameTextbox.Text;
                string newNumber = customerTelephoneTextbox.Text;
                bool success;
                if (customerGoldCheckBox.IsChecked == false)
                {
                    success = BusinessInstance.AddCustomer(newName, false, newNumber);
                }
                else
                {
                    if (ValidationMethods.HasInput(customerAddressTextBox.Text) == false)
                    {
                        MessageBox.Show("Error! Please input a valid Address!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    string newAddress = customerAddressTextBox.Text;
                    success = BusinessInstance.AddGoldMember(newName, true, newNumber, newAddress, System.DateTime.Now.AddYears(1));
                }

                if (success == false)
                {
                    MessageBox.Show("Error! Customer not edited!", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("Edit successful!");
                    ResetCustomerPage();
                }
            }
            else
            {
                //validate the inputs
                if (ValidationMethods.IsNumeric(customerNameTextbox.Text) == true || ValidationMethods.HasInput(customerNameTextbox.Text) == false)
                {
                    MessageBox.Show("Error! Please input a valid Name!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                if (ValidationMethods.ValidPhoneNumber(customerTelephoneTextbox.Text) == false || ValidationMethods.HasInput(customerTelephoneTextbox.Text) == false)
                {
                    MessageBox.Show("Error! Please input a valid Telephone Number!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
                //move the inputs to variables, check the customer is gold or not and attempt to build a new customer
                string newName = customerNameTextbox.Text;
                string newNumber = customerTelephoneTextbox.Text;
                bool success;
                if (customerGoldCheckBox.IsChecked == false)
                {
                    success = BusinessInstance.AddCustomer(newName, false, newNumber);
                }
                else
                {
                    if (ValidationMethods.HasInput(customerAddressTextBox.Text) == false)
                    {
                        MessageBox.Show("Error! Please input a valid Address!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    string newAddress = customerAddressTextBox.Text;
                    success = BusinessInstance.AddGoldMember(newName, true, newNumber, newAddress, System.DateTime.Now.AddYears(1));
                }

                if (success == false)
                {
                    MessageBox.Show("Error! Customer not saved!", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("Save successful!");
                    ResetCustomerPage();
                }
            }
        }
        private void PopulateCustomerBookingsList(int customerIndex)
        {
            customerBookingsListView.Items.Clear();
            List<Tickets> customerTickets = BusinessInstance.FetchTicketByCustomer(customerIndex.ToString());
            if (customerTickets.Count == 0)
            {
                return;
            }

            customerPlayIDs.Clear();
            for (int i = 0; i < customerTickets.Count; i++)
            {
                if (customerPlayIDs.Exists(e => e.Equals(customerTickets[i].PlayID)) == false)
                {
                    customerPlayIDs.Add(customerTickets[i].PlayID);
                }
            }

            List<Tickets> customerOrders = new List<Tickets>();
            for (int i = 0; i < customerTickets.Count; i++)
            {
                if (customerOrders.Count == 0)
                {
                    customerOrders.Add(customerTickets[i]);
                    continue;
                }
                else
                {
                    for (int j = 0; j < customerOrders.Count; j++)
                    {
                        if (customerOrders[j].PlayID == customerTickets[i].PlayID)
                        {
                            continue;
                        }
                        else if (j < customerOrders.Count && customerOrders[j].PlayID == customerTickets[i].PlayID)
                        {
                            continue;
                        }
                        else if (j <= customerOrders.Count && customerOrders[j].PlayID == customerTickets[i].PlayID)
                        {
                            customerOrders.Add(customerTickets[i]);
                            break;
                        }
                    }
                }
            }

            List<string> playList = new List<string>();
            int length = BusinessInstance.GetPlayDictionaryLength();
            Play playCatcher;
            for (int i = 0; i < customerOrders.Count; i++)
            {
                playCatcher = BusinessInstance.GetPlayDictionaryItem(customerOrders[i].PlayID);
                customerBookingsListView.Items.Add(playCatcher.GetName() + @"
" + playCatcher.playTime + " Confirmed purchase? " + customerOrders[i].IsConfirmed().ToString());
            }
        }

        private void customerSearchButton_Click(object sender, RoutedEventArgs e)
        {
            //get what kind of search we're doing
            int searchType = customerSearchComboBox.SelectedIndex;
            //validate the inputs
            if (ValidationMethods.HasInput(customerSearchTextbox.Text) == false)
            {
                MessageBox.Show("Please input a search term!", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }
            string searchKey;
            switch (searchType)
            {
                case 0: //if we're searching by name
                    //validate the input
                    if (ValidationMethods.IsNumeric(customerSearchTextbox.Text) == true)
                    {
                        MessageBox.Show("Please input a valid name to search!", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    else
                    {
                        //get the input
                        searchKey = customerSearchTextbox.Text;
                        Customer targetCustomer;
                        try
                        {
                            //attempt to search and output the chosen user
                            targetCustomer = BusinessInstance.FetchCustomerByName(searchKey);
                            customerListView.Items.Clear();
                            customerListView.Items.Add(targetCustomer.GetName() + @"
" + targetCustomer.GetNumber());
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show("Error, user not found! Please adjust search terms and retry.", "Alert!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            Debug.WriteLine(E);
                            return;
                        }
                        break;
                    }
                case 1: //if we're searching by ID
                    //validate the input
                    if (ValidationMethods.IsNumeric(customerSearchTextbox.Text) == false)
                    {
                        MessageBox.Show("Please input a valid ID to search!", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    else
                    {
                        //get the input and then attempt to search for it
                        searchKey = customerSearchTextbox.Text;
                        Customer targetCustomer;
                        try
                        {
                            targetCustomer = BusinessInstance.FetchCustomerByIndex(searchKey);
                            customerListView.Items.Clear();
                            customerListView.Items.Add(targetCustomer.GetName() + @"
" + targetCustomer.GetNumber());
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show("Error, user not found! Please adjust search terms and retry.", "Alert!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            Debug.WriteLine(E);
                            return;
                        }
                        break;
                    }
                case 2: //if we're searching by Phone Number
                    //validate the inputs
                    if (ValidationMethods.ValidPhoneNumber(customerSearchTextbox.Text) == false)
                    {
                        MessageBox.Show("Please input a valid Phone Number to search!", "Alert", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    else
                    {
                        //get the input and attempt to search the dictionary
                        searchKey = customerSearchTextbox.Text;
                        Customer targetCustomer;
                        try
                        {
                            targetCustomer = BusinessInstance.FetchCustomerByNumber(searchKey);
                            customerListView.Items.Clear();
                            customerListView.Items.Add(targetCustomer.GetName() + @"
" + targetCustomer.GetNumber());
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show("Error, user not found! Please adjust search terms and retry.", "Alert!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            Debug.WriteLine(E);
                            return;
                        }
                        break;
                    }

            }
        }

        private void viewGoldButton_Click(object sender, RoutedEventArgs e)
        {
            List<GoldMember> outputList = BusinessInstance.FindGoldMembers();
            if (outputList.Count == 0)
            {
                MessageBox.Show("No Gold Members to show!");
                return;
            }
            PopulateWithGoldCustomers(output: outputList);

        }
        private void PopulateWithGoldCustomers(List<GoldMember> output)
        {
            customerListView.Items.Clear();
            for (int i = 0; i < output.Count; i++)
            {
                customerListView.Items.Add(output[i].GetName() + @"
" + output[i].GetAddress());
            }
        }

        private void viewExpiringButton_Click(object sender, RoutedEventArgs e)
        {
            //get the selected month to search for
            int selectedMonth = ExpiryMonthComboBox.SelectedIndex + 1;
            //find any customers expiring in that month
            List<GoldMember> outputList = BusinessInstance.FindExpiringMembers(selectedMonth);
            if (outputList.Count == 0)
            {
                MessageBox.Show("No Expiring Members to show!");
                return;
            }
            PopulateWithGoldCustomers(output: outputList); //output it to a list view

            //if the user wants to print the list
            MessageBoxResult sendToPrint = MessageBox.Show("Do you wish to print the list of expiring members?", "Alert!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (sendToPrint == MessageBoxResult.Yes)
            {
                PrintDialog printDialog = new PrintDialog(); //open a new print dialog
                printDialog.ShowDialog(); //open up the print options dialog
                printDialog.PrintVisual(customerListView, "Expiring members for month " + ExpiryMonthComboBox.SelectedItem.ToString());
                //print the Customer List View Element
            }

        }

        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            ResetCustomerPage();
            Keyboard.Focus(customerNameTextbox);
        }
        public void ReturnEditInfo(List<string> inNewBookings, bool confirm, bool cancel, string inPlayID, string inCustomerID)
        {
            //when the EditBooking Dialog Box has closed
            if (confirm == true) //and the user is confirming a booking
            {
                BusinessInstance.ConfirmBooking(inPlayID: inPlayID, inCustomerID: inCustomerID);
                PopulateCustomerBookingsList(int.Parse(inCustomerID)); //confirm the booking
            }
            else if (cancel == true) //or the user is cancelling
            {
                BusinessInstance.CancelBookings(inPlayID, inCustomerID);
                PopulateCustomerBookingsList(int.Parse(inCustomerID)); //cancel the booking
            }
            else
            { //else set up the data for editing
                newBookings = inNewBookings;
                editDataMode = true;
            }
        }
        /// <summary>
        /// goes through a list of seat locations and calculates how much the whole booking is worth
        /// </summary>
        /// <param name="seatLocations">the chosen seat locations</param>
        /// <param name="prices">the price range for the play</param>
        /// <returns>a floating point containing the total price</returns>
        public double BookingPriceCalculator(List<string> seatLocations, PRICE_RANGE prices)
        {
            double price = 0;
            for (int i = 0; i < seatLocations.Count; i++)
            {
                string location = seatLocations[i].Substring(0, 2);
                switch (seatLocations[i].Substring(1, 1))
                {
                    case "U":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 10;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 20;
                        }
                        else
                        {
                            price += 30;
                        }
                        break;
                    case "D":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 30;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 40;
                        }
                        else
                        {
                            price += 50;
                        }
                        break;
                    case "S":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 20;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 30;
                        }
                        else
                        {
                            price += 40;
                        }
                        break;
                }
            }
            return price;
        }
        /// <summary>
        /// loops through a whole list of seat locations to
        /// work out the prices of each individual seating section
        /// </summary>
        /// <param name="seatLocations">the list of seat locations</param>
        /// <param name="prices">the price range</param>
        /// <param name="upperStalls">the returning price of the Upper Stalls</param>
        /// <param name="dressStalls">the returning price of the Dress Stalls</param>
        /// <param name="stalls"> the returning price of the pits</param>
        /// <returns>a floating point containing the total price</returns>
        public double TotalSalesCalculator(List<string> seatLocations, PRICE_RANGE prices, out double upperStalls, out double dressStalls, out double stalls)
        {
            double price = 0;
            upperStalls = 0;
            dressStalls = 0;
            stalls = 0;
            for (int i = 0; i < seatLocations.Count; i++)
            {
                string location = seatLocations[i].Substring(0, 2);
                switch (seatLocations[i].Substring(1, 1))
                {
                    case "U":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 10;
                            upperStalls += 10;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 20;
                            upperStalls += 20;
                        }
                        else
                        {
                            price += 30;
                            upperStalls += 30;
                        }
                        break;
                    case "D":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 30;
                            dressStalls += 30;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 40;
                            dressStalls += 40;
                        }
                        else
                        {
                            price += 50;
                            dressStalls += 50;
                        }
                        break;
                    case "S":
                        if (prices == PRICE_RANGE.low)
                        {
                            price += 20;
                            stalls += 20;
                        }
                        else if (prices == PRICE_RANGE.mid)
                        {
                            price += 30;
                            stalls += 30;
                        }
                        else
                        {
                            price += 40;
                            stalls += 30;
                        }
                        break;
                }
            }
            return price;
        }
        public void HomePageLoad()
        {
            /*TODO: - write method that catches a play by today's date - DONE
            - get the ticket sales for that play - DONE
            - get the schedule - DONE
            - get any of this month's expiring members - DONE
            - tie UI interactions
            */
            Play todaysPlay = BusinessInstance.GetTodaysPlay(inTime: System.DateTime.Today);
            if (todaysPlay == null)
            {
                scheduledPlayBlock.Text = "No plays scheduled for today!";
                PopulateSchedule();
                PopulateExpiringList();
                return;
            }
            List<Tickets> todaysSales = BusinessInstance.FetchBookings(todaysPlay.playID);
            scheduledPlayBlock.Text = "Todays play: " + todaysPlay.GetName() + ", by: " + todaysPlay.GetCompany();
            foreach (Tickets item in todaysSales)
            {
                ColourSeat("H" + item.GetTicketID().Substring(1, item.GetTicketID().Length - 1), Brushes.Green);
            }
            PopulateSchedule();
            PopulateExpiringList();
        }
        public void PopulateSchedule()
        {
            homescheduleListView.Items.Clear();
            List<Play> schedule = BusinessInstance.GetScheduleReport();
            if (schedule.Count <= 0)
            {
                return;
            }
            else
            {
                foreach (Play item in schedule)
                {
                    homescheduleListView.Items.Add(item.GetName() + @"
" + item.playTime);
                }
            }
        }
        public void PopulateExpiringList()
        {
            expiringMembersListView.Items.Clear();
            int month = System.DateTime.Now.Month;
            List<GoldMember> expiryList = BusinessInstance.FindExpiringMembers(month);
            if (expiryList.Count <= 0)
            {
                return;
            }
            else
            {
                foreach (GoldMember customer in expiryList)
                {
                    expiringMembersListView.Items.Add(customer.GetName() + @"
" + customer.GetAddress());
                }
            }
        }

        private void generateNewsletterButton_Click(object sender, RoutedEventArgs e)
        {
            List<Play> generatedSchedule = BusinessInstance.GetScheduleReport();
            if (generatedSchedule.Count <= 0)
            {
                return;
            }
            string fileName = "schedule.txt";
            StreamWriter fileWriter = new StreamWriter(fileName);
            try
            {
                foreach (Play item in generatedSchedule)
                {
                    fileWriter.WriteLine(item.playID);
                    fileWriter.WriteLine(item.GetName());
                    fileWriter.WriteLine(item.GetCompany());
                    fileWriter.WriteLine(item.GetPrice());
                    fileWriter.WriteLine(item.playTime);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Schedule creation failed!");
                return;
            }
            finally
            {
                fileWriter.Close();
                fileWriter.Dispose();
            }
            MessageBox.Show("Schedule successfully created!");
        }
    }
}