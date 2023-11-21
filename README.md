# Modul-295 Ski-Service Management Projekt

Willkommen im GitHub-Repository des Ski-Service Management-Projekts. Dieses Repository enthält alle Code-Dateien, Dokumentationen und Ressourcen, die für das Projekt benötigt werden.

## Projektstruktur

Das Repository ist wie folgt strukturiert:

  - [`/Controllers`](https://github.com/ArdaBs/JetStreamAPI/tree/master/JetStreamAPI/Controllers): Hier finden Sie die Controller für die Web-API.
  - [`/Models`](https://github.com/ArdaBs/JetStreamAPI/tree/master/JetStreamAPI/Models): Dieser Ordner enthält die Modelle unserer Anwendung.
  - [`/Services`](https://github.com/ArdaBs/JetStreamAPI/tree/master/JetStreamAPI/Services): Services, die in der Anwendung verwendet werden.
- [`/Docs`](https://github.com/ArdaBs/JetStreamAPI/tree/master/Docs): Dokumentation und sonstige relevante Dokumente wie Powerpoint oder Testprotokolle.
- [`/PostmanCollection`](https://github.com/ArdaBs/JetStreamAPI/tree/master/JetStreamAPI/PostmanCollection): Postman Collection V2.0. **Wichtig:**  Der Bearer Token muss angepasst werden. Um
 diese zu erhalten muss man sich einloggen mit den folgenden Daten:

```json
{
  "username": "Arda",
  "password": "1234"
}
```

## Datenbank initialisieren

### Schritt-für-Schritt-Anleitung zur Datenbankinitialisierung

Bevor Sie mit der Anwendung beginnen, stellen Sie sicher, dass keine bestehende Datenbank namens `JetStream_Backend` existiert, da der folgende Prozess eine neue Datenbank mit diesem Namen erstellt. Das Projekt wurde bereits mit benutzerdefinierten Befehlen migriert. Daher ist nur noch der Befehl `Update-Database` erforderlich.

1. Öffnen Sie das Projekt in Visual Studio, indem Sie auf die `.sln`-Datei des Projekts doppelklicken.
2. Sobald Visual Studio gestartet ist, öffnen Sie die `Developer PowerShell`, indem Sie im unteren Bereich von Visual Studio auf die entsprechende Option klicken.
3. Geben Sie den folgenden Befehl in die `Developer PowerShell` ein:

   ```powershell
   cd JetStreamAPI
   dotnet ef database update --connection "Server=localhost;Database=JetStream_Backend;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   ```

   Verwenden Sie diesen `cd`-Befehl, falls in der PowerShell nicht bereits `.../JetStreamAPI/JetStreamAPI>` angezeigt wird.

Durch Ausführen dieses Befehls wird die Datenbank basierend auf den vorhandenen Migrationen aktualisiert und für die Verwendung mit der Anwendung vorbereitet.

## Wie kommt man auf JetStream Website?

Nach dem starten des Backends und initalisieren der Datenbank, kann man dies eingeben:

```URL
https://localhost:7092/index.html
```