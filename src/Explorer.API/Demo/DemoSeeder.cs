using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.API.Demo
{
    public class DemoSeeder
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEquipmentService _equipmentService;
        private readonly IFacilityService _facilityService;
        private readonly ITourService _tourService;
        private readonly IUserLocationService _userLocationService;
        private readonly ITourExecutionService _tourExecutionService;
        private readonly ITourRatingService _tourRatingService;
        private readonly IRestaurantService _restaurantService;
        private readonly Explorer.Payments.API.Public.Author.ISaleService _saleService;
        private readonly ITouristStatsService _touristStatsService;

        public DemoSeeder(
            IAuthenticationService authenticationService, 
            IEquipmentService equipmentService, 
            IFacilityService facilityService, 
            ITourService tourService, 
            IUserLocationService userLocationService, 
            ITourExecutionService tourExecution, 
            ITourRatingService tourRatingService, 
            IRestaurantService restaurantService,
            Explorer.Payments.API.Public.Author.ISaleService saleService,
            ITouristStatsService touristStatsService)
        {
            _authenticationService = authenticationService;
            _equipmentService = equipmentService;
            _facilityService = facilityService;
            _tourService = tourService;
            _userLocationService = userLocationService;
            _tourExecutionService = tourExecution;
            _tourRatingService = tourRatingService;
            _restaurantService = restaurantService;
            _saleService = saleService;
            _touristStatsService = touristStatsService;
        }

        public void Seed()
        {
            SeedAdmin();
            SeedTourists();
            SeedAuthors();
            SeedEquipment();
            SeedFacilities();
            SeedTours();
            SeedUserLocation();
            SeedKeypoints();
            SeedTourExecution();
            SeedRatings();
            SeedRestaurants();
            SeedTouristStats();
        }

        private void SeedAdmin()
        {
            AccountRegistrationDto admin = new AccountRegistrationDto()
            {
                Username = "admin",
                Password = "admin",
                Email = "admin@gmail.com",
                Name = "Marko",
                Surname = "Markovic"
            };

            _authenticationService.RegisterAdmin(admin);
        }

        private void SeedTourists()
        {
            AccountRegistrationDto tourist1 = new AccountRegistrationDto()
            {
                Username = "tourist1",
                Password = "tourist1",
                Email = "tourist1@gmail.com",
                Name = "Ana",
                Surname = "Aleksic"
            };
            AccountRegistrationDto tourist2 = new AccountRegistrationDto()
            {
                Username = "tourist2",
                Password = "tourist2",
                Email = "tourist2@gmail.com",
                Name = "Stanko",
                Surname = "Stanisic"
            };
            AccountRegistrationDto tourist3 = new AccountRegistrationDto()
            {
                Username = "tourist3",
                Password = "tourist3",
                Email = "tourist3@gmail.com",
                Name = "Pera",
                Surname = "Peric"
            };

            _authenticationService.RegisterTourist(tourist1);
            _authenticationService.RegisterTourist(tourist2);
            _authenticationService.RegisterTourist(tourist3);
        }

        private void SeedAuthors()
        {
            AccountRegistrationDto author1 = new AccountRegistrationDto()
            {
                Username = "author1",
                Password = "author1",
                Email = "author1@gmail.com",
                Name = "Bosa",
                Surname = "Boskovic"
            };
            AccountRegistrationDto author2 = new AccountRegistrationDto()
            {
                Username = "author2",
                Password = "author2",
                Email = "author2@gmail.com",
                Name = "Goran",
                Surname = "Goric"
            };
            AccountRegistrationDto author3 = new AccountRegistrationDto()
            {
                Username = "author3",
                Password = "author3",
                Email = "author3@gmail.com",
                Name = "Fifi",
                Surname = "Fifkovic"
            };

            _authenticationService.RegisterAuthor(author1);
            _authenticationService.RegisterAuthor(author2);
            _authenticationService.RegisterAuthor(author3);
        }

        private void SeedEquipment()
        {
            EquipmentDto equipment1 = new EquipmentDto()
            {
                Name = "Skije",
                Description = "Za sneg"
            };
            EquipmentDto equipment2 = new EquipmentDto()
            {
                Name = "Naocare za sunce",
                Description = "Za sunce"
            };
            EquipmentDto equipment3 = new EquipmentDto()
            {
                Name = "Padobran",
                Description = "Za skakanje"
            };
            EquipmentDto equipment4 = new EquipmentDto()
            {
                Name = "Ranac",
                Description = "Za stvari"
            };
            EquipmentDto equipment5 = new EquipmentDto()
            {
                Name = "Patike za planinarenje",
                Description = "Za planinarenje"
            };

            _equipmentService.Create(equipment1);
            _equipmentService.Create(equipment2);
            _equipmentService.Create(equipment3);
            _equipmentService.Create(equipment4);
            _equipmentService.Create(equipment5);
        }

        private void SeedFacilities()
        {
            FacilityDto facility1 = new FacilityDto()
            {
                Name = "WC1",
                Latitude = 45.25306591208099,
                Longitude = 19.8308227334037,
                Category = FacilityCategory.WC
            };

            FacilityDto facility2 = new FacilityDto()
            {
                Name = "WC2",
                Latitude = 45.248830115936855,
                Longitude = 19.796845134264007,
                Category = FacilityCategory.WC
            };

            FacilityDto facility3 = new FacilityDto()
            {
                Name = "Restoran1",
                Latitude = 45.238375228762436,
                Longitude = 19.827834086207258,
                Category = FacilityCategory.Restaurant
            };
            FacilityDto facility4 = new FacilityDto()
            {
                Name = "Restoran2",
                Latitude = 45.24450939872726,
                Longitude = 19.84268331062374,
                Category = FacilityCategory.Restaurant
            };
            FacilityDto facility5 = new FacilityDto()
            {
                Name = "Lepi parking",
                Latitude = 45.24907182467496,
                Longitude = 19.841436630784937,
                Category = FacilityCategory.Parking
            };
            FacilityDto facility6 = new FacilityDto()
            {
                Name = "Nesto drugo",
                Latitude = 45.23698512776364,
                Longitude = 19.85422699837487,
                Category = FacilityCategory.Other
            };

            _facilityService.Create(facility1);
            _facilityService.Create(facility2);
            _facilityService.Create(facility3);
            _facilityService.Create(facility4);
            _facilityService.Create(facility5);
            _facilityService.Create(facility6);
        }

        private void SeedTours()
        {
            // Currently hardcoded - would be less error-prone to fetch authors from PersonService/UserService and then get someone's ID
            long author1Id = 5;
            long author2Id = 6;
            long author3Id = 7;

            // Author1 tours

            CreateTourDto t1 = new CreateTourDto()
            {
                CreatorId = author1Id,
                Title = "Šetnja Dorćolom i Gardošom",
                Description = "Lagano istraživanje starog jezgra Beograda, uključujući Kalemegdan, Donji Dorćol i uspon do Gardoš kule.",
                Difficulty = 1,
                Tags = ["grad", "istorija", "šetnja"],
                Price = 15
            };

            CreateTourDto t2 = new CreateTourDto()
            {
                CreatorId = author1Id,
                Title = "Avantura u Ovčarsko-kablarskoj klisuri",
                Description = "Umerena planinarska tura kroz jednu od najlepših klisura Srbije, sa pogledom sa Kablara.",
                Difficulty = 3,
                Tags = ["planinarenje", "priroda"],
                Price = 40
            };

            CreateTourDto t3 = new CreateTourDto()
            {
                CreatorId = author1Id,
                Title = "Pravoslavne svetinje Šumadije",
                Description = "Obilazak znamenitih manastira: Blagoveštenje, Kalenić, Ljubostinja. Idealno za ljubitelje istorije i duhovnosti.",
                Difficulty = 2,
                Tags = ["kultura", "istorija", "verski_turizam"],
                Price = 30
            };


            // Author2 tours

            CreateTourDto t4 = new CreateTourDto()
            {
                CreatorId = author2Id,
                Title = "Kajak avantura na jezeru Perućac",
                Description = "Opusti se na vodi i istraži skrivena mesta jezera Perućac uz kratku obuku i sigurnosnu opremu.",
                Difficulty = 2,
                Tags = ["voda", "avantura", "kajak"],
                Price = 45
            };

            CreateTourDto t5 = new CreateTourDto()
            {
                CreatorId = author2Id,
                Title = "Zlatibor – Gostilje i Stopića pećina",
                Description = "Jednodnevna tura na Zlatiboru koja uključuje prelepe Gostiljske vodopade i obilazak Stopića pećine.",
                Difficulty = 1,
                Tags = ["priroda", "vodopadi", "porodično"],
                Price = 25
            };

            // Author3 tours

            CreateTourDto t6 = new CreateTourDto()
            {
                CreatorId = author3Id,
                Title = "Urbana gastro tura Novog Sada",
                Description = "Degustacija autentičnih vojvođanskih jela u centru Novog Sada, uz kratku šetnju kroz Zmaj Jovinu.",
                Difficulty = 1,
                Tags = ["hrana", "grad", "gastronomija"],
                Price = 20
            };

            _tourService.Create(t1);
            _tourService.Create(t2);
            _tourService.Create(t3);
            _tourService.Create(t4);
            _tourService.Create(t5);
            _tourService.Create(t6);
        }

        private void SeedKeypoints()
        {
            long tour1Id = 1;
            long tour2Id = 2;
            long tour5Id = 5;
            long tour6Id = 6;

            long author1Id = 5;
            long author2Id = 6;
            long author3Id = 7;

            // Tour1

            KeypointDto t1_kp1 = new KeypointDto()
            {
                Title = "Kalemegdan – Veliki Gradski Park",
                Description = "Start ture kroz istorijski deo Beograda.",
                Secret = "Ovde se nalazio rimski kastrum pre skoro 2.000 godina.",
                Latitude = 44.8231,
                Longitude = 20.4519
            };

            KeypointDto t1_kp2 = new KeypointDto()
            {
                Title = "Donji Dorćol – Ulica Cara Dušana",
                Description = "Obilazak starog jevrejskog i trgovačkog centra Beograda.",
                Secret = "Nekada je ovde bilo najveće trgovačko jezgro osmanskog Beograda.",
                Latitude = 44.8179,
                Longitude = 20.4564
            };

            KeypointDto t1_kp3 = new KeypointDto()
            {
                Title = "Gardoš Kula",
                Description = "Najlepši vidikovac u Zemunu i završetak ture.",
                Secret = "Kula je izgrađena 1896. godine u čast 1000 godina mađarske države.",
                Latitude = 44.8483,
                Longitude = 20.4056
            };

            _tourService.AddKeypoint(tour1Id, t1_kp1, author1Id);
            _tourService.AddKeypoint(tour1Id, t1_kp2, author1Id);
            _tourService.AddKeypoint(tour1Id, t1_kp3, author1Id);

            // Tour2

            KeypointDto t2_kp1 = new KeypointDto()
            {
                Title = "Pogled sa vrha Kablara",
                Description = "Spektakularan pogled na meandre Zapadne Morave.",
                Secret = "Ovaj pogled je među najfotografisanijim prirodnim pejzažima u Srbiji.",
                Latitude = 43.8992,
                Longitude = 20.2150
            };

            KeypointDto t2_kp2 = new KeypointDto()
            {
                Title = "Manastir Nikolje",
                Description = "Jedan od duhovnih centara poznate Svete Gore srpske.",
                Secret = "Manastir je podignut u 15. veku, ali je nekoliko puta rušen i obnavljan.",
                Latitude = 43.8940,
                Longitude = 20.2040
            };

            _tourService.AddKeypoint(tour2Id, t2_kp1, author1Id);
            _tourService.AddKeypoint(tour2Id, t2_kp2, author1Id);

            // Tour5

            KeypointDto t5_kp1 = new KeypointDto()
            {
                Title = "Gostiljski vodopadi",
                Description = "Prelepi vodopadi visine 22m.",
                Secret = "Malo ljudi zna da se vodopadi nalaze na privatnom zemljištu, ali su otvoreni za javnost.",
                Latitude = 43.6582,
                Longitude = 19.9391
            };

            KeypointDto t5_kp2 = new KeypointDto()
            {
                Title = "Stopića pećina – Ulaz",
                Description = "Poznata po bigrenim kadama i visokom svodu.",
                Secret = "Pećina je dugačka oko 1.600 m, ali je turistima dostupno svega 330 m.",
                Latitude = 43.6387,
                Longitude = 19.9415
            };

            KeypointDto t5_kp3 = new KeypointDto()
            {
                Title = "Selo Sirogojno",
                Description = "Etno–selo i muzej na otvorenom.",
                Secret = "Sirogojno džemperi se ručno pletu još od 19. veka.",
                Latitude = 43.6309,
                Longitude = 19.8860
            };

            _tourService.AddKeypoint(tour5Id, t5_kp1, author2Id);
            _tourService.AddKeypoint(tour5Id, t5_kp2, author2Id);
            _tourService.AddKeypoint(tour5Id, t5_kp3, author2Id);

            // Tour6

            KeypointDto t6_kp1 = new KeypointDto()
            {
                Title = "Zmaj Jovina Ulica",
                Description = "Glavna pešačka zona sa restoranima i starim lokalima.",
                Secret = "Pod ulicom se nalaze ostaci rimskog naselja Cusum.",
                Latitude = 45.2540,
                Longitude = 19.8451
            };

            _tourService.AddKeypoint(tour6Id, t6_kp1, author3Id);
        }

        private void SeedUserLocation()
        {
            for (int i = 2; i <= 4; i++)
            {
                _userLocationService.Create(new UserLocationDto
                {
                    UserId = i,
                    Latitude = 43.6582,
                    Longitude = 19.8451
                });
            }
        }

        private void SeedTourExecution()
        {
            long tour1Id = 1;
            long tour5Id = 5;
            long tourist2Id = 2;
            long tourist3Id = 3;
            long tourist4Id = 4;
            long author1Id = 5;
            long author2Id = 6;

            // Add transport time 
            TransportTimeDto tt = new TransportTimeDto()
            {
                Type = TransportTypeDto.Car,
                Duration = 5
            };

            _tourService.AddTransportTime(tour1Id, tt, author1Id);
            _tourService.Publish(tour1Id);

            // Tourist 2 - Execution 1 (In Progress)
            TourExecutionDto tourExecution1 = new TourExecutionDto()
            {
                TouristId = tourist2Id,
                TourId = tour1Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = null,
                LastActivity = DateTime.UtcNow.AddMinutes(-15),
                PercentageCompleted = 33.33
            };
            _tourExecutionService.Create(tourExecution1);

            _tourService.AddTransportTime(tour5Id, tt, author2Id);
            _tourService.Publish(tour5Id);

            // Tourist 2 
            TourExecutionDto tourExecution2 = new TourExecutionDto()
            {
                TouristId = tourist2Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddHours(-5),
                EndTime = DateTime.UtcNow.AddHours(-2),
                LastActivity = DateTime.UtcNow.AddHours(-2),
                PercentageCompleted = 100.0
            };
            var execution2 = _tourExecutionService.Create(tourExecution2);
            _tourExecutionService.CompleteTour(tourist2Id, execution2.Id);

            // Tourist 3 
            TourExecutionDto tourExecution3 = new TourExecutionDto()
            {
                TouristId = tourist3Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddHours(-3),
                EndTime = DateTime.UtcNow.AddMinutes(-30),
                LastActivity = DateTime.UtcNow.AddMinutes(-30),
                PercentageCompleted = 66.67
            };
            var execution3 = _tourExecutionService.Create(tourExecution3);
            _tourExecutionService.CompleteTour(tourist3Id, execution3.Id);

            // Tourist 4 
            TourExecutionDto tourExecution4 = new TourExecutionDto()
            {
                TouristId = tourist4Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddDays(-1),
                EndTime = DateTime.UtcNow.AddHours(-6),
                LastActivity = DateTime.UtcNow.AddHours(-6),
                PercentageCompleted = 100.0
            };
            var execution4 = _tourExecutionService.Create(tourExecution4);
            _tourExecutionService.CompleteTour(tourist4Id, execution4.Id);
        }
        
        private void SeedTouristStats()
        {
            long tourist2Id = 2;
            long tourist3Id = 3;
            long tourist4Id = 4;

            var touristStats2 = _touristStatsService.Create(tourist2Id);
            var touristStats3 = _touristStatsService.Create(tourist3Id);
            var touristStats4 = _touristStatsService.Create(tourist4Id);

            touristStats2 = new TouristStatsDto()
            {
                TouristId = tourist2Id,
                TotalXp = 500,
                Level = 4,
                IsLocalGuide = false,
                RatingsGiven = 5,
                ThumbsUpsReceived = 499
            };

            touristStats3 = new TouristStatsDto()
            {
                TouristId = tourist3Id,
                TotalXp = 350,
                Level = 3,
                IsLocalGuide = false,
                RatingsGiven = 2,
                ThumbsUpsReceived = 150
            };
            touristStats4 = new TouristStatsDto()
            {
                TouristId = tourist4Id,
                TotalXp = 800,
                Level = 5,
                IsLocalGuide = true,
                RatingsGiven = 5,
                ThumbsUpsReceived = 600
            };

            _touristStatsService.Update(touristStats2);
            _touristStatsService.Update(touristStats3);
            _touristStatsService.Update(touristStats4);
        }

        private void SeedRatings()
        {
            long tour5Id = 5;
            long tourist2Id = 2;
            long tourist3Id = 3;
            long tourist4Id = 4;

            var execution1 = _tourExecutionService.GetTouristHistory(tourist3Id).FirstOrDefault(e => e.TourId == tour5Id);
            var execution2 = _tourExecutionService.GetTouristHistory(tourist4Id).FirstOrDefault(e => e.TourId == tour5Id);
            var execution3 = _tourExecutionService.GetTouristHistory(tourist2Id).FirstOrDefault(e => e.TourId == tour5Id);

            TourRatingDto rating1 = new TourRatingDto()
            {
                UserId = tourist3Id,
                TourExecutionId = execution1.Id, 
                Stars = 5,
                Comment = "Super! Sve preporuke.",
                CompletedProcentage = 100.0
            };

            TourRatingDto rating2 = new TourRatingDto()
            {
                UserId = tourist4Id,
                TourExecutionId = execution2.Id,
                Stars = 4,
                Comment = "Lepa tura, ali može bolje organizaciono.",
                CompletedProcentage = 100.0
            };

            TourRatingDto rating3 = new TourRatingDto()
            {
                UserId = tourist2Id,
                TourExecutionId = execution3.Id,
                Stars = 2,
                Comment = "Moglo biti bolje...",
                CompletedProcentage = 96.0
            };

            _tourRatingService.Create(rating1);
            _tourRatingService.Create(rating2);
            _tourRatingService.Create(rating3);
        }

        private void SeedRestaurants()
        {
            var r1 = new RestaurantDto
            {
                Name = "Project 72 Wine & Deli",
                Description = "Moderan restoran sa lokalnim i internacionalnim jelima i velikim izborom vina.",
                Latitude = 45.2551,
                Longitude = 19.8450,
                City = "Novi Sad",
                CuisineType = "srpska / internacionalna",
                AverageRating = 4.7,
                ReviewCount = 320,
                RecommendedDishes = "Teleći obrazi; domaći hleb; lokalna vina"
            };

            var r2 = new RestaurantDto
            {
                Name = "Fish & Zelenish",
                Description = "Poznat po ribljim specijalitetima i kreativnim kombinacijama sa povrćem.",
                Latitude = 45.2557,
                Longitude = 19.8443,
                City = "Novi Sad",
                CuisineType = "mediteranska / riblja",
                AverageRating = 4.8,
                ReviewCount = 410,
                RecommendedDishes = "Filet brancina; riblja čorba; domaći deserti"
            };

            var r3 = new RestaurantDto
            {
                Name = "Veliki",
                Description = "Restoran u staroj kući sa fokusom na vojvođansku kuhinju.",
                Latitude = 45.2559,
                Longitude = 19.8457,
                City = "Novi Sad",
                CuisineType = "vojvođanska / domaća",
                AverageRating = 4.6,
                ReviewCount = 290,
                RecommendedDishes = "Perkelt; domaće knedle; štrudla s makom"
            };

            var r4 = new RestaurantDto
            {
                Name = "Savoca",
                Description = "Italijanski restoran poznat po picama iz peći na drva i pastama.",
                Latitude = 45.2537,
                Longitude = 19.8468,
                City = "Novi Sad",
                CuisineType = "italijanska",
                AverageRating = 4.5,
                ReviewCount = 350,
                RecommendedDishes = "Pizza Savoca; pasta carbonara; tiramisu"
            };

            var r5 = new RestaurantDto
            {
                Name = "Zak",
                Description = "Fine dining restoran sa modernim pristupom lokalnim namirnicama.",
                Latitude = 45.2530,
                Longitude = 19.8425,
                City = "Novi Sad",
                CuisineType = "fine dining / moderna kuhinja",
                AverageRating = 4.9,
                ReviewCount = 210,
                RecommendedDishes = "Degustacioni meni; steak; deserti od lokalnog voća"
            };

            _restaurantService.Create(r1);
            _restaurantService.Create(r2);
            _restaurantService.Create(r3);
            _restaurantService.Create(r4);
            _restaurantService.Create(r5);
        }

    }
}
