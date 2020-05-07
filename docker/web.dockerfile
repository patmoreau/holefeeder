FROM nginx:alpine
LABEL author="Drifter Apps Inc."
VOLUME /var/cache/nginx
COPY publish/DrifterApps.Holefeeder.Presentations.UI /usr/share/nginx/html
COPY src/DrifterApps.Holefeeder.Presentations.UI/config/nginx.conf /etc/nginx/conf.d/default.conf
