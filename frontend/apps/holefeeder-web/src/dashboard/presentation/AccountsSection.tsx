import { LocalFormatter, tk, today } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import { Alert, Box, Button, Divider, IconButton, Paper, Skeleton, Stack, Typography } from '@mui/material';
import { type ReactNode, type RefObject, useCallback, useEffect, useRef, useState } from 'react';
import { translate } from '@/shared/i18n/translate';
import { useDashboardAccounts } from '../api/use-dashboard-accounts';
import type { BalanceCaption, DashboardAccount } from '../domain/dashboard-account';
import { toneColor, toneOf, unsignedCurrency } from './tone';

const cardHeight = 171;
const cardWidth = 264;

const captionKey: Record<BalanceCaption, string> = {
  availableBalance: tk.accountCard.availableBalance,
  amountDue: tk.accountCard.amountDue,
  creditBalance: tk.accountCard.creditBalance,
};

const lineHeight = 1.2;

const smallLabel = { fontSize: 11, lineHeight: lineHeight, color: 'text.secondary' };

const amountText = (value: number) => ({ fontSize: 20, fontWeight: 600, lineHeight: lineHeight, color: toneColor[toneOf(value)] });

type Overflow = { before: boolean; after: boolean };

const noOverflow: Overflow = { before: false, after: false };

const fade = (overflow: Overflow) => {
  if (!overflow.before && !overflow.after) return undefined;
  const start = overflow.before ? 'transparent, black 32px' : 'black';
  const end = overflow.after ? 'black calc(100% - 32px), transparent' : 'black';
  return `linear-gradient(to right, ${start}, ${end})`;
};

const useRowOverflow = (row: RefObject<HTMLDivElement | null>, itemCount: number) => {
  const [overflow, setOverflow] = useState<Overflow>(noOverflow);

  const measure = useCallback(() => {
    const element = row.current;
    if (!element) return;
    setOverflow({
      before: element.scrollLeft > 1,
      after: element.scrollLeft + element.clientWidth < element.scrollWidth - 1,
    });
  }, [row]);

  useEffect(() => {
    const element = row.current;
    if (!element) return;
    measure();
    const observer = new ResizeObserver(measure);
    observer.observe(element);
    element.addEventListener('scroll', measure, { passive: true });
    return () => {
      observer.disconnect();
      element.removeEventListener('scroll', measure);
    };
  }, [row, measure, itemCount]);

  return overflow;
};

const AccountRow = ({
  rowRef,
  overflow,
  children,
}: {
  rowRef?: RefObject<HTMLDivElement | null>;
  overflow?: Overflow;
  children: ReactNode;
}) => {
  const mask = overflow && fade(overflow);
  return (
    <Box
      ref={rowRef}
      data-testid="dashboard-accounts-row"
      sx={{
        display: 'flex',
        gap: 2,
        overflowX: 'auto',
        scrollSnapType: 'x mandatory',
        scrollBehavior: 'smooth',
        scrollbarWidth: 'none',
        '&::-webkit-scrollbar': { display: 'none' },
        mx: -1,
        px: 1,
        py: 1,
        my: -1,
        maskImage: mask,
        '& > *': { flex: `0 0 ${cardWidth}px`, scrollSnapAlign: 'start', scrollMarginLeft: 8 },
      }}
    >
      {children}
    </Box>
  );
};

const RowArrows = ({ rowRef, overflow }: { rowRef: RefObject<HTMLDivElement | null>; overflow: Overflow }) => {
  if (!overflow.before && !overflow.after) return null;

  const scrollByPage = (direction: 1 | -1) => {
    const element = rowRef.current;
    if (element) element.scrollBy({ left: direction * element.clientWidth });
  };

  return (
    <Stack direction="row" spacing={0.5} sx={{ display: { xs: 'none', md: 'flex' } }}>
      <IconButton
        size="small"
        disabled={!overflow.before}
        onClick={() => scrollByPage(-1)}
        aria-label={translate(tk.dashboard.previousAccounts)}
        data-testid="dashboard-accounts-previous"
      >
        <ChevronLeftIcon fontSize="small" />
      </IconButton>
      <IconButton
        size="small"
        disabled={!overflow.after}
        onClick={() => scrollByPage(1)}
        aria-label={translate(tk.dashboard.nextAccounts)}
        data-testid="dashboard-accounts-next"
      >
        <ChevronRightIcon fontSize="small" />
      </IconButton>
    </Stack>
  );
};

