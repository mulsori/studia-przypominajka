## Wymagania

- .NET 10 SDK
- .NET MAUI Workload
- SQL Server
- Visual Studio Code

### Sprawdzenie instalacji

```bash
dotnet --version
dotnet workload list
```

Jeśli MAUI nie jest zainstalowane:

```bash
dotnet workload install maui
```

## Konfiguracja bazy danych

1. Utwórz bazę danych `Przypominajka` na instancji SQL Server.
2. Wykonaj skrypt:

```text
Database/init_database.sql
```

3. W razie potrzeby zmień connection string w `MauiProgram.cs`:

```csharp
Server=localhost\MSSQLSERVER01;
Database=Przypominajka;
Trusted_Connection=True;
TrustServerCertificate=True;
```

## Budowanie projektu

```bash
dotnet build -f net10.0-windows10.0.19041.0
```

## Uruchamianie projektu

```bash
dotnet run -f net10.0-windows10.0.19041.0
```