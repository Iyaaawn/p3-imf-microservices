# IMF Funding Microservices (Project 3)

Iyan & Natalie · due Fri Nov 6, 2026

We split our IMF funding app into **three small ASP.NET Core APIs**, each with its own database, each running in its own Docker container:

| Service | Its one job | Owner | Port on your laptop |
|---|---|---|---|
| **Countries API** | Member country registry | Iyan | 5001 |
| **Requests API** | Funding requests + approve/reject | Iyan (Natalie adds `Clients/` + the create endpoint) | 5002 |
| **Eligibility API** | Lending rule: max 200% of quota | Natalie | 5003 |

Only the Requests API calls the other two. Countries and Eligibility never call anyone.

---

## 👋 Natalie: update from Iyan (Wed Nov 4)

### What's done
- **Repo skeleton:** `services/` folder with one folder per API, `.gitignore`, `.env.example`. `main` is protected, so every change goes through a PR.
- **Requests API** (`services/requests-api`)
  - `FundingRequest` model: `CountryCode`, `CountryName` (snapshot), `Fund`, `Amount`, `Status`, `EligibilityReason`, `CreatedAt`, `DecidedAt`. **No foreign key** to Countries.
  - Seeded with 2 test requests (Ghana and Kenya, both Submitted).
  - Endpoints: list, get by id, approve, reject. Approve/reject only work on a **Submitted** request, otherwise 409.
  - `/health` checks the database.
  - **Not done on purpose:** `POST /api/requests` (create). That's yours, because it needs your HTTP clients.
- **Countries API** (`services/countries-api`)
  - `Country` model keyed by its 3-letter **code**: `Code`, `Name`, `IsActive`.
  - Seeded with 6 countries. **Pakistan is inactive on purpose**, so we can test the "Country is not active" error.
  - Endpoints: list (`?activeOnly=true`), get by code, create, update. Plus `/health`.
- **Dockerfiles** for Countries and Requests. Both build, and both run in containers next to a SQL Server container. Tested in Swagger.

### Contracts you'll code against

**Countries: the call your CountriesClient makes**
```
GET /api/countries/{code}      → 200 { "code": "GHA", "name": "Ghana", "isActive": true }
                               → 404 if the code doesn't exist
```

**Requests**
```
GET  /api/requests              → list, newest first
GET  /api/requests/{id}         → one request (controller method name: GetById)
POST /api/requests/{id}/approve → 204, 404, or 409 if not Submitted
POST /api/requests/{id}/reject  → 204, 404, or 409 if not Submitted
POST /api/requests              → YOURS: return CreatedAtAction(nameof(GetById), new { id }, dto)
```

**Statuses** (sent as text): `Submitted`, `PendingEligibility`, `Approved`, `Rejected`

**Funds:** the real IMF facilities `SBA`, `EFF`, `ECF`, `FCL`, `RSF`

**Amount unit: millions of SDR.** `900` means 900 million SDR.

**Country codes + quotas** (Countries uses the codes; your Eligibility API owns the quotas):

| Code | Country | Quota (m SDR) | Max allowed (200%) |
|---|---|---|---|
| GHA | Ghana | 738.0 | 1,476.0 |
| KEN | Kenya | 542.8 | 1,085.6 |
| ARG | Argentina | 3,187.3 | 6,374.6 |
| EGY | Egypt | 2,037.1 | 4,074.2 |
| PAK | Pakistan | 2,031.0 | 4,062.0 |
| LKA | Sri Lanka | 578.8 | 1,157.6 |

**Proposal for Eligibility (please confirm):** a `CountryQuota` table (`CountryCode`, `QuotaSdrMillions`). The rule is *eligible if amount ≤ quota × 2*, for any fund. `maxAllowed` in the response = quota × 2.

### Where your code plugs into the Requests API
- Put your typed clients in `services/requests-api/Clients/`.
- Switch them on with **one line** in `Program.cs`, in the services section:
  `builder.Services.AddDownstreamClients(builder.Configuration);`
  Keeping everything else in `Clients/` means we won't get merge conflicts.
- Connection string key for Requests: `ConnectionStrings:RequestsDb`. Countries uses `ConnectionStrings:CountriesDb`.

### What I still need from you
1. Confirm the amount unit (millions of SDR) and the CountryQuota idea.
2. Review my open PRs (Requests, Countries, Dockerfiles).
3. When you build `docker-compose.yml`, it can use my Dockerfiles as they are.

---

## Running the APIs in Docker on your laptop

### Step 1: Install Docker Desktop (Windows)
1. Download from **docker.com/products/docker-desktop** → *Download for Windows* (AMD64).
2. Install it, leaving **"Use WSL 2"** checked. **Restart** when it asks.
3. Open **Docker Desktop**. Accept the terms; signing in is optional.
4. **If it says WSL isn't installed:** open **PowerShell as Administrator** and run:
   ```
   wsl --install
   ```
   Restart, then open Docker Desktop again.
