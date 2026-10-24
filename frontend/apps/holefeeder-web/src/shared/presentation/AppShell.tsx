import { tk } from '@holefeeder/shared/core';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import InsightsOutlinedIcon from '@mui/icons-material/InsightsOutlined';
import OpenInNewOutlinedIcon from '@mui/icons-material/OpenInNewOutlined';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import { Box, List, ListItemButton, ListItemIcon, ListItemText, Stack, Typography } from '@mui/material';
import { type ReactNode } from 'react';
import { Link, Outlet, useLocation } from 'react-router';
import { translate } from '@/shared/i18n/translate';
import { BrandTile } from './BrandTile';

type NavItem = {
  id: string;
  label: string;
  icon: ReactNode;
  to?: string;
};

const navItems: NavItem[] = [
  { id: 'dashboard', label: translate(tk.tabs.dashboard), icon: <DashboardOutlinedIcon />, to: '/' },
  { id: 'insights', label: translate(tk.tabs.insights), icon: <InsightsOutlinedIcon /> },
  { id: 'settings', label: translate(tk.tabs.settings), icon: <SettingsOutlinedIcon /> },
];

const drawerWidth = 248;

export const AppShell = () => {
  const { pathname } = useLocation();

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <Box
        component="nav"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          bgcolor: 'background.paper',
          p: 2,
          pt: 3,
          display: 'flex',
          flexDirection: 'column',
          position: 'sticky',
          top: 0,
          height: '100vh',
        }}
      >
        <Stack direction="row" spacing={1.5} alignItems="center" sx={{ pl: 1, mb: 3 }}>
          <BrandTile size={32} testID="nav-brand-tile" />
          <Typography sx={{ fontSize: 20, fontWeight: 700 }}>Holefeeder</Typography>
        </Stack>
        <List disablePadding sx={{ display: 'grid', gap: 0.5 }}>
          {navItems.map((item) => {
            const selected = item.to === pathname;
            return (
              <ListItemButton
                key={item.id}
                data-testid={`nav-${item.id}`}
                component={item.to ? Link : 'div'}
                to={item.to}
                disabled={!item.to}
                selected={selected}
                aria-current={selected ? 'page' : undefined}
                sx={{
                  borderRadius: 2,
                  color: selected ? 'primary.main' : 'text.primary',
                  '&.Mui-disabled': { opacity: 1 },
                }}
              >
                <ListItemIcon sx={{ minWidth: 32, color: selected ? 'primary.main' : 'text.secondary' }}>{item.icon}</ListItemIcon>
                <ListItemText primary={item.label} slotProps={{ primary: { fontSize: 15, fontWeight: selected ? 600 : 400 } }} />
              </ListItemButton>
            );
          })}
        </List>
        <ListItemButton
          data-testid="nav-legacy"
          component="a"
          href="/angular/"
          target="_blank"
          rel="noopener noreferrer"
          sx={{ mt: 'auto', borderRadius: 2, flexGrow: 0 }}
        >
          <ListItemIcon sx={{ minWidth: 32, color: 'text.secondary' }}>
            <OpenInNewOutlinedIcon />
          </ListItemIcon>
          <ListItemText primary={translate(tk.tabs.legacy)} slotProps={{ primary: { fontSize: 15 } }} />
        </ListItemButton>
      </Box>
      <Box component="main" sx={{ flexGrow: 1, minWidth: 0, p: 4, pt: 3.5 }}>
        <Outlet />
      </Box>
    </Box>
  );
};
