using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.API.Demo
{
    public class DemoSeeder
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEquipmentService _equipmentService;
        private readonly IFacilityService _facilityService;

        public DemoSeeder(IAuthenticationService authenticationService, IEquipmentService equipmentService, IFacilityService facilityService)
        {
            _authenticationService = authenticationService;
            _equipmentService = equipmentService;
            _facilityService = facilityService;
        }

        public void Seed()
        {
            SeedAdmin();
            SeedTourists();
            SeedAuthors();
            SeedEquipment();
            SeedFacilities();
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
    }
}
