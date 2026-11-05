import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../auth/AuthContext";

// Wraps pages that need a logged-in user. Not logged in? Send them to /login.
// (The API enforces this too with [Authorize]; this just gives a nicer experience.)
export default function ProtectedRoute({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  return <>{children}</>;
}
