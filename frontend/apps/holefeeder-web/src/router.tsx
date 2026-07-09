import { createBrowserRouter, Navigate, RouterProvider } from 'react-router';
import { AccountsPage } from './accounts/presentation/accounts-page';

const router = createBrowserRouter([
  { path: '/', element: <Navigate to="/accounts" replace /> },
  { path: '/accounts', element: <AccountsPage /> },
]);

export const Router = () => <RouterProvider router={router} />;
