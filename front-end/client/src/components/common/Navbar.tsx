import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from '../ui/button';
import { Bell } from 'lucide-react';

interface NavbarProps {
  title?: string;
}

interface Alert {
  id: string;
  title: string;
  description?: string;
  createdAt: string;
  timestamp: string;
}

const Navbar: React.FC<NavbarProps> = ({ title = "Dashboard" }) => {
  const [showNotifications, setShowNotifications] = useState(false);
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const navigate = useNavigate();

  const toggleNotifications = () => setShowNotifications(!showNotifications);

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    navigate('/', { replace: true });
    console.log("log out success");
  };

  useEffect(() => {
    const fetchAlerts = async () => {
      try {
        const token = localStorage.getItem("authToken");
        if (!token) return;

        const response = await fetch(
          "https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/TravelAlerts",
          {
            headers: {
              "Authorization": `Bearer ${token}`,
              "Accept": "*/*",
            },
          }
        );

        if (response.ok) {
          const data = await response.json();
          setAlerts(data);
        } else {
          console.warn("Failed to fetch alerts");
        }
      } catch (error) {
        console.error("Error fetching alerts:", error);
      }
    };

    fetchAlerts();
  }, []);

  return (
    <header className="bg-white shadow-sm px-6 py-4 flex justify-between items-center">
      <Link to="/" className="text-2xl font-bold text-gray-800">{title}</Link>
      
      <div className="flex items-center space-x-4">
        <div className="relative">
          <button
            onClick={toggleNotifications}
            className="relative p-2 text-gray-600 hover:text-gray-800 hover:bg-gray-100 rounded-full transition-colors"
          >
            <Bell className="w-5 h-5" />
            {alerts.length > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                {alerts.length}
              </span>
            )}
          </button>

          {showNotifications && (
            <div className="absolute right-0 mt-2 w-80 bg-white rounded-lg shadow-lg border z-50">
              <div className="p-4 border-b">
                <h3 className="font-semibold text-gray-800">Notifications</h3>
              </div>
              <div className="max-h-64 overflow-y-auto">
                {alerts.map((alert) => (
                  <div key={alert.id} className="p-3 hover:bg-gray-50 border-b">
                    <p className="text-sm font-medium text-gray-800">{alert.title}</p>
                    <p className="text-xs text-gray-500 mt-1">{new Date(alert.timestamp).toLocaleString(undefined, {
  dateStyle: "medium",
  timeStyle: "short",
})}
</p>
                  </div>
                ))}
                {alerts.length === 0 && (
                  <div className="p-3 text-sm text-gray-500 text-center">No new alerts</div>
                )}
              </div>

            </div>
          )}
        </div>

        <Button variant="destructive" onClick={handleLogout}>
          Logout
        </Button>
      </div>
    </header>
  );
};

export default Navbar;
