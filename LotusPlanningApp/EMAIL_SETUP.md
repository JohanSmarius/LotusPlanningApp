# E-mailnotificaties instellen voor personeelsinzet

Deze applicatie verstuurt automatisch e-mailnotificaties naar medewerkers wanneer ze aan diensten worden toegewezen. De e-mails bevatten details over de opdracht, de dienst en belangrijke instructies.

## E-mailconfiguratie

### 1. SMTP-instellingen

Configureer de SMTP-instellingen in `appsettings.json` of `appsettings.Development.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "LOTUS Planning App",
    "EnableSsl": "true"
  }
}
```

### 2. Gmail-configuratie

Bij gebruik van Gmail:

1. Schakel 2FA in op je Google-account
2. Genereer een "App-wachtwoord" voor de applicatie
3. Gebruik het app-wachtwoord (niet je gewone wachtwoord) in `SmtpPassword`
4. Stel `SmtpHost` in op `smtp.gmail.com`
5. Stel `SmtpPort` in op `587`
6. Stel `EnableSsl` in op `true`

### 3. Andere e-mailproviders

Voor andere providers gebruik je de juiste SMTP-instellingen:

- **Outlook/Hotmail**: `smtp-mail.outlook.com`, poort 587
- **Yahoo**: `smtp.mail.yahoo.com`, poort 587
- **Office 365**: `smtp.office365.com`, poort 587

## E-mailsjablonen

De notificatie-e-mail bevat:

- **Opdrachtdetails**: naam, locatie, duur, contactgegevens
- **Dienstdetails**: naam, tijd, duur, beschrijving
- **Rol medewerker**: gekleurde badge met rol
- **Belangrijke notities**: aankomstinstructies, benodigdheden
- **Professionele HTML-opmaak**: mobiel-responsief ontwerp

## Functionaliteit

### Automatisch e-mailen

- **Toewijzing maken**: e-mail wordt verstuurd zodra een medewerker aan een dienst wordt gekoppeld
- **Rijke HTML-content**: professionele opmaak met alle details
- **Mobiel responsief**: werkt op alle apparaten
- **Foutafhandeling**: mislukte e-mail blokkeert toewijzing niet
- **Logging**: gedetailleerde logs voor troubleshooting

### Inhoud van de e-mail

Elke notificatie bevat:

1. **Persoonlijke aanhef** met naam van de medewerker
2. **Opdrachtinformatie**:
   - Naam en locatie
   - Duur van de opdracht
   - Contactpersoon en telefoon
   - Omschrijving

3. **Dienstdetails**:
   - Naam en tijd
   - Berekening van de duur
   - Beschrijving van de dienst
   - Rol met kleurcodering

4. **Belangrijke instructies**:
   - Kom 15 minuten eerder
   - Neem certificeringsdocumenten mee
   - Kledingvoorschrift
   - Contactgegevens voor vragen

## E-mailconfiguratie testen

Test de instellingen zo:

1. Wijs een medewerker toe aan een dienst
2. Controleer in de applicatielogs of de e-mail is verstuurd
3. Kijk of de medewerker de notificatie ontvangt

## Probleemoplossing

### Veelvoorkomende issues

1. **E-mail wordt niet verzonden**:
   - Controleer SMTP-gegevens
   - Verifieer netwerkconnectiviteit
   - Check firewall-instellingen
   - Bekijk de applicatielogs

2. **Gmail-authenticatiefouten**:
   - Zorg dat 2FA aanstaat
   - Gebruik het app-wachtwoord, niet het normale wachtwoord
   - Controleer Google-beveiligingsinstellingen

3. **E-mail komt in spam**:
   - Voeg afzender toe aan contacten
   - Stel SPF/DKIM in bij een eigen domein

### Logging

De applicatie logt e-mailactiviteiten:

- `Information`: e-mail verzenden geslaagd
- `Warning`: e-mailinstellingen niet geconfigureerd
- `Error`: verzenden mislukt

Bekijk de logs in de console of stel een loggingprovider in voor monitoring.

## Beveiliging

- **Commit nooit echte wachtwoorden**
- Gebruik **omgevingsvariabelen** of **Key Vault** in productie
- Overweeg **managed identity** bij cloud-deployment
- **Versleutel gevoelige configuratie**

## E-mailnotificaties uitschakelen

Uitschakelen zonder code te verwijderen:

1. Laat e-mailinstellingen leeg in de configuratie
2. De service logt een waarschuwing en slaat verzenden over
3. Personeelstoewijzingen blijven werken

## Productie-deploy

Voor productie:

1. Gebruik **SendGrid**, **AWS SES** of vergelijkbare dienst
2. Zet **omgevingsvariabelen** voor gevoelige waarden
3. Richt **monitoring** in voor verzendfouten
4. Overweeg **rate limiting** om spam te voorkomen

## Voorbeeldconfiguratie productie

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUsername": "apikey",
    "SmtpPassword": "SG.your-sendgrid-api-key",
    "FromEmail": "noreply@yourcompany.com",
    "FromName": "LOTUS Planning App",
    "EnableSsl": "true"
  }
}
```

Deze implementatie levert een robuust, professioneel e-mailsysteem dat de communicatie met je LOTUS-team verbetert.