using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarWarsApiCSharp;

namespace JSzpilaTest
{
    public class EntityType
    {
        public const string
            People = "people",
            Films = "films",
            Starships = "starships",
            Vehicles = "vehicles",
            Species = "species",
            Planets = "planets";
    }

    internal class Helper<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets or sets the results downloaded from data service.
        /// </summary>
        /// <value>The results.</value>
        [JsonProperty]
        public ICollection<T> Results { get; set; }

        /// <summary>
        /// Gets or sets the count of the results.
        /// </summary>
        /// <value>The count of the results.</value>
        [JsonProperty]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the next page.
        /// </summary>
        /// <value>The next page.</value>
        [JsonProperty]
        public string Next { get; set; }

        /// <summary>
        /// Gets or sets the previous page.
        /// </summary>
        /// <value>The previous page.</value>
        [JsonProperty]
        public string Previous { get; set; }
    }



    public class ExtendedRepository<T> : Repository<T> where T : BaseEntity
    {
        /// <summary>
        /// The default API URL from where entities are downloaded.
        /// </summary>
        private const string Api = "https://swapi.dev/api/";

        /// <summary>
        /// The default page.
        /// </summary>
        private const int DefaultPage = 1;

        /// <summary>
        /// The default size of entities.
        /// </summary>
        private const int DefaultSize = 10;

        /// <summary>
        /// The URL end character. By default is "/" slash.
        /// </summary>
        private string urlEndCharacter = "/";

        /// <summary>
        /// The URL data that will be used in data service.
        /// </summary>
        private string urlData;

        /// <summary>
        /// The data service for entities.
        /// </summary>
        private IDataService dataService;

        /// <summary>
        /// The base entity.
        /// <seealso cref="StarWarsApiCSharp.BaseEntity" />
        /// </summary>
        private T entity;

        private bool CheckType(string type)
        {
            if (type.Equals("people")) return true;
            if (type.Equals("films")) return true;
            if (type.Equals("starships")) return true;
            if (type.Equals("species")) return true;
            if (type.Equals("planets")) return true;
            //if (!type.Equals("starships")) return false;

            return false;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}" /> class.
        /// Uses the default data service and URL for gather data.
        /// </summary>
        public ExtendedRepository()
            : this(new DefaultDataService(new WebHelper()), Api)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class. Uses a default URL for gather data.
        /// </summary>
        /// <param name="dataService">The data service to get entities.</param>
        /// <example>Data service getting data from JSON document, other database etc.</example>
        public ExtendedRepository(IDataService dataService)
            : this(dataService, Api)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="dataService">The data service to get entities.</param>
        /// <param name="url">The URL for consuming. It will be used in the service. Examples: http://mySite.com, http://mySite.com/ .</param>
        /// <example>Data service getting data from JSON document, other database etc.</example>
        public ExtendedRepository(IDataService dataService, string url)
        {
            this.entity = HelperInitializer<T>.Instance();
            this.dataService = dataService;

            if (!url.EndsWith(this.urlEndCharacter))
            {
                url += this.urlEndCharacter;
            }

            this.urlData = url;
        }

        /// <summary>
        /// Gets the base path for consuming entities.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get
            {
                return this.urlData;
            }
        }

        /// <summary>
        /// Gets the entity by it's identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns><see cref="StarWarsApiCSharp.IRepository{T}" /></returns>
        public T GetById(int id)
        {
            string url = $"{this.urlData}{this.entity.GetPath()}{id}";
            string jsonResponse = this.dataService.GetDataResult(url);
            if (jsonResponse == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        /// <summary>
        /// Gets entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="size">The size of the entities.</param>
        /// <returns>ICollection&lt; <see cref="StarWarsApiCSharp.IRepository{T}" /> &gt;.</returns>
        public List<T> SearchSwAPI(string type, string phrase)
        {
            if (!CheckType(type)) return null;
            string url = "https://swapi.dev/api/" + type + "/?search=" + phrase;
            string jsonResponse = "";

            var helper = new Helper<T>()
            {
                Next = url,
                Previous = null,
            };

            IEnumerable<T> results = new List<T>();

            jsonResponse = this.dataService.GetDataResult(url);
            if (jsonResponse == null)
            {
                return null;
            }

            //results = JsonConvert.DeserializeObject<ICollection<T>>(jsonResponse);
            helper = JsonConvert.DeserializeObject<Helper<T>>(jsonResponse);
            results = helper.Results;

            return results.ToList();
        }

    }
}
