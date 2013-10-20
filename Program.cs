/*
 * Traveling salesperson problem
 * 
 *   The assignment:
 Generere en graf som består av 100 byer ( n = 100 ). Hver by har en kant med alle byene.
 Hvis n = 100 , hver by har 99 kanter. Avstanden mellom byene skal være et tall mellom 1
 og 5.
 . Velg 2 algoritmer som kan gi en initielle løsning. Foreksempel ( random , greedy ).
 . Prøv å forbedre start løsning med en forbedring metode.
 ( greedy metoden for eksempel )
 . Test programmet med n= 500, 1000 , 2000.
 . Hva er konklusjonen ?
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace oblig1
{
    class Program
    {
        /*
         * Instillinger:
         * */
        const int strMat=10;  // størrelse på matrisen, nXn: strMat = 10 gir new int[10,10]
        const int minCost = 1; // minimum kostnad mellom byene
        const int maxCost = 6; // maksimum konstad mellom byene, n-1: maxCost=6 gir 5 som maksimum kostnad 
        const int visitedCityValue = -1; // verdien/kosten til en besøkt by

        const int minsOfImprovmets = 2; // run improvment loop for X minutes

        const bool printMatrix = true;  // printe matrisen?      
        const bool printRoute = true;   // printe rute?                        
        const bool printDebug = false;  // printe debugmeldinger? eks: "Fant vei mellom A og C, kost: 5"
        const int debugLevel = 2; /* 1 = få informative mld (hvor i koden programmet kjører), 
                                   * 2 = utprint av ruter, startCity
                                   * 3 = full debug (meldinger i while loops)
                                   * eks: 
                                        doPrintDebug("Generating variables, matrix size: "+strMat+"x"+strMat+"..", 1);
                                   * uten level = full debug:
                                        doPrintDebug("Generating variables, matrix size: "+strMat+"x"+strMat+"..");
                                   * */
        
	    const string timeFormat = "yy.MM.yy HH:mm:ss";    // Use this format
        

        static void Main(string[] args)
       {
           doPrintDebug("Generating variables, matrix size: "+strMat+"x"+strMat+"..",1);
            /*
             * Alle deklarerte Variabler:
             * */
            DateTime time = DateTime.Now;                           // Use current time
            int[,] Cities = new int[strMat,strMat];                 // City matrix
            // Rutevariabler:
            int[] VisitedCitiesRandom = new int[strMat];             // Random rute | Oversikt over besøkte byer
            int[] VisitedCitiesGreedy = new int[strMat];             // Greedy rute
            int[] VisitedCitiesRandomImproved = new int[strMat];     // Forbedret random rute
            int[] VisitedCitiesGreedyImproved = new int[strMat];     // Forbedret greedy rute
            int totalCostRandom = 0;                                 // Totalkost
            int totalCostGreedy = 0;
            int totalCostRandomImproved = 0;                        // Totalkost
            int totalCostGreedyImproved = 0;    

            // Randomness:
            Random rndCost = new Random();
            Random rndCity = new Random();

            // Variabler til gjenbruk:
            int numberOfVisitedCities = 0;                           // Hittil besøkte byer
            int startCity = 0;                                       // Startbyen
            int nextCity = 0;                                        // Next random city..
            int lastCity = 0;                                        // Last city
            int tmpCost = 0;
            int tmpCostLowest = maxCost + 1;
            int isVisited = 0;
            
            /*
             Flylle array:
             */
            for (int j = 0; j < strMat; j++)
            {
                for (int i = j+1; i < strMat; i++)
                {
                    int tmp = rndCost.Next(minCost, maxCost);
                    Cities[j, i] = tmp;
                    Cities[i, j] = tmp;
                    if (j == i) { Cities[j, i] = 0; }
                }
            }

            /* 
             * Printe array:
             * */
            doPrintMatrix(Cities);


            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             *  Random route 
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            //doPrintDebug("");
            doPrintDebug("Generating Random route..",1); //print ut

            startCity = getRandomCity(lastCity, rndCity); // sett startby
            doPrintDebug("Start city: " + startCity,2); 

            lastCity = startCity; // siste byen vi besøkte var statbyen              
            
            while ((numberOfVisitedCities < strMat) && (Array.IndexOf(VisitedCitiesRandom, 0) != -1)) // while all cities not visited
            {
                // numberOfVisitedCities er en teller, starter på 0, slutter på strMat-1
                VisitedCitiesRandom[numberOfVisitedCities] = lastCity; // VisitedCitiesRandom[0]=lastCity
                nextCity = notVisitedRandomCity(VisitedCitiesRandom, startCity, strMat, rndCity);  // Sett neste random by som vi ikke har besøkt

                //int tmpCity = Array.IndexOf(VisitedCitiesRandom, nextCity);
                //   

                tmpCost = Cities[lastCity-1, nextCity-1];
                totalCostRandom += tmpCost;
                doPrintDebug("Walk from " + lastCity + " to " + nextCity + ". Cost: " + tmpCost + " [" + (lastCity - 1) + "," + (nextCity - 1) + "]",3); 
                lastCity = nextCity;
                numberOfVisitedCities++;
            }
            doPrintDebug("Generating Random route.. DONE", 1); //print ut
            
            // Gå hjem til startbyen:
           // tmpCost = Cities[lastCity - 1, startCity-1];
           // totalCostRandom += tmpCost;
           // doPrintDebug("Walk from home " + lastCity + " to " + startCity + ". Cost: " + tmpCost + " [" + (lastCity-1) + "," + (startCity-1) + "]");
            // Print total cost:
            doPrintDebug("Total cost: " + totalCostRandom,2);
            //Console.ReadKey();                                      // "pause"

            
           // Console.ReadLine();                                      // "pause"
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             *  Greedy route
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            
            doPrintDebug("Generating Greedy route..",1);
            // reset variables:
            numberOfVisitedCities = 0;
            startCity = 0;
            lastCity = 0;
            nextCity = 0;
            tmpCost = 0;

            startCity = getRandomCity(0, rndCity);           // sett startby
            lastCity = startCity;
            doPrintDebug("Start city: " + startCity,2);

            while (numberOfVisitedCities < strMat)               // while all cities not visited
            {
                VisitedCitiesGreedy[numberOfVisitedCities] = lastCity;
                tmpCostLowest = maxCost;
                //nextCity = notVisitedRandomCity(VisitedCitiesRandom, startCity, strMat, rndCity);  // Sett neste random by som vi ikke har besøkt
                for (int i = 0; i < strMat; i++)
                {
                    isVisited = Array.IndexOf(VisitedCitiesGreedy, (i + 1));
                    
                    if ((isVisited == -1) && ((lastCity-1) != i)) {
                    tmpCost = Cities[lastCity-1, i];
                    doPrintDebug("    EVALUATING: from " + lastCity + " to " + (i+1) +".. Lc: " + tmpCostLowest + " Cost: "+tmpCost,3);
                    if ((tmpCost < tmpCostLowest) && (tmpCost != 0))
                    {
                        
                            doPrintDebug("          LOWER COST: from " + lastCity + " to " + (i + 1) + ". Cost: " + tmpCost,3);
                            tmpCostLowest = tmpCost;
                            nextCity = i+1;
                        }
                    }
                }

                if (tmpCostLowest != maxCost)
                {
                    totalCostGreedy += tmpCostLowest;
                    doPrintDebug("Walk from " + lastCity + " to " + nextCity + ". Cost: " + tmpCostLowest + " [" + (lastCity - 1) + "," + (nextCity - 1) + "]",3);
                }
                lastCity = nextCity;
                numberOfVisitedCities++;
            }

            // Gå hjem til startbyen:
            tmpCost = Cities[lastCity - 1, startCity - 1];
            totalCostGreedy += tmpCost;
            doPrintDebug("Walk home from " + lastCity + " to " + startCity + ". Cost: " + tmpCost + " [" + (lastCity - 1) + "," + (startCity - 1) + "]",3);
            doPrintDebug("Generating Greedy route.. DONE", 1);
            // Print total cost:
            doPrintDebug("Total cost: " + totalCostGreedy,2);
            Console.WriteLine();
            

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             *  IMPROVE ROUTE
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

            doPrintDebug("IMROVE ROUTE: Initializing variables and matrixes..", 1);
            doPrintDebug("Try improving for (" + minsOfImprovmets + ") minutes", 1);
                Stopwatch s = new Stopwatch();
                int[] RouteToImprove = new int[strMat];
                int cityOneNr = getRandomCity(0, rndCity); // city number one = random number between 0 and strMat-1
                int cityTwoNr = getRandomCity(0, rndCity);
                int cityOne = VisitedCitiesRandom[cityOneNr - 1];    // get the city
                int cityTwo = VisitedCitiesRandom[cityTwoNr - 1];
                int totalRouteToImproveCost = totalCostRandom + 1; 
                totalCostRandomImproved = totalCostRandom;
                int tmp12 = 1;
                int routeToImproveCost;
                //long foreverLoopMeNot = 0;

               
            /*
             * IMPROVE RANDOM ROUTE:
             * */
                // Ta inn ruten:
                    Array.Copy(VisitedCitiesRandom, RouteToImprove, strMat);

                    doPrintDebug("IMROVING ROUTE: Random route..", 1);
                    
                    s.Start();
                    while (s.Elapsed < TimeSpan.FromSeconds(60 * minsOfImprovmets))
                    //while (totalCostRandomImproved <= totalRouteToImproveCost) // gammel løsning før timerløsning
                    {
                        // switch places:
                         cityOneNr = getRandomCity(0, rndCity); // city number one = random number between 0 and strMat-1
                         cityTwoNr = getRandomCity(0, rndCity);
                         cityOne = RouteToImprove[cityOneNr - 1];    // get the city
                         cityTwo = RouteToImprove[cityTwoNr - 1];
                         RouteToImprove[cityOneNr - 1] = cityTwo;
                         RouteToImprove[cityTwoNr - 1] = cityOne;

                        // calculate new cost:
                        routeToImproveCost = 0;
                        foreach (int i in RouteToImprove)
                        {
                            if (tmp12 == strMat) { tmp12 = 0; }
                            routeToImproveCost += Cities[(i - 1), (RouteToImprove[tmp12] - 1)];
                            tmp12++;
                        }
                        totalRouteToImproveCost = routeToImproveCost;
                        doPrintDebug(" totalCostRandom - totalRouteToImproveCost " + totalCostRandomImproved + " - " + totalRouteToImproveCost,3); 
                        if (totalCostRandomImproved > totalRouteToImproveCost)
                        {
                            Array.Copy(RouteToImprove, VisitedCitiesRandomImproved, strMat);
                            totalCostRandomImproved = totalRouteToImproveCost; totalRouteToImproveCost++;
                        }
                        /*foreverLoopMeNot++; // gammel løsning før timerløsning
                         if (foreverLoopMeNot >= (strMat * strMat)) // gammel løsning før timerløsning
                         {
                             doPrintDebug("IMROVING ROUTING: Random route.. STOPPED", 1);
                             break;

                         }
                         */
                    }
                    s.Stop();
                    doPrintDebug("STOPPET Improving Random route after " + minsOfImprovmets + " minutes", 1);
            /*
             * IMPROVE GREEDY ROUTE:
             * */
                    // reset vars
                    doPrintDebug("IMROVE ROUTE: resetting variables..", 1);
                    Array.Copy(VisitedCitiesGreedy, RouteToImprove, strMat);

                    s.Reset(); // resett timer
                    totalRouteToImproveCost = totalCostGreedy + 1; // gammel løsning før timerløsning
                    totalCostGreedyImproved = totalCostGreedy;
                    tmp12 = 1; //spagetthi-code
                    //foreverLoopMeNot = 0;

                    doPrintDebug("IMROVING ROUTE: Greedy route..", 1);
                    s.Start(); // start timer
                    while (s.Elapsed < TimeSpan.FromSeconds(60 * minsOfImprovmets))
                    //while (totalCostGreedyImproved <= totalRouteToImproveCost) // gammel løsning før timerløsning
                    {
                        // switch places:
                        cityOneNr = getRandomCity(0, rndCity); // city number one = random number between 0 and strMat-1
                        cityTwoNr = getRandomCity(0, rndCity);
                        cityOne = RouteToImprove[cityOneNr - 1];    // get the city
                        cityTwo = RouteToImprove[cityTwoNr - 1];
                        RouteToImprove[cityOneNr - 1] = cityTwo;
                        RouteToImprove[cityTwoNr - 1] = cityOne;

                        // calculate new cost:
                        routeToImproveCost = 0;
                        foreach (int i in RouteToImprove)
                        {
                            if (tmp12 == strMat) { tmp12 = 0; }
                            routeToImproveCost += Cities[(i - 1), (RouteToImprove[tmp12] - 1)];
                            tmp12++;
                        }
                        totalRouteToImproveCost = routeToImproveCost;
                        doPrintDebug(" totalCostGreedy - totalRouteToImproveCost " + totalCostGreedyImproved + " - " + totalRouteToImproveCost,3);
                        if (totalCostGreedyImproved > totalRouteToImproveCost)
                        {
                            Array.Copy(RouteToImprove, VisitedCitiesGreedyImproved, strMat);
                            totalCostGreedyImproved = totalRouteToImproveCost; totalRouteToImproveCost++;
                        }
                        /*
                        foreverLoopMeNot++;
                        if (foreverLoopMeNot >= (strMat * strMat))
                        {
                            doPrintDebug("IMROVING ROUTE: Greedy route.. STOPPED", 1);
                            break;

                        }
                        */
                    }
                    s.Stop();
                    doPrintDebug("STOPPED Improving Greedy route after " + minsOfImprovmets + " minutes", 1);

                /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
                 *  Print route 
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                Console.WriteLine();
                Console.Write("Random route"); Console.WriteLine();
                doPrintRoute(VisitedCitiesRandom);
                Console.Write(" Total cost: " + totalCostRandom); Console.WriteLine();

                Console.WriteLine(); Console.Write("Improved Random route"); Console.WriteLine();
                doPrintRoute(VisitedCitiesRandomImproved);
                if (totalCostRandomImproved < totalCostRandom)
                {
                    Console.Write(" Total cost: " + totalCostRandomImproved); Console.WriteLine();
                }
                else
                {
                    Console.Write(" Total cost: " + totalCostRandomImproved + ". Not improved"); Console.WriteLine();
                }

                    Console.WriteLine(); Console.Write("Greedy route"); Console.WriteLine();
                doPrintRoute(VisitedCitiesGreedy);
                Console.Write(" Total cost: "+ totalCostGreedy);Console.WriteLine();

                Console.WriteLine(); Console.Write("Improved Greedy route "); Console.WriteLine();
                doPrintRoute(VisitedCitiesGreedyImproved);
                if (totalCostGreedyImproved < totalCostGreedy)
                {
                    Console.Write(" Total cost: " + totalCostGreedyImproved); Console.WriteLine();
                }
                else
                {
                    Console.Write(" Total cost: " + totalCostGreedyImproved + ". Not improved"); Console.WriteLine();
                }

            if (printRoute)
            {
                Console.Write("-----------------------------------------------------------"); Console.WriteLine();
                Console.Write("Random Total cost: " + totalCostRandom); Console.WriteLine();
                Console.Write("Random Improved Total cost: " + totalCostRandomImproved); Console.WriteLine();
                Console.Write("Greedy Total cost: " + totalCostGreedy); Console.WriteLine();
                Console.Write("Greedy Improved Total cost: " + totalCostGreedyImproved); Console.WriteLine();
            }
            
            Console.ReadLine();                                      // "pause"
             
        }
        
        /**/
        // Note to self: keep forgetting that strMat is global..
        public static int getRandomCity(int thisCity, Random rndCity)
        {
            int tmp = thisCity;
            while (tmp == thisCity)
            {
                tmp = rndCity.Next(1, strMat+1);
            }
            return tmp;
        }
    


        public static int notVisitedRandomCity(int[] VisitedCities,int startCity, int strMat, Random rndCity)
        {
            int nextCity = startCity; // VisitedCities= [1,2,3,4]
            int tmp = Array.IndexOf(VisitedCities,nextCity); // returns -1 if not found
            // tmp == -1
            if (Array.IndexOf(VisitedCities, 0) == -1)
                return startCity;
            while (tmp != -1)
            {
                nextCity = getRandomCity(nextCity, rndCity);
                tmp = Array.IndexOf(VisitedCities, nextCity);
               //mp1 = Array.IndexOf(VisitedCities, 0);
            }
            return nextCity;
        }
        public static void doPrintDebug(string stringToPrint)
        {
            if (printDebug)
            {
                DateTime time = DateTime.Now;
                Console.Write(time.ToString(timeFormat) + ": " + stringToPrint); Console.WriteLine();
            }
        }
        public static void doPrintDebug(string stringToPrint, int level)
        {
            if ((printDebug) && ((int)debugLevel >= level)) {
                DateTime time = DateTime.Now;
                Console.Write(time.ToString(timeFormat) + ": " + stringToPrint); Console.WriteLine(); 
            }
        }
        /* 
         * Printe array:
         * */
        public static void doPrintMatrix(int[,] Cities)
        {
            if (printMatrix)
            {
                Console.WriteLine();
                for (int w = 0; w < strMat; w++)
                {
                    for (int i = 0; i < strMat; i++)
                    {
                        Console.Write(Cities[w, i] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
        public static void doPrintRoute(int[] VisitedCities)
        {
            if (printRoute)
            {
                for (int i = 0; i < strMat; i++)
                {
                    Console.Write(VisitedCities[i] + "-");
                }
                Console.Write(VisitedCities[0]);
                Console.WriteLine();
            }
        }
        }
    }
    

