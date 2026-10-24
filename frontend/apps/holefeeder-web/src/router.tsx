import { createBrowserRouter, RouterProvider } from 'react-router';
import { AccountsPage } from './accounts/presentation/accounts-page';
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
          { path: '/accounts', element: <AccountsPage /> },
        ],
      },
    ],
  },
]);

export const Router = () => <RouterProvider router={router} />;