5. **If `wsl` (or `dism.exe`) says "not recognized":** your Windows PATH is missing system folders. This happened to Iyan. Use the full paths instead (admin PowerShell), then restart:
   ```
   C:\Windows\System32\dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart
   C:\Windows\System32\dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart
   C:\Windows\System32\wsl.exe --install
   ```
6. Check it works in a **new** terminal:
   ```
   docker --version
   docker run hello-world
   ```
   "Hello from Docker!" means you're ready.

> **On a Mac?** Download the Mac version (Apple chip or Intel). On an Apple-chip Mac, go to Docker Desktop → Settings → General and turn on **"Use Rosetta for x86/amd64 emulation"**, because SQL Server needs it.
>
> **Either way:** Docker Desktop → Settings → Resources → give it **at least 2 GB of memory** for SQL Server.

### Step 2: Get the code
```
git clone https://github.com/Iyaaawn/p3-imf-microservices.git
cd p3-imf-microservices
```
(Or `git pull` on `main` if you already have it.)

### Step 3: Start SQL Server in a container
Run these once. They create a private network and a SQL Server container on it:
```
docker network create imf-net
docker run -d --name sqlserver --network imf-net -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=ChangeMe_123! mcr.microsoft.com/mssql/server:2022-latest
```
Wait about 20 seconds for SQL to boot. Later, use `docker start sqlserver` / `docker stop sqlserver`.

### Step 4: Build and run an API
**Countries** (port 5001):
```
cd services/countries-api
docker build -t countries-api .
docker run --rm --name countries-api --network imf-net -p 5001:8080 -e ConnectionStrings__CountriesDb="Server=sqlserver,1433;Database=CountriesDb;User Id=sa;Password=ChangeMe_123!;TrustServerCertificate=True" countries-api
```

**Requests** (port 5002), in a second terminal:
```
cd services/requests-api
docker build -t requests-api .
docker run --rm --name requests-api --network imf-net -p 5002:8080 -e ConnectionStrings__RequestsDb="Server=sqlserver,1433;Database=RequestsDb;User Id=sa;Password=ChangeMe_123!;TrustServerCertificate=True" requests-api
```

The database creates itself on startup (migrations run automatically), with the seed data.

### Step 5: Test in the browser
- Countries: `http://localhost:5001/swagger` and `http://localhost:5001/health`
- Requests: `http://localhost:5002/swagger` and `http://localhost:5002/health`

| Try | Expect |
|---|---|
| `GET /api/countries` | 6 countries |
| `GET /api/countries?activeOnly=true` | 5 (no Pakistan) |
| `GET /api/countries/gha` | Ghana |
| `GET /api/countries/XYZ` | 404 |
| `GET /api/requests` | Ghana and Kenya, Submitted |
| `POST /api/requests/1/approve` | 204, then 409 if you try again |
| `/health` on either | `Healthy` |

**Ctrl+C** stops an API. `docker ps` shows what's running.

### What the pieces mean (one line each)
- **Dockerfile:** the recipe for packaging one API. Ours is *multi-stage*: build with the big .NET SDK image, then run on the small ASP.NET runtime image.
- **Image:** what `docker build` produces. **Container:** a running copy of an image.
- **`--network imf-net`:** puts containers on one private network, so an API reaches SQL by the name `sqlserver`.
- **`-p 5001:8080`:** your laptop's port 5001 → the container's port 8080 (.NET listens on 8080 in containers).
- **`-e ConnectionStrings__CountriesDb=...`:** an environment variable that overrides appsettings. `__` stands for `:`.

### Things that tripped me up (so they don't trip you)
- **`failed to read dockerfile`:** the file must be named exactly `Dockerfile`, with no `.txt` and saved (not empty).
- **`The application 'X.dll' does not exist`:** the last line of the Dockerfile must match the **.csproj name**, not the folder name: `RequestsApi.dll`, not `requests-api.dll`.
- **SQL connection errors right after starting:** SQL is still booting. Retry-on-failure handles it; if the API exits, run it again.
- **`localhost` inside a container means that container.** Containers talk to each other by name (`sqlserver`, `countries-api`). `localhost:5001` is only for your browser.

---

## Repo layout
```
p3-imf-microservices/
├── services/
│   ├── countries-api/      (Iyan)  Controllers, Data, Dtos, Models, Dockerfile
│   ├── requests-api/       (Iyan; Natalie adds Clients/)
│   └── eligibility-api/    (Natalie)
├── docker-compose.yml      (Natalie, coming soon)
├── .env.example
└── README.md
```

**Git workflow:** branch → PR → the other person reviews → merge. Never commit `.env` or a real password.
