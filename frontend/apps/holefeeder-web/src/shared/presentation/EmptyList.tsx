import { Paper, Typography } from '@mui/material';
import type { ReactNode } from 'react';

export const EmptyList = ({ testID, title, hint, action }: { testID: string; title: string; hint: string; action?: ReactNode }) => (
  <Paper elevation={0} data-testid={testID} sx={{ p: 3, borderRadius: 3, textAlign: 'center' }}>
    <Typography sx={{ fontSize: 17, fontWeight: 600 }}>{title}</Typography>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{hint}</Typography>
    {action}
  </Paper>
);
