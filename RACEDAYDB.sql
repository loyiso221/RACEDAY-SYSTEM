CREATE DATABASE RaceDayDB;
  

IF DB_ID('RaceDayDB') IS NULL
BEGIN
    CREATE DATABASE RaceDayDB;
END
GO



IF OBJECT_ID('dbo.Results', 'U') IS NOT NULL
    DROP TABLE dbo.Results;

IF OBJECT_ID('dbo.EventEnrollments', 'U') IS NOT NULL
    DROP TABLE dbo.EventEnrollments;

IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL
    DROP TABLE dbo.Categories;

IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL
    DROP TABLE dbo.Events;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;

IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
    DROP TABLE dbo.Roles;
GO



CREATE TABLE Roles
(
    RoleID INT IDENTITY(1,1) PRIMARY KEY,

    RoleName VARCHAR(50) NOT NULL UNIQUE,

    Description VARCHAR(255) NULL
);
GO


CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,

    RoleID INT NOT NULL,

    FirstName VARCHAR(100) NOT NULL,

    LastName VARCHAR(100) NOT NULL,

    Email VARCHAR(150) NOT NULL UNIQUE,

    PasswordHash VARCHAR(255) NOT NULL,

    PhoneNumber VARCHAR(20) NULL,

    CreatedAt DATETIME NOT NULL
        DEFAULT GETDATE(),

    IsActive BIT NOT NULL
        DEFAULT 1,

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleID)
        REFERENCES Roles(RoleID)
);
GO



CREATE TABLE Events
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,

    OrganizerID INT NOT NULL,

    EventName VARCHAR(150) NOT NULL,

    Description VARCHAR(500) NULL,

    EventDate DATE NOT NULL,

    Location VARCHAR(200) NOT NULL,

    Status VARCHAR(30) NOT NULL
        DEFAULT 'Upcoming',

    CreatedAt DATETIME NOT NULL
        DEFAULT GETDATE(),

    CONSTRAINT FK_Events_Users
        FOREIGN KEY (OrganizerID)
        REFERENCES Users(UserID),

    CONSTRAINT CHK_Events_Status
        CHECK (Status IN
        (
            'Upcoming',
            'Open',
            'Closed',
            'Completed',
            'Cancelled'
        ))
);
GO



CREATE TABLE Categories
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    CategoryName VARCHAR(100) NOT NULL,

    DistanceKM DECIMAL(6,2) NOT NULL,

    MinimumAge INT NOT NULL,

    MaximumAge INT NULL,

    EntryFee DECIMAL(10,2) NOT NULL
        DEFAULT 0.00,

    CONSTRAINT FK_Categories_Events
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID)
        ON DELETE CASCADE,

    CONSTRAINT CHK_Categories_Distance
        CHECK (DistanceKM > 0),

    CONSTRAINT CHK_Categories_MinimumAge
        CHECK (MinimumAge >= 0),

    CONSTRAINT CHK_Categories_MaximumAge
        CHECK
        (
            MaximumAge IS NULL
            OR MaximumAge >= MinimumAge
        ),

    CONSTRAINT CHK_Categories_EntryFee
        CHECK (EntryFee >= 0),

    CONSTRAINT UQ_Categories_Event_Category
        UNIQUE (EventID, CategoryName)
);
GO




CREATE TABLE EventEnrollments
(
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,

    ParticipantID INT NOT NULL,

    CategoryID INT NOT NULL,

    EnrollmentDate DATETIME NOT NULL
        DEFAULT GETDATE(),

    EnrollmentStatus VARCHAR(30) NOT NULL
        DEFAULT 'Confirmed',

    BibNumber VARCHAR(20) NULL UNIQUE,

    CONSTRAINT FK_Enrollments_Users
        FOREIGN KEY (ParticipantID)
        REFERENCES Users(UserID),

    CONSTRAINT FK_Enrollments_Categories
        FOREIGN KEY (CategoryID)
        REFERENCES Categories(CategoryID),

    CONSTRAINT CHK_Enrollments_Status
        CHECK
        (
            EnrollmentStatus IN
            (
                'Pending',
                'Confirmed',
                'Cancelled',
                'Completed'
            )
        ),

    CONSTRAINT UQ_Enrollment
        UNIQUE (ParticipantID, CategoryID)
);
GO



