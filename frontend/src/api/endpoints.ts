import { countriesClient, requestsClient } from "./client";
import type { Country, CreateFundingRequest, FundingRequest } from "../types";

// Countries API (port 5001)
export const countriesApi = {
  list: (activeOnly = true) =>
    countriesClient.get<Country[]>("/countries", { params: { activeOnly } }).then(r => r.data),
};

// Requests API (port 5002)
export const requestsApi = {
  list: () => requestsClient.get<FundingRequest[]>("/requests").then(r => r.data),
  get: (id: number) => requestsClient.get<FundingRequest>(`/requests/${id}`).then(r => r.data),
  create: (body: CreateFundingRequest) =>
    requestsClient.post<FundingRequest>("/requests", body).then(r => r.data),
  approve: (id: number) => requestsClient.post(`/requests/${id}/approve`),
  reject: (id: number) => requestsClient.post(`/requests/${id}/reject`),
};