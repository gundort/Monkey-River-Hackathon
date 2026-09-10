import  { useEffect, useState } from 'react';
import AppToggle from "../pages/AuthToggle"
import Dashboard from '../pages/Dashboard';
import { isAuthenticated } from '../utils/auth'; 
const Home = () => {
   const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    // Check if user is already logged in
    setIsLoggedIn(isAuthenticated());
  }, []);

  // Listen for storage changes (in case user logs in/out in another tab)
  useEffect(() => {
    const handleStorageChange = () => {
      setIsLoggedIn(isAuthenticated());
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  return (
    <div>
      <h1>Home Page</h1>
     {isLoggedIn ? <Dashboard /> : <AppToggle  />}
    </div>
  )
}

export default Home;