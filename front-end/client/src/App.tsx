import './App.css'
import AppRoutes from './routes/AppRoutes'
import {  BrowserRouter as Router } from 'react-router-dom';

function App() {

  return (
    <>

        <main>
        <Router>
          <AppRoutes />
        </Router>
      </main>
     
      {/* <Button ><Link to="/Preferences">Preferences</Link></Button> */}
      

    </>
  )
}

export default App
