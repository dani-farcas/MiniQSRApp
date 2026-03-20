CREATE TABLE IF NOT EXISTS Clients (
    ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    City TEXT,
    BirthDate TEXT
);

CREATE TABLE IF NOT EXISTS Accounts (
    AccountId INTEGER PRIMARY KEY AUTOINCREMENT,
    ClientId INTEGER,
    AccountNumber TEXT,
    Balance REAL,
    OpenedDate TEXT
);

CREATE TABLE IF NOT EXISTS Categories (
    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT
);

CREATE TABLE IF NOT EXISTS Transactions (
    TransactionId INTEGER PRIMARY KEY AUTOINCREMENT,
    AccountId INTEGER,
    Description TEXT,
    BookingDate TEXT,
    Amount REAL,
    CategoryId INTEGER
);

INSERT INTO Clients (Name, City, BirthDate)
SELECT 'Anna Müller', 'Berlin', '1990-05-12'
WHERE NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = 1);

INSERT INTO Clients (Name, City, BirthDate)
SELECT 'David Schmidt', 'Hamburg', '1985-09-21'
WHERE NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = 2);

INSERT INTO Clients (Name, City, BirthDate)
SELECT 'Laura Becker', 'München', '1993-02-03'
WHERE NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = 3);

INSERT INTO Clients (Name, City, BirthDate)
SELECT 'Sofia Wagner', 'Köln', '1988-11-14'
WHERE NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = 4);

INSERT INTO Clients (Name, City, BirthDate)
SELECT 'Lukas Hoffmann', 'Frankfurt', '1995-07-30'
WHERE NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = 5);

INSERT INTO Accounts (AccountId, ClientId, AccountNumber, Balance, OpenedDate)
SELECT 1, 1, 'DE001', 2500.50, '2020-01-10'
WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = 1);

INSERT INTO Accounts (AccountId, ClientId, AccountNumber, Balance, OpenedDate)
SELECT 2, 2, 'DE002', 1800.00, '2019-06-15'
WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = 2);

INSERT INTO Accounts (AccountId, ClientId, AccountNumber, Balance, OpenedDate)
SELECT 3, 3, 'DE003', 3200.75, '2021-03-22'
WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = 3);

INSERT INTO Accounts (AccountId, ClientId, AccountNumber, Balance, OpenedDate)
SELECT 4, 4, 'DE004', 1500.20, '2018-11-05'
WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = 4);

INSERT INTO Accounts (AccountId, ClientId, AccountNumber, Balance, OpenedDate)
SELECT 5, 5, 'DE005', 4100.00, '2022-07-19'
WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountId = 5);

INSERT INTO Categories (CategoryId, CategoryName)
SELECT 1, 'Einkauf'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 1);

INSERT INTO Categories (CategoryId, CategoryName)
SELECT 2, 'Miete'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 2);

INSERT INTO Categories (CategoryId, CategoryName)
SELECT 3, 'Gehalt'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 3);

INSERT INTO Categories (CategoryId, CategoryName)
SELECT 4, 'Freizeit'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 4);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 1, 1, 'Supermarkt', '2024-01-10', -50.25, 1
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 1);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 2, 1, 'Gehalt Januar', '2024-01-01', 2500.00, 3
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 2);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 3, 2, 'Miete Januar', '2024-01-03', -800.00, 2
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 3);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 4, 3, 'Kino', '2024-01-15', -20.00, 4
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 4);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 5, 4, 'Supermarkt', '2024-01-12', -60.00, 1
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 5);

INSERT INTO Transactions (TransactionId, AccountId, Description, BookingDate, Amount, CategoryId)
SELECT 6, 5, 'Gehalt Januar', '2024-01-01', 3000.00, 3
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE TransactionId = 6);