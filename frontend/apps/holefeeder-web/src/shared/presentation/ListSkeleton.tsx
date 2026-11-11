import { Paper, Skeleton, Stack } from '@mui/material';

export const ListSkeleton = ({ testID, rows }: { testID: string; rows: number }) => (
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
