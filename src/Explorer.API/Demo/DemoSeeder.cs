using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
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
        private readonly ITouristStatsService _touristStatsService;
        private readonly IUserManagementService _userManagementService;
        private readonly IWalletService _walletService;
        private readonly ITouristMapMarkerService _touristMapMarkerService;
        private readonly IMapMarkerService _mapMarkerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IBlogService _blogService;
        private readonly IPersonService _personService;

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
            IMapMarkerService mapMarkerService,
            IBlogService blogService,
            IPersonService personService,
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
            _userManagementService = userManagementService;
            _walletService = walletService;
            _touristMapMarkerService = touristMapMarkerService;
            _shoppingCartService = shoppingCartService;
            _mapMarkerService = mapMarkerService;
            _blogService = blogService;
            _personService = personService;
            _touristStatsService = touristStatsService;
        }

        public void Seed()
        {
            SeedDefaultMarker();
            SeedAdmin();
            SeedTourists();
            SeedStandaloneMarkers();
            SeedWallets();
            SeedAuthors();
            SeedBlogs();
            SeedEquipment();
            SeedFacilities();
            SeedTours();
            SeedUserLocation();
            SeedShop();
            SeedTourExecution();
            SeedRatings();
            SeedRestaurants();
            SeedTouristStats();
        }

        private void SeedBlogs()
        {
            long tourist1Id = 2;
            long tourist2Id = 3;
            long author1Id = 5;
            long author3Id = 7;

            // ====== TURISTA 1 (3 bloga)
            var t1b1 = _blogService.CreateBlog(tourist1Id, new BlogCreateDto
            {
                Title = "Skriveni delovi grada",
                Description =
                    "# Skriveni delovi grada\n\n" +
                    "## Spontano istraživanje\n\n" +
                    "Spontane šetnje često otkriju najzanimljivije delove grada. " +
                    "Bez jasnog plana, naišao sam na mirne ulice i male trgove koje većina ljudi preskoči.\n\n" +

                    "## Mir i svakodnevni život\n\n" +
                    "Ovi prostori deluju prijatno čak i u periodima veće gužve. " +
                    "Idealni su za laganu šetnju, kratki predah ili fotografisanje detalja.\n\n" +

                    "## Utisak\n\n" +
                    "Ako volite da istražujete bez pritiska i rasporeda, ovakvi delovi grada pružaju poseban doživljaj.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker1.png"
        }
            });

            var t1b2 = _blogService.CreateBlog(tourist1Id, new BlogCreateDto
            {
                Title = "Jutarnja šetnja",
                Description =
                    "# Jutarnja šetnja\n\n" +
                    "## Grad pre gužve\n\n" +
                    "Jutarnji sati nude poseban mir koji se kasnije gubi. " +
                    "Šetnja pre nego što se grad probudi ima svoju posebnu čar.\n\n" +

                    "## Atmosfera\n\n" +
                    "Vazduh je svež, ulice su tihe, a kafići se tek otvaraju. " +
                    "Grad u tom trenutku deluje sporije i opuštenije.\n\n" +

                    "## Preporuka\n\n" +
                    "Ovakva šetnja je idealna za one koji žele tišinu i lagan početak dana.",
                Images = new List<string>()
            });

            var t1b3 = _blogService.CreateBlog(tourist1Id, new BlogCreateDto
            {
                Title = "Neočekivani pogledi",
                Description =
                    "# Neočekivani pogledi\n\n" +
                    "## Bliski vidikovci\n\n" +
                    "Lepi pogledi ne zahtevaju uvek dugu šetnju ili napor. " +
                    "Neka mesta su iznenađujuće blizu, a nude sjajan pogled.\n\n" +

                    "## Kratka pauza\n\n" +
                    "Lokacija je lako dostupna i pogodna za kratko zadržavanje. " +
                    "Idealna je tokom obilaska grada.\n\n" +

                    "## Zaključak\n\n" +
                    "Savršeno mesto za predah bez većeg odstupanja od rute.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker2.png",
            "https://localhost:44333/images/marker3.png"
        }
            });

            _blogService.PublishBlog(t1b1.Id, t1b1.UserId);
            _blogService.PublishBlog(t1b2.Id, t1b2.UserId);
            _blogService.PublishBlog(t1b3.Id, t1b3.UserId);


            // ====== TURISTA 2 (2 bloga)
            var t2b1 = _blogService.CreateBlog(tourist2Id, new BlogCreateDto
            {
                Title = "Brzi beg iz grada",
                Description =
                    "# Brzi beg iz grada\n\n" +
                    "## Kratak predah\n\n" +
                    "Ponekad je dovoljan kratak izlet da se napravi pauza od rutine. " +
                    "Ovaj beg iz grada bio je jednostavan i lako isplaniran.\n\n" +

                    "## Lokacije\n\n" +
                    "Posetio sam mesta koja su blizu i ne zahtevaju celodnevnu organizaciju. " +
                    "Takav pristup omogućava opuštenije uživanje.\n\n" +

                    "## Utisak\n\n" +
                    "Idealan izbor kada imate malo vremena, ali vam je potrebna promena.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker1.png",
            "https://localhost:44333/images/marker2.png"
        }
            });

            var t2b2 = _blogService.CreateBlog(tourist2Id, new BlogCreateDto
            {
                Title = "Ulična hrana",
                Description =
                    "# Ulična hrana\n\n" +
                    "## Lokalni ukusi\n\n" +
                    "Ulična hrana često nudi najautentičniji ukus grada. " +
                    "Naišao sam na mali štand popularan među lokalnim stanovništvom.\n\n" +

                    "## Iskustvo\n\n" +
                    "Hrana je bila jednostavna, ali veoma ukusna i brzo pripremljena. " +
                    "Usluga je bila efikasna.\n\n" +

                    "## Preporuka\n\n" +
                    "Odličan izbor za brz i povoljan obrok tokom šetnje.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker3.png"
        }
            });

            _blogService.PublishBlog(t2b1.Id, t2b1.UserId);
            _blogService.PublishBlog(t2b2.Id, t2b2.UserId);


            // ====== AUTOR TURA 1 (5 blogova)
            var a1b1 = _blogService.CreateBlog(author1Id, new BlogCreateDto
            {
                Title = "Kada je najbolje posetiti grad",
                Description =
                    "# Kada je najbolje posetiti grad\n\n" +
                    "## Sezone\n\n" +
                    "Grad se može posetiti tokom cele godine, ali određeni periodi nude prijatnije uslove.\n\n" +

                    "## Najbolji periodi\n\n" +
                    "Proleće i rana jesen nude umerene temperature i manje gužve. " +
                    "Leti je grad živahniji, ali i topliji.\n\n" +

                    "## Savet\n\n" +
                    "Izbor termina zavisi od vaših navika i tolerancije na gužvu.",
                Images = new List<string>()
            });

            var a1b2 = _blogService.CreateBlog(author1Id, new BlogCreateDto
            {
                Title = "Najlepši vidikovci u okolini",
                Description =
                    "# Najlepši vidikovci u okolini\n\n" +
                    "## Pogledi\n\n" +
                    "U okolini grada postoji više vidikovaca sa pogledom na urbani i prirodni pejzaž.\n\n" +

                    "## Pristupačnost\n\n" +
                    "Većina lokacija je lako dostupna i ne zahteva posebnu opremu.\n\n" +

                    "## Kada posetiti\n\n" +
                    "Jutarnji i večernji sati pružaju najbolje svetlo.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker2.png"
        }
            });

            var a1b3 = _blogService.CreateBlog(author1Id, new BlogCreateDto
            {
                Title = "Saveti za kretanje po gradu",
                Description =
                    "# Saveti za kretanje po gradu\n\n" +
                    "## Prevoz\n\n" +
                    "Grad je dobro povezan javnim prevozom i lako se obilazi.\n\n" +

                    "## Pešačenje\n\n" +
                    "Centralni delovi su idealni za šetnju. " +
                    "Za udaljenije tačke praktični su autobusi i tramvaji.\n\n" +

                    "## Planiranje\n\n" +
                    "U špicu je preporučljivo planirati rutu unapred.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker1.png",
            "https://localhost:44333/images/marker3.png"
        }
            });

            var a1b4 = _blogService.CreateBlog(author1Id, new BlogCreateDto
            {
                Title = "Šta poneti na gradsko razgledanje",
                Description =
                    "# Šta poneti na gradsko razgledanje\n\n" +
                    "## Osnovne stvari\n\n" +
                    "Dobra priprema značajno utiče na ukupno iskustvo.\n\n" +

                    "## Oprema\n\n" +
                    "Udobna obuća, voda i zaštita od sunca su najvažniji elementi.\n\n" +

                    "## Vremenski uslovi\n\n" +
                    "Zimi je preporučljiva slojevita garderoba.",
                Images = new List<string>()
            });

            var a1b5 = _blogService.CreateBlog(author1Id, new BlogCreateDto
            {
                Title = "Večernja atmosfera grada",
                Description =
                    "# Večernja atmosfera grada\n\n" +
                    "## Drugačiji ritam\n\n" +
                    "Grad u večernjim satima menja tempo i karakter.\n\n" +

                    "## Ambijent\n\n" +
                    "Osvetljene ulice i mirnija atmosfera čine šetnju prijatnom.\n\n" +

                    "## Preporuka\n\n" +
                    "Veče je idealno za lagano istraživanje ili večeru.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker3.png"
        }
            });

            _blogService.PublishBlog(a1b1.Id, a1b1.UserId);
            _blogService.PublishBlog(a1b2.Id, a1b2.UserId);
            _blogService.PublishBlog(a1b3.Id, a1b3.UserId);
            _blogService.PublishBlog(a1b4.Id, a1b4.UserId);
            _blogService.PublishBlog(a1b5.Id, a1b5.UserId);


            // ====== AUTOR TURA 3 (1 blog)
            var a3b1 = _blogService.CreateBlog(author3Id, new BlogCreateDto
            {
                Title = "Prvi utisci o gradu",
                Description =
                    "# Prvi utisci o gradu\n\n" +
                    "## Dolazak\n\n" +
                    "Prvi susret sa gradom često ostavlja snažan utisak.\n\n" +

                    "## Snalaženje\n\n" +
                    "Preporučuje se obilazak šireg centra prvog dana.\n\n" +

                    "## Savet\n\n" +
                    "Tako se lakše stekne osećaj za raspored i atmosferu.",
                Images = new List<string>
        {
            "https://localhost:44333/images/marker1.png",
            "https://localhost:44333/images/marker2.png",
            "https://localhost:44333/images/marker3.png"
        }
            });

            _blogService.PublishBlog(a3b1.Id, a3b1.UserId);
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

            var t1 = _authenticationService.RegisterTourist(tourist1);
            var t2 = _authenticationService.RegisterTourist(tourist2);
            var t3 = _authenticationService.RegisterTourist(tourist3);

            var p1 = _personService.Get(t1.Id);
            var p2 = _personService.Get(t2.Id);
            var p3 = _personService.Get(t3.Id);

            _personService.Update(p1.Id, new PersonDto()
            {
                UserId = p1.UserId,
                Name = p1.Name,
                Surname = p1.Surname,
                Email = p1.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker1.png",
                Biography = "Ljubitelj spontanih putovanja, dugih šetnji i skrivenih mesta koja ne pišu u vodičima.",
                Quote = "Najlepša mesta su ona koja pronađeš slučajno."
            });

            _personService.Update(p2.Id, new PersonDto()
            {
                UserId = p2.UserId,
                Name = p2.Name,
                Surname = p2.Surname,
                Email = p2.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker1.png",
                Biography = "Uvek u potrazi za dobrom hranom, lokalnim pričama i autentičnim doživljajima.",
                Quote = "Grad se najbolje upozna kroz njegove ulice i ljude."
            });

            _personService.Update(p3.Id, new PersonDto()
            {
                UserId = p3.UserId,
                Name = p3.Name,
                Surname = p3.Surname,
                Email = p3.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker1.png",
                Biography = "Volim prirodu, kratke izlete i mesta gde vreme sporije prolazi.",
                Quote = "Nije bitno gde ideš, već kako doživljavaš put."
            });
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
                Name = "Milana",
                Surname = "Milic"
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

            var a1 = _authenticationService.RegisterAuthor(author1);
            var a2 = _authenticationService.RegisterAuthor(author2);
            var a3 = _authenticationService.RegisterAuthor(author3);

            var p1 = _personService.Get(a1.Id);
            var p2 = _personService.Get(a2.Id);
            var p3 = _personService.Get(a3.Id);

            _personService.Update(p1.Id, new PersonDto()
            {
                UserId = p1.UserId,
                Name = p1.Name,
                Surname = p1.Surname,
                Email = p1.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker2.png",
                Biography = "Strastveni vodič kroz istorijske ture i lokalne legende, uvek sa osmehom.",
                Quote = "Svaka tura priča svoju priču, samo je treba doživeti."
            });

            _personService.Update(p2.Id, new PersonDto()
            {
                UserId = p2.UserId,
                Name = p2.Name,
                Surname = p2.Surname,
                Email = p2.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker2.png",
                Biography = "Vodič avanturističkih tura, sa znanjem o skrivenim draguljima grada.",
                Quote = "Najbolje ture su one koje pamtiš zauvek."
            });

            _personService.Update(p3.Id, new PersonDto()
            {
                UserId = p3.UserId,
                Name = p3.Name,
                Surname = p3.Surname,
                Email = p3.Email,
                ProfileImageUrl = "https://localhost:44333/images/marker2.png",
                Biography = "Ekspert za gastronomske ture i autentična iskustva lokalnog života.",
                Quote = "Hrana, priče i smeh – to je prava tura."
            });
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
            long tourist2Id = 3;
            long tourist3Id = 4;

            long tour1Id = 1;
            long tour5Id = 5; // important for execution later, do not touch!!!!

            _shoppingCartService.AddToCart(tourist1Id, tour1Id);
            _shoppingCartService.Checkout(tourist1Id);


            // important for execution later, do not touch!!!!
            _shoppingCartService.AddToCart(tourist1Id, tour5Id);
            _shoppingCartService.Checkout(tourist1Id);

            _shoppingCartService.AddToCart(tourist2Id, tour5Id);
            _shoppingCartService.Checkout(tourist2Id);

            _shoppingCartService.AddToCart(tourist3Id, tour5Id);
            _shoppingCartService.Checkout(tourist3Id);

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
            long tourist3Id = 4;
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

            // Tourist 1 - Execution 1 (In Progress)
            var startTourDto = new StartTourDto()
            {
                TourId = tour1Id,
                InitialLatitude = 0,
                InitialLongitude = 0,
            };

            
            var execution = _tourExecutionService.StartTour(tourist1Id, startTourDto);





            // Completed tour executions for Tour 5
            // primarily for ratings

            //// Tourist 1
            TourExecutionDto tourExecution2 = new TourExecutionDto()
            {
                TouristId = tourist1Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddHours(-3),
                EndTime = DateTime.UtcNow.AddMinutes(-30),
                LastActivity = DateTime.UtcNow.AddMinutes(-30),
                PercentageCompleted = 66.67
            }
            ;
            var execution2 = _tourExecutionService.Create(tourExecution2);
            _tourExecutionService.CompleteTour(tourist1Id, execution2.Id);

            //// Tourist 2 
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

            // Tourist 3 
            TourExecutionDto tourExecution4 = new TourExecutionDto()
            {
                TouristId = tourist3Id,
                TourId = tour5Id,
                Status = TourExecutionStatusDto.InProgress,
                StartTime = DateTime.UtcNow.AddDays(-1),
                EndTime = DateTime.UtcNow.AddHours(-6),
                LastActivity = DateTime.UtcNow.AddHours(-6),
                PercentageCompleted = 100.0
            };
            var execution4 = _tourExecutionService.Create(tourExecution4);
            _tourExecutionService.CompleteTour(tourist3Id, execution4.Id);
        }

        private void SeedTouristStats()
        {
            long tourist2Id = 2;
            long tourist3Id = 3;
            long tourist4Id = 4;

            var touristStats2 = _touristStatsService.Create(tourist2Id);
            touristStats2.TouristId = tourist2Id;
            touristStats2.TotalXp = 500;
            touristStats2.Level = 4;
            touristStats2.IsLocalGuide = false;
            touristStats2.RatingsGiven = 5;
            touristStats2.ThumbsUpsReceived = 499;
            _touristStatsService.Update(touristStats2);

            var touristStats3 = _touristStatsService.Create(tourist3Id);
            touristStats3.TouristId = tourist3Id;
            touristStats3.TotalXp = 350;
            touristStats3.Level = 3;
            touristStats3.IsLocalGuide = false;
            touristStats3.RatingsGiven = 2;
            touristStats3.ThumbsUpsReceived = 150;
            _touristStatsService.Update(touristStats3);

            var touristStats4 = _touristStatsService.Create(tourist4Id);
            touristStats4.TouristId = tourist4Id;
            touristStats4.TotalXp = 800;
            touristStats4.Level = 5;
            touristStats4.IsLocalGuide = true;
            touristStats4.RatingsGiven = 5;
            touristStats4.ThumbsUpsReceived = 600;
            _touristStatsService.Update(touristStats4);
        }


        private void SeedRatings()
        {
            long tour5Id = 5;
            long tourist1Id = 2;
            long tourist2Id = 3;
            long tourist3Id = 4;

            var execution1 = _tourExecutionService.GetTouristHistory(tourist1Id).FirstOrDefault(e => e.TourId == tour5Id);
            var execution2 = _tourExecutionService.GetTouristHistory(tourist2Id).FirstOrDefault(e => e.TourId == tour5Id);
            var execution3 = _tourExecutionService.GetTouristHistory(tourist3Id).FirstOrDefault(e => e.TourId == tour5Id);

            TourRatingDto rating1 = new TourRatingDto()
            {
                UserId = tourist1Id,
                TourExecutionId = execution1.Id, 
                Stars = 5,
                Comment = "Super! Sve preporuke.",
                CompletedPercentage = 100.0
            };

            TourRatingDto rating2 = new TourRatingDto()
            {
                UserId = tourist2Id,
                TourExecutionId = execution2.Id,
                Stars = 4,
                Comment = "Lepa tura, ali može bolje organizaciono.",
                CompletedPercentage = 100.0
            };

            TourRatingDto rating3 = new TourRatingDto()
            {
                UserId = tourist3Id,
                TourExecutionId = execution3.Id,
                Stars = 2,
                Comment = "Moglo biti bolje...",
                CompletedPercentage = 96.0
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
