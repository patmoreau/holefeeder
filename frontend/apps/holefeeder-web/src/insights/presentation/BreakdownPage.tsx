import { tk } from '@holefeeder/shared/core';
import { type AsyncResult } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import { Alert, Box, FormControlLabel, IconButton, ListItem, Skeleton, Stack, Switch, Typography } from '@mui/material';
import { createContext, type ReactNode, useContext, useState } from 'react';
import { Link } from 'react-router';
import { translate } from '@/shared/i18n/translate';
import { Rows, Section } from '@/shared/presentation/Section';
import { currency, roundsToZero, type Tone, toneColor } from '@/shared/presentation/tone';
import { type Breakdown, type BudgetStatus, type CategoryBreakdown, type TagBreakdown } from '../domain/breakdown';
import { percent } from './percent';

export type BreakdownKind = {
  testID: string;
  title: string;
  currentTitle: string;
  categoriesTitle: string;
  categoriesEmpty: string;
  tagsTitle: string;
  tagsEmpty: string;
  higherIsGood: boolean;
  combinable: boolean;
  useBreakdown: () => AsyncResult<Breakdown>;
};

const KindContext = createContext<BreakdownKind | null>(null);

const useKind = () => useContext(KindContext)!;

const heroText = { fontSize: 15, color: 'primary.contrastText' };

const budgetTone: Record<BudgetStatus, Tone> = { none: 'neutral', under: 'positive', over: 'negative' };

const TrendBadge = ({ breakdown }: { breakdown: Breakdown }) => {
  const kind = useKind();
  return (
    <Stack
      direction="row"
      spacing={0.5}
      alignItems="center"
      data-testid={`${kind.testID}-trend`}
      sx={{
        py: 0.25,
        px: 1.25,
        borderRadius: 9999,
        bgcolor: 'background.paper',
        color: toneColor[breakdown.variation > 0 === kind.higherIsGood ? 'positive' : 'negative'],
      }}
    >
      <Box component="span" aria-hidden sx={{ fontSize: 14, fontWeight: 700 }}>
        {breakdown.variation > 0 ? '↗' : '↘'}
      </Box>
      <Typography component="span" sx={{ fontSize: 14, fontWeight: 700, color: 'inherit' }}>
        {translate(tk.dashboard.largeHeader.variation, { variation: currency(Math.abs(breakdown.variation)) })}
      </Typography>
    </Stack>
  );
};

const Hero = ({ breakdown }: { breakdown: Breakdown }) => {
  const kind = useKind();
  return (
    <Stack
      direction={{ xs: 'column', md: 'row' }}
      alignItems={{ xs: 'stretch', md: 'center' }}
      justifyContent="space-between"
      useFlexGap
      data-testid={`${kind.testID}-hero`}
      sx={{ p: { xs: 3, md: 4 }, gap: { xs: 2.5, md: 5 }, borderRadius: 4, bgcolor: 'primary.main' }}
    >
      <Stack spacing={1} alignItems="flex-start" sx={{ minWidth: 0 }}>
        <Typography sx={heroText}>{translate(kind.currentTitle)}</Typography>
        <Typography
          data-testid={`${kind.testID}-current`}
          sx={{ fontSize: { xs: 40, md: 48 }, fontWeight: 700, lineHeight: 1.2, color: 'primary.contrastText' }}
        >
          {currency(breakdown.current)}
        </Typography>
        {!roundsToZero(breakdown.average) && <TrendBadge breakdown={breakdown} />}
      </Stack>
      <Stack spacing={0.5} alignItems="flex-start">
        <Typography sx={heroText}>{translate(tk.insights.average)}</Typography>
        <Typography data-testid={`${kind.testID}-average`} sx={{ fontSize: 17, fontWeight: 600, color: 'primary.contrastText' }}>
          {currency(breakdown.average)}
        </Typography>
      </Stack>
    </Stack>
  );
};

const Dot = ({ color }: { color: string }) => (
  <Box aria-hidden sx={{ width: 12, height: 12, borderRadius: '50%', bgcolor: color, flexShrink: 0 }} />
);

