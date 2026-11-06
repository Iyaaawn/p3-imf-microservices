export type RequestStatus = "Submitted" | "PendingEligibility" | "Approved" | "Rejected";

export const funds = ["SBA", "EFF", "ECF", "FCL", "RSF"] as const;

export interface Country {
  code: string;        // "GHA"
  name: string;
  isActive: boolean;
}

export interface FundingRequest {
  id: number;
  countryCode: string;
  countryName: string;
  fund: string;
  amount: number;      // millions of SDR
  status: RequestStatus;
  eligibilityReason: string | null;
  createdAt: string;
}

export interface CreateFundingRequest {
  countryCode: string;
  fund: string;
  amount: number;
}