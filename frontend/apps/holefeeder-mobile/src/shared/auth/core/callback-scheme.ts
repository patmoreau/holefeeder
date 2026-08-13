// The browser session has to listen on the same scheme Auth0 will redirect to. The
// SDK derives its default from the bundle identifier, which does not match the
// configured URIs here (…holefeeder-react… against a bundle of …holefeeder), so the
// scheme is taken from the URI itself.
export const callbackSchemeOf = (uri: string): string | undefined => {
  const separator = uri.indexOf('://');
  if (separator <= 0) {
    return undefined;
  }
  return uri.slice(0, separator);
};
