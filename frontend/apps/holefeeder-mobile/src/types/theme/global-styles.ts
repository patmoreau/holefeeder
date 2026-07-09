import { StyleSheet } from 'react-native';

export const GlobalStyles = StyleSheet.create({
  // Common layouts
  flex1: { flex: 1 },
  flex2: { flex: 2 },
  flexGrow: { flexGrow: 1 },
  flexShrink: { flexShrink: 1 },

  // Flex directions
  row: { flexDirection: 'row' },
  column: { flexDirection: 'column' },
  rowReverse: { flexDirection: 'row-reverse' },
  columnReverse: { flexDirection: 'column-reverse' },

  // Justify content
  center: { alignItems: 'center', justifyContent: 'center' },
  justifyCenter: { justifyContent: 'center' },
  justifyStart: { justifyContent: 'flex-start' },
  justifyEnd: { justifyContent: 'flex-end' },
  spaceBetween: { justifyContent: 'space-between' },
  spaceAround: { justifyContent: 'space-around' },
  spaceEvenly: { justifyContent: 'space-evenly' },

  // Align items
  alignCenter: { alignItems: 'center' },
  alignStart: { alignItems: 'flex-start' },
  alignEnd: { alignItems: 'flex-end' },
  alignStretch: { alignItems: 'stretch' },
  alignBaseline: { alignItems: 'baseline' },

  // Self alignment
  selfCenter: { alignSelf: 'center' },
  selfStart: { alignSelf: 'flex-start' },
  selfEnd: { alignSelf: 'flex-end' },
  selfStretch: { alignSelf: 'stretch' },

  // Position
  absolute: { position: 'absolute' },
  relative: { position: 'relative' },

  // Positioning helpers
  fullScreen: { position: 'absolute', top: 0, left: 0, right: 0, bottom: 0 },
  topFull: { top: 0, left: 0, right: 0 },
  bottomFull: { bottom: 0, left: 0, right: 0 },

  // Width & Height
  fullWidth: { width: '100%' },
  fullHeight: { height: '100%' },
  halfWidth: { width: '50%' },

  // Common margins (all sides)
  m0: { margin: 0 },
  m2: { margin: 2 },
  m4: { margin: 4 },
  m8: { margin: 8 },
  m12: { margin: 12 },
  m16: { margin: 16 },
  m20: { margin: 20 },
  m24: { margin: 24 },
  m32: { margin: 32 },

  // Margin top
  mt0: { marginTop: 0 },
  mt2: { marginTop: 2 },
  mt4: { marginTop: 4 },
  mt8: { marginTop: 8 },
  mt12: { marginTop: 12 },
  mt16: { marginTop: 16 },
  mt20: { marginTop: 20 },
  mt24: { marginTop: 24 },
  mt32: { marginTop: 32 },

  // Margin bottom
  mb0: { marginBottom: 0 },
  mb2: { marginBottom: 2 },
  mb4: { marginBottom: 4 },
  mb8: { marginBottom: 8 },
  mb12: { marginBottom: 12 },
  mb16: { marginBottom: 16 },
  mb20: { marginBottom: 20 },
  mb24: { marginBottom: 24 },
  mb32: { marginBottom: 32 },

  // Margin horizontal
  mx0: { marginHorizontal: 0 },
  mx2: { marginHorizontal: 2 },
  mx4: { marginHorizontal: 4 },
  mx8: { marginHorizontal: 8 },
  mx12: { marginHorizontal: 12 },
  mx16: { marginHorizontal: 16 },
  mx20: { marginHorizontal: 20 },
  mx24: { marginHorizontal: 24 },
  mx32: { marginHorizontal: 32 },

  // Margin vertical
  my0: { marginVertical: 0 },
  my2: { marginVertical: 2 },
  my4: { marginVertical: 4 },
  my8: { marginVertical: 8 },
  my12: { marginVertical: 12 },
  my16: { marginVertical: 16 },
  my20: { marginVertical: 20 },
  my24: { marginVertical: 24 },
  my32: { marginVertical: 32 },

  // Common padding (all sides)
  p0: { padding: 0 },
  p2: { padding: 2 },
  p4: { padding: 4 },
  p8: { padding: 8 },
  p12: { padding: 12 },
  p16: { padding: 16 },
  p20: { padding: 20 },
  p24: { padding: 24 },
  p32: { padding: 32 },

  // Padding horizontal
  px0: { paddingHorizontal: 0 },
  px2: { paddingHorizontal: 2 },
  px4: { paddingHorizontal: 4 },
  px8: { paddingHorizontal: 8 },
  px12: { paddingHorizontal: 12 },
  px16: { paddingHorizontal: 16 },
  px20: { paddingHorizontal: 20 },
  px24: { paddingHorizontal: 24 },
  px32: { paddingHorizontal: 32 },

  // Padding vertical
  py0: { paddingVertical: 0 },
  py2: { paddingVertical: 2 },
  py4: { paddingVertical: 4 },
  py8: { paddingVertical: 8 },
  py12: { paddingVertical: 12 },
  py16: { paddingVertical: 16 },
  py20: { paddingVertical: 20 },
  py24: { paddingVertical: 24 },
  py32: { paddingVertical: 32 },

  // Gap
  gapSmall: { gap: 4 },
  gapMedium: { gap: 8 },
  gapLarge: { gap: 16 },

  // Text alignment
  textCenter: { textAlign: 'center' },
  textLeft: { textAlign: 'left' },
  textRight: { textAlign: 'right' },
  textJustify: { textAlign: 'justify' },

  // Text transform
  uppercase: { textTransform: 'uppercase' },
  lowercase: { textTransform: 'lowercase' },
  capitalize: { textTransform: 'capitalize' },

  // Font weights
  fontThin: { fontWeight: '100' },
  fontLight: { fontWeight: '300' },
  fontNormal: { fontWeight: '400' },
  fontMedium: { fontWeight: '500' },
  fontSemiBold: { fontWeight: '600' },
  fontBold: { fontWeight: '700' },
  fontExtraBold: { fontWeight: '800' },
  fontBlack: { fontWeight: '900' },

  // Common borders
  border1: { borderWidth: 1 },
  border2: { borderWidth: 2 },
  borderTop1: { borderTopWidth: 1 },
  borderBottom1: { borderBottomWidth: 1 },
  borderLeft1: { borderLeftWidth: 1 },
  borderRight1: { borderRightWidth: 1 },

  // Border radius
  rounded0: { borderRadius: 0 },
  roundedXs: { borderRadius: 2 },
  roundedSm: { borderRadius: 6 },
  roundedMd: { borderRadius: 10 },
  roundedLg: { borderRadius: 14 },
  roundedXl: { borderRadius: 20 },
  roundedFull: { borderRadius: 9999 },

  // Top border radius
  roundedTopXs: { borderTopLeftRadius: 2, borderTopRightRadius: 2 },
  roundedTopSm: { borderTopLeftRadius: 6, borderTopRightRadius: 6 },
  roundedTopMd: { borderTopLeftRadius: 10, borderTopRightRadius: 10 },
  roundedTopLg: { borderTopLeftRadius: 14, borderTopRightRadius: 14 },

  // Bottom border radius
  roundedBottomXs: { borderBottomLeftRadius: 2, borderBottomRightRadius: 2 },
  roundedBottomSm: { borderBottomLeftRadius: 6, borderBottomRightRadius: 6 },
  roundedBottomMd: { borderBottomLeftRadius: 10, borderBottomRightRadius: 10 },
  roundedBottomLg: { borderBottomLeftRadius: 14, borderBottomRightRadius: 14 },

  // Shadows (theme-agnostic base)
  shadowSm: {
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2,
    elevation: 1,
  },
  shadow: {
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  shadowMd: {
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.15,
    shadowRadius: 6,
    elevation: 6,
  },
  shadowLg: {
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.2,
    shadowRadius: 12,
    elevation: 12,
  },

  // Opacity
  opacity0: { opacity: 0 },
  opacity25: { opacity: 0.25 },
  opacity50: { opacity: 0.5 },
  opacity75: { opacity: 0.75 },
  opacity100: { opacity: 1 },

  // Overflow
  hidden: { overflow: 'hidden' },
  visible: { overflow: 'visible' },

  // Z-index
  z0: { zIndex: 0 },
  z10: { zIndex: 10 },
  z20: { zIndex: 20 },
  z30: { zIndex: 30 },
  z40: { zIndex: 40 },
  z50: { zIndex: 50 },
});
