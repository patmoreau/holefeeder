import { createBrowserRouter, RouterProvider } from 'react-router';
import { AccountPage } from './accounts/presentation/AccountPage';
import { DashboardPage } from './dashboard/presentation/DashboardPage';
import { SpendingPage } from './insights/presentation/SpendingPage';
import { CashflowsPage } from './settings/presentation/CashflowsPage';
import { CategoriesPage } from './settings/presentation/CategoriesPage';
import { SettingsPage } from './settings/presentation/SettingsPage';
import { TagsPage } from './settings/presentation/TagsPage';
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
          { path: '/insights/spending', element: <SpendingPage /> },
          { path: '/settings', element: <SettingsPage /> },
          { path: '/settings/categories', element: <CategoriesPage /> },
          { path: '/settings/cashflows', element: <CashflowsPage /> },
          { path: '/settings/tags', element: <TagsPage /> },
        ],
      },
    ],
  },
]);

export const Router = () => <RouterProvider router={router} />;