CREATE TABLE Results
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,

    EnrollmentID INT NOT NULL UNIQUE,

    FinishTimeSeconds INT NULL,

    Position INT NULL,

    ResultStatus VARCHAR(30) NOT NULL
        DEFAULT 'Finished',

    RecordedAt DATETIME NOT NULL
        DEFAULT GETDATE(),

    CONSTRAINT FK_Results_Enrollments
        FOREIGN KEY (EnrollmentID)
        REFERENCES EventEnrollments(EnrollmentID)
        ON DELETE CASCADE,

    CONSTRAINT CHK_Results_FinishTime
        CHECK
        (
            FinishTimeSeconds IS NULL
            OR FinishTimeSeconds > 0
        ),

    CONSTRAINT CHK_Results_Position
        CHECK
        (
            Position IS NULL
            OR Position > 0
        ),

    CONSTRAINT CHK_Results_Status
        CHECK
        (
            ResultStatus IN
            (
                'Finished',
                'DNF',
                'Disqualified'
            )
        )
);
GO



INSERT INTO Roles
(
    RoleName,
    Description
)
VALUES
(
    'Administrator',
    'Manages the RaceDay system'
),
(
    'Organizer',
    'Creates and manages race events'
),
(
    'Participant',
    'Registers and participates in events'
);
GO




INSERT INTO Users
(
    RoleID,
    FirstName,
    LastName,
    Email,
    PasswordHash,
    PhoneNumber
)
VALUES

/* Administrator */

(
    1,
    'System',
    'Administrator',
    'admin@raceday.co.za',
    'HASHED_PASSWORD_ADMIN',
    '+27110000000'
),

/* Organizer 1 */

(
    2,
    'Sarah',
    'Mokoena',
    'sarah.mokoena@raceday.co.za',
    'HASHED_PASSWORD_SARAH',
    '+27111234567'
),

/* Organizer 2 */

(
    2,
    'David',
    'Nkosi',
    'david.nkosi@raceday.co.za',
    'HASHED_PASSWORD_DAVID',
    '+27117654321'
),

/* Participant 1 */

(
    3,
    'Thabo',
    'Dlamini',
    'thabo.dlamini@email.com',
    'HASHED_PASSWORD_THABO',
    '+27821234567'
),

/* Participant 2 */

(
    3,
    'Lerato',
    'Khumalo',
    'lerato.khumalo@email.com',
    'HASHED_PASSWORD_LERATO',
    '+27827654321'
),

/* Additional Participant */

(
    3,
    'Sipho',
    'Mthembu',
    'sipho.mthembu@email.com',
    'HASHED_PASSWORD_SIPHO',
    '+27829876543'
);
GO



INSERT INTO Events
(
    OrganizerID,
    EventName,
    Description,
    EventDate,
    Location,
    Status
)
VALUES

(
    2,
    'Johannesburg City Run',
    'Annual road running event in Johannesburg',
    '2026-11-15',
    'Johannesburg',
    'Open'
),

(
    2,
    'Pretoria Marathon',
    'Annual marathon event in Pretoria',
    '2026-12-05',
    'Pretoria',
    'Open'
),

(
    3,
    'Cape Town Coastal Race',
    'Scenic coastal running event',
    '2027-01-20',
    'Cape Town',
    'Open'
);
GO




/* Johannesburg City Run */

INSERT INTO Categories
(
    EventID,
    CategoryName,
    DistanceKM,
    MinimumAge,
    MaximumAge,
    EntryFee
)
VALUES
(
    1,
    '5 KM Fun Run',
    5.00,
    12,
    NULL,
    100.00
),
(
    1,
    '10 KM Race',
    10.00,
    16,
    NULL,
    150.00
),
(
    1,
    '21 KM Half Marathon',
    21.10,
    18,
    NULL,
    250.00
);


