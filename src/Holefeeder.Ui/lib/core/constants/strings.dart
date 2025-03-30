import 'package:flutter_dotenv/flutter_dotenv.dart';

final String serverUrl = dotenv.env['API_SERVER_URL'] ?? '';
final auth0Domain = dotenv.env['AUTH0_DOMAIN'] ?? '';
final auth0ClientId = dotenv.env['AUTH0_CLIENT_ID'] ?? '';
final auth0Audience = dotenv.env['AUTH0_AUDIENCE'] ?? '';
final auth0RedirectUriWeb = dotenv.env['REDIRECT_URI'] ?? '';

const auth0Scopes = {
  'openid',
  'profile',
  'email',
  'offline_access',
  'read:user',
  'write:user',
};

const auth0Logo =
    'https://cdn.auth0.com/blog/hub/code-samples/hello-world/auth0-logo.svg';

const holefeederTitle = 'Holefeeder';
