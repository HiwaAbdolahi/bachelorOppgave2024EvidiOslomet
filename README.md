# Ansikt til ansikt med teknologi – Bachelorprosjekt 2024

## Prosjektoversikt

Dette prosjektet er en del av vår bacheloroppgave, utviklet i samarbeid med **Evidi AS** og **OsloMet** i løpet av vårsemesteret 2024. Vårt mål var å utvikle et digitalt innsjekkingssystem som benytter **Azure AI Services** for å tilby ansiktsgjenkjenning ved kontorinnsjekk. Løsningen inkluderer en webbasert **infotavle** som viser ansatte som er på kontoret, samt funksjonalitet for å opprette, endre og slette ansatte. Prosjektet ble utviklet av et team på fire personer:

- **Baldur Benedikt Bårdsson Indrevoll** 
- **Theodor Flatebø Jaarvik**
- **Trym Antonsen**
- **Hiwa Abdolahi**

### Hovedfunksjonalitet

- **Automatisk ansiktsgjenkjenning**: Ved hjelp av Azure Face API sjekkes ansatte automatisk inn ved ankomst og ut ved avreise, uten manuell interaksjon.
- **Infotavle**: Viser hvem som er på kontoret, med mulighet for filtrering, sortering og søk.
- **Administratorfunksjoner**: CRUD-funksjonalitet for administrasjon av ansatte (opprett, les, oppdater, slett).
- **Sanntidsoppdateringer**: Infotavlen oppdateres automatisk for å vise sanntidsdata om hvem som er på kontoret.
- **Sikkerhet**: Implementert ved hjelp av **Microsoft Identity**, med passordhashing og beskyttelse av brukerdata.

## Teknologier brukt

- **Back-end**: .NET Core
- **Front-end**: Razor Pages, Bootstrap, HTML, CSS, JavaScript
- **Database**: Azure SQL, Azure CosmosDB, Azure Blob Storage
- **API-er**: Azure Face API, Azure Computer Vision
- **Skytjenester**: Microsoft Azure for hosting og administrasjon
- **CI/CD**: Implementert ved hjelp av Azure DevOps for automatisk bygging og distribusjon (pipelines)
- **Versjonskontroll**: Azure DevOps for prosjektstyring og Git for versjonskontroll

## Installasjon og kjøring

1. **Klon repositoriet**:  
   `git clone https://github.com/HiwaAbdolahi/BachelorOppgave13.git`
2. **Installer nødvendige pakker** via NuGet.
3. **Kjør migrasjoner** for å opprette databasen:
   ```bash
   Update-Database
## Kjør applikasjonen

Kjør applikasjonen ved hjelp av Visual Studio eller en annen IDE med støtte for .NET-prosjekter.

### Azure Deployment
Følg instruksjonene i pipeline-filene (.yml) for distribusjon til Azure App Services.

## Brukerhistorier

### Bruker:
- Se hvem som er på kontoret, filtrere og sortere data etter avdeling, stilling og tid.

### Administrator:
- Opprette, endre, og slette ansattprofiler.
- Trene opp ansiktsgjenkjenning ved å legge til nye ansiktsbilder.

## Prosess og arbeidsmetoder

Vi benyttet **Scrum** som arbeidsmetodikk, og arbeidet var delt inn i fem sprinter:

1. **Sprint 1**: Implementering av Azure Face API og grunnleggende funksjoner for ansiktsgjenkjenning.
2. **Sprint 2**: Utvikling av infotavle med tilkobling til CosmosDB.
3. **Sprint 3**: CRUD-funksjonalitet for administrasjon av ansatte.
4. **Sprint 4**: Forbedring av design og brukergrensesnitt.
5. **Sprint 5**: Rapportskriving og ferdigstilling.

## Clean Architecture

Prosjektet følger prinsippene for **Clean Architecture**, med separasjon av logiske lag:

- **Core**: Inneholder domenelogikk.
- **Application**: Inneholder forretningslogikk og use cases.
- **Infrastructure**: Håndterer eksterne avhengigheter som API-er og databaser.
- **Presentation**: Inneholder UI og brukergrensesnitt.

## Pipeline og distribusjon

For å sikre kontinuerlig integrasjon og distribusjon (CI/CD) ble det satt opp pipelines ved hjelp av **Azure DevOps**. Dette inkluderer:

- **Byggefase**: Pakking av .NET-avhengigheter for både back-end og front-end.
- **Distribusjonsfase**: Deployment av applikasjonen til **Azure App Services** ved hjelp av **Azure Bicep**.

## Læringsutbytte

Gjennom dette prosjektet har vi lært mye om bruk av **Azure-tjenester**, integrering av ansiktsgjenkjenning og sikkerhet i skyen. Vi har også fått god erfaring med **CI/CD**, **Clean Code**, og utvikling av robuste **fullstack-løsninger**.

## Fremtidige forbedringer

- Integrering med eksterne kameraer for enda mer nøyaktig ansiktsgjenkjenning.
- Utvikling av en mobilapplikasjon som gjør det mulig å sjekke inn via telefon.
- Personalisering av infotavlen basert på roller og tilgangsnivåer.
