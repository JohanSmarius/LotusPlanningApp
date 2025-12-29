# User to Staff Member Linking System

## Overview

Dit document beschrijft hoe gebruikers (ApplicationUser) aan personeelsleden (Staff) worden gekoppeld en hoe de registratieprocedure is aangepast.

## Wijzigingen

### 1. ApplicationUser Model Uitgebreid
**Bestand**: `Infrastructure/Data/ApplicationUser.cs`

Nieuwe eigenschappen:
- `FirstName` - Voornaam van de gebruiker
- `LastName` - Achternaam van de gebruiker
- `StaffId` - Optionele verwijzing naar een personeelslid (Staff.Id)
- `FullName` - Berekende eigenschap die FirstName en LastName combineert

```csharp
public string? FirstName { get; set; }
public string? LastName { get; set; }
public int? StaffId { get; set; }
public string FullName => $"{FirstName} {LastName}".Trim();
```

### 2. Database Migratie
**Bestand**: `Infrastructure/Data/Migrations/20251229223757_AddUserNameAndStaffLink.cs`

Voegt drie kolommen toe aan de AspNetUsers tabel:
- `FirstName` (TEXT, nullable)
- `LastName` (TEXT, nullable)
- `StaffId` (INTEGER, nullable)

### 3. StaffRepository Uitgebreid
**Bestand**: `Infrastructure/StaffRepository.cs`

Nieuwe methode:
```csharp
public async Task<Entities.Staff?> GetStaffByEmailAsync(string email)
{
    return await _context.Staff
        .FirstOrDefaultAsync(s => s.Email == email);
}
```

Dit stelt je in staat om een personeelslid op basis van email-adres op te zoeken.

### 4. IStaffRepository Interface Bijgewerkt
**Bestand**: `Application/IStaffRepository.cs`

Toegevoegde methode signature:
```csharp
Task<Staff?> GetStaffByEmailAsync(string email);
```

### 5. Nieuwe UserStaffLinkingService
**Bestand**: `LotusPlanningApp/Services/UserStaffLinkingService.cs`

Deze service vereenvoudigt het koppelen/ontkoppelen van gebruikers aan personeelsleden:

```csharp
public interface IUserStaffLinkingService
{
    /// Koppel gebruiker aan personeelslid op basis van email
    Task<bool> LinkUserToStaffByEmailAsync(ApplicationUser user);
    
    /// Koppel gebruiker aan specifiek personeelslid
    Task<bool> LinkUserToStaffByIdAsync(string userId, int staffId);
    
    /// Ontkoppel gebruiker van personeelslid
    Task<bool> UnlinkUserFromStaffAsync(string userId);
}
```

### 6. Registratiepagina Aangepast
**Bestand**: `Components/Account/Pages/Register.razor`

Wijzigingen:
- Twee nieuwe velden toegevoegd: **First Name** en **Last Name** (beide verplicht)
- Deze worden ingevuld in het `InputModel`
- Bij gebruiker aanmaken worden FirstName en LastName ingesteld
- Na succesvolle registratie probeert het systeem automatisch de gebruiker aan een personeelslid te koppelen op basis van het email-adres

```csharp
// Set user properties
user.FirstName = Input.FirstName;
user.LastName = Input.LastName;

// Probeer automatisch aan personeelslid te koppelen
await UserStaffLinkingService.LinkUserToStaffByEmailAsync(user);
```

### 7. Gebruikersprofielpagina
**Bestand**: `Components/Account/Pages/Manage/Profile.razor`

Nieuwe pagina voor gebruikers om hun profiel te beheren:
- Voornaam en achternaam kunnen wijzigen
- Email-adres weergegeven (niet wijzigbaar)
- Gebruikersnaam weergegeven (niet wijzigbaar)
- **Status van personeelskoppeling**: Toont of de gebruiker aan een personeelslid is gekoppeld
  - Als gekoppeld: Toont Staff ID
  - Als niet gekoppeld: Toont waarschuwingsbericht

### 8. UserApprovals Dashboard Bijgewerkt
**Bestand**: `Components/Pages/UserApprovals.razor`

Wijzigingen voor beide tabellen (Pending & Approved):
- Toont nu **Full Name** in plaats van Username
- Toont First Name en Last Name samen
- Bootstrap iconen toegevoegd voor visuele verbetering

### 9. Service Registratie
**Bestand**: `Program.cs`

De `IUserStaffLinkingService` is geregistreerd als scoped service:
```csharp
builder.Services.AddScoped<IUserStaffLinkingService, UserStaffLinkingService>();
```

## Workflow

### Bij Registratie

```
1. Gebruiker vult formulier in:
   - Voornaam
   - Achternaam
   - Email
   - Wachtwoord

2. System maakt ApplicationUser aan met:
   - FirstName = ingevoerde voornaam
   - LastName = ingevoerde achternaam
   - Email = ingevoerde email
   - IsApproved = false
   - StaffId = null (initieel)

3. System probeert automatisch:
   - Zoek personeelslid met dezelfde email
   - Als gevonden: Koppel door StaffId in te stellen
   - Als niet gevonden: Gebruiker blijft ongekoppeld

4. Gebruiker krijgt melding van pending approval
```

