USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Przypominajka')
DROP DATABASE Przypominajka;
GO

CREATE DATABASE Przypominajka;
GO

USE Przypominajka;
GO

CREATE TABLE Kategorie (
    IdKategorii INT IDENTITY(1, 1) NOT NULL,
    NazwaKategorii NVARCHAR(50) NOT NULL,
    KolorEtykiety NVARCHAR(7) NOT NULL,
    CONSTRAINT PK_Kategorie PRIMARY KEY (IdKategorii),
    CONSTRAINT UQ_Kategorie_Nazwa UNIQUE (NazwaKategorii),
    CONSTRAINT CHK_Kategorie_KolorEtykiety CHECK (KolorEtykiety LIKE '#[0-9a-fA-F]%')
)

CREATE TABLE Serie (
    IdSerii INT IDENTITY(1, 1) NOT NULL,
    InterwalPowtarzania NVARCHAR(20) NOT NULL,
    CzyAktywna BIT NOT NULL CONSTRAINT DF_Serie_CzyAktywna DEFAULT 1,
    CONSTRAINT PK_Serie PRIMARY KEY (IdSerii),
    CONSTRAINT CHK_Serie_InterwalPowtarzania CHECK (InterwalPowtarzania IN ('codziennie', 'co tydzień', 'co miesiąc'))
)

CREATE TABLE Zadania (
    IdZadania INT IDENTITY(1, 1) NOT NULL,
    IdKategorii INT NULL,
    IdSerii INT NULL,
    NazwaZadania NVARCHAR(100) NOT NULL,
    OpisZadania NVARCHAR(MAX) NULL,
    TerminWykonania DATETIME2(0) NOT NULL,
    TerminPrzypomnienia DATETIME2(0) NOT NULL,
    StatusWykonania BIT NOT NULL CONSTRAINT DF_Zadania_StatusWykonania DEFAULT 0,
    CzyAktywnePrzypomnienie BIT NOT NULL CONSTRAINT DF_Zadania_CzyAktywnePrzypomnienie DEFAULT 1,
    InterwalPrzypominaniaPoTerminie INT NOT NULL,
    DataUtworzenia DATETIME2(0) NOT NULL CONSTRAINT DF_Zadania_DataUtworzenia DEFAULT SYSDATETIME(),
    DataModyfikacji DATETIME2(0) NOT NULL CONSTRAINT DF_Zadania_DataModyfikacji DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Zadania PRIMARY KEY (IdZadania),
    CONSTRAINT FK_Zadania_Kategorie FOREIGN KEY (IdKategorii) REFERENCES Kategorie(IdKategorii) ON DELETE SET NULL,
    CONSTRAINT FK_Zadania_Serie FOREIGN KEY (IdSerii) REFERENCES Serie(IdSerii) ON DELETE NO ACTION,
    CONSTRAINT CHK_Zadania_TerminPrzypomnienia CHECK (TerminPrzypomnienia <= TerminWykonania),
    CONSTRAINT CHK_Zadania_InterwalPrzypominaniaPoTerminie CHECK (InterwalPrzypominaniaPoTerminie > 0)
)

CREATE TABLE Powiadomienia (
    IdPowiadomienia INT IDENTITY(1, 1) NOT NULL,
    IdZadania INT NOT NULL,
    DataZaplanowania DATETIME2(0) NOT NULL,
    DataWyslania DATETIME2(0) NULL,
    StatusWyslania BIT NOT NULL CONSTRAINT DF_Powiadomienia_StatusWyslania DEFAULT 0,
    TrescPowiadomienia NVARCHAR(MAX) NOT NULL,
    CONSTRAINT PK_Powiadomienia PRIMARY KEY (IdPowiadomienia),
    CONSTRAINT FK_Powiadomienia_Zadania FOREIGN KEY (IdZadania) REFERENCES Zadania(IdZadania) ON DELETE CASCADE,
    CONSTRAINT CHK_Powiadomienia_Status_Logika CHECK (
        (StatusWyslania = 0 AND DataWyslania IS NULL) OR
        (StatusWyslania = 1 AND DataWyslania IS NOT NULL)
    )
)
