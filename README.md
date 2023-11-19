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

## Datenbankinitialisierung

Bevor Sie mit der Anwendung beginnen, stellen Sie sicher, dass keine bestehende Datenbank namens `JetStream_Backend` existiert, da der folgende Prozess eine neue Datenbank mit diesem Namen erstellt. Führen Sie die folgenden Befehle im Package Manager Console aus, um die Datenbank zu initialisieren:

1. Füge eine neue Migration hinzu:
   ```
   Add-Migration InitialCreate
   ```
2. Aktualisiere die Datenbank, um die Migration anzuwenden:
   ```
   Update-Database
   ```

Diese Befehle erstellen das Schema basierend auf dem aktuellen Datenmodell.