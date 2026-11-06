import type { RequestStatus } from "../types";

export default function StatusBadge({ status }: { status: RequestStatus }) {
  return <span className={`badge badge-${status.toLowerCase()}`}>{status}</span>;
}