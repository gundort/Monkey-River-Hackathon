import React, { useState, useEffect } from 'react';
import "../style/Dashboard.css";
import { Button } from "@/components/ui/button"
import { Link } from 'react-router-dom';
import Navbar from '@/components/common/Navbar';

interface User {
  firstName: string;
  lastName: string;
  email: string;
}

const Dashboard: React.FC = () => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  

  useEffect(() => {
    // Load user data from localStorage when component mounts
    loadUserData();
  }, []);

  const loadUserData = () => {
    try {
      const userString = localStorage.getItem('user');
      if (userString) {
        const userData = JSON.parse(userString);
        setUser(userData);
      } else {
        setError('No user data found. Please log in.');
      }
    } catch (err) {
      setError('Failed to load user data');
      console.error('Error loading user data:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="dashboard-container">
        <div className="loading">Loading...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-container">
        
        <div className="error-message">
          <p>{error}</p>
          <button onClick={() => window.location.href = '/'}>Go to Login</button>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
        <Navbar/>
     

      <main className="dashboard-content">
        {user && (
          <div className="user-profile-card">
            <div className="profile-header">
              <div className="avatar">
                {user.firstName.charAt(0)}{user.lastName.charAt(0)}
              </div>
              <h2>{user.firstName} {user.lastName}</h2>
            </div>
            
            <div className="user-details">
              <div className="detail-item">
                <span className="label">Name:</span>
                <span className="value">{user.firstName} {user.lastName}</span>
              </div>
              
              <div className="detail-item">
                <span className="label">Email:</span>
                <span className="value">{user.email}</span>
              </div>
              
              
            </div>
          </div>
        )}

        <div className="dashboard-sections">
          <div className="card">
            <h3>Welcome!</h3>
            <p>You have successfully logged in to your account.</p>
          </div>
          
          <div className="card">
            <h3>Quick Actions</h3>
            <ul>
              <li>View your profile</li>
                <li> <Button variant="outline"><Link to="/Preference">Update account settings</Link></Button></li>
            </ul>
          </div>
        </div>
        <div className="bg-white rounded-lg shadow-md p-6">
            <h3 className="text-lg font-semibold text-gray-800 mb-3">Monitored Destination</h3>
            <ul className="space-y-2">
              <li className="text-gray-600">View your profile</li>
              <li>
                <Link to="/view">
                  <Button variant="outline" className="mt-2">
                    View All
                  </Button>
                </Link>
              </li>
            </ul>
          </div>
      </main>
    </div>
  );
};

export default Dashboard;