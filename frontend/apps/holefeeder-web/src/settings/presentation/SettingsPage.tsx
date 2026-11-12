import { type AsyncResult, LanguageType, tk } from '@holefeeder/shared/core';
import AccountBalanceWalletOutlinedIcon from '@mui/icons-material/AccountBalanceWalletOutlined';
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
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  MenuItem,
  Select,
  Skeleton,
  Stack,
  Typography,
  useColorScheme,
} from '@mui/material';
import { type ReactNode, useState } from 'react';
import { useNavigate } from 'react-router';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { buildVersionLabel } from '@/shared/core/build-version';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast, type Notify } from '@/shared/presentation/DashboardToast';
import { useBudgetSettings } from '../api/use-budget-settings';
import type { BudgetSettings } from '../domain/budget-settings';
import { budgetSummary } from './budget-summary';
import { BudgetSettingsFlyout } from './BudgetSettingsFlyout';
import { Rows, Section } from './SettingsList';

const rowIcon = { minWidth: 32, color: 'text.secondary' };
const rowLabel = { primary: { fontSize: 17 } };

type LinkRowProps = { testID: string; icon: ReactNode; label: string; value?: ReactNode; onClick?: () => void };

const LinkRow = ({ testID, icon, label, value, onClick }: LinkRowProps) => (
  <ListItem disablePadding>
    <ListItemButton
      disabled={!onClick}
      data-testid={testID}
      onClick={onClick}
      sx={{ px: 0, py: 1.5, gap: 1, '&:hover': { bgcolor: 'transparent' } }}
    >
      <ListItemIcon sx={rowIcon}>{icon}</ListItemIcon>
      <ListItemText primary={label} slotProps={rowLabel} sx={{ flexShrink: 0 }} />
      {value}
      <ChevronRightIcon sx={{ color: 'text.secondary', flexShrink: 0 }} />
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

const budgetValueSx = { fontSize: 15, color: 'text.secondary', textAlign: 'right' };

const BudgetValue = ({ settings }: { settings: AsyncResult<BudgetSettings> }) => {
  if (settings.isLoading) return <Skeleton data-testid="settings-budget-summary" width={160} />;

  return (
    <Typography data-testid="settings-budget-summary" sx={budgetValueSx}>
      {settings.isSuccess ? budgetSummary(settings.value) : translate(tk.common.error)}
    </Typography>
  );
};

const HolefeederSection = ({ notify }: { notify: Notify }) => {
  const navigate = useNavigate();
  const settings = useBudgetSettings();
  const [editing, setEditing] = useState(false);

  return (
    <Section title={translate(tk.budgetSection.title)} testID="settings-holefeeder">
      <Rows>
        <LinkRow
          testID="settings-budget"
          icon={<AccountBalanceWalletOutlinedIcon />}
          label={translate(tk.budgetSection.budget)}
          value={<BudgetValue settings={settings} />}
          onClick={settings.isSuccess ? () => setEditing(true) : undefined}
        />
        <LinkRow
          testID="settings-categories"
          icon={<CategoryOutlinedIcon />}
          label={translate(tk.categoriesSection.title)}
          onClick={() => navigate('/settings/categories')}
        />
        <LinkRow
          testID="settings-cashflows"
          icon={<RepeatOutlinedIcon />}
          label={translate(tk.cashflowsSection.title)}
          onClick={() => navigate('/settings/cashflows')}
        />
        <LinkRow testID="settings-tags" icon={<LocalOfferOutlinedIcon />} label={translate(tk.tagsSection.title)} />
      </Rows>
      {settings.isSuccess && (
        <BudgetSettingsFlyout open={editing} settings={settings.value} onClose={() => setEditing(false)} notify={notify} />
      )}
    </Section>
  );
};

const SettingsContent = ({ notify }: { notify: Notify }) => (
  <Stack spacing={3.5} data-testid="settings-page">
    <Typography variant="h1" sx={{ fontSize: 28, fontWeight: 700 }}>
      {translate(tk.tabs.settings)}
    </Typography>
    <Stack spacing={3.5} sx={{ maxWidth: 720 }}>
      <ProfileSection />
      <HolefeederSection notify={notify} />
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

export const SettingsPage = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <SettingsContent notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
