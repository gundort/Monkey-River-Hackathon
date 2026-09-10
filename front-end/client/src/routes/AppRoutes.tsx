import { Routes, Route } from 'react-router-dom'
import Home from '../pages/Home'
import Preferences from '@/pages/Preferences';
import Dashboard from '@/pages/Dashboard';
import MonitoredDestinationCreate from '@/pages/monitoredDestination/MonitoredDestinationCreate';
import MonitoredDestinationView from '@/pages/monitoredDestination/MonitoredDestinationView';
import MonitoredDestinationUpdate from '@/pages/monitoredDestination/MonitoredDestinationUpdate';
// import About from '../pages/About'
// import Contact from '../pages/Contact'
// import NotFound from '../pages/NotFound'

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/preference" element={<Preferences />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/view" element={<MonitoredDestinationView/>}/>
      <Route path="/create" element={<MonitoredDestinationCreate/>}/>
      <Route path="/edit/:id" element={<MonitoredDestinationUpdate/>}/>
      {/* <Route path="/contact" element={<Contact />} />
      <Route path="*" element={<NotFound />} /> */}
    </Routes>
  )
}

export default AppRoutes;