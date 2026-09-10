import { Routes, Route } from 'react-router-dom'
import Home from '../pages/Home'
import Preferences from '@/pages/Preferences';
// import About from '../pages/About'
// import Contact from '../pages/Contact'
// import NotFound from '../pages/NotFound'

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/preference" element={<Preferences />} />
      {/* <Route path="/contact" element={<Contact />} />
      <Route path="*" element={<NotFound />} /> */}
    </Routes>
  )
}

export default AppRoutes;