/* Pretoria Marathon */

INSERT INTO Categories
(
    EventID,
    CategoryName,
    DistanceKM,
    MinimumAge,
    MaximumAge,
    EntryFee
)
VALUES
(
    2,
    '10 KM Race',
    10.00,
    16,
    NULL,
    150.00
),
(
    2,
    '21 KM Half Marathon',
    21.10,
    18,
    NULL,
    300.00
),
(
    2,
    '42 KM Marathon',
    42.20,
    18,
    NULL,
    450.00
);


/* Cape Town Coastal Race */

INSERT INTO Categories
(
    EventID,
    CategoryName,
    DistanceKM,
    MinimumAge,
    MaximumAge,
    EntryFee
)
VALUES
(
    3,
    '5 KM Coastal Run',
    5.00,
    12,
    NULL,
    120.00
),
(
    3,
    '10 KM Coastal Race',
    10.00,
    16,
    NULL,
    180.00
),
(
    3,
    '21 KM Coastal Challenge',
    21.10,
    18,
    NULL,
    320.00
);
GO



INSERT INTO EventEnrollments
(
    ParticipantID,
    CategoryID,
    EnrollmentStatus,
    BibNumber
)
VALUES

(
    4,
    1,
    'Confirmed',
    'JHB001'
),

(
    5,
    2,
    'Confirmed',
    'JHB002'
),

(
    6,
    3,
    'Confirmed',
    'JHB003'
),

(
    4,
    4,
    'Confirmed',
    'PTA001'
),

(
    5,
    5,
    'Confirmed',
    'PTA002'
),

(
    6,
    7,
    'Confirmed',
    'CPT001'
);
GO




INSERT INTO Results
(
    EnrollmentID,
    FinishTimeSeconds,
    Position,
    ResultStatus
)
VALUES

(
    1,
    1620,
    1,
    'Finished'
),

(
    2,
    3240,
    2,
    'Finished'
),

(
    3,
    7200,
    3,
    'Finished'
);
GO




/* Display all users and roles */

SELECT
    U.UserID,
    U.FirstName,
    U.LastName,
    U.Email,
    R.RoleName
FROM Users U
INNER JOIN Roles R
    ON U.RoleID = R.RoleID;
GO


/* Display events and organisers */

SELECT
    E.EventID,
    E.EventName,
    E.EventDate,
    E.Location,
    E.Status,
    U.FirstName + ' ' + U.LastName AS Organizer
FROM Events E
INNER JOIN Users U
    ON E.OrganizerID = U.UserID;
GO


/* Display categories for each event */

SELECT
    E.EventName,
    C.CategoryName,
    C.DistanceKM,
    C.EntryFee
FROM Categories C
INNER JOIN Events E
    ON C.EventID = E.EventID
ORDER BY E.EventName, C.DistanceKM;
GO


/* Display participant enrolments */

SELECT
    U.FirstName + ' ' + U.LastName AS Participant,
    E.EventName,
    C.CategoryName,
    EN.BibNumber,
    EN.EnrollmentStatus
FROM EventEnrollments EN
INNER JOIN Users U
    ON EN.ParticipantID = U.UserID
INNER JOIN Categories C
    ON EN.CategoryID = C.CategoryID
INNER JOIN Events E
    ON C.EventID = E.EventID;
GO


/* Display race results */

SELECT
    U.FirstName + ' ' + U.LastName AS Participant,
    E.EventName,
    C.CategoryName,
    R.FinishTimeSeconds,
    R.Position,
    R.ResultStatus
FROM Results R
INNER JOIN EventEnrollments EN
    ON R.EnrollmentID = EN.EnrollmentID
INNER JOIN Users U
    ON EN.ParticipantID = U.UserID
INNER JOIN Categories C
    ON EN.CategoryID = C.CategoryID
INNER JOIN Events E
    ON C.EventID = E.EventID
ORDER BY E.EventName, R.Position;
GO