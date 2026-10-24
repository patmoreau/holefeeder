import { Box, useColorScheme } from '@mui/material';

type BrandTileProps = {
  size: number;
  testID?: string;
};

const brandOrange = '#FF8D28';

export const BrandTile = ({ size, testID }: BrandTileProps) => {
  const { mode, systemMode } = useColorScheme();
  const isDark = (mode === 'system' ? systemMode : mode) === 'dark';
  const glyph = Math.round(size * 0.66);

  return (
    <Box
      data-testid={testID}
      role="img"
      aria-label="Holefeeder"
      sx={{
        width: size,
        height: size,
        flexShrink: 0,
        borderRadius: `${Math.round(size * 0.225)}px`,
        display: 'grid',
        placeItems: 'center',
        background: isDark ? `linear-gradient(to bottom, ${brandOrange}, #4A2A10)` : brandOrange,
        boxShadow: size >= 64 ? `0 0 ${Math.round(size * 0.2)}px rgba(255, 141, 40, 0.45)` : 'none',
      }}
    >
      <svg width={glyph} height={glyph} viewBox="0 0 456 456" fill="#FFFFFF" aria-hidden="true">
        <g transform="translate(33.6, 45.6) scale(22.8)">
          <path d="M1 1.5A1.5 1.5 0 0 1 2.5 0h12A1.5 1.5 0 0 1 16 1.5v13a1.5 1.5 0 0 1-1.5 1.5h-12A1.5 1.5 0 0 1 1 14.5V13H.5a.5.5 0 0 1 0-1H1V8.5H.5a.5.5 0 0 1 0-1H1V4H.5a.5.5 0 0 1 0-1H1V1.5zM2.5 1a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 .5.5h12a.5.5 0 0 0 .5-.5v-13a.5.5 0 0 0-.5-.5h-12z" />
          <path d="M13.5 6a.5.5 0 0 1 .5.5v3a.5.5 0 0 1-1 0v-3a.5.5 0 0 1 .5-.5zM4.828 4.464a.5.5 0 0 1 .708 0l1.09 1.09a3.003 3.003 0 0 1 3.476 0l1.09-1.09a.5.5 0 1 1 .707.708l-1.09 1.09c.74 1.037.74 2.44 0 3.476l1.09 1.09a.5.5 0 1 1-.707.708l-1.09-1.09a3.002 3.002 0 0 1-3.476 0l-1.09 1.09a.5.5 0 1 1-.708-.708l1.09-1.09a3.003 3.003 0 0 1 0-3.476l-1.09-1.09a.5.5 0 0 1 0-.708zM6.95 6.586a2 2 0 1 0 2.828 2.828A2 2 0 0 0 6.95 6.586z" />
        </g>
      </svg>
    </Box>
  );
};
