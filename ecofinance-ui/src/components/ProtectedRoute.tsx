import { Navigate } from 'react-router-dom';
import { isAuthenticated } from '../utils/auth';

interface Props {
  children: React.ReactElement;
}

const ProtectedRoute = ({ children }: Props) => {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

export default ProtectedRoute;