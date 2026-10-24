import { Box, CircularProgress } from '@mui/material';
import { Navigate, Outlet } from 'react-router';
import { useAuthentication } from './authentication-context';

export const RequireAuthenticated = () => {
  const { user, isLoading } = useAuthentication();

  if (isLoading) {
    return (
      <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  return user ? <Outlet /> : <Navigate to="/welcome" replace />;
};

export const RequireAnonymous = () => {
  const { user } = useAuthentication();

  return user ? <Navigate to="/" replace /> : <Outlet />;
};
