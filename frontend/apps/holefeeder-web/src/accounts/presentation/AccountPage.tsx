import { type AsyncResult, type DateOnly, LocalFormatter, tk, today } from '@holefeeder/shared/core';
import AddIcon from '@mui/icons-material/Add';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Alert, Box, Button, IconButton, Skeleton, Stack, Typography } from '@mui/material';
import type { Theme } from '@mui/material/styles';
import type {} from '@mui/material/themeCssVarsAugmentation';
import { type ReactNode, useState } from 'react';
import { Link, useParams } from 'react-router';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { accountTypeKey } from '@/shared/domain/account-type';
import { balanceCaptionKey } from '@/shared/domain/balance-caption';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast, type Notify } from '@/shared/presentation/DashboardToast';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { ListSkeleton } from '@/shared/presentation/ListSkeleton';
import { NewTransactionFlyout } from '@/shared/presentation/NewTransactionFlyout';
import { type Tone, toneColor, toneOf, unsignedCurrency } from '@/shared/presentation/tone';
import { TransactionList } from '@/shared/presentation/TransactionList';
import { useAccountDetail } from '../api/use-account-detail';
import { useAccountTransactions } from '../api/use-account-transactions';
import type { AccountDetail } from '../domain/account-detail';
import { EditAccountFlyout } from './EditAccountFlyout';

const heroText = { fontSize: 15, color: 'primary.contrastText' };

const toneBackground: Record<Exclude<Tone, 'neutral'>, string> = { positive: 'success.light', negative: 'error.light' };

const pillSx = (tone: Tone) => ({
  px: 1.25,
  py: 0.5,
  borderRadius: 9999,
  fontSize: 17,
  fontWeight: 600,
  whiteSpace: 'nowrap',
  ...(tone === 'neutral'
    ? {
        color: 'primary.contrastText',
        bgcolor: (theme: Theme) => `rgba(${theme.vars!.palette.primary.contrastTextChannel} / 0.2)`,
      }
    : { color: toneColor[tone], bgcolor: toneBackground[tone] }),
});

const HeroPill = ({ label, testID, tone, children }: { label: string; testID: string; tone: Tone; children: ReactNode }) => (
  <Stack spacing={0.75} alignItems="center" data-testid={testID}>
    <Typography sx={heroText}>{label}</Typography>
    <Typography data-testid={`${testID}-amount`} sx={pillSx(tone)}>
      {children}
    </Typography>
  </Stack>
);

const formatDate = (date: DateOnly) => LocalFormatter.date(date, today(), Language.locale(), translate);

const subtitleOf = (account: AccountDetail) =>
  [account.description, translate(accountTypeKey[account.type])].filter((part) => part.trim() !== '').join(' · ');

const Hero = ({ account }: { account: AccountDetail }) => (
  <Stack
    direction={{ xs: 'column', md: 'row' }}
    alignItems={{ xs: 'stretch', md: 'center' }}
    justifyContent="space-between"
    useFlexGap
    data-testid="account-hero"
    sx={{ p: { xs: 3, md: 4 }, gap: { xs: 2.5, md: 5 }, borderRadius: 4, bgcolor: 'primary.main' }}
  >
    <Stack spacing={1} sx={{ minWidth: 0 }}>
      <Typography data-testid="account-caption" sx={heroText}>
        {translate(balanceCaptionKey[account.caption])}
      </Typography>
      <Typography
        data-testid="account-balance"
        sx={{ fontSize: { xs: 40, md: 48 }, fontWeight: 700, lineHeight: 1.2, color: 'primary.contrastText' }}
      >
        {unsignedCurrency(account.balance)}
      </Typography>
      <Typography
        data-testid="account-subtitle"
        sx={{ fontSize: 15, color: (theme) => `rgba(${theme.vars!.palette.primary.contrastTextChannel} / 0.75)`, overflowWrap: 'anywhere' }}
      >
        {subtitleOf(account)}
      </Typography>
    </Stack>
    <Stack direction={{ xs: 'column', md: 'row' }} alignItems={{ xs: 'stretch', md: 'center' }} useFlexGap sx={{ gap: { xs: 2.5, md: 5 } }}>
      <Box
        sx={{
          width: { md: '1px' },
          height: { xs: '1px', md: 72 },
          bgcolor: (theme) => `rgba(${theme.vars!.palette.primary.contrastTextChannel} / 0.6)`,
        }}
      />
      <Stack direction="row" alignItems="center" justifyContent="space-between" useFlexGap sx={{ gap: { xs: 1, md: 5 } }}>
        <HeroPill label={translate(tk.accountCard.updated)} testID="account-updated" tone="neutral">
          {formatDate(account.updated)}
        </HeroPill>
        <HeroPill label={translate(tk.accountCard.upcoming)} testID="account-upcoming" tone={toneOf(account.upcoming)}>
          {unsignedCurrency(account.upcoming)}
        </HeroPill>
        <HeroPill label={translate(tk.accountCard.projected)} testID="account-projected" tone={toneOf(account.projectedBalance)}>
          {unsignedCurrency(account.projectedBalance)}
        </HeroPill>
      </Stack>
    </Stack>
  </Stack>
);

const actionButtonSx = {
  flexShrink: 0,
  whiteSpace: 'nowrap',
  fontSize: 15,
  minWidth: { xs: 40, md: 64 },
  width: { xs: 40, md: 'auto' },
  height: { xs: 40, md: 'auto' },
  borderRadius: { xs: '50%', md: 1 },
  p: { xs: 0, md: 1.25 },
  pl: { md: 2 },
  pr: { md: 2.25 },
};

