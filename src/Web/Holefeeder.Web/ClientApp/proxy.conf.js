const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
  ? env.ASPNETCORE_URLS.split(';')[0]
  : 'http://localhost:19081';

const PROXY_CONFIG = [
  {
    context: ['/config', '/gateway', '/hc-ui', '/healthz'],
    target: target,
    secure: false,
  },
];

module.exports = PROXY_CONFIG;
