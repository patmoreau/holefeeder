import { tk } from '@holefeeder/shared/core';
import { Box, Button, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast } from '@/shared/presentation/DashboardToast';
import { NewTransactionFlyout } from '@/shared/presentation/NewTransactionFlyout';
import { AccountsSection } from './AccountsSection';
import { AddAccountFlyout } from './AddAccountFlyout';
import { RecentTransactionsSection } from './RecentTransactionsSection';
import { SpendingHero } from './SpendingHero';
import { UpcomingFlowsSection } from './UpcomingFlowsSection';

export const DashboardPage = () => {
  const [toast, setToast] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);
  const [addingAccount, setAddingAccount] = useState(false);

  return (
    <ApiRefreshProvider>
      <Stack spacing={3.5} data-testid="dashboard-page">
        <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={2}>
          <Typography variant="h1" sx={{ fontSize: 28, fontWeight: 700 }}>
            {translate(tk.tabs.dashboard)}
          </Typography>
          <Button
            variant="contained"
            disableElevation
            data-testid="dashboard-new-transaction"
            onClick={() => setCreating(true)}
            startIcon={
              <Box component="span" aria-hidden sx={{ fontSize: 18, lineHeight: 1 }}>
                +
              </Box>
            }
            sx={{
              flexShrink: 0,
              whiteSpace: 'nowrap',
              fontSize: 15,
              py: 1.25,
              pl: 2,
              pr: 2.25,
            }}
          >
            {translate(tk.dashboard.newTransaction)}
          </Button>
        </Stack>

        <SpendingHero />

        <AccountsSection onAddAccount={() => setAddingAccount(true)} />

        <Box
          sx={{ display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', lg: 'repeat(2, minmax(0, 1fr))' }, gap: 3, alignItems: 'start' }}
        >
          <RecentTransactionsSection />
          <UpcomingFlowsSection notify={setToast} />
        </Box>
      </Stack>
      <NewTransactionFlyout open={creating} onClose={() => setCreating(false)} notify={setToast} />
      <AddAccountFlyout open={addingAccount} onClose={() => setAddingAccount(false)} notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
