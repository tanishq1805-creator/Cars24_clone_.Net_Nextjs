using MongoDB.Driver;

public class CityService
{
    private readonly IMongoCollection<City> _cities;

    // Major cities across every Indian state and union territory. Coordinates are
    // included so the client can match a detected location to a supported city.
    private static readonly City[] DefaultCities =
    [
              C("Agartala", "Tripura", 23.8315, 91.2868),
        C("Agra", "Uttar Pradesh", 27.1767, 78.0081),
        C("Ahmedabad", "Gujarat", 23.0225, 72.5714),
        C("Aizawl", "Mizoram", 23.7271, 92.7176),
        C("Ajmer", "Rajasthan", 26.4499, 74.6399),
        C("Alappuzha", "Kerala", 9.4981, 76.3388),
        C("Aligarh", "Uttar Pradesh", 27.8974, 78.0880),
        C("Alwar", "Rajasthan", 27.5530, 76.6346),
        C("Ambala", "Haryana", 30.3782, 76.7767),
        C("Amravati", "Maharashtra", 20.9374, 77.7796),
        C("Amritsar", "Punjab", 31.6340, 74.8723),
        C("Anand", "Gujarat", 22.5645, 72.9289),
        C("Anantnag", "Jammu and Kashmir", 33.7311, 75.1487),
        C("Asansol", "West Bengal", 23.6739, 86.9524),
        C("Aurangabad", "Maharashtra", 19.8762, 75.3433),
        C("Balasore", "Odisha", 21.4942, 86.9317),
        C("Ballari", "Karnataka", 15.1394, 76.9214),
        C("Baramulla", "Jammu and Kashmir", 34.2090, 74.3429),
        C("Bardhaman", "West Bengal", 23.2324, 87.8615),
        C("Bareilly", "Uttar Pradesh", 28.3670, 79.4304),
        C("Bathinda", "Punjab", 30.2110, 74.9455),
        C("Belagavi", "Karnataka", 15.8497, 74.4977),
        C("Bengaluru", "Karnataka", 12.9716, 77.5946),
        C("Berhampur", "Odisha", 19.3149, 84.7941),
        C("Bhagalpur", "Bihar", 25.2425, 86.9842),
        C("Bhavnagar", "Gujarat", 21.7645, 72.1519),
        C("Bhilai", "Chhattisgarh", 21.1938, 81.3509),
        C("Bhilwara", "Rajasthan", 25.3407, 74.6313),
        C("Bhopal", "Madhya Pradesh", 23.2599, 77.4126),
        C("Bhubaneswar", "Odisha", 20.2961, 85.8245),
        C("Bikaner", "Rajasthan", 28.0229, 73.3119),
        C("Bilaspur", "Chhattisgarh", 22.0797, 82.1409),
        C("Bokaro Steel City", "Jharkhand", 23.6693, 86.1511),
        C("Chandigarh", "Chandigarh", 30.7333, 76.7794),
        C("Chennai", "Tamil Nadu", 13.0827, 80.2707),
        C("Coimbatore", "Tamil Nadu", 11.0168, 76.9558),
        C("Cuttack", "Odisha", 20.4625, 85.8830),
        C("Dadra and Nagar Haveli", "Dadra and Nagar Haveli and Daman and Diu", 20.2666, 73.0169),
        C("Daman", "Dadra and Nagar Haveli and Daman and Diu", 20.4283, 72.8397),
        C("Darbhanga", "Bihar", 26.1542, 85.8918),
        C("Davanagere", "Karnataka", 14.4644, 75.9218),
        C("Dehradun", "Uttarakhand", 30.3165, 78.0322),
        C("Delhi NCR", "Delhi", 28.6139, 77.2090),
        C("Deoghar", "Jharkhand", 24.4820, 86.6950),
        C("Dhanbad", "Jharkhand", 23.7957, 86.4304),
        C("Dharamshala", "Himachal Pradesh", 32.2190, 76.3234),
        C("Dibrugarh", "Assam", 27.4728, 94.9120),
        C("Dimapur", "Nagaland", 25.9042, 93.7266),
        C("Diu", "Dadra and Nagar Haveli and Daman and Diu", 20.7144, 70.9874),
        C("Durg", "Chhattisgarh", 21.1904, 81.2849),
        C("Durgapur", "West Bengal", 23.5204, 87.3119),
        C("Erode", "Tamil Nadu", 11.3410, 77.7172),
        C("Faridabad", "Haryana", 28.4089, 77.3178),
        C("Gandhinagar", "Gujarat", 23.2156, 72.6369),
        C("Gangtok", "Sikkim", 27.3389, 88.6065),
        C("Gaya", "Bihar", 24.7914, 85.0002),
        C("Ghaziabad", "Uttar Pradesh", 28.6692, 77.4538),
        C("Gorakhpur", "Uttar Pradesh", 26.7606, 83.3732),
        C("Guntur", "Andhra Pradesh", 16.3067, 80.4365),
        C("Gurugram", "Haryana", 28.4595, 77.0266),
        C("Guwahati", "Assam", 26.1445, 91.7362),
        C("Gwalior", "Madhya Pradesh", 26.2183, 78.1828),
        C("Haldwani", "Uttarakhand", 29.2183, 79.5130),
        C("Haridwar", "Uttarakhand", 29.9457, 78.1642),
        C("Hisar", "Haryana", 29.1492, 75.7217),
        C("Hoshiarpur", "Punjab", 31.5143, 75.9115),
        C("Howrah", "West Bengal", 22.5958, 88.2636),
        C("Hubballi", "Karnataka", 15.3647, 75.1240),
        C("Hyderabad", "Telangana", 17.3850, 78.4867),
        C("Imphal", "Manipur", 24.8170, 93.9368),
        C("Indore", "Madhya Pradesh", 22.7196, 75.8577),
        C("Itanagar", "Arunachal Pradesh", 27.0844, 93.6053),
        C("Jabalpur", "Madhya Pradesh", 23.1815, 79.9864),
        C("Jaipur", "Rajasthan", 26.9124, 75.7873),
        C("Jalandhar", "Punjab", 31.3260, 75.5762),
        C("Jammu", "Jammu and Kashmir", 32.7266, 74.8570),
        C("Jamnagar", "Gujarat", 22.4707, 70.0577),
        C("Jamshedpur", "Jharkhand", 22.8046, 86.2029),
        C("Jhansi", "Uttar Pradesh", 25.4484, 78.5685),
        C("Jodhpur", "Rajasthan", 26.2389, 73.0243),
        C("Jorhat", "Assam", 26.7509, 94.2037),
        C("Junagadh", "Gujarat", 21.5222, 70.4579),
        C("Kakinada", "Andhra Pradesh", 16.9891, 82.2475),
        C("Kalaburagi", "Karnataka", 17.3297, 76.8343),
        C("Kannur", "Kerala", 11.8745, 75.3704),
        C("Kanpur", "Uttar Pradesh", 26.4499, 80.3319),
        C("Karaikal", "Puducherry", 10.9254, 79.8380),
        C("Kargil", "Ladakh", 34.5539, 76.1349),
        C("Karimnagar", "Telangana", 18.4386, 79.1288),
        C("Karnal", "Haryana", 29.6857, 76.9905),
        C("Kavaratti", "Lakshadweep", 10.5667, 72.6417),
        C("Khammam", "Telangana", 17.2473, 80.1514),
        C("Kharagpur", "West Bengal", 22.3460, 87.2319),
        C("Kochi", "Kerala", 9.9312, 76.2673),
        C("Kohima", "Nagaland", 25.6751, 94.1086),
        C("Kolhapur", "Maharashtra", 16.7050, 74.2433),
        C("Kolkata", "West Bengal", 22.5726, 88.3639),
        C("Kollam", "Kerala", 8.8932, 76.6141),
        C("Korba", "Chhattisgarh", 22.3595, 82.7501),
        C("Kota", "Rajasthan", 25.2138, 75.8648),
        C("Kottayam", "Kerala", 9.5916, 76.5222),
        C("Kozhikode", "Kerala", 11.2588, 75.7804),
        C("Kurnool", "Andhra Pradesh", 15.8281, 78.0373),
        C("Leh", "Ladakh", 34.1526, 77.5771),
        C("Lucknow", "Uttar Pradesh", 26.8467, 80.9462),
        C("Ludhiana", "Punjab", 30.9010, 75.8573),
        C("Lunglei", "Mizoram", 22.8671, 92.7654),
        C("Madurai", "Tamil Nadu", 9.9252, 78.1198),
        C("Malda", "West Bengal", 25.0108, 88.1411),
        C("Mandi", "Himachal Pradesh", 31.7089, 76.9326),
        C("Mangaluru", "Karnataka", 12.9141, 74.8560),
        C("Margao", "Goa", 15.2832, 73.9862),
        C("Mathura", "Uttar Pradesh", 27.4924, 77.6737),
        C("Meerut", "Uttar Pradesh", 28.9845, 77.7064),
        C("Mohali", "Punjab", 30.7046, 76.7179),
        C("Moradabad", "Uttar Pradesh", 28.8386, 78.7733),
        C("Mumbai", "Maharashtra", 19.0760, 72.8777),
        C("Muzaffarpur", "Bihar", 26.1209, 85.3647),
        C("Mysuru", "Karnataka", 12.2958, 76.6394),
        C("Nagaon", "Assam", 26.3480, 92.6838),
        C("Nagpur", "Maharashtra", 21.1458, 79.0882),
        C("Naharlagun", "Arunachal Pradesh", 27.1047, 93.6950),
        C("Namchi", "Sikkim", 27.1647, 88.3638),
        C("Nashik", "Maharashtra", 19.9975, 73.7898),
        C("Navi Mumbai", "Maharashtra", 19.0330, 73.0297),
        C("Nellore", "Andhra Pradesh", 14.4426, 79.9865),
        C("Nizamabad", "Telangana", 18.6725, 78.0941),
        C("Noida", "Uttar Pradesh", 28.5355, 77.3910),
        C("Panaji", "Goa", 15.4909, 73.8278),
        C("Panipat", "Haryana", 29.3909, 76.9635),
        C("Patiala", "Punjab", 30.3398, 76.3869),
        C("Patna", "Bihar", 25.5941, 85.1376),
        C("Port Blair", "Andaman and Nicobar Islands", 11.6234, 92.7265),
        C("Prayagraj", "Uttar Pradesh", 25.4358, 81.8463),
        C("Puducherry", "Puducherry", 11.9416, 79.8083),
        C("Pune", "Maharashtra", 18.5204, 73.8567),
        C("Puri", "Odisha", 19.8135, 85.8312),
        C("Purnia", "Bihar", 25.7771, 87.4753),
        C("Raipur", "Chhattisgarh", 21.2514, 81.6296),
        C("Rajahmundry", "Andhra Pradesh", 17.0005, 81.8040),
        C("Rajkot", "Gujarat", 22.3039, 70.8022),
        C("Ramagundam", "Telangana", 18.7550, 79.4740),
        C("Ranchi", "Jharkhand", 23.3441, 85.3096),
        C("Rewa", "Madhya Pradesh", 24.5362, 81.3037),
        C("Rohtak", "Haryana", 28.8955, 76.6066),
        C("Roorkee", "Uttarakhand", 29.8543, 77.8880),
        C("Rourkela", "Odisha", 22.2604, 84.8536),
        C("Rudrapur", "Uttarakhand", 28.9874, 79.4141),
        C("Sagar", "Madhya Pradesh", 23.8388, 78.7378),
        C("Salem", "Tamil Nadu", 11.6643, 78.1460),
        C("Sambalpur", "Odisha", 21.4669, 83.9812),
        C("Satna", "Madhya Pradesh", 24.6005, 80.8322),
        C("Shillong", "Meghalaya", 25.5788, 91.8933),
        C("Shimla", "Himachal Pradesh", 31.1048, 77.1734),
        C("Shivamogga", "Karnataka", 13.9299, 75.5681),
        C("Sikar", "Rajasthan", 27.6094, 75.1399),
        C("Silchar", "Assam", 24.8333, 92.7789),
        C("Siliguri", "West Bengal", 26.7271, 88.3953),
        C("Solan", "Himachal Pradesh", 30.9045, 77.0967),
        C("Solapur", "Maharashtra", 17.6599, 75.9064),
        C("Sonipat", "Haryana", 28.9931, 77.0151),
        C("Srinagar", "Jammu and Kashmir", 34.0837, 74.7973),
        C("Surat", "Gujarat", 21.1702, 72.8311),
        C("Tezpur", "Assam", 26.6528, 92.7926),
        C("Thane", "Maharashtra", 19.2183, 72.9781),
        C("Thiruvananthapuram", "Kerala", 8.5241, 76.9366),
        C("Thoothukudi", "Tamil Nadu", 8.7642, 78.1348),
        C("Thoubal", "Manipur", 24.6380, 93.9964),
        C("Thrissur", "Kerala", 10.5276, 76.2144),
        C("Tiruchirappalli", "Tamil Nadu", 10.7905, 78.7047),
        C("Tirunelveli", "Tamil Nadu", 8.7139, 77.7567),
        C("Tirupati", "Andhra Pradesh", 13.6288, 79.4192),
        C("Tura", "Meghalaya", 25.5140, 90.2020),
        C("Udaipur", "Rajasthan", 24.5854, 73.7125),
        C("Udaipur", "Tripura", 23.5330, 91.4845),
        C("Ujjain", "Madhya Pradesh", 23.1765, 75.7885),
        C("Vadodara", "Gujarat", 22.3072, 73.1812),
        C("Varanasi", "Uttar Pradesh", 25.3176, 82.9739),
        C("Vasco da Gama", "Goa", 15.3959, 73.8156),
        C("Vellore", "Tamil Nadu", 12.9165, 79.1325),
        C("Vijayawada", "Andhra Pradesh", 16.5062, 80.6480),
        C("Visakhapatnam", "Andhra Pradesh", 17.6868, 83.2185),
        C("Warangal", "Telangana", 17.9689, 79.5941),

    ];

    private static City C(string name, string state, double latitude, double longitude) =>
        new() { Name = name, State = state, Latitude = latitude, Longitude = longitude };

    public CityService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Cars24DB"));
        var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _cities = database.GetCollection<City>("Cities");
    }

    public async Task<List<City>> GetAllAsync()
    {
        var storedCities = await _cities.Find(_ => true).ToListAsync();
        var cityKeys = new HashSet<string>(
            storedCities.Select(city => CityKey(city.Name, city.State)),
            StringComparer.OrdinalIgnoreCase);
        var missingCities = DefaultCities
            .Where(city => !cityKeys.Contains(CityKey(city.Name, city.State)))
            .ToList();

        if (missingCities.Count > 0)
        {
            await _cities.InsertManyAsync(missingCities);
        }

        return await _cities
            .Find(_ => true)
            .SortBy(city => city.State)
            .ThenBy(city => city.Name)
            .ToListAsync();
    }

    private static string CityKey(string name, string state) => $"{name}\u001f{state}";
}