const AccountCard = ({ account }: { account: DashboardAccount }) => (
  <Paper
    elevation={0}
    data-testid="dashboard-account-card"
    sx={{
      p: 2,
      height: cardHeight,
      display: 'flex',
      flexDirection: 'column',
      gap: 1.5,
      borderRadius: 3,
      boxShadow: '0 2px 8px rgba(0, 0, 0, 0.08)',
    }}
  >
    <Typography data-testid="dashboard-account-name" noWrap sx={{ fontSize: 17, fontWeight: 600, lineHeight: lineHeight }}>
      {account.name}
    </Typography>
    <Stack spacing={0.25}>
      <Typography
        data-testid="dashboard-account-caption"
        sx={{ fontSize: 12, fontWeight: 600, lineHeight: lineHeight, color: 'text.secondary' }}
      >
        {translate(captionKey[account.caption])}
      </Typography>
      <Typography data-testid="dashboard-account-balance" sx={amountText(account.balance)}>
        {unsignedCurrency(account.balance)}
      </Typography>
    </Stack>
    <Divider />
    <Stack direction="row" alignItems="flex-end" justifyContent="space-between">
      <Stack data-testid="dashboard-account-updated">
        <Typography sx={smallLabel}>{translate(tk.accountCard.updated)}</Typography>
        <Typography sx={{ fontSize: 17, lineHeight: lineHeight }}>
          {LocalFormatter.date(account.updated, today(), 'en-CA', translate)}
        </Typography>
      </Stack>
      <Stack data-testid="dashboard-account-projected" alignItems="flex-end">
        <Typography sx={smallLabel}>{translate(tk.accountCard.projected)}</Typography>
        <Typography data-testid="dashboard-account-projected-amount" sx={amountText(account.projectedBalance)}>
          {unsignedCurrency(account.projectedBalance)}
        </Typography>
      </Stack>
    </Stack>
  </Paper>
);

const AddAccountTile = () => (
  <Button
    disabled
    data-testid="dashboard-add-account"
    sx={{
      height: cardHeight,
      flexDirection: 'column',
      gap: 1,
      borderRadius: 3,
      border: 2,
      borderStyle: 'dashed',
      borderColor: 'divider',
      textTransform: 'none',
      '&.Mui-disabled': { color: 'text.secondary', borderColor: 'divider' },
    }}
  >
    <Box component="span" aria-hidden sx={{ fontSize: 28, lineHeight: 1 }}>
      +
    </Box>
    <Box component="span" sx={{ fontSize: 17, fontWeight: 600 }}>
      {translate(tk.accountCard.add)}
    </Box>
  </Button>
);

const sectionLabel = { fontSize: 15, color: 'text.secondary' };

const SectionHeader = ({ arrows }: { arrows?: ReactNode }) => (
  <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ minHeight: 30 }}>
    <Typography sx={sectionLabel}>{translate(tk.tabs.accounts)}</Typography>
    {arrows}
  </Stack>
);

const AccountCards = ({ accounts }: { accounts: DashboardAccount[] }) => {
  const rowRef = useRef<HTMLDivElement>(null);
  const overflow = useRowOverflow(rowRef, accounts.length);

  return (
    <>
      <SectionHeader arrows={<RowArrows rowRef={rowRef} overflow={overflow} />} />
      <AccountRow rowRef={rowRef} overflow={overflow}>
        {accounts.map((account) => (
          <AccountCard key={account.id} account={account} />
        ))}
        <AddAccountTile />
      </AccountRow>
    </>
  );
};

const AccountsContent = () => {
  const accounts = useDashboardAccounts();

  if (accounts.isLoading) {
    return (
      <>
        <SectionHeader />
        <AccountRow>
          {Array.from({ length: 4 }, (_, i) => (
            <Skeleton key={i} variant="rounded" height={cardHeight} data-testid="dashboard-account-skeleton" sx={{ borderRadius: 3 }} />
          ))}
        </AccountRow>
      </>
    );
  }

  if (accounts.isFailure) {
    return (
      <>
        <SectionHeader />
        <Alert severity="error" data-testid="dashboard-accounts-error">
          {accounts.errors.join(', ')}
        </Alert>
      </>
    );
  }

  return <AccountCards accounts={accounts.value} />;
};

export const AccountsSection = () => (
  <Stack useFlexGap sx={{ gap: 1.5 }}>
    <AccountsContent />
  </Stack>
);