const Hash = ({ size }: { size: number }) => (
  <Typography aria-hidden sx={{ fontSize: size, fontWeight: 600, color: 'text.disabled', flexShrink: 0 }}>
    #
  </Typography>
);

const Footnote = ({ testID, color, children }: { testID: string; color?: string; children: ReactNode }) => (
  <Typography data-testid={testID} noWrap sx={{ fontSize: 13, color: color ?? 'text.secondary' }}>
    {children}
  </Typography>
);

const Average = ({ testID, average }: { testID: string; average: number }) =>
  roundsToZero(average) ? null : (
    <Footnote testID={testID}>{translate(tk.insights.categoryBreakdown.avgPerPeriod, { amount: currency(average) })}</Footnote>
  );

const Budget = ({ category }: { category: CategoryBreakdown }) => {
  const kind = useKind();
  if (category.budgetStatus === 'none') return null;
  if (category.budgetStatus === 'over')
    return (
      <Footnote testID={`${kind.testID}-category-budget`} color={toneColor.negative}>
        {translate(tk.insights.categoryBreakdown.overBudget)}
      </Footnote>
    );
  return <Footnote testID={`${kind.testID}-category-budget`}>{percent(category.budgetPercentage)}</Footnote>;
};

const Name = ({ testID, size = 17, children }: { testID: string; size?: number; children: ReactNode }) => (
  <Typography data-testid={testID} noWrap sx={{ fontSize: size, minWidth: 0, flexGrow: 1 }}>
    {children}
  </Typography>
);

const Line = ({ testID, children }: { testID: string; children: ReactNode }) => (
  <Stack direction="row" alignItems="center" data-testid={testID} sx={{ py: 1.25, gap: 1.5, minWidth: 0, width: '100%' }}>
    {children}
  </Stack>
);

const Amounts = ({ children }: { children: ReactNode }) => (
  <Stack alignItems="flex-end" sx={{ flexShrink: 0 }}>
    {children}
  </Stack>
);

const CategoryLine = ({ category, testID }: { category: CategoryBreakdown; testID: string }) => {
  const kind = useKind();
  return (
    <Line testID={testID}>
      <Dot color={category.color} />
      <Name testID={`${kind.testID}-category-name`}>{category.name}</Name>
      <Amounts>
        <Typography
          data-testid={`${kind.testID}-category-amount`}
          sx={{ fontSize: 15, fontWeight: 600, color: toneColor[budgetTone[category.budgetStatus]] }}
        >
          {currency(category.amount)}
        </Typography>
        <Budget category={category} />
        <Average testID={`${kind.testID}-category-average`} average={category.average} />
      </Amounts>
    </Line>
  );
};

const TagLine = ({ tag }: { tag: TagBreakdown }) => {
  const kind = useKind();
  return (
    <Line testID={`${kind.testID}-tag`}>
      <Hash size={17} />
      <Name testID={`${kind.testID}-tag-name`}>{tag.tag}</Name>
      <Amounts>
        <Typography data-testid={`${kind.testID}-tag-amount`} sx={{ fontSize: 15, fontWeight: 600 }}>
          {currency(tag.amount)}
        </Typography>
        <Average testID={`${kind.testID}-tag-average`} average={tag.average} />
      </Amounts>
    </Line>
  );
};

const Empty = ({ text }: { text: string }) => <Typography sx={{ py: 2, fontSize: 15, color: 'text.secondary' }}>{text}</Typography>;

const Separate = ({ breakdown }: { breakdown: Breakdown }) => {
  const kind = useKind();
  return (
    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', md: 'repeat(2, minmax(0, 1fr))' }, gap: 3, alignItems: 'start' }}>
      <Section title={translate(kind.categoriesTitle)} testID={`${kind.testID}-categories`}>
        {breakdown.categories.length === 0 ? (
          <Empty text={translate(kind.categoriesEmpty)} />
        ) : (
          <Rows>
            {breakdown.categories.map((category) => (
              <ListItem key={category.id} disablePadding>
                <CategoryLine category={category} testID={`${kind.testID}-category`} />
              </ListItem>
            ))}
          </Rows>
        )}
      </Section>
      <Section title={translate(kind.tagsTitle)} testID={`${kind.testID}-tags`}>
        {breakdown.tags.length === 0 ? (
          <Empty text={translate(kind.tagsEmpty)} />
        ) : (
          <Rows>
            {breakdown.tags.map((tag) => (
              <ListItem key={tag.tag} disablePadding>
                <TagLine tag={tag} />
              </ListItem>
            ))}
          </Rows>
        )}
      </Section>
    </Box>
  );
};

