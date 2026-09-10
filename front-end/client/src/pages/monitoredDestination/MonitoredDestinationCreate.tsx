import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from "@/components/common/Navbar";
import { Button } from '@/components/ui/button';

interface MonitoredDestinationRequest {
  location: string;
  riskLevel: string;
  lastChecked: string;
}

export default function MonitoredDestinationCreate() {
  const [formData, setFormData] = useState<MonitoredDestinationRequest>({
    location: '',
    riskLevel: '',
    lastChecked: new Date().toISOString(),
  });

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
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
      setError('No authentication token found. Please login.');
      setSubmitting(false);
      return;
    }

    try {
      const response = await fetch('https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/MonitoredDestination', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to create destination.');
      }

      setSuccess(true);
      setTimeout(() => navigate('/view')); 
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar title="Add Destination" />

      <div className="max-w-xl mx-auto px-6 py-12">
        <h2 className="text-3xl font-bold text-gray-800 mb-6">Create Monitored Destination</h2>

        <form onSubmit={handleSubmit} className="space-y-6 bg-white p-6 rounded-lg shadow-md">
          <div>
            <label className="block font-medium text-gray-700 mb-1">Location</label>
            <input
              type="text"
              name="location"
              value={formData.location}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border rounded"
              placeholder="Enter location"
            />
          </div>

          <div>
            <label className="block font-medium text-gray-700 mb-1">Risk Level</label>
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
            <label className="block font-medium text-gray-700 mb-1">Last Checked</label>
            <input
              type="datetime-local"
              name="lastChecked"
              value={new Date(formData.lastChecked).toISOString().slice(0, 16)}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border rounded"
            />
          </div>

          <Button
            type="submit"
            disabled={submitting}
            className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded w-full"
          >
            {submitting ? 'Submitting...' : 'Submit'}
          </Button>

          {error && <p className="text-red-600 mt-2">{error}</p>}
          {success && <p className="text-green-600 mt-2">Destination created successfully!</p>}
        </form>
      </div>
    </div>
  );
}
