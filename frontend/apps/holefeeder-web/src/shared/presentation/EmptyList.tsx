import { Paper, Typography } from '@mui/material';

export const EmptyList = ({ testID, title, hint }: { testID: string; title: string; hint: string }) => (
  <Paper elevation={0} data-testid={testID} sx={{ p: 3, borderRadius: 3, textAlign: 'center' }}>
    <Typography sx={{ fontSize: 17, fontWeight: 600 }}>{title}</Typography>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{hint}</Typography>
  </Paper>
);
