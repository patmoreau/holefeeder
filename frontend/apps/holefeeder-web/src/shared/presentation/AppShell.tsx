import { tk } from '@holefeeder/shared/core';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import InsightsOutlinedIcon from '@mui/icons-material/InsightsOutlined';
import MenuIcon from '@mui/icons-material/Menu';
import OpenInNewOutlinedIcon from '@mui/icons-material/OpenInNewOutlined';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import { Box, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText, Stack, Typography } from '@mui/material';
import { type ReactNode, useState } from 'react';
import { Link, Outlet, useLocation } from 'react-router';
import { buildVersionLabel } from '@/shared/core/build-version';
import { translate } from '@/shared/i18n/translate';
import { BrandTile } from './BrandTile';

type NavItem = {
  id: string;
  label: string;
  icon: ReactNode;
  to?: string;
  matches?: string[];
};

const navItems: NavItem[] = [
  { id: 'dashboard', label: translate(tk.tabs.dashboard), icon: <DashboardOutlinedIcon />, to: '/', matches: ['/accounts/'] },
  { id: 'insights', label: translate(tk.tabs.insights), icon: <InsightsOutlinedIcon /> },
  { id: 'settings', label: translate(tk.tabs.settings), icon: <SettingsOutlinedIcon />, to: '/settings' },
];

const drawerWidth = 248;
const flyoutWidth = 280;

const NavContent = ({ onNavigate }: { onNavigate?: () => void }) => {
  const { pathname } = useLocation();

  return (
    <>
      <Stack direction="row" spacing={1.5} alignItems="center" sx={{ pl: 1, mb: 3 }}>
        <BrandTile size={32} testID="nav-brand-tile" />
        <Typography sx={{ fontSize: 20, fontWeight: 700 }}>Holefeeder</Typography>
      </Stack>
      <List disablePadding sx={{ display: 'grid', gap: 0.5 }}>
        {navItems.map((item) => {
          const selected = item.to === pathname || !!item.matches?.some((prefix) => pathname.startsWith(prefix));
          return (
            <ListItemButton
              key={item.id}
              data-testid={`nav-${item.id}`}
              component={item.to ? Link : 'div'}
              to={item.to}
              disabled={!item.to}
              selected={selected}
              aria-current={selected ? 'page' : undefined}
              onClick={item.to ? onNavigate : undefined}
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
      <Typography data-testid="nav-version" sx={{ pt: 1, fontSize: 12, color: 'text.secondary', textAlign: 'center' }}>
        {buildVersionLabel}
      </Typography>
    </>
  );
};

const navSx = {
  bgcolor: 'background.paper',
  p: 2,
  pt: 3,
  display: 'flex',
  flexDirection: 'column',
  height: '100%',
};

const PhoneBar = ({ onMenu }: { onMenu: () => void }) => (
  <Stack
    direction="row"
    alignItems="center"
    spacing={1}
    sx={{
      display: { xs: 'flex', md: 'none' },
      position: 'sticky',
      top: 0,
      zIndex: 'appBar',
      height: 56,
      pl: 0.5,
      pr: 2,
      bgcolor: 'background.paper',
    }}
  >
    <IconButton data-testid="nav-menu" aria-label={translate(tk.tabs.openMenu)} onClick={onMenu} sx={{ color: 'text.primary' }}>
      <MenuIcon />
    </IconButton>
    <BrandTile size={28} testID="nav-bar-brand-tile" />
    <Typography sx={{ fontSize: 17, fontWeight: 700 }}>Holefeeder</Typography>
  </Stack>
);

export const AppShell = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const closeMenu = () => setMenuOpen(false);

  return (
    <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, minHeight: '100vh', bgcolor: 'background.default' }}>
      <PhoneBar onMenu={() => setMenuOpen(true)} />
      <Drawer
        open={menuOpen}
        onClose={closeMenu}
        sx={{ display: { md: 'none' } }}
        slotProps={{ paper: { sx: { width: flyoutWidth, backgroundImage: 'none' } } }}
      >
        <Box component="nav" sx={navSx}>
          <NavContent onNavigate={closeMenu} />
        </Box>
      </Drawer>
      <Box
        component="nav"
        sx={{
          ...navSx,
          display: { xs: 'none', md: 'flex' },
          width: drawerWidth,
          flexShrink: 0,
          position: 'sticky',
          top: 0,
          height: '100vh',
        }}
      >
        <NavContent />
      </Box>
      <Box component="main" sx={{ flexGrow: 1, minWidth: 0, p: { xs: 2, md: 4 }, pt: { xs: 2, md: 3.5 } }}>
        <Outlet />
      </Box>
    </Box>
  );
};
