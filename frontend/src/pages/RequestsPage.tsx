import { useEffect, useState } from "react";
import { requestsApi } from "../api/endpoints";
import { getErrorMessage } from "../api/client";
import type { FundingRequest, RequestStatus } from "../types";
import StatusBadge from "../components/StatusBadge";

const statuses: RequestStatus[] = ["Submitted", "PendingEligibility", "Approved", "Rejected"];

export default function RequestsPage() {
  const [requests, setRequests] = useState<FundingRequest[]>([]);
  const [filter, setFilter] = useState<RequestStatus | "">("");
  const [error, setError] = useState("");

  // Loads the list from the Requests API
  const load = async () => {
    try {
      setRequests(await requestsApi.list());
      setError("");
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  useEffect(() => { load(); }, []);

  // Approve or reject, then reload so the new status shows
  const decide = async (id: number, action: "approve" | "reject") => {
    try {
      await (action === "approve" ? requestsApi.approve(id) : requestsApi.reject(id));
      await load();
    } catch (err) {
      setError(getErrorMessage(err));   // e.g. the 409 message
    }
  };

  const shown = filter ? requests.filter(r => r.status === filter) : requests;

  return (
    <div>
      <h1>Funding requests</h1>

      <label>
        Status:{" "}
        <select value={filter} onChange={e => setFilter(e.target.value as RequestStatus | "")}>
          <option value="">All</option>
          {statuses.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
      </label>

      {error && <p className="error">{error}</p>}

      <table>
        <thead>
          <tr>
            <th>#</th><th>Country</th><th>Fund</th><th>Amount (m SDR)</th>
            <th>Status</th><th>Reason</th><th>Created</th><th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {shown.map(r => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.countryName} ({r.countryCode})</td>
              <td>{r.fund}</td>
              <td>{r.amount.toLocaleString()}</td>
              <td><StatusBadge status={r.status} /></td>
              <td>{r.eligibilityReason ?? "—"}</td>
              <td>{new Date(r.createdAt).toLocaleDateString()}</td>
              <td>
                {r.status === "Submitted" && (
                  <div className="actions">
                    <button onClick={() => decide(r.id, "approve")}>Approve</button>
                    <button onClick={() => decide(r.id, "reject")}>Reject</button>
                  </div>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}