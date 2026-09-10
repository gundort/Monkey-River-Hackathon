import Navbar from "@/components/common/Navbar";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Edit, Trash2, RefreshCw } from "lucide-react";
import { Link } from "react-router-dom";

// ---------- Types ----------
interface MonitoredDestination {
  id: string;
  location: string;
  riskLevel: string;
  lastChecked: string;
}

// ---------- Component ----------
export default function MonitoredDestinationView() {
  const [destinations, setDestinations] = useState<MonitoredDestination[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isRefreshing, setIsRefreshing] = useState(false);

  const API_BASE =
    "https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/MonitoredDestination";

  // ----- Initial fetch -----
  useEffect(() => {
    fetchDestinations();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // ----- Helpers -----
  const fetchDestinations = async () => {
    try {
      setIsRefreshing(true);
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("No authentication token found. Please log in.");
        setLoading(false);
        return;
      }

      const res = await fetch(API_BASE, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) throw new Error(`Fetch failed (${res.status})`);
      setDestinations(await res.json());
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setLoading(false);
      setIsRefreshing(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this destination?")) return;

    const token = localStorage.getItem("authToken");
    if (!token) {
      setError("No authentication token found. Please log in.");
      return;
    }

    try {
      const res = await fetch(`${API_BASE}/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || `Delete failed (${res.status})`);
      }

      // Remove from UI only after a successful response
      setDestinations((prev) => prev.filter((d) => d.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    }
  };

  const getRiskColor = (risk: string) =>
    ({
      high: "text-red-700 bg-red-100 border-red-200",
      medium: "text-yellow-700 bg-yellow-100 border-yellow-200",
      low: "text-green-700 bg-green-100 border-green-200",
    }[risk.toLowerCase()] ?? "text-gray-700 bg-gray-100 border-gray-200");

  const getRiskIcon = (risk: string) =>
    ({ high: "🔴", medium: "🟡", low: "🟢" }[risk.toLowerCase()] ?? "⚪");

  // ---------- Render ----------
  if (loading)
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto" />
          <p className="mt-4 text-xl text-gray-600">Loading destinations…</p>
        </div>
      </div>
    );

  if (error)
    return (
      <div className="min-h-screen bg-gray-50">
        <Navbar title="Monitored Destinations" />
        <div className="max-w-7xl mx-auto px-8 py-12">
          <div className="bg-red-50 border-2 border-red-200 text-red-800 px-8 py-6 rounded-xl">
            <h3 className="text-lg font-semibold mb-1">Error</h3>
            <p>{error}</p>
          </div>
        </div>
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar title="Monitored Destinations" />

      <div className="max-w-7xl mx-auto px-8 py-12">
        {/* Header */}
        <div className="flex items-center justify-between mb-12">
          <div>
            <h1 className="text-4xl font-bold text-gray-800">
              Monitored Destinations
            </h1>
            <p className="text-gray-600 mt-2">
              Track risk levels for your travel destinations
            </p>
          </div>

          <div className="flex items-center gap-4">
            <Button
              onClick={fetchDestinations}
              disabled={isRefreshing}
              variant="outline"
              className="flex items-center gap-2 h-12 px-6"
            >
              <RefreshCw
                className={`w-5 h-5 ${isRefreshing ? "animate-spin" : ""}`}
              />
              {isRefreshing ? "Refreshing…" : "Refresh"}
            </Button>

            <Link
              to="/create"
              className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white h-12 px-6 font-semibold rounded"
            >
              Add Destination
            </Link>
          </div>
        </div>

        {/* Grid */}
        {destinations.length === 0 ? (
          <div className="text-center py-16">
            <h3 className="text-2xl font-semibold text-gray-800 mb-4">
              No Destinations Found
            </h3>
            <p className="text-gray-600 mb-8">
              Start monitoring your travel destinations by adding them below.
            </p>
            <Link
              to="/create"
              className="inline-flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white h-12 px-8 font-semibold rounded"
            >
              Add Your First Destination
            </Link>
          </div>
        ) : (
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {destinations.map((d) => (
              <div
                key={d.id}
                className="bg-white rounded-xl shadow-lg hover:shadow-xl transition overflow-hidden"
              >
                {/* Card Body */}
                <div className="p-6">
                  <div className="flex justify-between items-start mb-4">
                    <h3 className="text-xl font-bold text-gray-800">
                      {d.location}
                    </h3>
                    <span className="text-2xl">{getRiskIcon(d.riskLevel)}</span>
                  </div>

                  <div className="space-y-4">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Risk Level:</span>
                      <span
                        className={`px-3 py-1 rounded-full text-sm font-bold border ${getRiskColor(
                          d.riskLevel
                        )}`}
                      >
                        {d.riskLevel.toUpperCase()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Last Checked:</span>
                      <span className="text-gray-800 font-semibold">
                        {new Date(d.lastChecked).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                          year: "numeric",
                        })}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Card Actions */}
                <div className="px-6 pb-6 pt-2 flex gap-3">
                  <Link
                    to={`/edit/${d.id}`}
                    className="flex-1 flex items-center justify-center gap-2 h-10"
                  >
                    <Edit className="w-4 h-4" />
                    Edit
                  </Link>

                  <Button
                    onClick={() => handleDelete(d.id)}
                    variant="outline"
                    className="flex-1 flex items-center justify-center gap-2 h-10 text-red-600 border-red-200 hover:bg-red-50 hover:border-red-300"
                  >
                    <Trash2 className="w-4 h-4" />
                    Delete
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
