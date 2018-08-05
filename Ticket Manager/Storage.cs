using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ticket_Manager
{
    public enum PRICE_RANGE { low, mid, high };//I moved this up here so "Tickets" could use the Enum PH

    public interface ICustomer // Need this for bulk saving 
    {
        string GetName();
        string GetNumber();
        bool IsGold();
  
    }
    public class Customer : ICustomer
    {
        public string customerID;
        protected string customerName;
        protected bool goldMember;
        protected string telNo;
        protected string address;
        protected DateTime renewalDate;
        public Customer(string incustomerID, string incustomerName, bool ingoldMember, string intelNo) //Build customer JAM
        {
            customerID = incustomerID;
            customerName = incustomerName;
            goldMember = ingoldMember;
            telNo = intelNo;
        }
        public string GetName() //Allow access to customer name JAM
        {
            return customerName;
        }
        public string GetNumber() //Allow access to customer telephone number JAM
        {
            return telNo;
        }
        public bool IsGold() //Check if customer is a gold member JAM
        {
            return goldMember;
        }

        public virtual void Save(System.IO.TextWriter textOut)      //Save method for a single customer class using TextWriter  GB 
        {                                                           //All data is stored as a string                     GB
            textOut.WriteLine(customerID);
            textOut.WriteLine(customerName);
            textOut.WriteLine(goldMember);
            textOut.WriteLine(telNo);
        }

        public virtual void Save(string filename)        //Using Streamwriter the Customer record is       
        {                                                //saved to file 'filename'  GB 
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                Save(textOut);                           //Callback to the save method above GB
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
        }

        public  static  Customer Load(System.IO.TextReader textIn)         //Load method for customer class using TextReader  GB 
        {
            Customer result = null;
            try
            {
                string customerID = textIn.ReadLine();
                string customerName = textIn.ReadLine();
                bool goldMember = Convert.ToBoolean(textIn.ReadLine());    //variable 'GoldMember' is converted from a string to a Bool GB  
                string telNo = textIn.ReadLine();
                result = new Customer(customerID, customerID, goldMember, telNo); 
            }
            catch
            {
                return null;
            }
            return result;                                                   //The loaded customer is returned in 'result' GB
        }

        public static Customer Load(string filename)                         //The above method is called via this Load method 
        {                                                                    //Which uses SteamReader to read fro a .txt file GB
            Customer result = null;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = Customer.Load(textIn);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return result;
        }

    }


    public class GoldMember : Customer //GoldMember is a child class of Customer, and will use the methods below in addition to those in the Customer class JAM
    {
        public GoldMember(string incustomerID, string incustomerName, bool ingoldMember, string intelNo,
        string inaddress, DateTime inrenewalDate) : base(incustomerID, incustomerName, ingoldMember, intelNo) //Build GoldMember JAM
        {
            address = inaddress;
            renewalDate = inrenewalDate;
        }
        public override void Save(System.IO.TextWriter textOut)          //Save method for 'GoldCustomer' class using StreamWriter  GB 
        {   
            base.Save(textOut);                                          //Customer save method used as a base   GB                    
            textOut.WriteLine(address);
            textOut.WriteLine(renewalDate);
        }

        public override void Save(string filename)                       //Save method in parent Customer class is Overidden GB
        {
            System.IO.StreamWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                Save(textOut);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null) textOut.Close();
            }
        }

        public static GoldMember LoadGoldMember(System.IO.TextReader textIn)         //Load method for Goldmember customer class using TextReader  GB 
        {
            GoldMember result = null;
            try
            {
                string customerID = textIn.ReadLine();
                string customerName = textIn.ReadLine();
                bool goldMember = Convert.ToBoolean(textIn.ReadLine());    //variable 'GoldMember' is converted from a string to a Bool GB  
                string telNo = textIn.ReadLine();
                string Address = textIn.ReadLine();
                DateTime RenewalDate = Convert.ToDateTime(textIn.ReadLine());
                result = new GoldMember(customerID, customerName, goldMember, telNo ,Address ,RenewalDate);
            }
            catch
            {
                return null;
            }
            return result;                                                   //The loaded Goldcustomer is returned in 'result' GB
        }

        public static GoldMember LoadGoldMember(string filename)             //The above method is called via this Load method 
        {                                                                    //Which uses SteamReader to read fro a .txt file GB
            GoldMember result = null;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = GoldMember.LoadGoldMember(textIn);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return result;
        }
       
        public string GetAddress() //Allow access to GoldMember's address JAM 
        {
            return address;
        }
        public DateTime GetRenewalDate() //Allow access to GoldMember's renewal date JAM
        {
            return renewalDate;
        }
    }
    public class Play 
    {
        public string playID;
        protected string playName;
        protected string productionCompany;

        public PRICE_RANGE priceRange;
        public DateTime playTime;
        protected int ticketsSold = 0;
        ///Gets For the play variables.
        public DateTime GetTime()
        {
            return playTime;
        }
        public PRICE_RANGE GetPrice()
        {
            return priceRange;
        }
        public string GetName()
        {
            return playName;
        }
        public string GetCompany()
        {
            return productionCompany;
        }
        public int getTicketsSold()
        {
            return ticketsSold;
        }
        public bool Addticket()
        {
            ticketsSold++;
            return true;
        }
        public Play(string inPlayID, string inplayName, string inproductionCompany, PRICE_RANGE inpriceRange, DateTime inplayTime) //Build Play JAM
        {
            playID = inPlayID;
            playName = inplayName;
            productionCompany = inproductionCompany;
            priceRange = inpriceRange;
            playTime = inplayTime;
        }

        public virtual void Save(System.IO.TextWriter textOut)      //Save method for customer class using StreamWriter  GB 
        {                                                           //All data is stored as a string                     GB
            textOut.WriteLine(playID);
            textOut.WriteLine(playName);
            textOut.WriteLine(productionCompany);
            textOut.WriteLine(priceRange);
            textOut.WriteLine(playTime);
        }

        public virtual void Save(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                Save(textOut);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
        }

        public static Play Load(System.IO.TextReader textIn)                      //Load method for customer class using StreamReader  GB 
        {
            Play result = null;
            try
            {
                string playID = textIn.ReadLine();
                string playName = textIn.ReadLine();
                string productioncompany = textIn.ReadLine();    //variable 'GoldMember' is converted from a string to a Bool GB  
                PRICE_RANGE priceRange = (PRICE_RANGE)Enum.Parse(typeof(PRICE_RANGE), textIn.ReadLine());
                DateTime playtime = Convert.ToDateTime(textIn.ReadLine());
                result = new Play(playID, playName, productioncompany, priceRange, playtime); ;
            }
            catch
            {
                return null;
            }
            return result;
        }

        public static Play Load(string filename)
        {
            Play result = null;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = Play.Load(textIn);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return result;
        }

    }

    public class Tickets
    {
        // PH 15/11/16
        public string PlayID;
        protected string ticketID;
        public string ticketNo;
        public string customerID;
        public PRICE_RANGE priceRange;
        protected DateTime playTime;
        protected bool confirmed; // Why is this here??? - because we need to see that the tickets have been confirmed as it states in the spec?
        protected DateTime orderDate;

        public Tickets(string inPID, string inTID, string inCID, PRICE_RANGE inPrice, DateTime orderDateIn, string ticketNoIn) //Build Tickets JAM
        {
            PlayID = inPID;
            ticketID = inTID;
            ticketNo = ticketNoIn;
            customerID = inCID;
            priceRange = inPrice;
            orderDate = orderDateIn;
            confirmed = false;
        }
        public bool IsConfirmed() //Check whether the booking has been confirmed JAM
        {
            return confirmed;
        }
        public string GetTicketID()
        {
            return ticketID;
        }
        public void ConfirmTicket()
        {
            confirmed = true;
        }
        public DateTime GetOrderDate()
        {
            return orderDate;
        }

        public virtual void Save(System.IO.TextWriter textOut)      //Save method for Ticket class using StreamWriter  GB 
        {
            textOut.WriteLine(PlayID);
            textOut.WriteLine(ticketID);
            textOut.WriteLine(customerID);
            textOut.WriteLine(priceRange);
            textOut.WriteLine(orderDate);
            textOut.WriteLine(ticketNo);
        }

        public virtual void Save(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                Save(textOut);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
        }

        public static Tickets Load(System.IO.TextReader textIn)                      //Load method for customer class using StreamReader  GB 
        {
            Tickets result = null;
            try
            {
                string PlayID = textIn.ReadLine();
                string TicketID = textIn.ReadLine();
                string CustomerID = textIn.ReadLine();
                PRICE_RANGE priceRange = (PRICE_RANGE)Enum.Parse(typeof(PRICE_RANGE), textIn.ReadLine());
                DateTime playtime = Convert.ToDateTime(textIn.ReadLine());
                string TicketNO = textIn.ReadLine();
                result = new Tickets(PlayID, TicketID, CustomerID, priceRange, playtime, TicketNO);
            }
            catch
            {
                return null;
            }
            return result;
        }

        public static Tickets Load(string filename)
        {
            Tickets result = null;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = Tickets.Load(textIn);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return result;
        }
    }
    public class Business //Class for the storage of Play, Customer and Ticket Dictionaries, and methods to allow the creation and editing of these classes JAM
    {
        public static  Dictionary<string, Play> PlayDictionary = new Dictionary<string, Play>(); //Create a dictionary to store all created Plays JAM
        public static  Dictionary<string, Customer> CustomerDictionary = new Dictionary<string, Customer>(); //Create a dictionary to store all created Customers JAM
        private Dictionary<string, Tickets> TicketsDictionary = new Dictionary<string, Tickets>(); //Create a dictionary to store all created Tickets JAM

        public  void SaveCustomer(System.IO.TextWriter textout)
        {
            textout.WriteLine(CustomerDictionary.Count);
                foreach(Customer Customer in CustomerDictionary.Values)
                {
                  Customer.Save(textout);
                }
                
        }

        public  void SaveCustomer(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                SaveCustomer(textOut);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
            
        }


        public static Business LoadCustomer(System.IO.TextReader textIn)
        {
            Business result = new Business();
            string countString = textIn.ReadLine();
            int count = int.Parse(countString);
            for ( int i = 0; i<count; i++)
            {
                Customer customer = Customer.Load(textIn);
                Business.CustomerDictionary.Add(customer.GetName(), customer);
            }
            return result;
        }

        public static Business LoadCustomer(string filename)
        {
            System.IO.TextReader textIn = null;
            Business result = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = Business.LoadCustomer(textIn);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }

            return result;

        }



            public  void SavePlay(System.IO.TextWriter textout)
        {
            textout.WriteLine(PlayDictionary.Count);
            foreach (Play Play in PlayDictionary.Values)
            {
                Play.Save(textout);
            }

        }

        public  void SavePlay(string filename)
        {
            System.IO.TextWriter textOut = null;
            try
            {
                textOut = new System.IO.StreamWriter(filename);
                SavePlay(textOut);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (textOut != null)
                {
                    textOut.Close();
                }
            }
            
        }


        public static Business LoadPlay(System.IO.TextReader textIn)
        {
            Business result = new Business();
            string countString = textIn.ReadLine();
            int count = int.Parse(countString);
            for (int i = 0; i < count; i++)
            {
                Play play = Play.Load(textIn);
                PlayDictionary.Add(play.GetName(), play);
            }
            return result;
        }

        public static Business LoadPlay(string filename)
        {
            System.IO.TextReader textIn = null;
            Business result = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                result = Business.LoadPlay(textIn);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }

            return result;

        }
        public int GetPlayDictionaryLength()
        {
            int Length = PlayDictionary.Count;
            return Length;
        }
        public Play GetPlayDictionaryItem(string index) //Use a given index to find an item within the PlayDictionary JAM
        {
            return PlayDictionary[index];
        }
        public bool AddPlay(string inplayName, string inproductionCompany, PRICE_RANGE inpriceRange, DateTime inplayTime) //Add a new play and save it to the PlayDictionary JAM
        {
            int playID = PlayDictionary.Count;
            string playIDString = playID.ToString();
            Play newPlay = new Play(playIDString, inplayName, inproductionCompany, inpriceRange, inplayTime);
            PlayDictionary.Add(playIDString, newPlay);
            return true;
        }
        public bool EditPlay(string inplayID, string inplayName, string inproductionCompany, PRICE_RANGE inpriceRange, DateTime inplayTime) //Edit an existing play and overwrite it in the PlayDictionary JAM
        {
            string playID = inplayID.ToString();
            Play overwritePlay = new Play(playID, inplayName, inproductionCompany, inpriceRange, inplayTime);
            PlayDictionary[playID] = overwritePlay;
            return true;
        }
        //Creates a booking(s) for a given customer ID and Play.
        public bool AddBooking(string incustomerID, string inPlayID, int Amount, List<string> seatLocations)
        {
            try
            {

                string CustomerId = incustomerID;
                string playID = inPlayID;
                for (int i = 0; i < Amount; i++)
                {
                    string TicketID = seatLocations[i];
                    string TicketNo = (PlayDictionary[playID].getTicketsSold() + 1).ToString();
                    PlayDictionary[playID].Addticket();
                    Tickets NewBooking = new Tickets(inPlayID, TicketID, CustomerId, PlayDictionary[playID].GetPrice(), DateTime.Now, TicketNo);

                    TicketsDictionary.Add(TicketID, NewBooking);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public List<Tickets> FetchBookings(string inPlayID)
        {
            List<Tickets> outBookings = new List<Tickets>();
            foreach (Tickets item in TicketsDictionary.Values)
            {
                if (item.PlayID == inPlayID)
                {
                    outBookings.Add(item);
                }
            }
            return outBookings;
        }

        /// <summary>
        /// Cancels a Play by replacing all data about a play except its date, Id and Cost with the term "CANCELED".
        /// Price and date are preserved for the perpous of customer records. 
        /// </summary>
        /// <param name="inplayID"></param> The ID of the Play to be canceled. 
        /// <returns></returns>
        public void CancelPlay(string inplayID)
        {

            string playID = inplayID.ToString();
            String playName = PlayDictionary[playID].GetName();
            DateTime InDate = PlayDictionary[playID].GetTime();
            PRICE_RANGE inpriceRange = PlayDictionary[playID].GetPrice();
            Play overwritePlay = new Play(playID, "CANCELED", "CANCELED", inpriceRange, InDate);
            PlayDictionary[playID] = overwritePlay;
            //sorry lads, the whole printing message stuff only happens in the MainWindow.xaml.cs file          JK
        }
        public int GetCustomerDictionaryLength()
        {
            int Length = CustomerDictionary.Count;
            return Length;
        }
        public Customer FetchCustomerByIndex(string index)
        {
            try
            {
                return CustomerDictionary[index];
            }
            catch(Exception)
            {
                return null;
            }
        }
        public int FetchBiggestCustomerIndex()
        {
            int currentBiggest = 0;
            foreach (string key in CustomerDictionary.Keys)
            {
                if (currentBiggest < int.Parse(key))
                {
                    currentBiggest = int.Parse(key);
                }
            }
            return currentBiggest;
        }
        public Customer FetchCustomerByName(string inName)
        {
            Customer foundCustomer = null;
            foreach(KeyValuePair<string, Customer> item in CustomerDictionary)
            {
                if (item.Value.GetName() == inName)
                {
                    foundCustomer = item.Value;
                }
            }
            return foundCustomer;
        }
        public Customer FetchCustomerByNumber(string inNumber)
        {
            Customer foundCustomer = null;
            foreach(KeyValuePair<string, Customer> item in CustomerDictionary)
            {
                if (item.Value.GetNumber() == inNumber)
                {
                    foundCustomer = item.Value;
                }
            }
            return foundCustomer;
        }
        public bool AddCustomer(string incustomerName, bool ingoldMember, string intelNo) //Add a new customer and save it to the CustomerDictionary JAM
        {
            string customerID = CustomerDictionary.Count.ToString();
            Customer newCustomer = new Customer(customerID, incustomerName, ingoldMember, intelNo);
            CustomerDictionary.Add(customerID, newCustomer);
            return true;
        }

        public bool AddGoldMember(string incustomerName, bool ingoldMember, string intelNo, string inaddress, DateTime inrenewalDate) //Add a new GoldMember and save it to the CustomerDictionary JAM
        {
            string customerID = CustomerDictionary.Count.ToString();
            GoldMember newGoldMember = new GoldMember(customerID, incustomerName, ingoldMember, intelNo, inaddress, inrenewalDate);
            CustomerDictionary.Add(customerID, newGoldMember);
            return true;
        }
        
        public List<GoldMember> FindGoldMembers()
        {
            List<GoldMember> memberList = new List<GoldMember>();
            foreach (Customer customer in CustomerDictionary.Values)
            {
                if (customer.IsGold())
                {
                    memberList.Add((GoldMember)customer);
                }
            }
            return memberList;
        }
        public List<GoldMember> FindExpiringMembers(int expiryMonth)
        {
            List<GoldMember> expiringList = new List<GoldMember>();
            foreach (Customer customer in CustomerDictionary.Values)
            {
                if (customer.IsGold())
                {
                    if (((GoldMember)customer).GetRenewalDate().Month == expiryMonth && ((GoldMember)customer).GetRenewalDate().Year == DateTime.Now.Year)
                    {
                        expiringList.Add((GoldMember)customer);
                    }
                }
            }
            return expiringList;

        }
        public bool RemoveCustomer(string inCustomerID)
        {
            try
            {
                foreach (Customer item in CustomerDictionary.Values)
                {
                    if (item.customerID == inCustomerID)
                    {
                        CustomerDictionary.Remove(inCustomerID);
                        break;
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }
        public int GetTicketsDictionaryLength()
        {
            int Length = TicketsDictionary.Count;
            return Length;
        }
        public List<Tickets> FetchTicketByCustomer(string inCustomerID)
        {
            List<Tickets> output = new List<Tickets>();
            foreach (Tickets ticket in TicketsDictionary.Values)
            {
                if (inCustomerID == ticket.customerID)
                {
                    output.Add(ticket);
                }
            }
            return output;
        }
        public List<Tickets> FetchTicketsByCustomerAndPlay(string inCustomerID, string inPlayID)
        {
            List<Tickets> output = new List<Tickets>();
            foreach (Tickets ticket in TicketsDictionary.Values)
            {
                if (inCustomerID == ticket.customerID && inPlayID == ticket.PlayID)
                {
                    output.Add(ticket);
                }
            }
            return output;
        }
        public Play FetchPlayByIndex(string inPlayID)
        {
            return PlayDictionary[inPlayID];
        }
        public List<Play> FetchPlayByName(string inPlayName)
        {
            List<Play> output = new List<Play>();
            foreach (Play performance in PlayDictionary.Values)
            {
                if (performance.GetName() == inPlayName)
                {
                    output.Add(performance);
                }
            }
            return output;
        }
        public bool CancelBookings(string inPlayID, string inCustomerID)
        {
            try
            {
                List<string> removalID = new List<string>();
                foreach (Tickets item in TicketsDictionary.Values)
                {
                    if (item.customerID == inCustomerID && item.PlayID == inPlayID)
                    {
                        removalID.Add(item.GetTicketID());
                    }
                }
                foreach (string ID in removalID)
                {
                    TicketsDictionary.Remove(ID);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool ConfirmBooking(string inPlayID, string inCustomerID)
        {
            try
            {
                foreach (Tickets item in TicketsDictionary.Values)
                {
                    if (item.customerID == inCustomerID && inPlayID == item.PlayID)
                    {
                        item.ConfirmTicket();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool RemoveExpiredReservation()
        {
            List<string> targetIndexes = new List<string>();
            try
            {
                foreach (Tickets item in TicketsDictionary.Values)
                {
                    if (item.GetOrderDate() > System.DateTime.Now)
                    {
                        targetIndexes.Add(item.GetTicketID());
                    }
                }
                foreach (string item in targetIndexes)
                {
                    TicketsDictionary.Remove(item);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public Play GetTodaysPlay(DateTime inTime)
        {
            foreach (Play item in PlayDictionary.Values)
            {
                if (inTime.DayOfYear == item.playTime.DayOfYear && inTime.Year == item.playTime.Year)
                {
                    return item;
                }
            }
            return null;
        }

        public List<Play> GetScheduleReport()
        {
            List<Play> outputSchedule = new List<Play>();
            for (int i = 0; i < GetPlayDictionaryLength(); i++)
            {
                if (PlayDictionary[i.ToString()].playTime > System.DateTime.Now.AddMonths(6))
                {
                    continue;
                }
                else
                {
                    outputSchedule.Add(PlayDictionary[i.ToString()]);
                }
            }
            return outputSchedule;
        }
        //public bool CheckSales()
        //{

        //}


    }
   public static class ValidationMethods
    {
        /// <summary>
        /// simple validation method that loops through a string and checks if any character has a numeric value
        /// </summary>
        /// <param name="input">the string to be validated</param>
        /// <returns></returns>
        public static bool IsNumeric(string input)
        {
            double result;
            foreach (char letter in input)
            {
                double.TryParse(letter.ToString(), out result);
                if (result != 0 || input == "0")
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// another simple validation method that checks if a date selected has already occured or not
        /// </summary>
        /// <param name="input">the date to be validated</param>
        /// <returns></returns>
        public static bool ValidDate(DateTime input)
        {
            if (input.Month != 5 && input.Month != 6 && input.Month !=7 && input.Month !=8 && input.Month != 9 && input.Month != 11 && input.Month != 12)
            {
                return false;
            }
            return true;
        }
        public static bool ValidPhoneNumber(string input)
        {
            //had to use double as int could not store 11 numerical characters JAM
            double parseResult;
            double.TryParse(input, out parseResult);
            if (parseResult != 0 && input.Length == 11)
            {
                return true;
            }
            return false;
        }
        public static bool HasInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input) == true || string.IsNullOrEmpty(input) == true)
            {
                return false;
            }
            return true;
        }
    }
}
