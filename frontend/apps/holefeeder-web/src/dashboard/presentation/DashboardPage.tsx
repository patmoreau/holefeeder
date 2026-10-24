import { tk } from '@holefeeder/shared/core';
import { Box, Paper, Skeleton, Stack, Typography } from '@mui/material';
import { translate } from '@/shared/i18n/translate';

const sectionLabel = { fontSize: 15, color: 'text.secondary' };

const ListSkeleton = ({ testID, rows }: { testID: string; rows: number }) => (
  <Paper elevation={0} data-testid={testID} sx={{ px: 2, borderRadius: 3 }}>
    {Array.from({ length: rows }, (_, i) => (
      <Stack key={i} direction="row" spacing={1.5} alignItems="center" sx={{ py: 1.5, borderTop: i ? 1 : 0, borderColor: 'divider' }}>
        <Skeleton variant="rounded" width={20} height={18} />
        <Stack sx={{ flexGrow: 1 }}>
          <Skeleton width="30%" />
          <Skeleton width="15%" />
        </Stack>
        <Stack alignItems="flex-end">
          <Skeleton width={72} />
          <Skeleton width={88} />
        </Stack>
      </Stack>
    ))}
  </Paper>
);

export const DashboardPage = () => (
  <Stack spacing={3.5} data-testid="dashboard-page">
    <Typography variant="h1" sx={{ fontSize: 28, fontWeight: 700 }}>
      {translate(tk.tabs.dashboard)}
    </Typography>

    <Skeleton variant="rounded" height={189} data-testid="dashboard-spending-skeleton" sx={{ borderRadius: 4 }} />

    <Stack spacing={1.5}>
      <Skeleton width={80} sx={sectionLabel} />
      <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: 2 }}>
        {Array.from({ length: 4 }, (_, i) => (
          <Skeleton key={i} variant="rounded" height={171} data-testid="dashboard-account-skeleton" sx={{ borderRadius: 3 }} />
        ))}
      </Box>
    </Stack>

    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 3, alignItems: 'start' }}>
      <Stack spacing={1.5}>
        <Skeleton width={140} sx={sectionLabel} />
        <ListSkeleton testID="dashboard-transactions-skeleton" rows={4} />
      </Stack>
      <Stack spacing={1.5}>
        <Skeleton width={120} sx={sectionLabel} />
        <ListSkeleton testID="dashboard-upcoming-skeleton" rows={3} />
      </Stack>
    </Box>
  </Stack>
);
