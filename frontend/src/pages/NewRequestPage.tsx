import { useEffect, useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { countriesApi, requestsApi } from "../api/endpoints";
import { getErrorMessage } from "../api/client";
import { funds, type Country } from "../types";

export default function NewRequestPage() {
  const navigate = useNavigate();
  const [countries, setCountries] = useState<Country[]>([]);
  const [countryCode, setCountryCode] = useState("");
  const [fund, setFund] = useState<string>(funds[0]);
  const [amount, setAmount] = useState("");
  const [error, setError] = useState("");

  // Dropdown shows only active countries, straight from the Countries API
  useEffect(() => {
    countriesApi.list(true).then(setCountries).catch(err => setError(getErrorMessage(err)));
  }, []);

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      await requestsApi.create({ countryCode, fund, amount: Number(amount) });
      navigate("/");
    } catch (err) {
      setError(getErrorMessage(err));   // e.g. the 422 "exceeds 200% of quota" reason
    }
  };

  return (
    <div>
      <h1>New funding request</h1>
      <form onSubmit={submit}>
        <label>
          Country
          <select value={countryCode} onChange={e => setCountryCode(e.target.value)} required>
            <option value="">Choose a country</option>
            {countries.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
          </select>
        </label>

        <label>
          Fund
          <select value={fund} onChange={e => setFund(e.target.value)}>
            {funds.map(f => <option key={f} value={f}>{f}</option>)}
          </select>
        </label>

        <label>
          Amount (millions of SDR)
          <input type="number" min="1" step="0.1" value={amount}
                 onChange={e => setAmount(e.target.value)} required />
        </label>

        {error && <p className="error">{error}</p>}

        <button type="submit">Submit request</button>
      </form>
    </div>
  );
}