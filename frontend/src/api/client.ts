import axios from "axios";

// One client per microservice: the browser talks to each API directly
export const countriesClient = axios.create({
  baseURL: `${import.meta.env.VITE_COUNTRIES_API ?? "http://localhost:5001"}/api`,
});

export const requestsClient = axios.create({
  baseURL: `${import.meta.env.VITE_REQUESTS_API ?? "http://localhost:5002"}/api`,
});

// Pulls a readable message out of an API error (ProblemDetails or plain text)
export function getErrorMessage(err: unknown): string {
  if (axios.isAxiosError(err)) {
    const data = err.response?.data;
    if (typeof data === "string") return data;
    if (data?.detail) return data.detail;
    if (data?.title) return data.title;
    if (!err.response) return "Can't reach the API. Is it running?";
  }
  return "Something went wrong.";
}