const ActionLabel = ({ children }: { children: ReactNode }) => (
  <Box component="span" sx={{ display: { xs: 'none', md: 'inline' } }}>
    {children}
  </Box>
);

type HeaderProps = { name: ReactNode; onEdit?: () => void; onNewTransaction: () => void };

const Header = ({ name, onEdit, onNewTransaction }: HeaderProps) => (
  <Stack direction="row" alignItems="center" spacing={1.5}>
    <IconButton
      component={Link}
      to="/"
      data-testid="account-back"
      aria-label={translate(tk.common.back)}
      sx={{ bgcolor: 'background.paper', color: 'text.primary', flexShrink: 0 }}
    >
      <ChevronLeftIcon />
    </IconButton>
    <Box sx={{ flexGrow: 1, minWidth: 0 }}>{name}</Box>
    {onEdit && (
      <Button variant="outlined" data-testid="account-edit" aria-label={translate(tk.accountCard.edit)} onClick={onEdit} sx={actionButtonSx}>
        <EditOutlinedIcon fontSize="small" sx={{ mr: { md: 1 } }} />
        <ActionLabel>{translate(tk.accountCard.edit)}</ActionLabel>
      </Button>
    )}
    <Button
      variant="contained"
      disableElevation
      data-testid="account-new-transaction"
      aria-label={translate(tk.dashboard.newTransaction)}
      onClick={onNewTransaction}
      sx={actionButtonSx}
    >
      <AddIcon fontSize="small" sx={{ mr: { md: 1 } }} />
      <ActionLabel>{translate(tk.dashboard.newTransaction)}</ActionLabel>
    </Button>
  </Stack>
);

const AccountName = ({ name }: { name: string }) => (
  <Typography variant="h1" data-testid="account-name" noWrap sx={{ fontSize: 28, fontWeight: 700 }}>
    {name}
  </Typography>
);

const AccountSummary = ({ account }: { account: AsyncResult<AccountDetail> }) => {
  if (account.isLoading) return <Skeleton variant="rounded" height={174} data-testid="account-loading" sx={{ borderRadius: 4 }} />;

  if (account.isFailure)
    return (
      <Alert severity="error" data-testid="account-error">
        {account.errors.join(', ')}
      </Alert>
    );

  return <Hero account={account.value} />;
};

const TransactionsContent = ({
  accountId,
  accountName,
  onAddExpense,
}: {
  accountId: string;
  accountName: string;
  onAddExpense: () => void;
}) => {
  const { transactions, hasMore, loadingMore, loadMore } = useAccountTransactions(accountId);

  if (transactions.isLoading) return <ListSkeleton testID="account-transactions-skeleton" rows={6} />;

  if (transactions.isFailure)
    return (
      <Alert severity="error" data-testid="account-transactions-error">
        {transactions.errors.join(', ')}
      </Alert>
    );

  if (transactions.value.length === 0)
    return (
      <EmptyList
        testID="account-transactions-empty"
        title={translate(tk.emptyStates.accountTransactions.title, { account: accountName })}
        hint={translate(tk.emptyStates.accountTransactions.hint)}
        action={
          <Button
            variant="contained"
            disableElevation
            data-testid="account-add-expense"
            onClick={onAddExpense}
            startIcon={<AddIcon />}
            sx={{ mt: 2 }}
          >
            {translate(tk.emptyStates.addExpense)}
          </Button>
        }
      />
    );

  return (
    <TransactionList transactions={transactions.value} testID="account-transaction">
      {hasMore && (
        <Box sx={{ borderTop: 1, borderColor: 'divider', py: 1, textAlign: 'center' }}>
          <Button data-testid="account-load-more" disabled={loadingMore} onClick={loadMore} sx={{ fontSize: 15, fontWeight: 600 }}>
            {translate(tk.accountDetail.loadMore)}
          </Button>
        </Box>
      )}
    </TransactionList>
  );
};

const AccountContent = ({ accountId, notify }: { accountId: string; notify: Notify }) => {
  const account = useAccountDetail(accountId);
  const [creating, setCreating] = useState(false);
  const [editing, setEditing] = useState(false);
  const accountName = account.isSuccess ? account.value.name : '';

  return (
    <>
      <Stack spacing={3.5} data-testid="account-page">
        <Header
          name={account.isLoading ? <Skeleton width="40%" height={42} /> : <AccountName name={accountName} />}
          onEdit={account.isSuccess ? () => setEditing(true) : undefined}
          onNewTransaction={() => setCreating(true)}
        />
        <AccountSummary account={account} />
        <Stack useFlexGap sx={{ gap: 1.5 }}>
          <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.transactionList.title)}</Typography>
          <TransactionsContent accountId={accountId} accountName={accountName} onAddExpense={() => setCreating(true)} />
        </Stack>
      </Stack>
      <NewTransactionFlyout open={creating} accountId={accountId} onClose={() => setCreating(false)} notify={notify} />
      {account.isSuccess && <EditAccountFlyout open={editing} account={account.value} onClose={() => setEditing(false)} notify={notify} />}
    </>
  );
};

export const AccountPage = () => {
  const { id = '' } = useParams();
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <AccountContent key={id} accountId={id} notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
