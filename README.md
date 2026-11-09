# RaceDay POE – Part 3

ASP.NET Core 8 MVC front end integrated with the existing RaceDay Part 2 REST API. The MVC application does **not** access SQL Server directly. All business/data operations are sent through `RaceDay.Api`.

## Solution structure

- `RaceDay.Api` – Part 2 API, extended with Azure Blob Storage endpoints for event banners and participant profile pictures.
- `RaceDay.Web` – Part 3 MVC website with session-aware navigation, login/registration, organiser dashboard, event/category management, enrolments, result capture, participant event browsing, enrolment/results history and profile editing.
- `RaceDay.Tests` – existing Part 2 unit tests.
- `scripts/Part3_AddImageColumns.sql` – additive SQL script for the two image URL columns.

## Requirements

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- SQL Server / SQL Server Express and the existing `RaceDayDB` from Part 1
- Azure Storage account for image upload features
- Docker Desktop for container testing

## 1. Configure Part 2 API database connection

Open `RaceDay.Api/appsettings.json`. Replace `ADD_YOUR_SQL_SERVER_HERE` with your SQL Server instance name. Example JSON (the backslash is doubled):

```json
"RaceDay": "Server=YOUR_INSTANCE\\SQLEXPRESS;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

The project deliberately does not contain a pre-filled server instance. Use Windows Authentication and confirm that `RaceDayDB` exists and that your Windows account has access. This project expects the Part 1 tables and columns used by the supplied Part 2 API; inspect your original Part 1 SQL script before running database updates.

## 2. Add the image columns to the existing database

Back up `RaceDayDB`, then run `scripts/Part3_AddImageColumns.sql` in SSMS. It only adds `Events.BannerImageUrl` and `Users.ProfileImageUrl` if they do not exist. Do not run an EF migration blindly against the coursework database.

## 3. Configure Azure Blob Storage

In a terminal:

```powershell
cd RaceDay.Api
dotnet user-secrets init
dotnet user-secrets set "AzureBlob:ConnectionString" "YOUR_AZURE_STORAGE_CONNECTION_STRING"
dotnet user-secrets set "AzureBlob:ContainerName" "raceday-media"
```

The API creates a private blob container and returns a read-only seven-day SAS URL. Accepted images are JPG/JPEG, PNG and WEBP, up to 5 MB. Never commit storage credentials.

## 4. Restore, build and run

From the repository root:

```bash
dotnet restore RaceDay.sln
dotnet build RaceDay.sln
dotnet test RaceDay.Tests/RaceDay.Tests.csproj
```

Start `RaceDay.Api` first:

```bash
dotnet run --project RaceDay.Api/RaceDay.Api.csproj
```

The supplied launch profile uses `http://localhost:5080`. Start a second terminal and run:

```bash
dotnet run --project RaceDay.Web/RaceDay.Web.csproj
```

The MVC launch profile uses `http://localhost:5081`. `RaceDay.Web/appsettings.json` has `ApiSettings:BaseUrl` set to `http://localhost:5080/`; change it to the actual API address printed by your run profile if needed.

## 5. Authentication integration

The API sets `.RaceDay.Session`. The MVC server stores that API cookie value in its own server-side session and forwards it on subsequent API requests. The browser receives only the MVC session cookie. Role names expected from Part 2 are `Organizer` and `Participant`. Public registration deliberately excludes `Administrator`.

## 6. Docker – MVC container

Build from the repository root:

```bash
docker build -f RaceDay.Web/Dockerfile -t raceday-web .
```

Run (replace the API URL with an address reachable **from inside the container**):

```bash
docker run --rm -p 8080:8080 -e ApiSettings__BaseUrl="http://host.docker.internal:5080/" raceday-web
```

On Linux, `host.docker.internal` may need `--add-host=host.docker.internal:host-gateway`. Do not use `localhost` for the API address from inside the MVC container because it points to the MVC container itself. For deployment, use a Docker network and the API service DNS name.

## API routes used

`POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/me`, `GET /api/events`, `GET /api/events/{id}`, `POST/PUT/DELETE /api/events`, `GET/POST /api/events/{eventId}/categories`, `POST /api/enrollments`, `GET /api/enrollments/mine`, `GET /api/enrollments/event/{eventId}`, `POST /api/enrollments/{enrollmentId}/result`, `GET /api/results/mine`, `GET/PUT /api/profile/me` plus the Part 3 extensions `POST /api/events/{eventId}/banner` and `POST /api/profile/me/picture`.

## Important integration notes / verify before submission

- The Part 2 API and database schema supplied in earlier work were not accompanied by a verified Part 1 SQL creation script in this build session. Confirm exact table/column names, identity settings and foreign keys against your Part 1 database.
- `RaceDay.Api` must be running before the MVC app can authenticate or retrieve data.
- Azure upload features require valid Azure Storage configuration and the two SQL columns above.
- API route payloads and session cookie names are based on the Part 2 source supplied earlier. If you changed Part 2, align the MVC client accordingly.
- Build and test on your Windows/Visual Studio machine before submission. This environment did not have the .NET SDK installed, so a local compile/test result cannot be claimed here.
- The assignment asks for at least 20 meaningful Git commits. Use real incremental commits as you implement and test each feature; do not manufacture empty commits. Suggested milestones: MVC project, routing/session, API client, cookie forwarding, registration, login/logout, role navigation, organiser dashboard, event create, event edit/delete, categories, enrolment list, results capture, participant browse, event detail, participant enrolment, enrolment history, result history, profile, Blob API, upload UI, responsive styling, validation, Docker, test fixes and README.

## GitHub submission

```bash
git init
git add .
git commit -m "Add RaceDay Part 3 MVC application"
git branch -M main
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
git push -u origin main
```
