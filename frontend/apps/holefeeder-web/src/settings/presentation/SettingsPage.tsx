import { LanguageType, tk } from '@holefeeder/shared/core';
import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import ContrastOutlinedIcon from '@mui/icons-material/ContrastOutlined';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import LocalOfferOutlinedIcon from '@mui/icons-material/LocalOfferOutlined';
import RepeatOutlinedIcon from '@mui/icons-material/RepeatOutlined';
import TranslateOutlinedIcon from '@mui/icons-material/TranslateOutlined';
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
  MenuItem,
  Paper,
  Select,
  Stack,
  Typography,
  useColorScheme,
} from '@mui/material';
import { Children, Fragment, type ReactNode } from 'react';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { buildVersionLabel } from '@/shared/core/build-version';
import { Language } from '@/shared/i18n/language';
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

type SelectOption<T extends string> = { value: T; label: string };

const SelectRow = <T extends string>({
  testID,
  icon,
  label,
  value,
  options,
  onChange,
}: {
  testID: string;
  icon: ReactNode;
  label: string;
  value: T;
  options: SelectOption<T>[];
  onChange: (value: T) => void;
}) => (
  <ValueRow icon={icon} label={label}>
    <Select
      size="small"
      value={value}
      onChange={(event) => onChange(event.target.value as T)}
      inputProps={{ 'aria-label': label }}
      SelectDisplayProps={{ 'data-testid': testID } as object}
      sx={{ flexShrink: 0, fontSize: 15, bgcolor: 'background.default' }}
    >
      {options.map((option) => (
        <MenuItem key={option.value} value={option.value}>
          {option.label}
        </MenuItem>
      ))}
    </Select>
  </ValueRow>
);

type ThemeMode = 'system' | 'light' | 'dark';

const themeOptions: SelectOption<ThemeMode>[] = [
  { value: 'system', label: translate(tk.displaySection.system) },
  { value: 'light', label: translate(tk.displaySection.light) },
  { value: 'dark', label: translate(tk.displaySection.dark) },
];

const languageOptions: SelectOption<LanguageType>[] = [
  { value: LanguageType.en, label: translate(tk.displaySection.english) },
  { value: LanguageType.fr, label: translate(tk.displaySection.french) },
];

const chooseLanguage = (language: LanguageType) => {
  Language.choose(language);
  window.location.reload();
};

const DisplaySection = () => {
  const { mode, setMode } = useColorScheme();

  return (
    <Section title={translate(tk.displaySection.title)} testID="settings-display">
      <Rows>
        <SelectRow
          testID="settings-language"
          icon={<TranslateOutlinedIcon />}
          label={translate(tk.displaySection.language)}
          value={Language.current()}
          options={languageOptions}
          onChange={chooseLanguage}
        />
        <SelectRow
          testID="settings-theme"
          icon={<ContrastOutlinedIcon />}
          label={translate(tk.displaySection.theme)}
          value={mode ?? 'system'}
          options={themeOptions}
          onChange={setMode}
        />
      </Rows>
    </Section>
  );
};

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
      <DisplaySection />
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
