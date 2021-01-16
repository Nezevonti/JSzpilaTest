using NUnit.Framework;
using StarWarsApiCSharp;
using System;
using System.Collections.Generic;

namespace JSzpilaTest
{
    //https://swapi.dev/api/people/?search=r2
    class SwapiTest
    {

        ExtendedRepository<Person> PersonRepo = new ExtendedRepository<Person>();
        Repository<Planet> PlanetRepo = new Repository<Planet>();

        private int GetIDFromLink(string link)
        {
            string[] SplitLink = link.Split('/');
            return Int32.Parse(SplitLink[SplitLink.Length - 2]);
        }

        [TestCase("Luke Skywalker", "Tatooine")]
        //[TestCase("R2-D2", "Tatooine")] - not needed, commented out
        public void IsThisThisPersonsHomeworld(string person, string Homeworld)
        {
            List<Person> SearchResults = PersonRepo.SearchSwAPI("people", person);
            var PersonDetails = SearchResults[0];
            int HomePlanetId = GetIDFromLink(PersonDetails.Homeworld);

            Planet planet =  PlanetRepo.GetById(HomePlanetId);
            if (planet.Name == Homeworld)
            {
                Console.WriteLine("Correct\n");
                //Assert.Succ

            }
            else
            {
                Console.WriteLine("Not Correct\n");
                Assert.Fail("This is not this person homeworld!");
            }
            
        }

    }
}
