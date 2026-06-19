# Przypominajka – Aplikacja MAUI 10 + EF Core 10

## Krok 1 – Utwórz projekt

```bash
dotnet new maui -n Przypominajka
cd Przypominajka
```

Jeśli szablon MAUI nie jest dostępny, zainstaluj go najpierw:

```bash
dotnet workload install maui
```

---

## Krok 2 – Edytuj `Przypominajka.csproj`

Zastąp całą zawartość plikiem poniżej (aplikacja tylko dla Windows, bez pakowania MSIX – działa od razu z `dotnet run`):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0-windows10.0.19041.0</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>Przypominajka</RootNamespace>
    <AssemblyName>Przypominajka</AssemblyName>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <ApplicationTitle>Przypominajka</ApplicationTitle>
    <ApplicationId>com.przypominajka.app</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <WindowsPackageType>None</WindowsPackageType>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
  </PropertyGroup>

  <ItemGroup>
    <MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#512BD4" />
    <MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#512BD4" BaseSize="128,128" />
    <MauiFont Include="Resources\Fonts\*" />
    <MauiImage Include="Resources\Images\*" />
    <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Maui.Controls" Version="10.0.*" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.*" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

---

## Krok 3 – Zainstaluj pakiety

```bash
dotnet restore
```

---

## Krok 4 – Skopiuj pliki z tego przewodnika

Skopiuj kolejno wszystkie pliki z sekcji poniżej do właściwych folderów.

---

## Krok 5 – Uruchom aplikację

```bash
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

---

## Parametry połączenia (connection string)

Domyślnie aplikacja łączy się przez **LocalDB** (`(localdb)\MSSQLLocalDB`).  
Jeśli używasz SQL Server Express lub pełnego SQL Server, zmień connection string w `Data/AppDbContext.cs`:

| Instancja | Connection string |
|---|---|
| LocalDB | `Server=(localdb)\\MSSQLLocalDB;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server Express | `Server=.\\SQLEXPRESS;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server lokalny | `Server=localhost;Database=Przypominajka;Trusted_Connection=True;TrustServerCertificate=True;` |
