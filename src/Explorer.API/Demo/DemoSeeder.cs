using Explorer.Payments.API.Public.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using static System.Net.WebRequestMethods;

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
        private readonly IUserManagementService _userManagementService;
        private readonly IWalletService _walletService;
        private readonly ITouristMapMarkerService _touristMapMarkerService;
        private readonly IMapMarkerService _mapMarkerService;
        private readonly IShoppingCartService _shoppingCartService;

        private readonly int predefinedMarkersNumber = 10;
        private readonly string imageRootUrl = "https://localhost:44333/images/";

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
            IUserManagementService userManagementService,
            IWalletService walletService,
            ITouristMapMarkerService touristMapMarkerService,
            IShoppingCartService shoppingCartService,
            IMapMarkerService mapMarkerService)
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
            _userManagementService = userManagementService;
            _walletService = walletService;
            _touristMapMarkerService = touristMapMarkerService;
            _shoppingCartService = shoppingCartService;
            _mapMarkerService = mapMarkerService;
        }

        public void Seed()
        {
            SeedDefaultMarker();
            SeedAdmin();
            SeedTourists();
            SeedStandaloneMarkers();
            SeedWallets();
            SeedAuthors();
            SeedEquipment();
            SeedFacilities();
            SeedTours();
            SeedUserLocation();
            SeedShop();
            SeedTourExecution();
            SeedRatings();
            SeedRestaurants();
        }

        private void SeedStandaloneMarkers()
        {
            var markerIds = new List<long>();
            long tourist1Id = 2;
            for(int i = 1; i <= predefinedMarkersNumber; i++)
            {
                var marker = _mapMarkerService.Create(new MapMarkerDto
                {
                    ImageUrl = imageRootUrl + $"marker{i}.png",
                    IsStandalone = true
                });
                markerIds.Add(marker.Id);
            }

            // Tourist 1 gets a bunch of markers
            foreach(var markerId in markerIds)
            {
                _touristMapMarkerService.Collect(tourist1Id, markerId);
            }
        }

        private void SeedDefaultMarker()
        {
            var defaultMarkerDto = _mapMarkerService.Create(new MapMarkerDto
            {
                ImageUrl = imageRootUrl + "defaultMarker.png",
                IsStandalone = true,
            });
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

        private void SeedWallets()
        {
            var tourist1 = _userManagementService.GetByUsername("tourist1");
            var tourist2 = _userManagementService.GetByUsername("tourist2");
            var tourist3 = _userManagementService.GetByUsername("tourist3");

            var wallet1 = _walletService.Create(tourist1.Id);
            var wallet2 = _walletService.Create(tourist2.Id);
            var wallet3 = _walletService.Create(tourist3.Id);

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
                Category = FacilityCategory.Store
            };
            FacilityDto facility4 = new FacilityDto()
            {
                Name = "Restoran2",
                Latitude = 45.24450939872726,
                Longitude = 19.84268331062374,
                Category = FacilityCategory.Store
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

        // Author 1 (ID = 5):
        //  - Tour 1: "Šetnja Dorćolom i Gardošom"        → PUBLISHED
        //  - Tour 2: "Avantura u Ovčarsko-kablarskoj klisuri" → PUBLISHED
        //  - Tour 3: "Pravoslavne svetinje Šumadije"     → NOT published
        //
        // Author 2 (ID = 6):
        //  - Tour 4: "Kajak avantura na jezeru Perućac"  → PUBLISHED
        //  - Tour 5: "Zlatibor – Gostilje i Stopića pećina" → PUBLISHED
        //
        // Author 3 (ID = 7):
        //  - Tour 6: "Urbana gastro tura Novog Sada"      → NOT published
        private void SeedTours()
        {
            long author1Id = 5;
            long author2Id = 6;
            long author3Id = 7;

            // ================= CREATE TOURS =================

            var tour1 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author1Id,
                Title = "Šetnja Dorćolom i Gardošom",
                Description = "Lagano istraživanje starog jezgra Beograda.",
                Difficulty = 1,
                Tags = ["grad", "istorija"],
                Price = 15
            });

            var tour2 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author1Id,
                Title = "Avantura u Ovčarsko-kablarskoj klisuri",
                Description = "Planinarska tura sa pogledom sa Kablara.",
                Difficulty = 3,
                Tags = ["planinarenje", "priroda"],
                Price = 40
            });

            var tour3 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author1Id,
                Title = "Pravoslavne svetinje Šumadije",
                Description = "Obilazak znamenitih manastira.",
                Difficulty = 2,
                Tags = ["kultura", "istorija"],
                Price = 30
            });

            var tour4 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author2Id,
                Title = "Kajak avantura na jezeru Perućac",
                Description = "Opuštanje i avantura na vodi.",
                Difficulty = 2,
                Tags = ["voda", "kajak"],
                Price = 45
            });

            var tour5 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author2Id,
                Title = "Zlatibor – Gostilje i Stopića pećina",
                Description = "Jednodnevna prirodna tura.",
                Difficulty = 1,
                Tags = ["priroda", "vodopadi"],
                Price = 25
            });

            var tour6 = _tourService.Create(new CreateTourDto
            {
                CreatorId = author3Id,
                Title = "Urbana gastro tura Novog Sada",
                Description = "Gastro šetnja kroz centar grada.",
                Difficulty = 1,
                Tags = ["hrana", "grad"],
                Price = 20
            });

            // ================= TOUR 1 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour1.Id,
                new TransportTimeDto { Type = TransportTypeDto.Car, Duration = 5 },
                tour1.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour1.Id, 1, tour1.CreatorId);
            _tourService.AddEquipment(tour1.Id, 4, tour1.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour1.Id, new KeypointDto
            {
                Title = "Kalemegdan",
                Description = "Start ture.",
                Secret = "Rimski kastrum.",
                Latitude = 44.8231,
                Longitude = 20.4519
            }, tour1.CreatorId);

            _tourService.AddKeypoint(tour1.Id, new KeypointDto
            { 
                Title = "Donji Dorćol – Ulica Cara Dušana",
                Description = "Obilazak starog jevrejskog i trgovačkog centra Beograda.",
                Secret = "Nekada je ovde bilo najveće trgovačko jezgro osmanskog Beograda.",
                Latitude = 44.8179,
                Longitude = 20.4564
            }, tour1.CreatorId);

            _tourService.AddKeypoint(tour1.Id, new KeypointDto
            {
                Title = "Gardoš kula",
                Description = "Vidikovac.",
                Secret = "Izgrađena 1896.",
                Latitude = 44.8483,
                Longitude = 20.4056
            }, tour1.CreatorId);

            // ----- Map marker -----
            _tourService.AddMapMarker(
                tour1.Id,
                new MapMarkerDto
                {
                    ImageUrl = imageRootUrl + "markerTour1.png"
                },
                tour1.CreatorId);

            // Publish
            _tourService.Publish(tour1.Id);

            // ================= TOUR 2 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour2.Id,
                new TransportTimeDto { Type = TransportTypeDto.Foot, Duration = 180 },
                tour2.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour2.Id, 1, tour2.CreatorId);
            _tourService.AddEquipment(tour2.Id, 5, tour2.CreatorId);
            _tourService.AddEquipment(tour2.Id, 4, tour2.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour2.Id, new KeypointDto
            {
                Title = "Kablar – vidikovac",
                Description = "Pogled na meandre.",
                Secret = "Fotografisano mesto.",
                Latitude = 43.8992,
                Longitude = 20.2150
            }, tour2.CreatorId);

            _tourService.AddKeypoint(tour2.Id, new KeypointDto
            {
                Title = "Manastir Nikolje",
                Description = "Duhovni centar.",
                Secret = "15. vek.",
                Latitude = 43.8940,
                Longitude = 20.2040
            }, tour2.CreatorId);

            // ----- Map marker -----
            _tourService.AddMapMarker(
                tour2.Id,
                new MapMarkerDto
                {
                    ImageUrl = imageRootUrl + "markerTour2.png"
                },
                tour2.CreatorId);

            _tourService.Publish(tour2.Id);

            // ================= TOUR 3 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour3.Id,
                new TransportTimeDto { Type = TransportTypeDto.Car, Duration = 30 },
                tour3.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour3.Id, 2, tour3.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour3.Id, new KeypointDto
            {
                Title = "Manastir Blagoveštenje",
                Description = "Istorijska svetinja.",
                Secret = "Više puta obnavljan.",
                Latitude = 43.897,
                Longitude = 20.209
            }, tour3.CreatorId);

            _tourService.AddKeypoint(tour3.Id, new KeypointDto
            {
                Title = "Manastir Ljubostinja",
                Description = "Zadužbina kneginje Milice.",
                Secret = "14. vek.",
                Latitude = 43.637,
                Longitude = 21.003
            }, tour3.CreatorId);

            // not published

            // ================= TOUR 4 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour4.Id,
                new TransportTimeDto { Type = TransportTypeDto.Bike, Duration = 45 },
                tour4.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour4.Id, 3, tour4.CreatorId);
            _tourService.AddEquipment(tour4.Id, 4, tour4.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour4.Id, new KeypointDto
            {
                Title = "Jezero Perućac",
                Description = "Početna tačka.",
                Secret = "Veštačko jezero.",
                Latitude = 43.952,
                Longitude = 19.522
            }, tour4.CreatorId);

            _tourService.AddKeypoint(tour4.Id, new KeypointDto
            {
                Title = "Uvala",
                Description = "Mirno mesto.",
                Secret = "Idealno za kajak.",
                Latitude = 43.948,
                Longitude = 19.515
            }, tour4.CreatorId);

            // ----- Map marker -----
            _tourService.AddMapMarker(
                tour4.Id,
                new MapMarkerDto
                {
                    ImageUrl = imageRootUrl + "markerTour4.png"
                },
                tour4.CreatorId);

            _tourService.Publish(tour4.Id);

            // ================= TOUR 5 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour5.Id,
                new TransportTimeDto { Type = TransportTypeDto.Car, Duration = 25 },
                tour5.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour5.Id, 5, tour5.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour5.Id, new KeypointDto
            {
                Title = "Gostiljski vodopadi",
                Description = "Vodopadi.",
                Secret = "22m visine.",
                Latitude = 43.6582,
                Longitude = 19.9391
            }, tour5.CreatorId);

            _tourService.AddKeypoint(tour5.Id, new KeypointDto
            {
                Title = "Stopića pećina",
                Description = "Bigrene kade.",
                Secret = "330m otvoreno.",
                Latitude = 43.6387,
                Longitude = 19.9415
            }, tour5.CreatorId);

            // ----- Map marker -----
            _tourService.AddMapMarker(
                tour5.Id,
                new MapMarkerDto
                {
                    ImageUrl = imageRootUrl + "markerTour5.png"
                },
                tour5.CreatorId);

            _tourService.Publish(tour5.Id);

            // ================= TOUR 6 =================

            // ----- Transport times -----
            _tourService.AddTransportTime(
                tour6.Id,
                new TransportTimeDto { Type = TransportTypeDto.Foot, Duration = 60 },
                tour6.CreatorId);

            // ----- Equipment -----
            _tourService.AddEquipment(tour6.Id, 2, tour6.CreatorId);

            // ----- Keypoints -----
            _tourService.AddKeypoint(tour6.Id, new KeypointDto
            {
                Title = "Zmaj Jovina",
                Description = "Pešačka zona.",
                Secret = "Rimski ostaci.",
                Latitude = 45.2540,
                Longitude = 19.8451
            }, tour6.CreatorId);

            _tourService.AddKeypoint(tour6.Id, new KeypointDto
            {
                Title = "Dunavski park",
                Description = "Zeleni deo grada.",
                Secret = "Nekada močvara.",
                Latitude = 45.2555,
                Longitude = 19.8480
            }, tour6.CreatorId);

            // not published
        }

        // Tourist 1 bought tour 1
        private void SeedShop()
        {
            long tourist1Id = 2;
            long tour1Id = 1;

            _shoppingCartService.AddToCart(tourist1Id, tour1Id);
            _shoppingCartService.Checkout(tourist1Id);
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
            long tourist1Id = 2;
            long tourist2Id = 3;
            //long tourist4Id = 4;
            //long author1Id = 5;
            //long author2Id = 6;

            //// Add transport time 
            //TransportTimeDto tt = new TransportTimeDto()
            //{
            //    Type = TransportTypeDto.Car,
            //    Duration = 5
            //};

            //_tourService.AddTransportTime(tour1Id, tt, author1Id);
            //_tourService.Publish(tour1Id);

            // Tourist 2 - Execution 1 (In Progress)
            var startTourDto = new StartTourDto()
            {
                TourId = tour1Id,
                InitialLatitude = 0,
                InitialLongitude = 0,
            };

            
            var execution = _tourExecutionService.StartTour(tourist1Id, startTourDto);
            

            //_tourService.AddTransportTime(tour5Id, tt, author2Id);
            //_tourService.Publish(tour5Id);

            // Tourist 2 
            //TourExecutionDto tourExecution2 = new TourExecutionDto()
            //{
            //    TouristId = tourist2Id,
            //    TourId = tour5Id,
            //    Status = TourExecutionStatusDto.InProgress,
            //    StartTime = DateTime.UtcNow.AddHours(-5),
            //    EndTime = DateTime.UtcNow.AddHours(-2),
            //    LastActivity = DateTime.UtcNow.AddHours(-2),
            //    PercentageCompleted = 100.0
            //};
            //var execution2 = _tourExecutionService.Create(tourExecution2);
            //_tourExecutionService.CompleteTour(tourist2Id, execution2.Id);

            //// Tourist 3 
            TourExecutionDto tourExecution3 = new TourExecutionDto()
            {
                TouristId = tourist2Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddHours(-3),
                EndTime = DateTime.UtcNow.AddMinutes(-30),
                LastActivity = DateTime.UtcNow.AddMinutes(-30),
                PercentageCompleted = 66.67
            };
            var execution3 = _tourExecutionService.Create(tourExecution3);
            _tourExecutionService.CompleteTour(tourist2Id, execution3.Id);

            // Tourist 4 
            //TourExecutionDto tourExecution4 = new TourExecutionDto()
            //{
            //    TouristId = tourist4Id,
            //    TourId = tour5Id,
            //    Status = TourExecutionStatusDto.InProgress,
            //    StartTime = DateTime.UtcNow.AddDays(-1),
            //    EndTime = DateTime.UtcNow.AddHours(-6),
            //    LastActivity = DateTime.UtcNow.AddHours(-6),
            //    PercentageCompleted = 100.0
            //};
            //var execution4 = _tourExecutionService.Create(tourExecution4);
            //_tourExecutionService.CompleteTour(tourist4Id, execution4.Id);
        }

        private void SeedRatings()
        {
            long tour5Id = 5;
            long tourist2Id = 3;
            //long tourist4Id = 4;

            var execution1 = _tourExecutionService.GetTouristHistory(tourist2Id).FirstOrDefault(e => e.TourId == tour5Id);
            //var execution2 = _tourExecutionService.GetTouristHistory(tourist4Id).FirstOrDefault(e => e.TourId == tour5Id);

            TourRatingDto rating1 = new TourRatingDto()
            {
                UserId = tourist2Id,
                TourExecutionId = execution1.Id, 
                Stars = 5,
                Comment = "Super! Sve preporuke.",
                CompletedProcentage = 100.0
            };

            //TourRatingDto rating2 = new TourRatingDto()
            //{
            //    UserId = tourist4Id,
            //    TourExecutionId = execution2.Id,
            //    Stars = 4,
            //    Comment = "Lepa tura, ali može bolje organizaciono.",
            //    CompletedProcentage = 100.0
            //};

            _tourRatingService.Create(rating1);
            //_tourRatingService.Create(rating2);
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
