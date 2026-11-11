import { LocalFormatter, tk, today } from '@holefeeder/shared/core';
import { Alert, Box, Button, Divider, Paper, Skeleton, Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';
import { translate } from '@/shared/i18n/translate';
import { useDashboardAccounts } from '../api/use-dashboard-accounts';
import type { BalanceCaption, DashboardAccount } from '../domain/dashboard-account';
import { toneColor, toneOf, unsignedCurrency } from './tone';

const cardHeight = 171;

const captionKey: Record<BalanceCaption, string> = {
  availableBalance: tk.accountCard.availableBalance,
  amountDue: tk.accountCard.amountDue,
  creditBalance: tk.accountCard.creditBalance,
};

const lineHeight = 1.2;

const smallLabel = { fontSize: 11, lineHeight: lineHeight, color: 'text.secondary' };

const amountText = (value: number) => ({ fontSize: 20, fontWeight: 600, lineHeight: lineHeight, color: toneColor[toneOf(value)] });

const AccountGrid = ({ children }: { children: ReactNode }) => (
  <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: 2 }}>{children}</Box>
);

const AccountCard = ({ account }: { account: DashboardAccount }) => (
  <Paper
    elevation={0}
    data-testid="dashboard-account-card"
    sx={{
      p: 2,
      height: cardHeight,
      display: 'flex',
      flexDirection: 'column',
      gap: 1.5,
      borderRadius: 3,
      boxShadow: '0 2px 8px rgba(0, 0, 0, 0.08)',
    }}
  >
    <Typography data-testid="dashboard-account-name" noWrap sx={{ fontSize: 17, fontWeight: 600, lineHeight: lineHeight }}>
      {account.name}
    </Typography>
    <Stack spacing={0.25}>
      <Typography
        data-testid="dashboard-account-caption"
        sx={{ fontSize: 12, fontWeight: 600, lineHeight: lineHeight, color: 'text.secondary' }}
      >
        {translate(captionKey[account.caption])}
      </Typography>
      <Typography data-testid="dashboard-account-balance" sx={amountText(account.balance)}>
        {unsignedCurrency(account.balance)}
      </Typography>
    </Stack>
    <Divider />
    <Stack direction="row" alignItems="flex-end" justifyContent="space-between">
      <Stack data-testid="dashboard-account-updated">
        <Typography sx={smallLabel}>{translate(tk.accountCard.updated)}</Typography>
        <Typography sx={{ fontSize: 17, lineHeight: lineHeight }}>
          {LocalFormatter.date(account.updated, today(), 'en-CA', translate)}
        </Typography>
      </Stack>
      <Stack data-testid="dashboard-account-projected" alignItems="flex-end">
        <Typography sx={smallLabel}>{translate(tk.accountCard.projected)}</Typography>
        <Typography data-testid="dashboard-account-projected-amount" sx={amountText(account.projectedBalance)}>
          {unsignedCurrency(account.projectedBalance)}
        </Typography>
      </Stack>
    </Stack>
  </Paper>
);

const AddAccountTile = () => (
  <Button
    disabled
    data-testid="dashboard-add-account"
    sx={{
      height: cardHeight,
      flexDirection: 'column',
      gap: 1,
      borderRadius: 3,
      border: 2,
      borderStyle: 'dashed',
      borderColor: 'divider',
      textTransform: 'none',
      '&.Mui-disabled': { color: 'text.secondary', borderColor: 'divider' },
    }}
  >
    <Box component="span" aria-hidden sx={{ fontSize: 28, lineHeight: 1 }}>
      +
    </Box>
    <Box component="span" sx={{ fontSize: 17, fontWeight: 600 }}>
      {translate(tk.accountCard.add)}
    </Box>
  </Button>
);

const AccountsContent = () => {
  const accounts = useDashboardAccounts();

  if (accounts.isLoading) {
    return (
      <AccountGrid>
        {Array.from({ length: 4 }, (_, i) => (
          <Skeleton key={i} variant="rounded" height={cardHeight} data-testid="dashboard-account-skeleton" sx={{ borderRadius: 3 }} />
        ))}
      </AccountGrid>
    );
  }

  if (accounts.isFailure) {
    return (
      <Alert severity="error" data-testid="dashboard-accounts-error">
        {accounts.errors.join(', ')}
      </Alert>
    );
  }

  return (
    <AccountGrid>
      {accounts.value.map((account) => (
        <AccountCard key={account.id} account={account} />
      ))}
      <AddAccountTile />
    </AccountGrid>
  );
};

export const AccountsSection = () => (
  <Stack spacing={1.5}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.tabs.accounts)}</Typography>
    <AccountsContent />
  </Stack>
);
