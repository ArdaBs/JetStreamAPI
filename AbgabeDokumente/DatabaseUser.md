# Datenbank Dokumentation

## Datenbankstruktur

### Tabellenbeschreibung
#### Tabelle: `ServiceOrders`
| Spalte          | Datentyp        | Beschreibung                    |
|-----------------|-----------------|---------------------------------|
| `Id`            | int             | Primärschlüssel der Tabelle     |
| `CustomerName`  | varchar(100)    | Name des Kunden                 |
| `Email`         | varchar(max)    | E-Mail des Kunden               |
| `PhoneNumber`   | varchar(max)    | Telefonnummer des Kunden        |
| `Priority`      | varchar(max)    | Priorität des Serviceauftrags   |
| `ServiceType`   | varchar(max)    | Art des Service                 |
| `CreationDate`  | datetime        | Erstellungsdatum des Auftrags   |
| `PickupDate`    | datetime        | Abholdatum des Auftrags         |
| `Comments`      | varchar(max)    | Kommentare zum Auftrag          |
| `Status`        | varchar(max)    | Status des Auftrags             |

#### Tabelle: `Employees`
| Spalte               | Datentyp        | Beschreibung                           |
|----------------------|-----------------|----------------------------------------|
| `EmployeeId`         | int             | Primärschlüssel der Tabelle            |
| `Username`           | varchar(100)    | Benutzername des Mitarbeiters          |
| `Password`           | varchar(max)    | Passwort des Mitarbeiters              |
| `IsLocked`           | boolean         | Sperrstatus des Benutzerkontos         |
| `FailedLoginAttempts`| int             | Anzahl der fehlgeschlagenen Anmeldungen|

## Benutzerberechtigungen

### Eingeschränkter Datenbankbenutzer

- **Benutzername:** `restricted_user`
- **Berechtigungen:** 
  - Lesen (`SELECT`) nur von der `ServiceOrder` Tabelle.
  - Einfügen (`INSERT`) und Aktualisieren (`UPDATE`) von Daten in der `ServiceOrder` Tabelle.
  - Keine Löschberechtigungen (`DELETE`).
  - Keine Berechtigungen zum Ändern der Datenbankstruktur (`ALTER`, `CREATE`, `DROP`).

## Setup und Konfiguration

### Erstellen des eingeschränkten Benutzers
```sql
-- Erstellen des Logins
CREATE LOGIN [restricted_user_login] WITH PASSWORD = '1234';

CREATE USER [restricted_user] FOR LOGIN [restricted_user_login];

-- Gewähren von Berechtigungen
GRANT SELECT, INSERT, UPDATE ON ServiceOrders TO [restricted_user];
```

### Konfiguration in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\;Database=JetStream_Backend;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Serilog-Konfiguration
- Logs werden im Ordner `Logs` gespeichert.
- Tägliche Aufteilung der Logdateien.
- Log-Level: Information

## Anmerkung zur Sicherheit und Datenschutz

- Passwörter werden nicht als Hashwerte gespeichert