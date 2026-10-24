import { tk } from '@holefeeder/shared/core';
import { Box, Button, CircularProgress, Stack, Typography } from '@mui/material';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { translate } from '@/shared/i18n/translate';
import { BrandTile } from '@/shared/presentation/BrandTile';

const wash = (opacity: number) => `linear-gradient(to bottom, rgba(255, 141, 40, ${opacity}), rgba(255, 141, 40, 0) 60%)`;

export const WelcomePage = () => {
  const { isLoading, login } = useAuthentication();

  return (
    <Box
      data-testid="welcome-page"
      sx={(theme) => ({
        minHeight: '100vh',
        display: 'grid',
        placeItems: 'center',
        px: 2,
        bgcolor: 'background.paper',
        backgroundImage: wash(0.12),
        ...theme.applyStyles('dark', { backgroundImage: wash(0.18) }),
      })}
    >
      <Stack spacing={5} alignItems="center" sx={{ width: '100%', maxWidth: 400, mb: 5 }}>
        <Stack spacing={1} alignItems="center" textAlign="center">
          <Box sx={{ mb: 2 }}>
            <BrandTile size={120} testID="welcome-brand-tile" />
          </Box>
          <Typography variant="h1" sx={{ fontSize: 34, fontWeight: 700 }}>
            Holefeeder
          </Typography>
          {isLoading ? (
            <CircularProgress data-testid="welcome-loading" sx={{ mt: 2 }} />
          ) : (
            <>
              <Typography variant="h2" sx={{ fontSize: 22, fontWeight: 600 }}>
                {translate(tk.auth.welcomeTitle)}
              </Typography>
              <Typography sx={{ fontSize: 17, color: 'text.secondary' }}>{translate(tk.auth.welcomeSubtitle)}</Typography>
            </>
          )}
        </Stack>
        {!isLoading && (
          <Stack spacing={1.5} sx={{ width: '100%' }}>
            <Button
              variant="contained"
              size="large"
              disableElevation
              data-testid="welcome-signup-button"
              onClick={() => login({ signUp: true })}
              sx={{ py: 1.5, fontSize: 17, borderRadius: '12px' }}
            >
              {translate(tk.auth.createAccountButton)}
            </Button>
            <Button
              variant="outlined"
              size="large"
              data-testid="welcome-signin-button"
              onClick={() => login({ signUp: false })}
              sx={{ py: 1.5, fontSize: 17, borderRadius: '12px', color: 'text.primary', borderColor: 'divider', bgcolor: 'background.default' }}
            >
              {translate(tk.auth.signInButton)}
            </Button>
          </Stack>
        )}
      </Stack>
    </Box>
  );
};
