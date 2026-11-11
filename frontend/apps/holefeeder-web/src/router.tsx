import { createBrowserRouter, RouterProvider } from 'react-router';
import { AccountPage } from './accounts/presentation/AccountPage';
import { DashboardPage } from './dashboard/presentation/DashboardPage';
import { RequireAnonymous, RequireAuthenticated } from './shared/auth/AuthenticationGuards';
import { AppShell } from './shared/presentation/AppShell';
import { WelcomePage } from './welcome/presentation/WelcomePage';

const router = createBrowserRouter([
  {
    element: <RequireAnonymous />,
    children: [{ path: '/welcome', element: <WelcomePage /> }],
  },
  {
    element: <RequireAuthenticated />,
    children: [
      {
        element: <AppShell />,
        children: [
          { path: '/', element: <DashboardPage /> },
          { path: '/accounts/:id', element: <AccountPage /> },
        ],
      },
    ],
  },
]);

export const Router = () => <RouterProvider router={router} />;
