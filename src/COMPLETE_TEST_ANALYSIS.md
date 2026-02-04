# Sveobuhvatna Analiza Neuspešnih Testova

## Status Testova (Trenutno)

**UKUPNO: 313/342 (91.5%) testova prolazi**

### Po Modulima:
- ? **Explorer.Encounters.Tests**: 14/14 (100%)
- ? **Explorer.Payments.Tests**: 41/41 (100%)
- ? **Explorer.Blog.Tests**: 22/23 (95.7%) - 1 failed
- ?? **Explorer.Architecture.Tests**: 32/33 (97%) - 1 failed
- ?? **Explorer.Stakeholders.Tests**: 95/102 (93.1%) - 7 failed
- ?? **Explorer.Tours.Tests**: 110/129 (85.3%) - 19 failed

---

## Detaljno Stanje Po Kategorijama

### 1. Architecture.Tests (1 Failed) ?

**Problem**: ProjectAutopsy modul krši arhitektonske principe

```
Explorer.ProjectAutopsy.Core.Domain.RiskSnapshot depends on 
Explorer.ProjectAutopsy.Core.Services.GitHubMetrics

Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces.IGitHubDataService depends on 
Explorer.ProjectAutopsy.Core.Services.CommitData and PullRequestData
```

**Uzrok**: Domain sloj zavisi od Services sloja - krši clean architecture

**Rešenje**: Refaktorisanje ProjectAutopsy modula:
- Premestiti `GitHubMetrics`, `CommitData`, `PullRequestData` u Domain ili izdvojiti u zasebne DTO-ove
- Možda kreirati zasebne modele u Domain sloju

**Prioritet**: ?? Nizak (nije deo glavnih funkcionalnosti)

---

### 2. Blog.Tests (1 Failed) ??

**Test**: `BlogControllerTests` (verovatno vote related)

**Prioritet**: ?? Nizak (blog funkcionalnost nije kriti?na)

---

### 3. Stakeholders.Tests (7 Failed) ??

#### 3.1 Registration Tests (3 failed)
- `Successfully_registers_tourist`
- `Successfully_registers_admins`
- `Successfully_registers_authors`

**Problem**: `personId` claim je null u JWT tokenu

**Mogu?i uzroci**:
1. Test baza ne kreira Person entitet ispravno
2. Problem sa `GenerateAccessToken` u test okruženju
3. Nedostaju potrebni seed podaci

**Debug koraci**:
```csharp
// Proveriti da li se Person kreira
var person = _personRepository.Create(new Person(...));
Console.WriteLine($"Created person ID: {person.Id}");

// Proveriti da li token sadrži personId
var token = _tokenGenerator.GenerateAccessToken(user, person.Id);
var decoded = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);
var claim = decoded.Claims.FirstOrDefault(c => c.Type == "personId");
Console.WriteLine($"PersonId claim: {claim?.Value}");
```

#### 3.2 User Management Tests (2 failed)
- `Successfully_get_users`
- `Successfully_block_users`

**Problem**: Verovatno nedostaju seed podaci ili User entiteti nisu kreirani

#### 3.3 Journal Tests (1 failed)
- `Delete_journal_succeeds_for_owner`

**Problem**: Nedostaju Journal test podaci

#### 3.4 Login Tests (1 failed)
- `Successfully_logs_in`

**Problem**: Povezano sa personId claim problemom

**Prioritet**: ?? Visok (autentifikacija je kriti?na)

---

### 4. Tours.Tests (19 Failed) ??

#### 4.1 Equipment Tests (2 failed)
- `EquipmentCommandTests.Creates` - Hardkodovan ID
- `EquipmentCommandTests.Deletes` - Reference problem

**Rešenje**:
```csharp
// Umesto:
result.Id.ShouldBe(30);

// Koristiti:
result.Id.ShouldNotBe(0);
result.Id.ShouldBeGreaterThan(0);
```

#### 4.2 Tour Command Tests (~10 failed)
- `Creates`, `Updates`, `Deletes`
- `Adds_equipment_to_tour` (multiple scenarios)
- `Deletes_keypoint`
- `Updates_map_marker_from_tour`
- `Delete_map_marker_from_tour`
- `Removes_equipment_from_tour`

**Problem**: Konkurentni pristup, nedostaju?i seed podaci, transakcijska izolacija

**Tipi?an scenario**:
```csharp
// Test 1 kreira tour sa ID 1
// Test 2 pokušava da pristupi tour sa ID 1
// Ali test 1 još nije završio transakciju
```

**Rešenje**:
1. Dodati `[Collection("Sequential")]` atribut
2. Koristiti `TransactionScope` za svaki test
3. Kreirati test-specific podatke umesto koriš?enja seed-ovanih

#### 4.3 Monument/MeetUp Tests (~4 failed)
- `MonumentCommandTests.Creates`, `Deletes`
- `MeetUpCommandTests.Creates`, `Deletes`
- `MeetUpQueryTests.Retrieves_all`

**Problem**: Nedostaju seed podaci za ove entitete

**Rešenje**: Dodati seed podatke ili kreirati ih u testovima

**Prioritet**: ?? Visok (core funkcionalnost)

---

## Korenski Uzroci Problema

### 1. Test Izolacija
- Testovi dele isto stanje baze
- Nedovoljna transakcijska izolacija
- Konkurentni pristup resursima

### 2. Seed Podaci
- Nedostaju potrebni podaci za neke testove
- Hardkodovani ID-jevi umesto dinami?kih
- O?ekivanja bazirana na seed podacima koji možda nisu u?itani

### 3. JWT Token u Testovima
- `personId` claim nije setovan u nekim scenarijima
- Možda problem sa repozitorijumom u test okruženju

---

## Plan Popravki (Prioritizovano)

### Faza 1: Kriti?no (Stakeholders Registration) ??
1. Debugovati `RegisterTourist` metodu u testu
2. Proveriti kreiranje Person entiteta
3. Proveriti JWT token generisanje
4. Dodati logovanje za debug

### Faza 2: Visok Prioritet (Tours Command Tests) ??
1. Dodati `TransactionScope` u testove
2. Kreirati test-specific podatke
3. Ukloniti hardkodovane ID-jeve
4. Dodati potrebne seed podatke za Monument/MeetUp

### Faza 3: Srednji Prioritet (Equipment Tests) ??
1. Zameniti hardkodovane ID-jeve sa dinami?kim proveriama
2. Popraviti Delete test sa proper cleanup-om

### Faza 4: Nizak Prioritet ??
1. Blog test
2. Architecture test (zahteva refaktorisanje modula)

---

## Ukupna Procena

**Vreme za kompletnu popravku**: ~4-6 sati

**Brza popravka (90%+ pass rate)**: ~2 sata
- Stakeholders registration (personId problem)
- Equipment hardkodovani ID-jevi
- Monument/MeetUp seed podaci

**Trenutni napredak**:
- Popravljeno: TourRatingTests (18 testova) ?
- Popravljeno: TouristFacilityTests ?
- Ukupno poboljšanje: +8.5% (od 83% do 91.5%)

---

## Preporuke

1. **Ne trošiti vreme na Architecture test** - zahteva refaktorisanje cele ProjectAutopsy funkcionalnosti

2. **Fokus na Stakeholders** - personId problem uti?e na mnogo testova

3. **Dodati bolju test izolaciju** - koristiti `IClassFixture` i `ICollectionFixture` pravilno

4. **Kreirati Helper metode** za kreiranje test podataka umesto oslanjanja na seed

5. **Dokumentovati test zavisnosti** - koji testovi dele podatke

