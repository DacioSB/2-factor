import { useState } from 'react'
import './App.css'
import TwoFactorValidation from './components/TwoFactorValidation'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div className="App">
      <div className="card">
        <TwoFactorValidation />
      </div>
    </div>
  )
}

export default App
