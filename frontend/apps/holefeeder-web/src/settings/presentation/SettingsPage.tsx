import { tk } from '@holefeeder/shared/core';
import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import LocalOfferOutlinedIcon from '@mui/icons-material/LocalOfferOutlined';
import RepeatOutlinedIcon from '@mui/icons-material/RepeatOutlined';
import {
  Avatar,
  Box,
  Button,
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Paper,
  Stack,
  Typography,
} from '@mui/material';
import { Children, Fragment, type ReactNode } from 'react';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { buildVersionLabel } from '@/shared/core/build-version';
import { translate } from '@/shared/i18n/translate';

const Section = ({ title, testID, children }: { title: string; testID: string; children: ReactNode }) => (
  <Stack spacing={1.5} data-testid={testID}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{title}</Typography>
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {children}
    </Paper>
  </Stack>
);

const Rows = ({ children }: { children: ReactNode }) => (
  <List disablePadding>
    {Children.toArray(children).map((child, index) => (
      <Fragment key={index}>
        {index > 0 && <Divider component="li" />}
        {child}
      </Fragment>
    ))}
  </List>
);

const rowIcon = { minWidth: 32, color: 'text.secondary' };
const rowLabel = { primary: { fontSize: 17 } };

const LinkRow = ({ testID, icon, label }: { testID: string; icon: ReactNode; label: string }) => (
  <ListItem disablePadding>
    <ListItemButton disabled data-testid={testID} sx={{ px: 0, py: 1.5, '&:hover': { bgcolor: 'transparent' } }}>
      <ListItemIcon sx={rowIcon}>{icon}</ListItemIcon>
      <ListItemText primary={label} slotProps={rowLabel} />
      <ChevronRightIcon sx={{ color: 'text.secondary' }} />
    </ListItemButton>
  </ListItem>
);

const ValueRow = ({ icon, label, children }: { icon: ReactNode; label: string; children: ReactNode }) => (
  <ListItem disablePadding sx={{ py: 1.5 }}>
    <ListItemIcon sx={rowIcon}>{icon}</ListItemIcon>
    <ListItemText primary={label} slotProps={rowLabel} />
    {children}
  </ListItem>
);

const initialsOf = (name: string | undefined) =>
  (name ?? '')
    .split(/\s+/)
    .filter((word) => word !== '')
    .slice(0, 2)
    .map((word) => word[0].toUpperCase())
    .join('');

const ProfileSection = () => {
  const { user, logout } = useAuthentication();

  return (
    <Section title={translate(tk.profileSection.title)} testID="settings-profile">
      <Stack direction="row" alignItems="center" spacing={1.75} sx={{ py: 2 }}>
        <Avatar
          src={user?.picture}
          data-testid="settings-profile-avatar"
          sx={{ width: 48, height: 48, fontSize: 17, fontWeight: 600, color: 'primary.main', bgcolor: 'action.selected' }}
        >
          {initialsOf(user?.name)}
        </Avatar>
        <Box sx={{ flexGrow: 1, minWidth: 0 }}>
          <Typography data-testid="settings-profile-name" noWrap sx={{ fontSize: 17, fontWeight: 600 }}>
            {user?.name}
          </Typography>
          <Typography data-testid="settings-profile-email" noWrap sx={{ fontSize: 13, color: 'text.secondary' }}>
            {user?.email}
          </Typography>
        </Box>
        <Button variant="outlined" color="error" data-testid="settings-logout" onClick={logout} sx={{ flexShrink: 0, fontSize: 15 }}>
          {translate(tk.auth.logoutButton)}
        </Button>
      </Stack>
    </Section>
  );
};

export const SettingsPage = () => (
  <Stack spacing={3.5} data-testid="settings-page">
    <Typography variant="h1" sx={{ fontSize: 28, fontWeight: 700 }}>
      {translate(tk.tabs.settings)}
    </Typography>
    <Stack spacing={3.5} sx={{ maxWidth: 720 }}>
      <ProfileSection />
      <Section title={translate(tk.budgetSection.title)} testID="settings-holefeeder">
        <Rows>
          <LinkRow testID="settings-categories" icon={<CategoryOutlinedIcon />} label={translate(tk.categoriesSection.title)} />
          <LinkRow testID="settings-cashflows" icon={<RepeatOutlinedIcon />} label={translate(tk.cashflowsSection.title)} />
          <LinkRow testID="settings-tags" icon={<LocalOfferOutlinedIcon />} label={translate(tk.tagsSection.title)} />
        </Rows>
      </Section>
      <Section title={translate(tk.settings.aboutSection.title)} testID="settings-about">
        <Rows>
          <ValueRow icon={<InfoOutlinedIcon />} label={translate(tk.settings.aboutSection.version)}>
            <Typography data-testid="settings-version" sx={{ fontSize: 15, color: 'text.secondary', flexShrink: 0 }}>
              {buildVersionLabel}
            </Typography>
          </ValueRow>
        </Rows>
      </Section>
    </Stack>
  </Stack>
);
