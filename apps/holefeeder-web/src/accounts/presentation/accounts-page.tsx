import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import {
  Alert,
  Box,
  Card,
  CardContent,
  CircularProgress,
  Chip,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Typography,
} from '@mui/material';
import { useAccounts } from '../api/use-accounts';

export const AccountsPage = () => {
  const { data: accounts, isLoading, error } = useAccounts();

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ p: 2 }}>
        <Alert severity="error">{error.message}</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Accounts
      </Typography>
      <Card>
        <CardContent sx={{ p: 0 }}>
          <List disablePadding>
            {accounts?.map((account) => (
              <ListItem key={account.id} divider>
                <ListItemIcon>
                  <AccountBalanceIcon color={account.inactive ? 'disabled' : 'primary'} />
                </ListItemIcon>
                <ListItemText
                  primary={account.name}
                  secondary={account.description || account.type}
                />
                {account.favorite && (
                  <Chip label="Favorite" size="small" color="secondary" sx={{ mr: 1 }} />
                )}
                {account.inactive && (
                  <Chip label="Inactive" size="small" variant="outlined" />
                )}
              </ListItem>
            ))}
            {accounts?.length === 0 && (
              <ListItem>
                <ListItemText primary="No accounts found." />
              </ListItem>
            )}
          </List>
        </CardContent>
      </Card>
    </Box>
  );
};
