using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;



namespace Question1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* The min of the Next method is inclusive, but the max is not so add 1 to the range of values 
               to ensure 2000 is included. The namelength and listlength sizes were arbitrarily chosen to complete the problem. */

            int namelength = r.Next(5, 11);
            int listlength = r.Next(100, 1001);
            string filename = "C:\\Temp\\People.Txt";
            NameGenerator namer = new NameGenerator();
            List<Person> people = new List<Person>();
            //StringBuilder fileoutput = new StringBuilder();
            using (FileStream foutstream = new FileStream(filename, FileMode.OpenOrCreate))
            using (StreamWriter filewriter = new StreamWriter(foutstream))
            {
                filewriter.WriteLine("Name,Birth_Year,End_Year");
            }
                //fileoutput.AppendLine("Name,Birth_Year,End_Year");
            // Generate a list of people to use and write their string op

            Console.WriteLine("Creating list of people for question...");
            for (int i = 0; i < listlength; i++)
            {
                string pname = namer.Generate(r,namelength);
                int pbirthyear = r.Next(1900, 2001);
                int pendyear = r.Next(pbirthyear, 2001);
                Person p = new Person(pname,pbirthyear,pendyear);
                people.Add(p);
                //fileoutput.AppendLine(p.name.ToString() + "," + p.birthyear.ToString() + "," + p.endyear.ToString());
            }

            using (FileStream foutstream = new FileStream(filename, FileMode.Append))
            using (StreamWriter filewriter = new StreamWriter(foutstream))
            {
                foreach(var person in people)
                {
                    filewriter.WriteLine(person.name + "," + person.birthyear.ToString() + "," + person.endyear.ToString());
                }
            }

                /*Write people list to file to have data for validation later.
                  Basic idea for stream builder and writing the buffer to a file found at various places online */

                /*The solution to the problem starts here. My original solution was to
                 * 1. Create an array of integers of length 101 all starting at 0
                 * 2. Iterate through the list of people
                 *    a. Translate their birthyear to an insertion point into the array
                 *    b. Step through the array adding 1 to each index value the person is alive, excluding end year
                 *    c. Returning the index of the max value of the array, adjusting for the years.
                 *
                 * As I considered my solution again I thought it would be very slow because we iterate through the
                 * years array for the age of the person for as many people there are in the list.
                 * 
                 * That means in the wost case, if our interval of years is y and our number of people is n we'd
                 * end up taking around O(m*n) time.
                 * 
                 * My next thought was is that if we laid the years out on the number line and represented the life of person
                 * as different lines that run parallel to that line, we're looking for the integer that overlaps with the most
                 * number of lines. Another way I thought about it was considering the birthyear as the beginning of an interval
                 * and the deathyear as the end and the year the lies within the most intervals is the year that has the most
                 * people alive. This lead me to the efficient solution online found at the URL below:
                 * http://www.geeksforgeeks.org/find-the-point-where-maximum-intervals-overlap/
                 * 
                 * Implements part of a merge sort which runs in O(nlgn)
                 */
                Console.WriteLine("Determining year with the most people alive...");
            int[] birthyears = people.Select(x => x.birthyear).ToArray();
            int[] endyears = people.Select(x => x.endyear).ToArray();
            Array.Sort(birthyears);
            Array.Sort(endyears);

            // Year should represent the years people are alive.
            int people_alive = 1;
            int people_max = 1;
            var maxyear = birthyears.GetValue(0);
            int byi = 1;
            int eyi = 0;
            
            while (byi < birthyears.Length && eyi < endyears.Length)
            {
                if(birthyears[byi] <= endyears[eyi])
                {
                    people_alive++;
                    
                    if (people_alive > people_max)
                    {
                        people_max = people_alive;
                        maxyear = birthyears[byi];
                    }
                    byi++;
                }
                else
                {
                    people_alive--;
                    eyi++;
                }
            }
            Console.WriteLine("The maximum number of people alive is " + people_max + " in the year " + maxyear + ".");
            Console.Read();

        }

        /* Per best practice create single instance of random number for generating variable length lists of people
          as well as generating Names, birthYears, and endYears.*/

        public static Random r = new Random();

        // Create a simple class for persons that contain the interesting properties.

        public class Person
        {
            /* Choose to make Person read only as data should be set at instatiation.
               Since this is a simple class for the question only I chose not to do
               any data validation on the inputs as I will ensure the inputs lie
               within the range 1900 - 2000 and I am not concerned about the names being random strings.*/
            public string name { get; private set; }
            public int birthyear {get; private set;}
            public int endyear { get; private set; }

            public Person(string name, int birthyear, int endyear)
            {
                this.name = name;
                this.birthyear = birthyear;
                this.endyear = endyear;
            }

            ~Person() { }
        }
        /* Basic idea for name generator here
           https://social.msdn.microsoft.com/Forums/vstudio/en-US/5a46c6eb-3c45-4442-9da4-958bed0ba788/generating-a-random-string-of-letters?forum=csharpgeneral */

        public class NameGenerator
        {
            public const string _alphabet = "abcdefghijklmnopqrstuvwxyz";

            public NameGenerator() {}

            public string Generate(Random random, int namelength = 1)
            {
                if (namelength < 1)
                    throw new ArgumentException("Name length must be a positive integer");
                StringBuilder strbuffer = new StringBuilder(namelength);

                while (namelength > 0)
                {
                    int randomnum = random.Next(0, _alphabet.Length);
                    strbuffer.Append(_alphabet, randomnum, 1);
                    namelength--;
                }
                return char.ToUpper(strbuffer.ToString()[0]) + strbuffer.ToString().Substring(1);
                
            }
        }
    }
}
