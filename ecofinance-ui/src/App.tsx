import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import ExpensesPage from './pages/ExpensesPage';
import FootprintPage from './pages/FootprintPage';
import ProtectedRoute from './components/ProtectedRoute';
import RegisterPage from './pages/RegisterPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/expenses" element={
          <ProtectedRoute>
            <ExpensesPage />
          </ProtectedRoute>
        }/>
        <Route path="/footprint" element={
          <ProtectedRoute>
            <FootprintPage />
          </ProtectedRoute>
        }/>
        <Route path="*" element={<Navigate to="/expenses" />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;