const CombinedTagLine = ({ tag }: { tag: { tag: string; amount: number } }) => {
  const kind = useKind();
  return (
    <Stack
      direction="row"
      alignItems="center"
      data-testid={`${kind.testID}-combined-row`}
      sx={{ pl: 3, pb: 1, gap: 1, minWidth: 0, width: '100%' }}
    >
      <Hash size={13} />
      <Name testID={`${kind.testID}-combined-tag`} size={13}>
        {tag.tag}
      </Name>
      <Typography sx={{ fontSize: 13, color: 'text.secondary', flexShrink: 0 }}>{currency(tag.amount)}</Typography>
    </Stack>
  );
};

const Combined = ({ breakdown }: { breakdown: Breakdown }) => {
  const kind = useKind();
  return (
    <Section title={translate(tk.insights.combinedBreakdown.title)} testID={`${kind.testID}-combined-breakdown`}>
      {breakdown.categories.length === 0 ? (
        <Empty text={translate(tk.insights.combinedBreakdown.empty)} />
      ) : (
        <Rows>
          {breakdown.categories.map((category) => (
            <ListItem key={category.id} disablePadding sx={{ flexDirection: 'column', alignItems: 'stretch' }}>
              <CategoryLine category={category} testID={`${kind.testID}-combined-row`} />
              {category.tags.map((tag) => (
                <CombinedTagLine key={tag.tag} tag={tag} />
              ))}
            </ListItem>
          ))}
        </Rows>
      )}
    </Section>
  );
};

const Content = ({ combined }: { combined: boolean }) => {
  const kind = useKind();
  const breakdown = kind.useBreakdown();

  if (breakdown.isLoading) return <Skeleton variant="rounded" height={320} data-testid={`${kind.testID}-loading`} sx={{ borderRadius: 4 }} />;

  if (breakdown.isFailure)
    return (
      <Alert severity="error" data-testid={`${kind.testID}-load-error`}>
        {translate(tk.insights.loadFailed)}
      </Alert>
    );

  return (
    <>
      <Hero breakdown={breakdown.value} />
      {combined ? <Combined breakdown={breakdown.value} /> : <Separate breakdown={breakdown.value} />}
    </>
  );
};

export const BreakdownPage = ({ kind }: { kind: BreakdownKind }) => {
  const [combined, setCombined] = useState(false);

  return (
    <KindContext.Provider value={kind}>
      <Stack spacing={3.5} data-testid={`${kind.testID}-page`} sx={{ minWidth: 0 }}>
        <Stack direction="row" alignItems="center" spacing={1.5}>
          <IconButton
            component={Link}
            to="/"
            data-testid={`${kind.testID}-back`}
            aria-label={translate(tk.insights.back)}
            sx={{ bgcolor: 'background.paper', color: 'text.primary', flexShrink: 0 }}
          >
            <ChevronLeftIcon />
          </IconButton>
          <Typography variant="h1" noWrap sx={{ flexGrow: 1, minWidth: 0, fontSize: 28, fontWeight: 700 }}>
            {translate(kind.title)}
          </Typography>
          {kind.combinable && (
            <FormControlLabel
              data-testid={`${kind.testID}-combined`}
              control={<Switch color="success" checked={combined} onChange={(_, checked) => setCombined(checked)} />}
              label={translate(tk.insights.combined)}
              labelPlacement="start"
              sx={{ mr: 0, flexShrink: 0 }}
            />
          )}
        </Stack>
        <Content combined={combined} />
      </Stack>
    </KindContext.Provider>
  );
};
