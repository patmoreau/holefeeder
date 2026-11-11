import { tk } from '@holefeeder/shared/core';
import { Box, Button, Stack, Typography } from '@mui/material';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { AccountsSection } from './AccountsSection';
import { RecentTransactionsSection } from './RecentTransactionsSection';
import { SpendingHero } from './SpendingHero';
import { UpcomingFlowsSection } from './UpcomingFlowsSection';

export const DashboardPage = () => (
  <ApiRefreshProvider>
    <Stack spacing={3.5} data-testid="dashboard-page">
      <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={2}>
        <Typography variant="h1" sx={{ fontSize: 28, fontWeight: 700 }}>
          {translate(tk.tabs.dashboard)}
        </Typography>
        <Button
          variant="contained"
          disableElevation
          disabled
          data-testid="dashboard-new-transaction"
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
            '&.Mui-disabled': { bgcolor: 'primary.main', color: 'primary.contrastText', opacity: 0.6 },
          }}
        >
          {translate(tk.dashboard.newTransaction)}
        </Button>
      </Stack>

      <SpendingHero />

      <AccountsSection />

      <Box
        sx={{ display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', lg: 'repeat(2, minmax(0, 1fr))' }, gap: 3, alignItems: 'start' }}
      >
        <RecentTransactionsSection />
        <UpcomingFlowsSection />
      </Box>
    </Stack>
  </ApiRefreshProvider>
);
