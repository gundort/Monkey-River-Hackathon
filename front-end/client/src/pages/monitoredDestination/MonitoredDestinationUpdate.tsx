import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Navbar from '@/components/common/Navbar';
import { Button } from '@/components/ui/button';

// ---------- Types ----------
interface MonitoredDestination {
  location: string;
  riskLevel: string;
  lastChecked: string;
}

// ---------- Constants ----------
const API_BASE =
  'https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/MonitoredDestination';

// ---------- Component ----------
export default function MonitoredDestinationUpdate() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [formData, setFormData] = useState<MonitoredDestination>({
    location: '',
    riskLevel: '',
    lastChecked: new Date().toISOString(),
  });

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  // ----- Fetch current record -----
  useEffect(() => {
    (async () => {
      const token = localStorage.getItem('authToken');
      if (!token) {
        setError('No authentication token found. Please log in.');
        setLoading(false);
        return;
      }

      try {
        const res = await fetch(`${API_BASE}/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error(`Fetch failed (${res.status})`);

        const data = await res.json();
        setFormData({
          location: data.location,
          riskLevel: data.riskLevel,
          lastChecked: data.lastChecked,
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  // ----- Handlers -----
  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'lastChecked' ? new Date(value).toISOString() : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    const token = localStorage.getItem('authToken');
    if (!token) {
      setError('No authentication token found. Please log in.');
      setSubmitting(false);
      return;
    }

    try {
      const res = await fetch(`${API_BASE}/${id}`, {
        method: 'PUT', // Use PATCH if your API expects it
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || `Update failed (${res.status})`);
      }

      setSuccess(true);
      setTimeout(() => navigate('/'), 1500); // back to list
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setSubmitting(false);
    }
  };

  // ----- Render -----
  if (loading)
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <p className="text-xl text-gray-600">Loading…</p>
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar title="Update Destination" />

      <div className="max-w-xl mx-auto px-6 py-12">
        <h2 className="text-3xl font-bold text-gray-800 mb-6">
          Update Monitored Destination
        </h2>

        <form
          onSubmit={handleSubmit}
          className="space-y-6 bg-white p-6 rounded-lg shadow-md"
        >
          <div>
            <label className="block font-medium text-gray-700 mb-1">
              Location
            </label>
            <input
              type="text"
              name="location"
              value={formData.location}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border rounded"
            />
          </div>

          <div>
            <label className="block font-medium text-gray-700 mb-1">
              Risk Level
            </label>
            <select
              name="riskLevel"
              value={formData.riskLevel}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border rounded"
            >
              <option value="">Select risk level</option>
              <option value="High">High</option>
              <option value="Medium">Medium</option>
              <option value="Low">Low</option>
            </select>
          </div>

          <div>
            <label className="block font-medium text-gray-700 mb-1">
              Last Checked
            </label>
            <input
              type="datetime-local"
              name="lastChecked"
              value={new Date(formData.lastChecked)
                .toISOString()
                .slice(0, 16)}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border rounded"
            />
          </div>

          <Button
            type="submit"
            disabled={submitting}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded"
          >
            {submitting ? 'Updating…' : 'Save Changes'}
          </Button>

          {error && <p className="text-red-600 mt-2">{error}</p>}
          {success && (
            <p className="text-green-600 mt-2">
              Destination updated successfully!
            </p>
          )}
        </form>
      </div>
    </div>
  );
}