### Na Admin Goedkeuring

```
1. Admin gaat naar /admin/user-approvals
2. Admin ziet gebruikers met volle naam
3. Admin klikt "Approve"
4. Gebruiker kan nu inloggen
5. Gebruiker kan naar /Account/Manage/profile gaan
6. Gebruiker ziet of ze gekoppeld zijn aan personeelslid
```

### Handmatige Koppeling (Optioneel)

Als een gebruiker niet automatisch is gekoppeld (omdat email niet overeenkomt), kan een admin dit handmatig doen met de service:

```csharp
// In een admin-pagina
await userStaffLinkingService.LinkUserToStaffByIdAsync(userId, staffId);
```

## Voorbeeld Scenario

### Scenario 1: Automatische Koppeling

1. Admin maakt een personeelslid aan:
   - Naam: Jan de Vries
   - Email: `jan.devries@lotus-tilburg.nl`

2. Jan registreert zich met:
   - Voornaam: Jan
   - Achternaam: de Vries
   - Email: `jan.devries@lotus-tilburg.nl`
   - Wachtwoord: ...

3. System:
   - Maakt gebruiker aan
   - Zoekt personeelslid met email `jan.devries@lotus-tilburg.nl`
   - Vindt Jan's staff record
   - Stelt StaffId in op Staff.Id

4. Resultaat: Gebruiker is automatisch gekoppeld! ✅

### Scenario 2: Geen Automatische Koppeling

1. Admin maakt personeelslid aan:
   - Naam: Marie Janssen
   - Email: `marie@lotus-tilburg.nl`

2. Marie registreert zich met:
   - Voornaam: Marie
   - Achternaam: Janssen
   - Email: `marie.j@lotus-tilburg.nl` (ander email!)
   - Wachtwoord: ...

3. System:
   - Maakt gebruiker aan
   - Zoekt personeelslid met email `marie.j@lotus-tilburg.nl`
   - Vindt geen match
   - StaffId blijft null

4. Resultaat: Gebruiker is NOT gekoppeld
   - Admin ziet warning in UserApprovals
   - Admin kan handmatig koppeling doen (in toekomst)

## Voordelen

✅ **Automatische Koppeling**: Gebruikers met hetzelfde email als personeelslid worden automatisch gekoppeld
✅ **Voornaam/Achternaam**: Betere UI met volledige naam in plaats van username
✅ **Profiel Management**: Gebruikers kunnen hun gegevens inzien
✅ **Transparantie**: Gebruikers weten of ze gekoppeld zijn
✅ **Toekomstbestendig**: Makkelijk om handmatige koppeling toe te voegen in admin interface

## Toekomstige Verbeteringen

- [ ] Admin interface voor handmatig koppelen van gebruikers aan personeelsleden
- [ ] Email notifications wanneer gebruiker wordt goedgekeurd
- [ ] Dashboard widget met aantal gekoppelde vs ongekoppelde gebruikers
- [ ] Bulk import van personeelsleden en automatische account aanmaak
- [ ] Audit log van alle koppelingen/ontkoppelingen

## Database Schema

```
AspNetUsers
├── Id (PK)
├── Email
├── FirstName (NEW)
├── LastName (NEW)
├── StaffId (NEW) → Foreign Key naar Staff.Id (optional)
├── IsApproved
├── ApprovedAt
├── ApprovedByUserId
├── RegisteredAt
└── ... (andere Identity velden)
```

## API Endpoints/Pages

| Route | Beschrijving | Toegang |
|-------|-------------|---------|
| `/Account/Register` | Registratieformulier | Iedereen |
| `/Account/PendingApproval` | Wachten op goedkeuring | Nieuwe gebruikers |
| `/Account/Manage/profile` | Profiel beheren | Ingelogde gebruikers |
| `/admin/user-approvals` | Gebruikers goedkeuren | Admin |

## Testing Checklist

- [ ] Registreer met voornaam, achternaam en email
- [ ] Verifieer dat First/Last Name worden opgeslagen
- [ ] Check dat AutoLink werkt (personeelslid met zelfde email)
- [ ] Verifieer PendingApproval pagina toont volle naam
- [ ] Log in als admin
- [ ] Ga naar /admin/user-approvals
- [ ] Verifieer dat volle naam wordt weergegeven
- [ ] Approve een gebruiker
- [ ] Log in als de nieuwe gebruiker
- [ ] Ga naar /Account/Manage/profile
- [ ] Verifieer dat profiel correct wordt weergegeven
- [ ] Test met gebruiker waarvan email niet overeenkomt met personeelslid
- [ ] Verifieer dat warning "Not yet linked" wordt weergegeven
