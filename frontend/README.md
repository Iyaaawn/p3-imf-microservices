# IMF Funding Management: Frontend

React + TypeScript single-page app (Vite), React Router for routing, axios for typed API calls, JWT auth.

## Run it locally

1. Start the API first (`dotnet run` in `backend/ImfFunding.Api`) and note its https URL.
2. In this folder, copy `.env.example` to `.env` and set `VITE_API_URL` to that URL.
3. Install and start:
   ```bash
   npm install
   npm run dev
   ```
4. Open http://localhost:5173

The API's CORS policy must allow http://localhost:5173.

## How it's organized

| File | Job |
|---|---|
| `src/types.ts` | TypeScript interfaces matching the C# DTOs |
| `src/api/client.ts` | One axios instance; an interceptor adds `Authorization: Bearer <token>` to every request |
| `src/api/endpoints.ts` | Typed functions for each API endpoint |
| `src/auth/AuthContext.tsx` | Login state shared app-wide with React Context (`useAuth()`) |
| `src/components/ProtectedRoute.tsx` | Sends logged-out users to `/login` |
| `src/pages/*` | Login, Register, Requests list (admin status buttons), New request form |
| `src/App.tsx` | The routes |
