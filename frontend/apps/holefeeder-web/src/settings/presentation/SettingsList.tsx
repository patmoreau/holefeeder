import { Divider, List, Paper, Stack, Typography } from '@mui/material';
import { Children, Fragment, type ReactNode } from 'react';

export const Section = ({ title, testID, children }: { title: string; testID: string; children: ReactNode }) => (
  <Stack spacing={1.5} data-testid={testID}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{title}</Typography>
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {children}
    </Paper>
  </Stack>
);

export const Rows = ({ children }: { children: ReactNode }) => (
  <List disablePadding>
    {Children.toArray(children).map((child, index) => (
      <Fragment key={index}>
        {index > 0 && <Divider component="li" />}
        {child}
      </Fragment>
    ))}
  </List>
);
